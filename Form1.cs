using DataGridViewAutoFilter;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Dynamic_If
{
    public partial class Form1 : Form
    {
        List<Data> data, data1;
        public Form1()
        {
            InitializeComponent();

            data = new List<Data>
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
            dataGridView.Update();
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