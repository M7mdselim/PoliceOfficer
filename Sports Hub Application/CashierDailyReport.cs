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
    public partial class CashierDailyReport : Form
    {
        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;

        private string _username;

        public CashierDailyReport(string username)
        {



            InitializeComponent();
            // Store initial form size
            _initialFormWidth = this.Width;
            _initialFormHeight = this.Height;

            LoadAutoCompleteNames();

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
            await LoadTransactionsAsync(selectedDate , CashierNametxt.Text);
        }

        private async Task LoadTransactionsAsync(DateTime date, string username)
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
        P.CreatedDate,
        P.LastModified,
        P.CreatedBy,
        P.ModifiedBy
    FROM 
        vw_PrisonerReport P
    WHERE 
        (@CreatedDate IS NULL OR CAST(P.CreatedDate AS DATE) = @CreatedDate)
        AND (@CreatedBy IS NULL OR P.CreatedBy = @CreatedBy)

    UNION ALL

    SELECT
        NULL AS PrisonerID,
        'إجمالي السجناء' AS FullName,       -- just the label
        CAST(COUNT(*) AS NVARCHAR(10)) AS ReservationNumber, -- total count under ReservationNumber
        NULL AS CaseID,
        NULL AS DangerousLevel,
        NULL AS PrisonerStatus,
        NULL AS Accused,
        NULL AS PrinciplesType,
        NULL AS ServiceTime,
        NULL AS HospitalDate,
        NULL AS LeaveDate,
        NULL AS NIDNumber,
        NULL AS CreatedDate,
        NULL AS LastModified,
        NULL AS CreatedBy,
        NULL AS ModifiedBy
    FROM 
        vw_PrisonerReport P
    WHERE 
        (@CreatedDate IS NULL OR CAST(P.CreatedDate AS DATE) = @CreatedDate)
        AND (@CreatedBy IS NULL OR P.CreatedBy = @CreatedBy)
";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CreatedDate", (object)date ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedBy", (object)username ?? DBNull.Value);

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            transactionsGridView.DataSource = dataTable;

                            // Customize headers in Arabic
                            transactionsGridView.Columns["FullName"].HeaderText = "الاسم";
                            transactionsGridView.Columns["ReservationNumber"].HeaderText = "رقم الحجز";
                            transactionsGridView.Columns["CaseID"].HeaderText = "رقم القضية";
                            transactionsGridView.Columns["DangerousLevel"].HeaderText = "درجة الخطورة";
                            transactionsGridView.Columns["PrisonerStatus"].HeaderText = "الحالة";
                            transactionsGridView.Columns["Accused"].HeaderText = "التهمه";
                            transactionsGridView.Columns["PrinciplesType"].HeaderText = "نوع المبادئ";
                            transactionsGridView.Columns["ServiceTime"].HeaderText = "مدة الخدمة";
                            transactionsGridView.Columns["HospitalDate"].HeaderText = "تاريخ المستشفى";
                            transactionsGridView.Columns["LeaveDate"].HeaderText = "تاريخ الخروج";
                            transactionsGridView.Columns["NIDNumber"].HeaderText = "رقم الهوية";
                            transactionsGridView.Columns["CreatedDate"].HeaderText = "تاريخ الإنشاء";
                            transactionsGridView.Columns["LastModified"].HeaderText = "آخر تعديل";
                            transactionsGridView.Columns["CreatedBy"].HeaderText = "تم الإنشاء بواسطة";
                            transactionsGridView.Columns["ModifiedBy"].HeaderText = "تم التعديل بواسطة";

                            // Hide internal IDs
                            transactionsGridView.Columns["PrisonerID"].Visible = false;

                            // Ensure grid supports Arabic display
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
            if (e.RowIndex >= 0)
            {
                // Ensure the UserID column exists and is not empty
                if (transactionsGridView.Rows[e.RowIndex].Cells["UserID"].Value != DBNull.Value)
                {
                    int userId = Convert.ToInt32(transactionsGridView.Rows[e.RowIndex].Cells["UserID"].Value);

                    using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
                    {
                        Image profileImage = await GetUserProfileImageAsync(userId, connection);

                        if (profileImage != null)
                        {
                            // Create a form to display the image
                            Form imageForm = new Form
                            {
                                Width = 400,
                                Height = 400,
                                StartPosition = FormStartPosition.CenterScreen,
                                Text = "User Profile Image"
                            };
                            PictureBox pictureBox = new PictureBox
                            {
                                Dock = DockStyle.Fill,
                                Image = profileImage,
                                SizeMode = PictureBoxSizeMode.Zoom
                            };
                            imageForm.Controls.Add(pictureBox);
                            imageForm.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("No profile image found for this user.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("User ID is missing for this row.");
                }
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

            transactionsGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            transactionsGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            return null;
        }

        private void DailyReport_Load_1(object sender, EventArgs e)
        {
            SettextVisibilityBasedOnRole();

            CashierNametxt.Text = _username;
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
     { "TransactionID", "ID" },
    { "UserName", "الاسم" },
    { "CheckNumber", "رقم الايصال" },
    { "SportName", "النشاط" },
    { "SportPrice", "سعر النشاط" },
    { "Category", "الفئه" },
    { "MobileNumber", "تليفون" },
    { "AmountPaid", "مدفوع" },
    { "RemainingAmount", "متبقي" },
    { "DiscountPercentage", "%" },
    { "DateAndTime", "تاريخ" },
    { "CashierName", "كاشير" },
    { "Notes", "ملحوظه" }
};


        



        private void PrintButton_Click(object sender, EventArgs e)
        {

            columnsToPrint = transactionsGridView.Columns.Cast<DataGridViewColumn>()
                .Where(col => col.Visible && col.Name != "UserID").ToList(); // Exclude UserID column from printing

            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            // Set the default page settings to landscape
            printDocument.DefaultPageSettings.Landscape = true;

            // Set wider margins
            printDocument.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);

            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            int x = e.MarginBounds.Left;
            int y = e.MarginBounds.Top;
            int rowSpacing = 5; // Space between rows
            int rowsPerPage = 20; // Define how many rows per page you want

            int headerHeight = 50; // Height for the header
            string headerText = "Spot Check";
            string reportDateText = $"التاريخ: {datePicker.Value.Date.ToShortDateString()}";

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

            // Add additional space between date and content
            y += (int)headerSize.Height + (int)dateSize.Height + 40; // Increase the space as needed

            int totalWidth = columnsToPrint.Sum(col => col.Width);
            int printableWidth = e.MarginBounds.Width;

            float scaleFactor = 1.0f;
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

            y += transactionsGridView.RowTemplate.Height + rowSpacing; // Add space after header
            x = e.MarginBounds.Left;

            // Print rows
            int rowsPrinted = 0;
            while (rowsPrinted < rowsPerPage && currentPageIndex < transactionsGridView.Rows.Count)
            {
                DataGridViewRow row = transactionsGridView.Rows[currentPageIndex];
                if (row.IsNewRow)
                {
                    currentPageIndex++;
                    continue;
                }

                // Check if the next row would fit on the current page
                if (y + transactionsGridView.RowTemplate.Height + rowSpacing > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                remainingWidth = printableWidth;

                foreach (var cell in row.Cells.Cast<DataGridViewCell>().Where(c => c.OwningColumn.Name != "UserID"))
                {
                    int cellWidth = (int)(cell.OwningColumn.Width * scaleFactor);
                    if (remainingWidth < cellWidth)
                    {
                        break;
                    }

                    RectangleF rect = new RectangleF(x, y, cellWidth, transactionsGridView.RowTemplate.Height);
                    e.Graphics.DrawString(cell.Value?.ToString(), transactionsGridView.Font, Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    x += cellWidth;
                    remainingWidth -= cellWidth;
                }

                y += transactionsGridView.RowTemplate.Height + rowSpacing; // Add space after each row
                x = e.MarginBounds.Left;
                currentPageIndex++;
                rowsPrinted++;
            }

            e.HasMorePages = false;
            currentPageIndex = 0; // Reset for the next print job
        }





        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Cashier cashier = new Cashier(_username);

            cashier.ShowDialog();
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

        private void cashierform_Click(object sender, EventArgs e)
        {

        }


        private void SettextVisibilityBasedOnRole()
        {
            int roleID = GetRoleIdForCurrentUser();

            if (roleID == 1)
            {

                CashierNametxt.ReadOnly = true;
                CashierNametxt.Enabled = false;
                // Hide the button if role ID is 1

               
            }
            else if (roleID == 2)
            {
                CashierNametxt.ReadOnly = false;
                CashierNametxt.Enabled = true;
            }
        }

        private int GetRoleIdForCurrentUser()
        {
            int roleID = 0;
            string username = Login.LoggedInUsername; // Assuming this is how you store the logged-in username

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                string query = @"
               SELECT RoleID
            FROM CashierDetails
            WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out roleID))
                        {
                            return roleID;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }

            return roleID;
        }


        private async void LoadAutoCompleteNames()
        {
            string query = "SELECT DISTINCT Username FROM CashierDetails"; // Query to fetch names from the database

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();

                        AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string name = reader["Username"].ToString();
                                autoCompleteCollection.Add(name);
                            }
                        }

                        // Configure the nameTextBox for auto-complete
                        CashierNametxt.AutoCompleteCustomSource = autoCompleteCollection;
                        CashierNametxt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        CashierNametxt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while loading names: " + ex.Message);
                    }
                }
            }
        }


        private void CashierNametxt_TextChanged(object sender, EventArgs e)
        {
            

        }

        private void titleLabel_Click(object sender, EventArgs e)
        {

        }

        private void backbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            Cashier cashier = new Cashier(_username);

            cashier.ShowDialog();
            this.Close();
        }
    }
}


