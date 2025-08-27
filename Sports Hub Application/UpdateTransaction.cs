using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Mixed_Gym_Application
{
    public partial class UpdateTransaction : Form
    {
        private readonly string ConnectionString = DatabaseConfig.connectionString;
        private readonly string _username;
        private BindingSource bindingSource = new BindingSource();

        // ✅ DateTimePicker متغير عالمي
        private DateTimePicker dtp;

        public UpdateTransaction(string username)
        {
            InitializeComponent();
            _username = username;

            usersDataGridView.DataSource = bindingSource;
            usersDataGridView.AutoGenerateColumns = false;

            LoadPrisoners();

            usersDataGridView.EditingControlShowing += UsersDataGridView_EditingControlShowing;
            usersDataGridView.CellBeginEdit += usersDataGridView_CellBeginEdit; // ✅ تفعيل DateTimePicker
        }

        // تحميل بيانات السجناء
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
                        usersDataGridView.Columns.Clear();

                        // ✅ أعمدة مترجمة
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrisonerInfoID", HeaderText = "كود السجين", ReadOnly = true });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "الاسم", ReadOnly = true }); // ✅ ReadOnly
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ReservationNumber", HeaderText = "رقم الحجز" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CaseID", HeaderText = "رقم القضية" });

                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DangerousLevel", HeaderText = "درجة الخطورة" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrisonerStatus", HeaderText = "الحالة" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Accused", HeaderText = "التهمه" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrinciplesType", HeaderText = "مبدأ الحبس" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ServiceTime", HeaderText = "مده الحكم" });

                        // ✅ تاريخ المستشفى + الخروج (هنحط فيهم DateTimePicker)
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HospitalDate", HeaderText = "تاريخ المستشفى" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LeaveDate", HeaderText = "تاريخ الخروج" });

                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NIDNumber", HeaderText = "رقم الهوية" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CriminalRecord", HeaderText = "الفيش الجنائي" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ImprisonmentDetails", HeaderText = "نماذج الحبس" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SecurityRevealed", HeaderText = "كشف أمن عام" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CensorshipInfo", HeaderText = "خطاب الرقابة" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Notes", HeaderText = "ملاحظات" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CreatedDate", HeaderText = "تاريخ الإنشاء", ReadOnly = true });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LastModified", HeaderText = "آخر تعديل", ReadOnly = true });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CreatedBy", HeaderText = "تم الإنشاء بواسطة", ReadOnly = true });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ModifiedBy", HeaderText = "تم التعديل بواسطة", ReadOnly = true });

                        usersDataGridView.DataSource = bindingSource;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تحميل بيانات السجناء: " + ex.Message);
            }
        }

        // ✅ DateTimePicker يظهر فقط عند الأعمدة المحددة
        private void usersDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            string columnName = usersDataGridView.Columns[e.ColumnIndex].DataPropertyName;

            if (columnName == "HospitalDate" || columnName == "LeaveDate")
            {
                dtp = new DateTimePicker();
                dtp.Format = DateTimePickerFormat.Short;
                dtp.Visible = true;

                Rectangle rect = usersDataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                dtp.Size = new Size(rect.Width, rect.Height);
                dtp.Location = new Point(rect.X, rect.Y);

                if (usersDataGridView.CurrentCell.Value != DBNull.Value && usersDataGridView.CurrentCell.Value != null)
                {
                    dtp.Value = Convert.ToDateTime(usersDataGridView.CurrentCell.Value);
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }

                dtp.CloseUp += new EventHandler(dtp_CloseUp);
                dtp.TextChanged += new EventHandler(dtp_OnTextChange);

                usersDataGridView.Controls.Add(dtp);
            }
        }

        private void dtp_OnTextChange(object sender, EventArgs e)
        {
            usersDataGridView.CurrentCell.Value = dtp.Value;
        }

        private void dtp_CloseUp(object sender, EventArgs e)
        {
            dtp.Visible = false;
        }

        // منع الكومبوبوكس في الصف الجديد
        private void UsersDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (usersDataGridView.CurrentRow != null && usersDataGridView.CurrentRow.IsNewRow)
            {
                if (e.Control is ComboBox combo)
                {
                    combo.DropDownStyle = ComboBoxStyle.DropDownList;
                    combo.Items.Clear();
                }
            }
        }

        // تحديث البيانات مع تسجيل المستخدم
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
                MessageBox.Show("✅ تم تحديث بيانات السجناء بنجاح.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ أثناء تحديث البيانات: " + ex.Message);
            }
        }

        private void loadtbtn_Click(object sender, EventArgs e) => LoadPrisoners();
        private void updatebtn_Click(object sender, EventArgs e) => UpdateData();
        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            UserUpdate usrupdate = new UserUpdate(_username);
            usrupdate.ShowDialog();
            this.Close();
        }

        // ✅ خريطة الأعمدة (عربي → إنجليزي)
        private Dictionary<string, string> columnMap = new Dictionary<string, string>
        {
            { "الاسم", "FullName" },
            { "الرقم القومي", "NIDNumber" },
            { "درجة الخطورة", "DangerousLevel" },
            { "الحالة", "PrisonerStatus" },
            { "رقم الحجز", "ReservationNumber" },
            { "رقم القضية", "CaseID" },
            { "التهمه", "Accused" },
            { "مبدأ الحبس", "PrinciplesType" },
            { "مده الحكم", "ServiceTime" },
            { "تاريخ المستشفى", "HospitalDate" },
            { "تاريخ الخروج", "LeaveDate" },
            { "الفيش الجنائي", "CriminalRecord" },
            { "نماذج الحبس", "ImprisonmentDetails" },
            { "كشف أمن عام", "SecurityRevealed" },
            { "خطاب الرقابة", "CensorshipInfo" },
            { "ملاحظات", "Notes" }
        };

        private void LoadColumnsToComboBox()
        {
            columnnamecombobox.Items.Clear();
            foreach (var col in columnMap.Keys)
            {
                columnnamecombobox.Items.Add(col);
            }
            if (columnnamecombobox.Items.Count > 0)
                columnnamecombobox.SelectedIndex = 0;
        }

        private void searchtxt_TextChanged(object sender, EventArgs e)
        {
            if (columnnamecombobox.SelectedItem == null)
                return;

            string selectedArabic = columnnamecombobox.SelectedItem.ToString();
            string columnName = columnMap[selectedArabic];
            string searchValue = searchtxt.Text.Trim();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = $"SELECT * FROM Prisoner WHERE {columnName} LIKE @value ORDER BY CreatedDate DESC";

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@value", "%" + searchValue + "%");

                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        bindingSource.DataSource = dataTable;
                        usersDataGridView.DataSource = bindingSource;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ أثناء البحث: " + ex.Message);
            }
        }

        private void UpdateTransaction_Load(object sender, EventArgs e)
        {
            LoadColumnsToComboBox();
        }

        private void columnnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
