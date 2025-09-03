using Mixed_Gym_Application;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Police_officer_Application
{
    public partial class DepositPlaceForm: Form
    {

        private string _username;

        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;

        private string ConnectionString;
        public DepositPlaceForm(string username)
        {



            _username = username;



         

        InitializeComponent();
            ConnectionString = DatabaseConfig.connectionString;
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


        private void FormDepositPlaces_Load(object sender, EventArgs e)
        {
            LoadDepositPlaces();
            cmbStatus.SelectedIndex = 0; // default to first option (حبس احتياطي)
        }

        private void LoadDepositPlaces()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    string query = @"SELECT DISTINCT DepositPlace 
                             FROM PrisonerInfo
                             WHERE DepositPlace IS NOT NULL
                             ORDER BY DepositPlace";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmbDepositPlaces.Items.Clear();

                    // 🟢 إضافة خيار "الكل" كأول عنصر
                    cmbDepositPlaces.Items.Add("الكل");

                    while (reader.Read())
                    {
                        cmbDepositPlaces.Items.Add(reader["DepositPlace"].ToString());
                    }

                    // 🟢 خلي الاختيار الافتراضي "الكل"
                    cmbDepositPlaces.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تحميل أماكن الإيداع: " + ex.Message);
            }
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            string selectedDeposit = cmbDepositPlaces.SelectedItem?.ToString();
            string selectedStatus = cmbStatus.SelectedItem?.ToString();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT FullName, NIDNumber, PrisonerStatus, DepositPlace, CreatedDate " +
                               "FROM PrisonerInfo WHERE 1=1";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                // فلترة بمكان الإيداع لو تم اختياره وليس "الكل"
                if (!string.IsNullOrEmpty(selectedDeposit) && selectedDeposit != "الكل")
                {
                    ApplyFilter(ref query, cmd, "DepositPlace", "@DepositPlace", selectedDeposit);
                }

                // فلترة بالحالة لو تم اختيارها وليس "الكل"
                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "الكل")
                {
                    ApplyFilter(ref query, cmd, "PrisonerStatus", "@PrisonerStatus", selectedStatus);
                }

                cmd.CommandText = query;

                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // 🟢 هنا تضيف الكود الجديد
                    if (dt.Rows.Count > 0)
                    {
                        DataRow totalRow = dt.NewRow();
                        totalRow["FullName"] = "إجمالي عدد السجناء:";
                        totalRow["NIDNumber"] = dt.Rows.Count.ToString();
                        dt.Rows.Add(totalRow);
                    }

                    dgvPrisoners.DataSource = dt;

                    // 🟢 تغيير أسماء الأعمدة حسب الـ columnHeaderMappings
                    foreach (DataGridViewColumn col in dgvPrisoners.Columns)
                    {
                        if (columnHeaderMappings.ContainsKey(col.Name))
                        {
                            col.HeaderText = columnHeaderMappings[col.Name];
                        }
                    }

                    // 🟢 تلوين صف الإجمالي
                    int lastRowIndex = dgvPrisoners.Rows.Count - 1;
                    if (lastRowIndex >= 0)
                    {
                        dgvPrisoners.Rows[lastRowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                        dgvPrisoners.Rows[lastRowIndex].DefaultCellStyle.Font = new Font(dgvPrisoners.Font, FontStyle.Bold);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error searching prisoners: " + ex.Message);
                }
            }
        }

        // ميثود لإضافة فلتر بشكل عام
        private void ApplyFilter(ref string query, SqlCommand cmd, string columnName, string paramName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                query += $" AND {columnName} LIKE {paramName}";
                cmd.Parameters.AddWithValue(paramName, "%" + value + "%");
            }
        }


        private int currentPageIndex = 0;
        private List<DataGridViewColumn> columnsToPrint;

        private Dictionary<string, string> columnHeaderMappings = new Dictionary<string, string>
{
    { "FullName", "الاسم" },
    { "ReservationNumber", "رقم الحجز" },
    { "CaseID", "رقم القضية" },
      { "diseasestatus", "الحاله المرضيه" },
    { "DangerousLevel", "درجة الخطورة" },
    { "PrisonerStatus", "الحالة" },
    { "Accused", "التهمه" },
    { "PrinciplesType", "مبدأ الحبس" },
     {"NextSession","الجلسه القادمه" },
    { "ServiceTime", "مده الحكم" },
    { "HospitalDate", "تاريخ المستشفى" },
    { "LeaveDate", "تاريخ الخروج" },
    { "NIDNumber", "رقم الهوية" },
    { "CriminalRecord", "الفيش الجنائي" },
    { "ImprisonmentDetails", "نماذج الحبس" },
    { "SecurityRevealed", "كشف أمن عام" },
    { "CensorshipInfo", "خطاب الرقابة" },
    { "Notes", "ملاحظات" },
     {"DepositPlace" , "مكان الايداع" },

    { "CreatedDate", "تاريخ الإنشاء" },
    { "LastModified", "آخر تعديل" },
    { "CreatedBy", "تم الإنشاء بواسطة" },
    { "ModifiedBy", "تم التعديل بواسطة" },

};



        private void PrintButton_Click(object sender, EventArgs e)
        {
            currentPageIndex = 0;
            columnsToPrint = dgvPrisoners.Columns.Cast<DataGridViewColumn>()
        .Where(col => col.Visible && col.Name != "PrisonerID").ToList();  // Exclude PrisonerID column from printing

            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            // Set landscape mode
            printDocument.DefaultPageSettings.Landscape = true;

            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
        private int currentRow = 0;

        private void PrintDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            currentRow = 0;
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font headerFont = new Font("Tahoma", 16, FontStyle.Bold);
            Font subHeaderFont = new Font("Tahoma", 12, FontStyle.Regular);
            Font cellFont = new Font("Tahoma", 9, FontStyle.Regular);

            int leftMargin = e.MarginBounds.Left;
            int topMargin = e.MarginBounds.Top;
            int rightMargin = e.MarginBounds.Right;

            // 🟢 هيدر رئيسي
            string headerTitle = "نظام إدارة السجناء (SPS)";
            e.Graphics.DrawString(headerTitle, headerFont, Brushes.Black,
                (e.PageBounds.Width - e.Graphics.MeasureString(headerTitle, headerFont).Width) / 2,
                topMargin - 80);

            // 🟢 العنوان مع الشهر اللي اختاره المستخدم
            string monthText = $"تقرير يومي - {(DateTime.Now).Date.ToShortDateString()}";
            e.Graphics.DrawString(monthText, subHeaderFont, Brushes.Black,
                (e.PageBounds.Width - e.Graphics.MeasureString(monthText, subHeaderFont).Width) / 2,
                topMargin - 50);

            e.Graphics.DrawLine(Pens.Black, leftMargin, topMargin - 10, rightMargin, topMargin - 10);

            // 🟢 تحديد عرض الأعمدة ديناميكياً بحيث الكل يتوزع
            int totalWidth = rightMargin - leftMargin;
            int columnCount = columnsToPrint.Count;
            int columnWidth = totalWidth / columnCount;
            int cellHeight = 30;

            int startY = topMargin;
            int startX;

            // 🟢 رأس الجدول (أسماء الأعمدة)
            startX = leftMargin;
            foreach (var col in columnsToPrint)
            {
                string headerText = columnHeaderMappings.ContainsKey(col.Name) ? columnHeaderMappings[col.Name] : col.HeaderText;

                Rectangle rect = new Rectangle(startX, startY, columnWidth, cellHeight);
                e.Graphics.FillRectangle(Brushes.LightGray, rect);
                e.Graphics.DrawRectangle(Pens.Black, rect);

                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                e.Graphics.DrawString(headerText, cellFont, Brushes.Black, rect, format);
                startX += columnWidth;
            }

            startY += cellHeight;

            // 🟢 البيانات
            while (currentRow < dgvPrisoners.Rows.Count)
            {
                DataGridViewRow row = dgvPrisoners.Rows[currentRow];
                if (row.IsNewRow)
                {
                    currentRow++;
                    continue;
                }

                startX = leftMargin;
                foreach (var col in columnsToPrint)
                {
                    string value = row.Cells[col.Name].Value?.ToString() ?? "";
                    Rectangle rect = new Rectangle(startX, startY, columnWidth, cellHeight);

                    e.Graphics.DrawRectangle(Pens.Black, rect);

                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    e.Graphics.DrawString(value, cellFont, Brushes.Black, rect, format);
                    startX += columnWidth;
                }

                startY += cellHeight;
                currentRow++;

                // 🛑 لو الصفحة خلصت
                if (startY + cellHeight > e.MarginBounds.Bottom - 100)
                {
                    e.HasMorePages = true;
                    return;
                }
            }




            // 🟢 الفوتر
            string footer = $"تمت الطباعة بواسطة SPS - {_username}  |   التاريخ: {DateTime.Now:yyyy/MM/dd}";
            e.Graphics.DrawString(footer, cellFont, Brushes.Gray,
                (e.PageBounds.Width - e.Graphics.MeasureString(footer, cellFont).Width) / 2,
                e.MarginBounds.Bottom + 40);

            e.HasMorePages = false;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Home homeform = new Home(_username);

            homeform.ShowDialog();
            this.Close();
        }

        private void cmbDepositPlaces_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}