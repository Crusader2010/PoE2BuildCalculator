using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using Domain;
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
            // Turn off AutoSize so the form can be sized by code.
            this.AutoSize = false;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // Remove all columns from TableDisplayData on form load.
            TableDisplayData.Columns.Clear();

            // Make it read-only and adjust sizing behaviour.
            TableDisplayData.ReadOnly = true;
            TableDisplayData.AllowUserToAddRows = false;
            TableDisplayData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            TableDisplayData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            TableDisplayData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Anchor the grid on all sides so when the form grows the grid expands to use the space.
            TableDisplayData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            ImportDisplayData_Click(sender, e);
        }

        private void ImportDisplayData_Click(object sender, EventArgs e)
        {
            if (_fileParser == null)
            {
                MessageBox.Show("No FileParser provided. Set DataDisplay.FileParser before importing.");
                return;
            }

            var items = _fileParser.GetParsedItems();
            if (items == null || items.Count == 0)
            {
                MessageBox.Show("No parsed items to display.");
                return;
            }

            // Clear any existing columns/rows and ensure base columns exist.
            TableDisplayData.Columns.Clear();
            TableDisplayData.Rows.Clear();

            // Add column headers for non-stats
            var baseColumns = new[] { "Id", "Name", "Class" };
            foreach (var c in baseColumns)
            {
                TableDisplayData.Columns.Add(new DataGridViewTextBoxColumn { Name = c, HeaderText = c });
            }

            // Efficiently get descriptors (cached, ordered) and compute which are required.
            // Add stat columns using descriptor.PropertyName as column Name and descriptor.Header as header text.
            var descriptors = PropertyDescriptionHelper.GetStatDescriptors();
            foreach (var d in descriptors)
            {
                // Ensure unique column name (should be unique by property name).
                if (!TableDisplayData.Columns.Contains(d.PropertyName))
                    TableDisplayData.Columns.Add(new DataGridViewTextBoxColumn { Name = d.PropertyName, HeaderText = d.Header });
            }

            // Populate rows using PropertyDescriptionHelper.ToDictionary for each item (efficient).
            foreach (var item in items)
            {
                // Build a header->value map for this item's stats.
                var statsMap = PropertyDescriptionHelper.ToDictionary(item.ItemStats);

                var rowValues = new object[3 + descriptors.Count];
                rowValues[0] = item.Id;
                rowValues[1] = item.Name;
                rowValues[2] = item.Class;

                for (int i = 0; i < descriptors.Count; i++)
                {
                    var d = descriptors[i];
                    // The dictionary keys are the descriptor.Header values.
                    if (statsMap.TryGetValue(d.Header, out var v) && PropertyDescriptionHelper.HasValue(v))
                        rowValues[3 + i] = v;
                    else
                        rowValues[3 + i] = 0;
                }

                TableDisplayData.Rows.Add(rowValues);
            }

            // Resize columns to fit the content after population.
            TableDisplayData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // Adjust the form size so as much of TableDisplayData as possible is visible.
            AdjustFormSizeToDataGrid();
        }

        /// <summary>
        /// Resize the form so the DataGridView fits as much content as possible while staying within the screen working area.
        /// </summary>
        private void AdjustFormSizeToDataGrid()
        {
            if (TableDisplayData.Columns.Count == 0)
                return;

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

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
