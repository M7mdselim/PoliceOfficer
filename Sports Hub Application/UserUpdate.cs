using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mixed_Gym_Application
{
    public partial class UserUpdate : Form
    {
        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;
        private string _username;
        private string ConnectionString;
        private BindingSource bindingSource;

        public UserUpdate(string username)
        {
            InitializeComponent();

            _username = username;
            ConnectionString = DatabaseConfig.connectionString;

            // Initialize DataGridView and BindingSource
            InitializeControls();

            _initialFormWidth = this.Width;
            _initialFormHeight = this.Height;

            _controlsInfo = new ControlInfo[this.Controls.Count];
            for (int i = 0; i < this.Controls.Count; i++)
            {
                Control c = this.Controls[i];
                _controlsInfo[i] = new ControlInfo(c.Left, c.Top, c.Width, c.Height, c.Font.Size);
            }
            usersDataGridView.EditingControlShowing += usersDataGridView_EditingControlShowing;

            this.Resize += Home_Resize;
        }

        private void InitializeControls()
        {
            bindingSource = new BindingSource();

            if (usersDataGridView == null)
            {
                usersDataGridView = new DataGridView();
                usersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                usersDataGridView.Dock = DockStyle.Fill;
                usersDataGridView.DataSource = bindingSource;
                this.Controls.Add(usersDataGridView);
            }
        }

        private void Home_Resize(object sender, EventArgs e)
        {
            float widthRatio = this.Width / _initialFormWidth;
            float heightRatio = this.Height / _initialFormHeight;
            ResizeControls(this.Controls, widthRatio, heightRatio);
        }

        private void ResizeControls(Control.ControlCollection controls, float widthRatio, float heightRatio)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                Control control = controls[i];
                ControlInfo controlInfo = _controlsInfo[i];

                control.Left = (int)(controlInfo.Left * widthRatio);
                control.Top = (int)(controlInfo.Top * heightRatio);
                control.Width = (int)(controlInfo.Width * widthRatio);
                control.Height = (int)(controlInfo.Height * heightRatio);

                control.Font = new Font(control.Font.FontFamily, controlInfo.FontSize * Math.Min(widthRatio, heightRatio));
            }
        }

        private class ControlInfo
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public float FontSize { get; set; }

            public ControlInfo(int left, int top, int width, int height, float fontSize)
            {
                Left = left;
                Top = top;
                Width = width;
                Height = height;
                FontSize = fontSize;
            }
        }

        // inside PrisonerInfoUpdate class

        // === Load PrisonerInfo data ===
        private void LoadData(string filterName = null, string filterNID = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = @"SELECT PrisonerInfoID, FullName, NIDNumber, DangerousLevel, PrisonerStatus, CreatedDate, LastModified, CreatedBy, ModifiedBy
                             FROM PrisonerInfo";

                    if (!string.IsNullOrEmpty(filterName))
                    {
                        query += " WHERE FullName LIKE @NameFilter";
                    }
                    else if (!string.IsNullOrEmpty(filterNID))
                    {
                        query += " WHERE NIDNumber LIKE @NIDFilter";
                    }

                    query += " ORDER BY LastModified DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    if (!string.IsNullOrEmpty(filterName))
                        adapter.SelectCommand.Parameters.AddWithValue("@NameFilter", "%" + filterName + "%");
                    if (!string.IsNullOrEmpty(filterNID))
                        adapter.SelectCommand.Parameters.AddWithValue("@NIDFilter", "%" + filterNID + "%");

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    bindingSource.DataSource = dt;
                    usersDataGridView.DataSource = bindingSource;

                    // Make some columns read-only
                    usersDataGridView.Columns["PrisonerInfoID"].ReadOnly = true;
                    usersDataGridView.Columns["CreatedDate"].ReadOnly = true;
                    usersDataGridView.Columns["CreatedBy"].ReadOnly = true;

                    // Replace DangerousLevel column with ComboBox
                    if (usersDataGridView.Columns.Contains("DangerousLevel"))
                    {
                        DataGridViewComboBoxColumn comboDanger = new DataGridViewComboBoxColumn();
                        comboDanger.DataPropertyName = "DangerousLevel";
                        comboDanger.HeaderText = "DangerousLevel";
                        comboDanger.Items.AddRange("أ", "ب", "ج");

                        int index = usersDataGridView.Columns["DangerousLevel"].Index;
                        usersDataGridView.Columns.Remove("DangerousLevel");
                        usersDataGridView.Columns.Insert(index, comboDanger);
                    }

                    // Replace PrisonerStatus column with ComboBox
                    if (usersDataGridView.Columns.Contains("PrisonerStatus"))
                    {
                        DataGridViewComboBoxColumn comboStatus = new DataGridViewComboBoxColumn();
                        comboStatus.DataPropertyName = "PrisonerStatus";
                        comboStatus.HeaderText = "PrisonerStatus";
                        comboStatus.Items.AddRange("حبس احتياطي", "حكم عليه", "اخلاء سبيل");

                        int index = usersDataGridView.Columns["PrisonerStatus"].Index;
                        usersDataGridView.Columns.Remove("PrisonerStatus");
                        usersDataGridView.Columns.Insert(index, comboStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading data: " + ex.Message);
            }
        }
        private void usersDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (usersDataGridView.CurrentRow != null &&
                usersDataGridView.CurrentRow.IsNewRow)
            {
                // If it's the new row, force the column to be simple textbox instead of combo
                if (e.Control is ComboBox combo)
                {
                    combo.DropDownStyle = ComboBoxStyle.DropDown; // behaves like textbox
                    combo.Items.Clear(); // remove predefined values
                }
            }
        }
        // === Update back to DB ===
        private void UpdateData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT PrisonerInfoID, FullName, NIDNumber, DangerousLevel, PrisonerStatus, CreatedDate, LastModified, CreatedBy, ModifiedBy FROM PrisonerInfo", connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                    // Override UpdateCommand to always update ModifiedBy + LastModified
                    adapter.UpdateCommand = new SqlCommand(
                        @"UPDATE PrisonerInfo
                  SET FullName = @FullName,
                      NIDNumber = @NIDNumber,
                      DangerousLevel = @DangerousLevel,
                      PrisonerStatus = @PrisonerStatus,
                      LastModified = GETDATE(),
                      ModifiedBy = @ModifiedBy
                  WHERE PrisonerInfoID = @PrisonerInfoID", connection);

                    adapter.UpdateCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 200, "FullName");
                    adapter.UpdateCommand.Parameters.Add("@NIDNumber", SqlDbType.NVarChar, 50, "NIDNumber");
                    adapter.UpdateCommand.Parameters.Add("@DangerousLevel", SqlDbType.NVarChar, 5, "DangerousLevel");
                    adapter.UpdateCommand.Parameters.Add("@PrisonerStatus", SqlDbType.NVarChar, 100, "PrisonerStatus");
                    adapter.UpdateCommand.Parameters.Add("@PrisonerInfoID", SqlDbType.Int, 0, "PrisonerInfoID").SourceVersion = DataRowVersion.Original;

                    // always use current username
                    adapter.UpdateCommand.Parameters.AddWithValue("@ModifiedBy", _username);

                    DataTable dt = (DataTable)bindingSource.DataSource;
                    if (dt != null)
                    {
                        adapter.Update(dt);
                    }
                }
                MessageBox.Show("Prisoner info updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating data: " + ex.Message);
            }
        }

        // === Button Events ===


        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Home home = new Home(_username);
            home.ShowDialog();
            this.Close();
        }

        private void loadtbtn_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void updatebtn_Click_1(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void updatetransbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            UpdateTransaction updates = new UpdateTransaction(_username);
            updates.ShowDialog();
            this.Close();
        }
    }
}
