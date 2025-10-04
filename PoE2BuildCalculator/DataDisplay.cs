using Domain.Helpers;
using Manager;

namespace PoE2BuildCalculator
{
    public partial class DataDisplay : Form
    {
        // Caller should set this before ImportDisplayData_Click is invoked.
        public FileParser _fileParser { get; set; }

        public DataDisplay()
        {
            InitializeComponent();

            this.Load += DataDisplay_Load;
        }

        private void DataDisplay_Load(object sender, EventArgs e)
        {
            this.AutoSize = false; // Turn off AutoSize so the form can be sized by code.
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            TableDisplayData.Columns.Clear(); // Remove all columns from TableDisplayData on form load.

            // Make it read-only and adjust sizing behaviour.
            TableDisplayData.ReadOnly = true;
            TableDisplayData.AllowUserToAddRows = false;
            TableDisplayData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            TableDisplayData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            TableDisplayData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Anchor the grid on all sides so when the form grows the grid expands to use the space.
            TableDisplayData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            if (_fileParser == null)
            {
                MessageBox.Show("No FileParser provided. Set DataDisplay.FileParser before importing.");
                return;
            }

            ImportDataToDisplay_Click(sender, e);
        }

        private void ImportDataToDisplay_Click(object sender, EventArgs e)
        {
            var items = _fileParser.GetParsedItems();
            if (items == null || items.Count == 0)
            {
                MessageBox.Show("No parsed items to display.");
                return;
            }

            try
            {
                // Clear any existing columns/rows and ensure base columns exist.
                TableDisplayData.Columns.Clear();
                TableDisplayData.Rows.Clear();

                IReadOnlyList<ItemStatsHelper.StatDescriptor> descriptors = ItemStatsHelper.GetStatDescriptors();

                AddNonStatsColumnHeadersToTable();
                AddStatsColumnHeadersToTable(descriptors);

                const int baseColumnCount = 5; // Id, Name, Class, IsMine, Corrupted
                foreach (var item in items) // Populate rows using PropertyDescriptionHelper.ToDictionary for each item.
                {
                    // Build a PropertyName->value map for this item's stats.
                    var statsMap = ItemStatsHelper.ToDictionary(item.ItemStats);

                    var rowValues = new object[baseColumnCount + descriptors.Count];
                    rowValues[0] = item.Id;
                    rowValues[1] = item.Name;
                    rowValues[2] = item.Class;
                    rowValues[3] = item.IsMine ? "YES" : "NO";
                    rowValues[4] = item.IsCorrupted ? "YES" : "NO";

                    for (int i = 0; i < descriptors.Count; i++)
                    {
                        var d = descriptors[i];
                        rowValues[baseColumnCount + i] = statsMap.TryGetValue(d.PropertyName, out var v) ? v : GetEmptyValueForTableRow(d.PropertyType);
                    }

                    TableDisplayData.Rows.Add(rowValues);
                }

                // Resize columns to fit the content after population.
                TableDisplayData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                ErrorHelper.ShowError(ex, nameof(ImportDataToDisplay_Click));
                throw;
            }

            // Adjust the form size so as much of TableDisplayData as possible is visible.
            AdjustFormSizeToDataGrid();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }



        #region Private helpers

        private static object GetEmptyValueForTableRow(Type propertyType)
        {
            return propertyType == typeof(string) ? string.Empty : ItemStatsHelper.ConvertToType(propertyType, 0);
        }

        private void AddNonStatsColumnHeadersToTable()
        {
            try
            {
                // Add column header for item ID.
                var idColumn = new DataGridViewTextBoxColumn
                {
                    Name = "Id",
                    HeaderText = "Id",
                    ValueType = typeof(int)
                };
                TableDisplayData.Columns.Add(idColumn);

                // Add column headers for other non-stat properties.
                var otherBaseColumns = new[] { "Name", "Class", "Mine?", "Corrupted" };
                foreach (var c in otherBaseColumns)
                {
                    var col = new DataGridViewTextBoxColumn
                    {
                        Name = c,
                        HeaderText = c,
                        ValueType = typeof(string)
                    };

                    TableDisplayData.Columns.Add(col);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AddStatsColumnHeadersToTable(IReadOnlyList<ItemStatsHelper.StatDescriptor> descriptors)
        {
            try
            {
                if (descriptors == null || descriptors.Count == 0) return;

                // Add stat columns using descriptor.PropertyName as column Name and descriptor.Header as header text.            
                foreach (var d in descriptors)
                {
                    // Ensure unique column name (should be unique by property name).
                    if (!TableDisplayData.Columns.Contains(d.PropertyName))
                    {
                        var col = new DataGridViewTextBoxColumn
                        {
                            Name = d.PropertyName,
                            DataPropertyName = d.PropertyName,
                            HeaderText = d.Header,
                            ValueType = d.PropertyType
                        };
                        TableDisplayData.Columns.Add(col);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Resize the form so the DataGridView fits as much content as possible while staying within the screen working area.
        /// </summary>
        private void AdjustFormSizeToDataGrid()
        {
            try
            {
                if (TableDisplayData.Columns.Count == 0) return;

                // Ensure layout is up-to-date so column widths are valid.
                TableDisplayData.PerformLayout();
                TableDisplayData.Update();


                // Compute required client size for the grid (width = row header + all column widths; height = column header + sum of row heights).
                int requiredWidth = TableDisplayData.RowHeadersVisible ? TableDisplayData.RowHeadersWidth : 0;
                foreach (DataGridViewColumn col in TableDisplayData.Columns)
                    requiredWidth += col.Width;

                int requiredHeight = TableDisplayData.ColumnHeadersHeight;
                // Sum the heights of rows (handles variable row heights)
                foreach (DataGridViewRow row in TableDisplayData.Rows)
                    requiredHeight += row.Height;

                // Small padding so borders/lines aren't cut off.
                const int gridPadding = 6;
                requiredWidth += gridPadding;
                requiredHeight += gridPadding;

                // Convert grid client size to a desired form size by adding non-client chrome.
                int chromeWidth = this.Width - this.ClientSize.Width;
                int chromeHeight = this.Height - this.ClientSize.Height;

                int desiredFormWidth = requiredWidth + chromeWidth;
                int desiredFormHeight = requiredHeight + chromeHeight;

                // Clamp to screen working area with a margin so form isn't fullscreen or off-screen.
                Rectangle wa = Screen.GetWorkingArea(this);
                const int margin = 36;
                int maxWidth = Math.Max(wa.Width - margin, 200);
                int maxHeight = Math.Max(wa.Height - margin, 100);

                desiredFormWidth = Math.Min(desiredFormWidth, maxWidth);
                desiredFormHeight = Math.Min(desiredFormHeight, maxHeight);

                // Apply new size
                this.Size = new Size(desiredFormWidth, desiredFormHeight);

                // Ensure form is still on-screen; nudge if necessary.
                int newX = Math.Min(this.Location.X, wa.Right - this.Width - 10);
                int newY = Math.Min(this.Location.Y, wa.Bottom - this.Height - 10);
                newX = Math.Max(newX, wa.Left + 10);
                newY = Math.Max(newY, wa.Top + 10);
                this.Location = new Point(newX, newY);
            }
            catch (Exception ex)
            {
                ErrorHelper.ShowError(ex, $"{nameof(DataDisplay)} - {nameof(AdjustFormSizeToDataGrid)}");
                throw;
            }
        }

        #endregion
    }
}
