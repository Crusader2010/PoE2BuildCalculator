using System.Text.Json;

using Domain.Enums;
using Domain.Main;
using Domain.Serialization;
using Domain.Validation;
using Domain.Static;

namespace Manager
{
	/// <summary>
	/// Manages configuration in memory with async persistence to disk.
	/// Forms pull their config on creation rather than being pushed to.
	/// </summary>
	public sealed class ConfigurationManager
	{
		private static readonly Version _currentVersion = new(Constants.CURRENT_VERSION);

		private SaveData _configData = new() { Version = Constants.CURRENT_VERSION };
		private readonly Dictionary<string, Action<SaveData>> _migrationHandlers = [];

		public ConfigurationManager()
		{
			InitializeMigrationHandlers();
		}

		/// <summary>
		/// Checks if a configuration section has data in memory.
		/// </summary>
		public bool JSONHasConfigData(ConfigSections section) => section switch
		{
			ConfigSections.Tiers => _configData.Tiers?.Count > 0,
			ConfigSections.Validator => _configData.Groups?.Count > 0 || _configData.Operations?.Count > 0,
			_ => false
		};

		/// <summary>
		/// Gets configuration data for a specific section.
		/// Returns null if section has no data.
		/// </summary>
		public object GetConfigData(ConfigSections section) => section switch
		{
			ConfigSections.Tiers => _configData?.Tiers?.Count > 0 ? _configData.Tiers : null,
			ConfigSections.Validator => (_configData?.Groups?.Count > 0 || _configData?.Operations?.Count > 0)
											? (_configData.Groups ?? new List<GroupDto>(), _configData.Operations ?? new List<ValidationModel>())
											: null,
			_ => null
		};

		/// <summary>
		/// Updates configuration data for a specific section in memory.
		/// </summary>
		public void SetConfigData(ConfigSections section, object data)
		{
			switch (section)
			{
				case ConfigSections.Tiers:
					_configData.Tiers = data as List<Tier> ?? [];
					break;
				case ConfigSections.Validator:
					if (data is ValueTuple<List<GroupDto>, List<ValidationModel>> validatorData)
					{
						_configData.Groups = validatorData.Item1;
						_configData.Operations = validatorData.Item2;
					}
					break;
			}
			_configData.SavedAt = DateTime.Now;
		}

		/// <summary>
		/// Clears all configuration data from memory.
		/// </summary>
		public void ClearAllConfig()
		{
			_configData = new SaveData { Version = Constants.CURRENT_VERSION };
		}

		/// <summary>
		/// Atomically saves all in-memory configuration to disk.
		/// Creates backup before save, restores on failure.
		/// </summary>
		public async Task SaveAllAsync(string filePath, CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

			string backupPath = null;
			string tempPath = Path.GetTempFileName();

			try
			{
				// Create backup if file exists
				if (File.Exists(filePath))
				{
					backupPath = $"{filePath}.backup";
					await Task.Run(() => File.Copy(filePath, backupPath, overwrite: true), cancellationToken).ConfigureAwait(false);
				}

				// Update metadata
				_configData.Version = Constants.CURRENT_VERSION;
				_configData.SavedAt = DateTime.Now;

				// Write to temp file (async)
				await SerializationHelper.SaveToFileAsync(_configData, tempPath, cancellationToken).ConfigureAwait(false);

				// Validate temp file (async)
				_ = await SerializationHelper.LoadFromFileAsync(tempPath, cancellationToken).ConfigureAwait(false);

				// Atomic rename
				await Task.Run(() => File.Move(tempPath, filePath, overwrite: true), cancellationToken).ConfigureAwait(false);

				// Delete backup after successful save
				if (backupPath != null && File.Exists(backupPath))
					await Task.Run(() => File.Delete(backupPath), cancellationToken).ConfigureAwait(false);
			}
			catch
			{
				// Restore backup on failure
				if (backupPath != null && File.Exists(backupPath))
				{
					await Task.Run(() =>
					{
						File.Copy(backupPath, filePath, overwrite: true);
						File.Delete(backupPath);
					}, cancellationToken).ConfigureAwait(false);
				}
				throw;
			}
			finally
			{
				// Clean up temp file
				if (File.Exists(tempPath))
					await Task.Run(() => File.Delete(tempPath), CancellationToken.None).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Loads configuration from disk into memory with version validation and migration.
		/// Returns tuple: (success, errorMessage, migratedFrom).
		/// </summary>
		public async Task<(bool Success, string ErrorMessage, string MigratedFrom)> LoadAllAsync(string filePath, CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

			if (!File.Exists(filePath)) return (false, "Configuration file not found.", null);

			try
			{
				var saveData = await SerializationHelper.LoadFromFileAsync(filePath, cancellationToken).ConfigureAwait(false);

				// Version validation
				if (!Version.TryParse(saveData.Version, out var fileVersion))
					return (false, $"Invalid version format: {saveData.Version}", null);

				var comparison = CompareVersions(fileVersion, _currentVersion);
				string migratedFrom = null;

				switch (comparison)
				{
					case VersionComparison.IncompatibleMajor:
						return (false,
							$"Configuration file version {fileVersion} is incompatible with current version {_currentVersion}.\r\n\r\n" +
							"This file was created with a newer/older major version and cannot be loaded.\r\n" +
							"Please export your data from the original version before upgrading.",
							null);

					case VersionComparison.MinorNewer:
						// File is from newer minor version - warn but allow
						var result = await Task.Run(() => MessageBox.Show(
							$"Configuration file is from a newer version ({fileVersion} vs {_currentVersion}).\r\n\r\n" +
							"Some features may not load correctly. Continue?",
							"Version Warning",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Warning), cancellationToken).ConfigureAwait(false);

						if (result != DialogResult.Yes)
							return (false, "Load cancelled by user.", null);
						break;

					case VersionComparison.MinorOlder:
					case VersionComparison.PatchDifferent:
						// Attempt migration
						migratedFrom = saveData.Version;
						MigrateData(saveData, fileVersion);
						saveData.Version = Constants.CURRENT_VERSION;
						break;

					case VersionComparison.Exact:
						// Perfect match
						break;
				}

				// Store in memory
				_configData = saveData;

				return (true, null, migratedFrom);
			}
			catch (JsonException ex)
			{
				return (false, $"Invalid JSON format: {ex.Message}", null);
			}
			catch (Exception ex)
			{
				return (false, $"Failed to load configuration: {ex.Message}", null);
			}
		}

		private static VersionComparison CompareVersions(Version file, Version current)
		{
			if (file.Major != current.Major)
				return VersionComparison.IncompatibleMajor;

			if (file.Minor > current.Minor)
				return VersionComparison.MinorNewer;

			if (file.Minor < current.Minor)
				return VersionComparison.MinorOlder;

			if (file.Build != current.Build)
				return VersionComparison.PatchDifferent;

			return VersionComparison.Exact;
		}

		private void MigrateData(SaveData data, Version fromVersion)
		{
			var migrations = _migrationHandlers
				.Where(kvp => Version.Parse(kvp.Key) > fromVersion && Version.Parse(kvp.Key) <= _currentVersion)
				.OrderBy(kvp => Version.Parse(kvp.Key));

			foreach (var migration in migrations)
				migration.Value(data);
		}

		private static void InitializeMigrationHandlers()
		{
			// Future migrations go here
			// Example for future use:
			// _migrationHandlers["1.1.0"] = data => { /* migration logic */ };
		}

		private enum VersionComparison
		{
			Exact,
			PatchDifferent,
			MinorOlder,
			MinorNewer,
			IncompatibleMajor
		}
	}
}
