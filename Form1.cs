using DataGridViewAutoFilter;
using System;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
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
                new Data (1, 220, 23.5f, (UInt16)34, "test1"),
                new Data (2, 220, -34.2f, (UInt16)45, "test2"),
                new Data (3, 220, 67.3f, (UInt16)56, "test3"),
                new Data (4, 220, 45.1f, (UInt16)67, "test4"),
                new Data (5, 220, 56.7f, (UInt16)78, "test5"),
                new Data (6, 220, 78.8f, (UInt16)89, "test6"),
                new Data (7, 220, 89.9f, (UInt16)90, "test7"),
                new Data (8, 220, 90.0f, (UInt16)91, "test8")
            };

            dataGridView.DataSource = data;
            dataGridView.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.Update();
        }

        private Dictionary<int, int> HEADER_CLICK_CNTR = new Dictionary<int, int>();
        private void DataGridView_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (!HEADER_CLICK_CNTR.ContainsKey(int.Parse(e.ColumnIndex.ToString())))
            {
                HEADER_CLICK_CNTR.Add(int.Parse(e.ColumnIndex.ToString()), 0);
            }

            var column = dataGridView.Columns[e.ColumnIndex];
            SortOrder order = SortOrder.None;
            switch (HEADER_CLICK_CNTR[int.Parse(e.ColumnIndex.ToString())] % 3)
            {
                case 0:
                    order = SortOrder.Ascending;
                    break;
                case 1:
                    order = SortOrder.Descending;
                    break;
                case 2:
                    order = SortOrder.None;
                    break;
            }

            SortRows(column.Name, order);
            UpdateColumnHeader(column, order);

            HEADER_CLICK_CNTR[int.Parse(e.ColumnIndex.ToString())] += 1;
        }

        private void SortRows(string columnName, SortOrder order)
        {
            if (order == SortOrder.None)
            {
                dataGridView.DataSource = data;
                ClearColumnHeaders();
                return;
            }                

            List<Data> tempList = data.ToList();
            switch (columnName)
            {
                case "a":
                    tempList = order == SortOrder.Ascending ? data.OrderBy(d => d.a).ToList() : data.OrderByDescending(d => d.a).ToList();
                    break;
                case "b":
                    tempList = order == SortOrder.Ascending ? data.OrderBy(d => d.b).ToList() : data.OrderByDescending(d => d.b).ToList();
                    break;
                case "c":
                    tempList = order == SortOrder.Ascending ? data.OrderBy(d => d.c).ToList() : data.OrderByDescending(d => d.c).ToList();
                    break;
                case "d":
                    tempList = order == SortOrder.Ascending ? data.OrderBy(d => d.d).ToList() : data.OrderByDescending(d => d.d).ToList();
                    break;
                case "e":
                    tempList = order == SortOrder.Ascending ? data.OrderBy(d => d.e).ToList() : data.OrderByDescending(d => d.e).ToList();
                    break;
            }

            dataGridView.DataSource = new BindingList<Data>(tempList);
        }

        private void UpdateColumnHeader(DataGridViewColumn column, SortOrder sortOrder)
        {
            if (column.HeaderText.EndsWith(" ▲") || column.HeaderText.EndsWith(" ▼"))
            {
                column.HeaderText = column.HeaderText.Substring(0, column.HeaderText.Length - 2);
            }

            if (sortOrder == SortOrder.Ascending)
            {
                column.HeaderText += " ▲";
            }
            else if (sortOrder == SortOrder.Descending)
            {
                column.HeaderText += " ▼";
            }
        }

        private void ClearColumnHeaders()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (column.HeaderText.EndsWith(" ▲") || column.HeaderText.EndsWith(" ▼"))
                {
                    column.HeaderText = column.HeaderText.Substring(0, column.HeaderText.Length - 2);
                }
            }
        }

        private void sendButton_Click(object sender, EventArgs e) { ActionFunc(); }

        private void expressionText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ActionFunc();
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