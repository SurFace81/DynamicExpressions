using DataGridViewAutoFilter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Dynamic_If
{
    public partial class Form1 : Form
    {
        BindingList<Data> data;
        
        public Form1()
        {
            InitializeComponent();

            data = new BindingList<Data>
            {
                new Data(1, 100, 23.5f, (UInt16)34, "test1"),
                new Data(2, 200, -23.5f, (UInt16)45, "test2"),
                new Data(3, 100, 45.1f, (UInt16)56, "test3"),
                new Data(4, 200, 67.3f, (UInt16)67, "test4"),
                new Data(5, 100, 56.7f, (UInt16)78, "test5"),
                new Data(6, 200, 45.1f, (UInt16)89, "test6"),
                new Data(7, 100, 89.9f, (UInt16)90, "test7"),
                new Data(8, 200, 78.8f, (UInt16)91, "test8"),
                new Data(9, 250, 23.5f, (UInt16)34, "test9"),
                new Data(10, 250, 89.9f, (UInt16)90, "test10")
            };

            dataGridView.DataSource = data;
            dataGridView.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.Update();
        }

        private List<(int columnIndex, SortOrder sortOrder)> sortOrders = new List<(int columnIndex, SortOrder sortOrder)>();

        private void DataGridView_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            var column = dataGridView.Columns[e.ColumnIndex];

            var existingSortOrder = sortOrders.FirstOrDefault(so => so.columnIndex == e.ColumnIndex);
            SortOrder newOrder = SortOrder.Ascending;
            if (existingSortOrder.sortOrder == SortOrder.Ascending)
            {
                newOrder = SortOrder.Descending;
            }
            else if (existingSortOrder.sortOrder == SortOrder.Descending)
            {
                newOrder = SortOrder.None;
            }

            if (newOrder == SortOrder.None)
            {
                sortOrders.Remove(existingSortOrder);
            }
            else
            {
                if (existingSortOrder.sortOrder == SortOrder.None)
                {
                    sortOrders.Add((e.ColumnIndex, newOrder));
                }
                else
                {
                    var index = sortOrders.FindIndex(so => so.columnIndex == e.ColumnIndex);
                    sortOrders[index] = (e.ColumnIndex, newOrder);
                }
            }

            SortRows();
            UpdateColumnHeaders();
        }

        private void SortRows()
        {
            IOrderedEnumerable<Data>? sortedData = null;

            foreach (var sortOrder in sortOrders)
            {
                var columnName = dataGridView.Columns[sortOrder.columnIndex].Name;
                var currentSorting = sortOrder.sortOrder;

                switch (columnName)
                {
                    case "a":
                        sortedData = Sort(sortedData, d => d.a, currentSorting);
                        break;
                    case "b":
                        sortedData = Sort(sortedData, d => d.b, currentSorting);
                        break;
                    case "c":
                        sortedData = Sort(sortedData, d => d.c, currentSorting);
                        break;
                    case "d":
                        sortedData = Sort(sortedData, d => d.d, currentSorting);
                        break;
                    case "e":
                        sortedData = Sort(sortedData, d => d.e, currentSorting);
                        break;
                }
            }

            dataGridView.DataSource = new BindingList<Data>(sortedData?.ToList() ?? data.ToList());
        }

        private IOrderedEnumerable<Data> Sort(IOrderedEnumerable<Data>? sortedData, Func<Data, object> func, SortOrder currentSorting)
        {
            return sortedData == null
                        ? currentSorting == SortOrder.Ascending
                            ? data.OrderBy(func)
                            : data.OrderByDescending(func)
                        : currentSorting == SortOrder.Ascending
                            ? sortedData.ThenBy(func)
                            : sortedData.ThenByDescending(func);
        }

        private void UpdateColumnHeaders()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            foreach (var sortOrder in sortOrders)
            {
                dataGridView.Columns[sortOrder.columnIndex].HeaderCell.SortGlyphDirection = sortOrder.sortOrder;
            }
        }

        private void sendButton_Click(object sender, EventArgs e) { ActionFunc(); }

        private void expressionText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ActionFunc();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ActionFunc()
        {
            if (string.IsNullOrEmpty(expressionText.Text))
            {
                dataGridView.DataSource = data;
                return;
            }
            PropertySelector<Data> ps = new PropertySelector<Data>(SelectorFunc);
            dataGridView.DataSource = (new DynamicExpressions<Data>(ps)).Evaluate(data, expressionText.Text);
        }

        private object SelectorFunc(Data data, string property)
        {
            switch (property)
            {
                case "a": return data.a;
                case "b": return data.b;
                case "c": return data.c;
                case "d": return data.d;
                case "e": return data.e;
                case "true": return true;
                case "false": return false;
                default: throw new ArgumentException("Invalid property: " + property);
            }
        }
    }
}
