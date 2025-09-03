using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;

namespace Mixed_Gym_Application
{
    public partial class SessionForm : Form
    {
        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;

        private string _username;

        private BindingSource bindingSource = new BindingSource();




        public SessionForm(string username)
        {



            InitializeComponent();
            // Store initial form size
            _initialFormWidth = this.Width;
            _initialFormHeight = this.Height;
            transactionsGridView.DataSource = bindingSource;
            // Store initial size and location of all controls
            _controlsInfo = new ControlInfo[this.Controls.Count];
            for (int i = 0; i < this.Controls.Count; i++)
            {
                Control c = this.Controls[i];
                _controlsInfo[i] = new ControlInfo(c.Left, c.Top, c.Width, c.Height, c.Font.Size);
            }

            // Set event handler for form resize
            this.Resize += DailyReport_Resize;
            _username = username;
        }

        private async void loadReportButton_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = datePicker.Value.Date;

            if (columnnamecombobox.SelectedItem != null)
            {
                string selectedArabic = columnnamecombobox.SelectedItem.ToString();
                string columnName = columnMap[selectedArabic];
                await LoadTransactionsAsync(selectedDate, columnName);
            }
            else
            {
                await LoadTransactionsAsync(selectedDate, "CreatedDate");
            }
        }

        private async Task LoadTransactionsAsync(DateTime? date, string columnName = null)
        {
            string query = @"
        SELECT 
            P.PrisonerID,
            P.FullName,
            P.ReservationNumber,
            P.CaseID,
            P.diseasestatus,
            P.DangerousLevel,
            P.PrisonerStatus,
            P.Accused,
            P.PrinciplesType,
            P.NextSession,
            P.ServiceTime,
            P.HospitalDate,
            P.LeaveDate,
            P.NIDNumber,
            P.CriminalRecord,
            P.ImprisonmentDetails,
            P.SecurityRevealed,
            P.CensorshipInfo,
            P.Notes,
            P.DepositPlace,
            P.CreatedDate,
            P.LastModified,
            P.CreatedBy,
            P.ModifiedBy
        FROM vw_PrisonerReport P
        WHERE (@SelectedDate IS NULL OR CAST({0} AS DATE) = @SelectedDate)

        UNION ALL

        SELECT
            NULL AS PrisonerID,
            N'إجمالي السجناء' AS FullName,       
            CAST(COUNT(*) AS NVARCHAR(10)) AS ReservationNumber, 
            NULL AS CaseID,
            NULL as diseasestatus,
            NULL AS DangerousLevel,
            NULL AS PrisonerStatus,
            NULL AS Accused,
            NULL AS PrinciplesType,
            NULL AS NextSession,
            NULL AS ServiceTime,
            NULL AS HospitalDate,
            NULL AS LeaveDate,
            NULL AS NIDNumber,
            NULL AS CriminalRecord,
            NULL AS ImprisonmentDetails,
            NULL AS SecurityRevealed,
            NULL AS CensorshipInfo,
            NULL AS Notes,
            NULL AS DepositPlace,
            NULL AS CreatedDate,
            NULL AS LastModified,
            NULL AS CreatedBy,
            NULL AS ModifiedBy
        FROM vw_PrisonerReport P
        WHERE (@SelectedDate IS NULL OR CAST({0} AS DATE) = @SelectedDate);
    ";

            // ✅ Default to CreatedDate if nothing chosen
            string safeColumn = string.IsNullOrEmpty(columnName) ? "CreatedDate" : columnName;
            query = string.Format(query, safeColumn);

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SelectedDate", (object)date ?? DBNull.Value);

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            transactionsGridView.DataSource = dataTable;

                            // ✅ Arabic headers
                            transactionsGridView.Columns["FullName"].HeaderText = "الاسم";
                            transactionsGridView.Columns["ReservationNumber"].HeaderText = "رقم الحجز";
                            transactionsGridView.Columns["CaseID"].HeaderText = "رقم القضية";
                            transactionsGridView.Columns["DangerousLevel"].HeaderText = "درجة الخطورة";
                            transactionsGridView.Columns["PrisonerStatus"].HeaderText = "الحالة";
                            transactionsGridView.Columns["Accused"].HeaderText = "التهمه";
                            transactionsGridView.Columns["PrinciplesType"].HeaderText = "مبدأ الحبس";
                            transactionsGridView.Columns["NextSession"].HeaderText = "الجلسه القادمه";
                            transactionsGridView.Columns["ServiceTime"].HeaderText = "مده الحكم";
                            transactionsGridView.Columns["HospitalDate"].HeaderText = "تاريخ المستشفى";
                            transactionsGridView.Columns["LeaveDate"].HeaderText = "تاريخ الخروج";
                            transactionsGridView.Columns["NIDNumber"].HeaderText = "رقم الهوية";
                            transactionsGridView.Columns["diseasestatus"].HeaderText = "الحاله المرضيه";
                            transactionsGridView.Columns["CriminalRecord"].HeaderText = "الفيش الجنائي";
                            transactionsGridView.Columns["ImprisonmentDetails"].HeaderText = "نماذج الحبس";
                            transactionsGridView.Columns["SecurityRevealed"].HeaderText = "كشف أمن عام";
                            transactionsGridView.Columns["CensorshipInfo"].HeaderText = "خطاب الرقابة";
                            transactionsGridView.Columns["Notes"].HeaderText = "ملاحظات";
                            transactionsGridView.Columns["DepositPlace"].HeaderText = "مكان الايداع";
                            transactionsGridView.Columns["CreatedDate"].HeaderText = "تاريخ الإنشاء";
                            transactionsGridView.Columns["LastModified"].HeaderText = "آخر تعديل";
                            transactionsGridView.Columns["CreatedBy"].HeaderText = "تم الإنشاء بواسطة";
                            transactionsGridView.Columns["ModifiedBy"].HeaderText = "تم التعديل بواسطة";

                            transactionsGridView.Columns["PrisonerID"].Visible = false;
                            transactionsGridView.DefaultCellStyle.Font = new Font("Tahoma", 10);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("حدث خطأ أثناء تحميل السجناء: " + ex.Message);
                    }
                }
            }
        }







        private async void transactionsGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < transactionsGridView.Rows.Count)
            {
                DataGridViewRow row = transactionsGridView.Rows[e.RowIndex];

                string prisonerId = row.Cells["PrisonerID"].Value?.ToString();
                if (string.IsNullOrEmpty(prisonerId))
                    return; // skip if it's the "إجمالي السجناء" row

                string fullName = row.Cells["FullName"].Value?.ToString();
                string criminalRecord = row.Cells["CriminalRecord"].Value?.ToString();
                string imprisonmentDetails = row.Cells["ImprisonmentDetails"].Value?.ToString();
                string securityRevealed = row.Cells["SecurityRevealed"].Value?.ToString();
                string censorshipInfo = row.Cells["CensorshipInfo"].Value?.ToString();
                string notes = row.Cells["Notes"].Value?.ToString();

                string details =
                    $"👤 الاسم: {fullName}\n" +
                    $"🆔 رقم السجين: {prisonerId}\n\n" +
                    $"📜 الفيش الجنائي:\n{criminalRecord}\n\n" +
                    $"⛓️ نماذج الحبس:\n{imprisonmentDetails}\n\n" +
                    $"🔒 كشف أمن عام:\n{securityRevealed}\n\n" +
                    $"📝 خطاب الرقابة:\n{censorshipInfo}\n\n" +
                    $"📌 ملاحظات:\n{notes}";

                MessageBox.Show(details, "تفاصيل السجين", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //private async Task<Image> GetUserProfileImageAsync(int userId, SqlConnection connection)
        //{
        //    string query = "SELECT ProfileImage FROM Users WHERE UserID = @UserID";

        //    using (SqlCommand command = new SqlCommand(query, connection))
        //    {
        //        command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

        //        try
        //        {
        //            await connection.OpenAsync();
        //            object result = await command.ExecuteScalarAsync();

        //            if (result != DBNull.Value && result != null)
        //            {
        //                byte[] imageData = result as byte[];
        //                if (imageData != null && imageData.Length > 0)
        //                {
        //                    using (MemoryStream ms = new MemoryStream(imageData))
        //                    {
        //                        try
        //                        {
        //                            return Image.FromStream(ms);
        //                        }
        //                        catch (ArgumentException ex)
        //                        {
        //                            MessageBox.Show("Invalid image data: " + ex.Message);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Profile image data is empty.");
        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("An error occurred while retrieving the profile image: " + ex.Message);
        //        }
        //    }

        //    transactionsGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //    transactionsGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        //    return null;
        //}

        private void DailyReport_Load_1(object sender, EventArgs e)
        {
            LoadColumnsToComboBox();// Additional initialization if needed
        }

        private void DailyReport_Resize(object sender, EventArgs e)
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

        private void transactionsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
            columnsToPrint = transactionsGridView.Columns.Cast<DataGridViewColumn>()
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
            string monthText = $"تقرير جلسات - {datePicker.Value.Date.ToShortDateString()}";
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
            while (currentRow < transactionsGridView.Rows.Count)
            {
                DataGridViewRow row = transactionsGridView.Rows[currentRow];
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

        private void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Save as Excel File";
                saveFileDialog.FileName = "DailyReport.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Daily Report");

                            int colIndex = 1; // Column index in Excel starts from 1
                                              // Add column headers
                            for (int i = 0; i < transactionsGridView.Columns.Count; i++)
                            {
                                if (transactionsGridView.Columns[i].Visible)
                                {
                                    worksheet.Cell(1, colIndex).Value = transactionsGridView.Columns[i].HeaderText;
                                    colIndex++;
                                }
                            }

                            // Add rows
                            for (int i = 0; i < transactionsGridView.Rows.Count; i++)
                            {
                                colIndex = 1;
                                for (int j = 0; j < transactionsGridView.Columns.Count; j++)
                                {
                                    if (transactionsGridView.Columns[j].Visible)
                                    {
                                        worksheet.Cell(i + 2, colIndex).Value = transactionsGridView.Rows[i].Cells[j].Value?.ToString() ?? string.Empty;
                                        colIndex++;
                                    }
                                }
                            }

                            // Auto-size columns based on content
                            worksheet.Columns().AdjustToContents();

                            workbook.SaveAs(saveFileDialog.FileName);
                        }

                        MessageBox.Show("Data successfully exported to Excel.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while exporting data to Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CashierReport_Click(object sender, EventArgs e)
        {
            this.Hide();
            CashierDailyReport cashierDailyReport = new CashierDailyReport(_username);
            cashierDailyReport.ShowDialog();
            this.Close();
        }


        private Dictionary<string, string> columnMap = new Dictionary<string, string>
        {
            { "الاسم", "FullName" },
            { "الرقم القومي", "NIDNumber" },
            { "درجة الخطورة", "DangerousLevel" },
            { "الحالة", "PrisonerStatus" },
              { "الحاله المرضيه", "diseasestatus" },
            { "رقم الحجز", "ReservationNumber" },
            { "رقم القضية", "CaseID" },
            { "التهمه", "Accused" },
            {"مكان الايداع" , "DepositPlace" },
             {"الجلسه القادمه","NextSession" },
            { "مبدأ الحبس", "PrinciplesType" },
            { "مده الحكم", "ServiceTime" },
            { "تاريخ المستشفى", "HospitalDate" },
            { "تاريخ الخروج", "LeaveDate" },
            { "الفيش الجنائي", "CriminalRecord" },
            { "نماذج الحبس", "ImprisonmentDetails" },
            { "كشف أمن عام", "SecurityRevealed" },
            { "خطاب الرقابة", "CensorshipInfo" },
            { "ملاحظات", "Notes" },

        };
        private void LoadColumnsToComboBox()
        {
            columnnamecombobox.Items.Clear();

            // ✅ Only date columns
            var dateColumns = new Dictionary<string, string>
    {
        { "تاريخ الإنشاء", "CreatedDate" },
          { "الجلسه القادمه", "NextSession" },
         { "مبدأ الحبس", "PrinciplesType" },
        { "تاريخ كشف الطبي", "HospitalDate" },
        { "موعد الترحيل", "LeaveDate" },
      
    };

            foreach (var col in dateColumns.Keys)
            {
                columnnamecombobox.Items.Add(col);
            }

            // Save for mapping
            columnMap = dateColumns;

            if (columnnamecombobox.Items.Count > 0)
                columnnamecombobox.SelectedIndex = 0;
        }

        private async void searchtxt_TextChanged(object sender, EventArgs e)
        {
            if (columnnamecombobox.SelectedItem == null)
                return;

            string selectedArabic = columnnamecombobox.SelectedItem.ToString();
            string columnName = columnMap[selectedArabic];
          //  string searchValue = searchtxt.Text.Trim();

            DateTime? selectedDate = datePicker.Value.Date;

            // ✅ Reuse the same function with UNION ALL
            await LoadTransactionsAsync(selectedDate, columnName);
        }


        private void columnnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}


