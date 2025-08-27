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

namespace Mixed_Gym_Application
{
    public partial class DailyReport : Form
    {
        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;

        private string _username;


       

       

        public DailyReport(string username)
        {



            InitializeComponent();
            // Store initial form size
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
            this.Resize += DailyReport_Resize;
            _username = username;
        }

        private async void loadReportButton_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = datePicker.Value.Date;
            await LoadTransactionsAsync(selectedDate);
        }
        private async Task LoadTransactionsAsync(DateTime? date)
        {
            string query = @"
        SELECT 
            P.PrisonerID,
            P.FullName,
            P.ReservationNumber,
            P.CaseID,
            P.DangerousLevel,
            P.PrisonerStatus,
            P.Accused,
            P.PrinciplesType,
            P.ServiceTime,
            P.HospitalDate,
            P.LeaveDate,
            P.NIDNumber,
            P.CriminalRecord,
            P.ImprisonmentDetails,
            P.SecurityRevealed,
            P.CensorshipInfo,
            P.Notes,
            P.CreatedDate,
            P.LastModified,
            P.CreatedBy,
            P.ModifiedBy
        FROM 
            vw_PrisonerReport P
        WHERE 
            (@CreatedDate IS NULL OR CAST(P.CreatedDate AS DATE) = @CreatedDate)
            

        UNION ALL

        SELECT
            NULL AS PrisonerID,
            'Total Abused' AS FullName,       
            CAST(COUNT(*) AS NVARCHAR(10)) AS ReservationNumber, 
            NULL AS CaseID,
            NULL AS DangerousLevel,
            NULL AS PrisonerStatus,
            NULL AS Accused,
            NULL AS PrinciplesType,
            NULL AS ServiceTime,
            NULL AS HospitalDate,
            NULL AS LeaveDate,
            NULL AS NIDNumber,
            NULL AS CriminalRecord,
            NULL AS ImprisonmentDetails,
            NULL AS SecurityRevealed,
            NULL AS CensorshipInfo,
            NULL AS Notes,
            NULL AS CreatedDate,
            NULL AS LastModified,
            NULL AS CreatedBy,
            NULL AS ModifiedBy
        FROM 
            vw_PrisonerReport P
        WHERE 
            (@CreatedDate IS NULL OR CAST(P.CreatedDate AS DATE) = @CreatedDate)
            
    ";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CreatedDate", (object)date ?? DBNull.Value);


                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            transactionsGridView.DataSource = dataTable;

                            // ✅ Customize headers in Arabic
                            transactionsGridView.Columns["FullName"].HeaderText = "الاسم";
                            transactionsGridView.Columns["ReservationNumber"].HeaderText = "رقم الحجز";
                            transactionsGridView.Columns["CaseID"].HeaderText = "رقم القضية";
                            transactionsGridView.Columns["DangerousLevel"].HeaderText = "درجة الخطورة";
                            transactionsGridView.Columns["PrisonerStatus"].HeaderText = "الحالة";
                            transactionsGridView.Columns["Accused"].HeaderText = "التهمه";
                            transactionsGridView.Columns["PrinciplesType"].HeaderText = "مبدأ الحبس";
                            transactionsGridView.Columns["ServiceTime"].HeaderText = "مده الحكم";
                            transactionsGridView.Columns["HospitalDate"].HeaderText = "تاريخ المستشفى";
                            transactionsGridView.Columns["LeaveDate"].HeaderText = "تاريخ الخروج";
                            transactionsGridView.Columns["NIDNumber"].HeaderText = "رقم الهوية";

                            // ✅ New fields
                            transactionsGridView.Columns["CriminalRecord"].HeaderText = "الفيش الجنائي";
                            transactionsGridView.Columns["ImprisonmentDetails"].HeaderText = "نماذج الحبس";
                            transactionsGridView.Columns["SecurityRevealed"].HeaderText = "كشف أمن عام";
                            transactionsGridView.Columns["CensorshipInfo"].HeaderText = "خطاب الرقابة";
                            transactionsGridView.Columns["Notes"].HeaderText = "ملاحظات";

                            transactionsGridView.Columns["CreatedDate"].HeaderText = "تاريخ الإنشاء";
                            transactionsGridView.Columns["LastModified"].HeaderText = "آخر تعديل";
                            transactionsGridView.Columns["CreatedBy"].HeaderText = "تم الإنشاء بواسطة";
                            transactionsGridView.Columns["ModifiedBy"].HeaderText = "تم التعديل بواسطة";

                            // Hide internal ID
                            transactionsGridView.Columns["PrisonerID"].Visible = false;

                            // Arabic-friendly font
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
            // Additional initialization if needed
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
    { "DangerousLevel", "درجة الخطورة" },
    { "PrisonerStatus", "الحالة" },
    { "Accused", "التهمه" },
    { "PrinciplesType", "مبدأ الحبس" },
    { "ServiceTime", "مده الحكم" },
    { "HospitalDate", "تاريخ المستشفى" },
    { "LeaveDate", "تاريخ الخروج" },
    { "NIDNumber", "رقم الهوية" },
    { "CriminalRecord", "الفيش الجنائي" },
    { "ImprisonmentDetails", "نماذج الحبس" },
    { "SecurityRevealed", "كشف أمن عام" },
    { "CensorshipInfo", "خطاب الرقابة" },
    { "Notes", "ملاحظات" },
    { "CreatedDate", "تاريخ الإنشاء" },
    { "LastModified", "آخر تعديل" },
    { "CreatedBy", "تم الإنشاء بواسطة" },
    { "ModifiedBy", "تم التعديل بواسطة" }
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
        private int currentPage = 0; // Track the current page number
        private int rowsPerPage; // Number of rows per page
        private int totalRows; // Total number of rows
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Calculate scale factor for fitting content to page width
            int totalWidth = columnsToPrint.Sum(col => col.Width);
            int printableWidth = e.MarginBounds.Width;
            float scaleFactor = (float)printableWidth / totalWidth;

            // Calculate rows per page
            rowsPerPage = (int)((e.MarginBounds.Height - e.MarginBounds.Top) / (transactionsGridView.RowTemplate.Height + 5)); // Adjust spacing as needed

            // Print header with title and date on each page
            string headerText = "تقرير يومي";
            string reportDateText = $"التاريخ: {datePicker.Value.Date.ToShortDateString()}";

            // Adjust the y position to decrease space above the header
            float y = e.MarginBounds.Top - 30; // Start closer to the top of the page
            float x = e.MarginBounds.Left;

            // Define font sizes
            Font headerFont = new Font(transactionsGridView.Font.FontFamily, 14, FontStyle.Bold);
            Font dateFont = new Font(transactionsGridView.Font.FontFamily, 12, FontStyle.Regular);

            // Measure the width of the header and date texts
            SizeF headerSize = e.Graphics.MeasureString(headerText, headerFont);
            SizeF dateSize = e.Graphics.MeasureString(reportDateText, dateFont);

            // Set x positions for right-aligned text
            float headerX = e.MarginBounds.Right - headerSize.Width;
            float dateX = e.MarginBounds.Right - dateSize.Width;

            // Print the header text and date
            e.Graphics.DrawString(headerText, headerFont, Brushes.Black, new PointF(headerX, y));
            e.Graphics.DrawString(reportDateText, dateFont, Brushes.Black, new PointF(dateX, y + headerSize.Height + 5)); // Add space between header and date

            // Add less additional space between date and content
            y += (int)headerSize.Height + (int)dateSize.Height + 30; // Reduce the space as needed

            if (totalWidth > printableWidth)
            {
                scaleFactor = (float)printableWidth / totalWidth;
            }

            int remainingWidth = printableWidth;
            int columnsPrinted = 0;

            // Print column headers
            foreach (var column in columnsToPrint)
            {
                int columnWidth = (int)(column.Width * scaleFactor);
                if (remainingWidth < columnWidth)
                {
                    break;
                }

                RectangleF rect = new RectangleF(x, y, columnWidth, transactionsGridView.RowTemplate.Height);
                string headerColumnText = columnHeaderMappings.ContainsKey(column.Name) ? columnHeaderMappings[column.Name] : column.HeaderText;
                e.Graphics.DrawString(headerColumnText, transactionsGridView.Font, Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                x += columnWidth;
                remainingWidth -= columnWidth;
                columnsPrinted++;
            }

            y += 25 + 5; // Move down for rows, adjust spacing as needed
            x = e.MarginBounds.Left;

            // Calculate total rows if not already done
            if (totalRows == 0)
            {
                totalRows = transactionsGridView.Rows.Count;
            }

            // Track rows printed on current page
            int rowsPrinted = 0;

            // Print rows
            for (int i = currentPage * rowsPerPage; i < totalRows; i++)
            {
                if (transactionsGridView.Rows[i].IsNewRow) continue;

                x = e.MarginBounds.Left;
                foreach (var cell in transactionsGridView.Rows[i].Cells.Cast<DataGridViewCell>().Where(c => c.OwningColumn.Name != "UserID"))
                {
                    int cellWidth = (int)(cell.OwningColumn.Width * scaleFactor);
                    RectangleF rect = new RectangleF(x, y, cellWidth, transactionsGridView.RowTemplate.Height);
                    e.Graphics.DrawString(cell.Value?.ToString(), transactionsGridView.Font, Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    x += cellWidth;
                }

                y += transactionsGridView.RowTemplate.Height + 5; // Move down for the next row
                rowsPrinted++;

                // Check if we need to create a new page
                if (rowsPrinted >= rowsPerPage) // If printed rows exceed the number of rows per page
                {
                    currentPage++; // Increment page number
                    e.HasMorePages = true;
                    return; // Exit method to trigger the next page
                }
            }

            // If we've finished printing all rows, reset for the next print job
            e.HasMorePages = false;
            currentPage = 0; // Reset page number for the next print job
            totalRows = 0; // Reset total rows
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
    }
}
    

