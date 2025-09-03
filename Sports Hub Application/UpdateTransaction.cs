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


        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;


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
            _initialFormWidth = this.Width;
            _initialFormHeight = this.Height;

            // Store initial size and location of all controls
            _controlsInfo = new ControlInfo[this.Controls.Count];
            for (int i = 0; i < this.Controls.Count; i++)
            {
                Control c = this.Controls[i];
                _controlsInfo[i] = new ControlInfo(c.Left, c.Top, c.Width, c.Height, c.Font.Size);
            }

            // Set event handler for form resize
            this.Resize += Home_Resize;
            _username = username;
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

                // Adjust font size
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
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "diseasestatus", HeaderText = "الحاله المرضيه" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DangerousLevel", HeaderText = "درجة الخطورة" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrisonerStatus", HeaderText = "الحالة" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Accused", HeaderText = "التهمه" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrinciplesType", HeaderText = "مبدأ الحبس" });
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NextSession", HeaderText = "الجلسه القادمه" });
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
                        usersDataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DepositPlace", HeaderText = "مكان الايداع" });

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

            if (columnName == "HospitalDate" || columnName == "LeaveDate" || columnName == "NextSession" || columnName == "PrinciplesType")
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
             { "الحاله المرضيه", "diseasestatus" },
            { "مبدأ الحبس", "PrinciplesType" },
             {"الجلسه القادمه" , "NextSession" },
            { "مده الحكم", "ServiceTime" },
            { "تاريخ المستشفى", "HospitalDate" },
            { "تاريخ الخروج", "LeaveDate" },
            { "الفيش الجنائي", "CriminalRecord" },
            { "نماذج الحبس", "ImprisonmentDetails" },
            { "كشف أمن عام", "SecurityRevealed" },
            { "خطاب الرقابة", "CensorshipInfo" },
            { "ملاحظات", "Notes" },
            {"مكان الايداع" , "DepositPlace" }
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

        private void usersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
