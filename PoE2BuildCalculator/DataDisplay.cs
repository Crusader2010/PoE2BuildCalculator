using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
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

            // Keep existing patterns: wire load handler here.
            this.Load += DataDisplay_Load;
        }

        private void DataDisplay_Load(object sender, EventArgs e)
        {
            // Remove all columns from TableDisplayData on form load.
            TableDisplayData.Columns.Clear();

            // Make it read-only and adjust sizing behaviour.
            TableDisplayData.ReadOnly = true;
            TableDisplayData.AllowUserToAddRows = false;
            TableDisplayData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            TableDisplayData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            TableDisplayData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
        }
    }
}
