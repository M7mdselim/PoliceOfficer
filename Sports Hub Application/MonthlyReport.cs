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
using static System.ComponentModel.Design.ObjectSelectorEditor;
using UglyToad.PdfPig.Core;
using DocumentFormat.OpenXml.Bibliography;

namespace Mixed_Gym_Application
{
    public partial class MonthlyReport : Form
    {



        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;
        private string _username;
        public MonthlyReport(string username)
        {
            InitializeComponent();
            // Set UserID column to be invisible initially
            //transactionsGridView.Columns["UserID"].Visible = false;



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
    

    private async void loadReportButton_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = datePicker.Value.Date;
            await LoadTransactionsAsync(selectedDate);
        }

        private async Task LoadTransactionsAsync(DateTime date, string columnName = null, string searchValue = null)
        {
            string filterCondition = "";
            if (!string.IsNullOrEmpty(searchValue))
            {
                filterCondition = $" AND {columnName} LIKE @SearchValue";
            }

            string query = $@"
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
    YEAR(P.CreatedDate) = @Year 
    AND MONTH(P.CreatedDate) = @Month
    {filterCondition}

UNION ALL

SELECT
    NULL AS PrisonerID,
    N'إجمالي السجناء' AS FullName,        -- Arabic total row
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
    YEAR(P.CreatedDate) = @Year 
    AND MONTH(P.CreatedDate) = @Month
    {filterCondition}
";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Year", date.Year);
                    command.Parameters.AddWithValue("@Month", date.Month);

                    if (!string.IsNullOrEmpty(searchValue))
                        command.Parameters.AddWithValue("@SearchValue", "%" + searchValue + "%");

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            transactionsGridView.DataSource = dataTable;

                            // ✅ Arabic column headers
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
                            transactionsGridView.Columns["NIDNumber"].HeaderText = "الرقم القومي";
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

                            // ✅ Arabic-friendly font
                            transactionsGridView.DefaultCellStyle.Font = new Font("Tahoma", 10);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while loading monthly prisoners: " + ex.Message);
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


        private async Task<Image> GetUserProfileImageAsync(int userId, SqlConnection connection)
        {
            string query = "SELECT ProfileImage FROM Users WHERE UserID = @UserID";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();

                    if (result != DBNull.Value && result != null)
                    {
                        byte[] imageData = result as byte[];
                        if (imageData != null && imageData.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                try
                                {
                                    return Image.FromStream(ms);
                                }
                                catch (ArgumentException ex)
                                {
                                    MessageBox.Show("Invalid image data: " + ex.Message);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Profile image data is empty.");
                        }
                    }
                   
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while retrieving the profile image: " + ex.Message);
                }
            }
            transactionsGridView.Columns["UserID"].Visible = false;
            transactionsGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            transactionsGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            return null;
        }
        private void MonthlyReport_Load(object sender, EventArgs e)
        {
            LoadColumnsToComboBox();// Additional initialization if needed
        }

        private void transactionsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
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

        private Dictionary<string, object> printDataContainer = new Dictionary<string, object>();


        public class PrintHeaderInfo
        {
            public string HeaderText { get; set; }
            public string ReportDateText { get; set; }
            public string MonthText { get; set; }
        }


        private void PrintButton_Click(object sender, EventArgs e)
        {
            currentPageIndex = 0;
            columnsToPrint = transactionsGridView.Columns.Cast<DataGridViewColumn>()
                .Where(col => col.Visible && col.Name != "UserID").ToList();  // Exclude UserID column from printing

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
            string headerTitle = "تقرير شهري";
            string headerDate = datePicker.Value.ToString("MMMM yyyy");  // Format the date as needed
            e.Graphics.DrawString(headerTitle, new Font("Arial", 16, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top);
            e.Graphics.DrawString(headerDate, new Font("Arial", 12), Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top + 30);  // Add some space below the title

            // Move y position below the header
            float y = e.MarginBounds.Top + 60; // Adjust as needed
            float x = e.MarginBounds.Left;

            // Print column headers
            foreach (var column in columnsToPrint)
            {
                int columnWidth = (int)(column.Width * scaleFactor);
                RectangleF rect = new RectangleF(x, y, columnWidth, 40); // Header height
                string headerText = columnHeaderMappings.ContainsKey(column.Name) ? columnHeaderMappings[column.Name] : column.HeaderText;
                e.Graphics.DrawString(headerText, transactionsGridView.Font, Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                x += columnWidth;
            }

            y += 45; // Move down for rows, adjust spacing as needed
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

                float rowHeight = transactionsGridView.RowTemplate.Height + 5;

                // Check if the row fits on the current page
                if (y + rowHeight > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    currentPage++;
                    return;
                }

                x = e.MarginBounds.Left;
                foreach (var cell in transactionsGridView.Rows[i].Cells.Cast<DataGridViewCell>().Where(c => c.OwningColumn.Name != "UserID"))
                {
                    int cellWidth = (int)(cell.OwningColumn.Width * scaleFactor);
                    RectangleF rect = new RectangleF(x, y, cellWidth, transactionsGridView.RowTemplate.Height);
                    e.Graphics.DrawString(cell.Value?.ToString(), transactionsGridView.Font, Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    x += cellWidth;
                }

                y += rowHeight; // Move down for the next row
                rowsPrinted++;
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

        private void ExportToExcelButton_Click_1(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Save as Excel File";
                saveFileDialog.FileName = "MonthlyReport.xlsx";

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

        private async void searchtxt_TextChanged(object sender, EventArgs e)
        {
            if (columnnamecombobox.SelectedItem == null)
                return;

            string selectedArabic = columnnamecombobox.SelectedItem.ToString();
            string columnName = columnMap[selectedArabic];
            string searchValue = searchtxt.Text.Trim();

            DateTime selectedDate = datePicker.Value.Date;

            // ✅ Reuse the same function with UNION ALL
            await LoadTransactionsAsync(selectedDate, columnName, searchValue);
        }


        private void columnnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       
    }
}








