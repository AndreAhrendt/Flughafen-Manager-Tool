using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FlughafenManagerTool
{
    public partial class Form1 : Form
    {
        private BindingSource bindingSource = new BindingSource();
        private Controller controller = new Controller();
        private bool newRow = false;
        private bool cellChanged = false;

        public Form1()
        {
            InitializeComponent();
            this.bindingSource.DataSource = this.controller.GetData();
            this.dataGridView.DataSource = this.bindingSource;
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.bindingSource.DataSource = controller.ReadCsv(openFileDialog.FileName);
                if (!string.IsNullOrWhiteSpace(this.textBoxSearch.Text))
                    this.textBoxSearch.Text = string.Empty;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.bindingSource.DataSource = this.controller.SortList();
                this.controller.WriteCsv(this.saveFileDialog.FileName);
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.cellChanged = true;

            if (!string.IsNullOrEmpty(this.labelError.Text))
                this.labelError.Text = string.Empty;
        }

        private void dataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            this.newRow = true;
        }
        
        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            this.controller.RemoveItem(e.Row.Index);
        }

        private void dataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!this.dataGridView.IsCurrentRowDirty)
                return;

            Data rowData = (this.bindingSource.DataSource as List<Data>)[e.RowIndex];
            if (string.IsNullOrWhiteSpace(rowData.Name) || string.IsNullOrWhiteSpace(rowData.Id)|| string.IsNullOrWhiteSpace(rowData.Country))
            {
                e.Cancel=true;
                this.labelError.Text = "Bitte alle Felder ausfüllen!";
                return;
            }

            if(rowData.Id.Length != 3)
            {
                e.Cancel = true;
                this.labelError.Text = "Bitte richtige Id eingeben (3-stellig)!";
                return;
            }

            if (this.newRow)
            {
                this.controller.AddItem(rowData);
                this.newRow = false;
                this.cellChanged = false;
            }
            else if(this.cellChanged)
            {
                this.controller.ItemChanged(rowData, e.RowIndex);
                this.cellChanged = false;
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            this.bindingSource.DataSource = this.controller.SearchList(this.textBoxSearch.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
        }
    }
}
