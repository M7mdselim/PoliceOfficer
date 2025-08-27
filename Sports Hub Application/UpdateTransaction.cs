using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Mixed_Gym_Application
{
    public partial class UpdateTransaction : Form
    {
        private readonly string ConnectionString = DatabaseConfig.connectionString;
        private readonly string _username;
        private BindingSource bindingSource = new BindingSource();

        public UpdateTransaction(string username)
        {
            InitializeComponent();
            _username = username;

            usersDataGridView.DataSource = bindingSource;
            usersDataGridView.AutoGenerateColumns = true;

            LoadPrisoners();

            usersDataGridView.EditingControlShowing += UsersDataGridView_EditingControlShowing;
        }

        // Load prisoners
        private void LoadPrisoners()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "SELECT * FROM Prisoner ORDER BY CreatedDate DESC";

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        bindingSource.DataSource = dataTable;
                        usersDataGridView.DataSource = bindingSource;

                        // Remove old combo columns if they exist
                        if (usersDataGridView.Columns.Contains("DangerousLevel"))
                            usersDataGridView.Columns.Remove("DangerousLevel");
                        if (usersDataGridView.Columns.Contains("PrisonerStatus"))
                            usersDataGridView.Columns.Remove("PrisonerStatus");

                        // DangerousLevel ComboBox
                        DataGridViewComboBoxColumn dangerousLevelColumn = new DataGridViewComboBoxColumn
                        {
                            Name = "DangerousLevel",
                            DataPropertyName = "DangerousLevel",
                            HeaderText = "درجة الخطورة",
                            DataSource = new[] { "أ", "ب", "ج" }
                        };
                        usersDataGridView.Columns.Add(dangerousLevelColumn);

                        // Status ComboBox
                        DataGridViewComboBoxColumn statusColumn = new DataGridViewComboBoxColumn
                        {
                            Name = "PrisonerStatus",
                            DataPropertyName = "PrisonerStatus",
                            HeaderText = "الحالة",
                            DataSource = new[] { "حبس احتياطي", "حكم عليه", "اخلاء سبيل" }
                        };
                        usersDataGridView.Columns.Add(statusColumn);

                        // Make unwanted columns read-only
                        string[] readOnlyColumns = {
                            "PrisonerInfoID", "FullName", "NIDNumber",
                            "CreatedDate", "LastModified", "CreatedBy", "ModifiedBy"
                        };

                        foreach (string col in readOnlyColumns)
                        {
                            if (usersDataGridView.Columns.Contains(col))
                                usersDataGridView.Columns[col].ReadOnly = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading prisoners: " + ex.Message);
            }
        }

        // Prevent ComboBox in last row
        private void UsersDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (usersDataGridView.CurrentRow != null && usersDataGridView.CurrentRow.IsNewRow)
            {
                if (e.Control is ComboBox combo)
                {
                    combo.Items.Clear(); // remove dropdown in new row
                }
            }
        }

        // Update prisoners with ModifiedBy and LastModified
        private void UpdateData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Prisoner", connection);
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                    DataTable dataTable = (DataTable)bindingSource.DataSource;

                    if (dataTable != null)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (row.RowState == DataRowState.Modified)
                            {
                                row["ModifiedBy"] = _username;
                                row["LastModified"] = DateTime.Now;
                            }
                        }

                        dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                        dataAdapter.Update(dataTable);
                    }
                }
                MessageBox.Show("Prisoners updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating prisoners: " + ex.Message);
            }
        }

        private void loadtbtn_Click(object sender, EventArgs e)
        {
            LoadPrisoners();
        }

        private void updatebtn_Click(object sender, EventArgs e)
        {
            UpdateData();
        }
    }
}
