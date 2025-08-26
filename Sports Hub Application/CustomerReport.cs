using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Diagnostics;

namespace Mixed_Gym_Application
{
    public partial class CustomerReport : Form
    {


        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;



        private string _username;
        public CustomerReport(string username)
        {
            InitializeComponent();
            LoadAutoCompleteNames();
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


        private async void searchButton_Click(object sender, EventArgs e)
        {
            string customerName = nametxt.Text.Trim();
            if (string.IsNullOrEmpty(customerName))
            {
                MessageBox.Show("Please enter a customer name.");
                return;
            }

            await SearchUserByNameAsync(customerName);
            await LoadCustomerTransactionsAsync(customerName);
        }

        private string NormalizeArabicText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return text
                .Replace('أ', 'ا')  // Normalize 'أ' to 'ا'
                .Replace('إ', 'ا')  // Normalize 'إ' to 'ا'
                .Replace('آ', 'ا')  // Normalize 'آ' to 'ا'
                .Replace('ى', 'ي')  // Normalize 'ى' to 'ي'
                 // Normalize 'ئ' to 'ي'
                  // Normalize 'ة' to 'ه'
               ; // Normalize 'ؤ' to 'و'
        }


        private string ReverseNormalizeArabicText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text; // Return input as is if it's null or empty

            // Perform reverse normalization
            return text
                .Replace('ا', 'أ')  // Reverse normalize 'ا' to 'أ'
                .Replace('ا', 'إ')  // Reverse normalize 'ا' to 'إ'
                .Replace('ا', 'آ')  // Reverse normalize 'ا' to 'آ'
                .Replace('ي', 'ى')  // Reverse normalize 'ي' to 'ى'
                // Reverse normalize 'ي' to 'ئ'
                  // Reverse normalize 'ه' to 'ة'
                ; // Reverse normalize 'و' to 'ؤ'
        }


        private void SetPictureBoxImage(Image newImage)
        {
            if (profileimg.Image != null)
            {
                profileimg.Image.Dispose();
            }
            profileimg.Image = newImage;
        }



        private async Task SearchUserByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ResetUserFields();
                return;
            }

            // Normalize the input name
            string normalizedInput = NormalizeArabicText(name);
            Debug.WriteLine($"Normalized Input: {normalizedInput}");

            string query = @"
SELECT ID, Mobilenumber, Name, Category, ProfileImage 
FROM Users 
WHERE Name LIKE '%' + @Name + '%' OR dbo.NormalizeArabicText(Name) LIKE '%' + dbo.NormalizeArabicText(@Name) + '%'
ORDER BY DateUpdated DESC";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Pass the normalized input to the query
                    command.Parameters.AddWithValue("@Name", normalizedInput);

                    Debug.WriteLine($"Query: {query}");
                    Debug.WriteLine($"Parameter @Name: {normalizedInput}");

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string id = reader["ID"]?.ToString() ?? "N/A";
                                string mobileNumber = reader["Mobilenumber"]?.ToString() ?? "N/A";
                                string names = reader["Name"]?.ToString() ?? "N/A";
                                string category = reader["Category"]?.ToString() ?? "N/A";

                                if (InvokeRequired)
                                {
                                    Invoke(new Action(() =>
                                    {
                                        membershipIDtxt.Text = id;
                                        phonenumbertxt.Text = mobileNumber;
                                        nametxt.Text = names;
                                        Categorytxt.Text = category;
                                    }));
                                }
                                else
                                {
                                    membershipIDtxt.Text = id;
                                    phonenumbertxt.Text = mobileNumber;
                                    nametxt.Text = names;
                                    Categorytxt.Text = category;
                                }

                                byte[] imageData = reader["ProfileImage"] as byte[];
                                if (imageData != null)
                                {
                                    try
                                    {
                                        using (MemoryStream ms = new MemoryStream(imageData))
                                        {
                                            string tempFilePath = Path.GetTempFileName();
                                            File.WriteAllBytes(tempFilePath, ms.ToArray());
                                            SetPictureBoxImage(Image.FromFile(tempFilePath));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Failed to load image: " + ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        SetPictureBoxImage(null);
                                    }
                                }
                                else
                                {
                                    SetPictureBoxImage(null);
                                }


                                // Set fields to read-only
                                membershipIDtxt.ReadOnly = true;

                                Categorytxt.ReadOnly = true;
                                phonenumbertxt.ReadOnly = true;
                                profileimg.Enabled = false;
                            }
                            else
                            {
                                Debug.WriteLine("No user found with the given name.");
                                ResetUserFields();
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("A SQL error occurred: " + ex.Message);
                        Debug.WriteLine($"SQL Exception: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                        Debug.WriteLine($"Exception: {ex.Message}");
                    }
                }
            }
        }



        private void ResetUserFields()
        {
            membershipIDtxt.Clear();
            nametxt.Clear();
            Categorytxt.Clear();
            phonenumbertxt.Clear();
            profileimg.Image = null;

            // Make fields editable again
            membershipIDtxt.ReadOnly = false;
            nametxt.ReadOnly = false;
            Categorytxt.ReadOnly = false;
            phonenumbertxt.ReadOnly = false;
            profileimg.Enabled = true;
        }

        private async Task LoadCustomerTransactionsAsync(string customerName)
        {
            string transactionsQuery = @"
        SELECT 
            T.TransactionID,
            S.SportName,
            CASE 
                WHEN U.Category = 'جيش' THEN S.MilitaryPrice * (1 - (T.DiscountPercentage / 100))
                WHEN U.Category = 'عضو' THEN S.MemberPrice * (1 - (T.DiscountPercentage / 100))
                WHEN U.Category = 'مدني' THEN S.CivilianPrice * (1 - (T.DiscountPercentage / 100))
                WHEN U.Category = 'درجة 1' THEN S.Degree1Price * (1 - (T.DiscountPercentage / 100))
                
            END AS Price,
            T.DateAndTime,
            T.AmountPaid,
            T.RemainingAmount,
            T.DiscountPercentage,
            T.CashierName
        FROM 
            Transactions T
        INNER JOIN 
            Users U ON T.UserID = U.UserID
        INNER JOIN 
            Sports S ON T.SportID = S.SportID
        WHERE 
            U.Name LIKE @CustomerName
    ";

            string totalQuery = @"
        SELECT 
            NULL AS TransactionID,
            'Total' AS SportName,
            NULL AS Price,
            NULL AS DateAndTime,
            SUM(T.AmountPaid) AS AmountPaid,
            SUM(T.RemainingAmount) AS RemainingAmount,
            NULL AS DiscountPercentage,
            NULL AS CashierName
        FROM 
            Transactions T
        INNER JOIN 
            Users U ON T.UserID = U.UserID
        INNER JOIN 
            Sports S ON T.SportID = S.SportID
        WHERE 
            U.Name LIKE @CustomerName
    ";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // Execute the transactions query
                    using (SqlCommand transactionsCommand = new SqlCommand(transactionsQuery, connection))
                    {
                        transactionsCommand.Parameters.AddWithValue("@CustomerName", "%" + customerName + "%");

                        DataTable dataTable = new DataTable();
                        using (SqlDataReader reader = await transactionsCommand.ExecuteReaderAsync())
                        {
                            // Define the schema
                            dataTable.Columns.Add("TransactionID", typeof(string));
                            dataTable.Columns.Add("SportName", typeof(string));
                            dataTable.Columns.Add("Price", typeof(decimal));
                            dataTable.Columns.Add("DateAndTime", typeof(DateTime));
                            dataTable.Columns.Add("AmountPaid", typeof(decimal));
                            dataTable.Columns.Add("RemainingAmount", typeof(decimal));
                            dataTable.Columns.Add("DiscountPercentage", typeof(decimal));
                            dataTable.Columns.Add("CashierName", typeof(string));

                            dataTable.Load(reader);
                        }

                        // Execute the total query
                        using (SqlCommand totalCommand = new SqlCommand(totalQuery, connection))
                        {
                            totalCommand.Parameters.AddWithValue("@CustomerName", "%" + customerName + "%");

                            using (SqlDataReader reader = await totalCommand.ExecuteReaderAsync())
                            {
                                if (reader.HasRows)
                                {
                                    DataTable totalTable = new DataTable();
                                    totalTable.Load(reader);

                                    // Add the total row to the existing dataTable
                                    if (totalTable.Rows.Count > 0)
                                    {
                                        DataRow totalRow = totalTable.Rows[0];
                                        if (totalRow["AmountPaid"] != DBNull.Value || totalRow["RemainingAmount"] != DBNull.Value)
                                        {
                                            // Create a new row for totals and add it to the dataTable
                                            DataRow newRow = dataTable.NewRow();
                                            newRow["TransactionID"] = "Total"; // Set special value for total row
                                            newRow["SportName"] = totalRow["SportName"];
                                            newRow["Price"] = DBNull.Value;
                                            newRow["DateAndTime"] = DBNull.Value;
                                            newRow["AmountPaid"] = totalRow["AmountPaid"];
                                            newRow["RemainingAmount"] = totalRow["RemainingAmount"];
                                            newRow["DiscountPercentage"] = DBNull.Value;
                                            newRow["CashierName"] = DBNull.Value;

                                            dataTable.Rows.Add(newRow);
                                        }
                                    }
                                }
                            }
                        }

                        // Bind the DataTable to the DataGridView
                        transactionsGridView.DataSource = dataTable;

                        // Optionally customize column headers
                        transactionsGridView.Columns["TransactionID"].HeaderText = "Transaction ID";
                        transactionsGridView.Columns["SportName"].HeaderText = "Sport Name";
                        transactionsGridView.Columns["Price"].HeaderText = "Price";
                        transactionsGridView.Columns["DateAndTime"].HeaderText = "Date & Time";
                        transactionsGridView.Columns["AmountPaid"].HeaderText = "Amount Paid";
                        transactionsGridView.Columns["RemainingAmount"].HeaderText = "Remaining Amount";
                        transactionsGridView.Columns["DiscountPercentage"].HeaderText = "Discount %";
                        transactionsGridView.Columns["CashierName"].HeaderText = "Cashier Name";

                        // Format the Price column
                        transactionsGridView.CellFormatting += transactionsGridView_CellFormatting;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading customer transactions: " + ex.Message);
                }
            }
        }

        // Event handler to format the Price column
        private void transactionsGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (transactionsGridView.Columns[e.ColumnIndex].Name == "Price" && e.Value != null && e.Value != DBNull.Value)
            {
                e.Value = string.Format("{0:F2}", e.Value);
                e.FormattingApplied = true;
            }
        }



        private async void LoadAutoCompleteNames()
        {
            string query = "SELECT DISTINCT Name FROM Users"; // Query to fetch names from the database

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
                                string name = reader["Name"].ToString();
                                autoCompleteCollection.Add(name);
                            }
                        }

                        // Configure the nameTextBox for auto-complete
                        nametxt.AutoCompleteCustomSource = autoCompleteCollection;
                        nametxt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        nametxt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while loading names: " + ex.Message);
                    }
                }
            }
        }

        private void exportimgbtn_Click(object sender, EventArgs e)
        {
            // Check if there is an image in the PictureBox
            if (profileimg.Image != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    // Set the default file name and filter
                    saveFileDialog.FileName = "exported_image.png";
                    saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                    saveFileDialog.Title = "Save Image As";

                    // Show the SaveFileDialog and check if the user selected a file
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get the selected file path
                        string filePath = saveFileDialog.FileName;

                        // Save the image in the selected format
                        ImageFormat format = ImageFormat.Png;
                        switch (Path.GetExtension(filePath).ToLower())
                        {
                            case ".jpg":
                            case ".jpeg":
                                format = ImageFormat.Jpeg;
                                break;
                            case ".bmp":
                                format = ImageFormat.Bmp;
                                break;
                        }

                        // Save the image
                        profileimg.Image.Save(filePath, format);
                        MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("No image to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomerReport_Load(object sender, EventArgs e)
        {

        }

        private int currentPageIndex = 0;
        private List<DataGridViewColumn> columnsToPrint;

        private Dictionary<string, string> columnHeaderMappings = new Dictionary<string, string>
{
    { "TransactionID", "ID" },
    { "UserName", "User" },
    { "CheckNumber", "Check" },
    { "SportName", "Sport" },
    { "SportPrice", "Price" },
    { "MobileNumber", "Phone" },
    { "AmountPaid", "Paid" },
    { "RemainingAmount", "Remaining" },
    { "DiscountPercentage", "%" },
    { "DateAndTime", "Date" },
    { "CashierName", "Cashier" },
    { "Notes", "Note" }
};


        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Set the page orientation to landscape
            e.PageSettings.Landscape = true;

            int x = e.MarginBounds.Left;
            int y = e.MarginBounds.Top;
            int rowSpacing = 15;  // Add space between rows
            int columnSpacing = 30;  // Add space between columns
            int headerHeight = 40;  // Height of the header
            int headerItemSpacing = (e.MarginBounds.Width / 5);  // Space between header items
            Font headerFont = new Font(transactionsGridView.Font, FontStyle.Bold);




        

            // Draw header items with spacing

            e.Graphics.DrawString($" {membershipIDtxt.Text} : الاقدميه", headerFont, Brushes.Black, new PointF(x, y));
            e.Graphics.DrawString($" {Categorytxt.Text} : الفئه", headerFont, Brushes.Black, new PointF(x + headerItemSpacing, y));
            e.Graphics.DrawString($"{phonenumbertxt.Text} : التليفون",     headerFont, Brushes.Black, new PointF(x + 2 * headerItemSpacing, y));
            e.Graphics.DrawString($"{nametxt.Text} :  الاسم", headerFont, Brushes.Black, new PointF(x + 3 * headerItemSpacing, y));




            // Adjust y to account for header
            y += headerHeight + rowSpacing;

            int totalWidth = columnsToPrint.Sum(col => col.Width);
            int printableWidth = e.MarginBounds.Width;

            float scaleFactor = 1.0f;
            if (totalWidth > printableWidth)
            {
                scaleFactor = (float)printableWidth / totalWidth;
            }

            int remainingWidth = printableWidth;
            int columnsPrinted = 0;

            // Calculate individual column widths to fit within the printable width
            int numColumns = columnsToPrint.Count;
            int[] columnWidths = new int[numColumns];
            for (int i = 0; i < numColumns; i++)
            {
                columnWidths[i] = (int)(columnsToPrint[i].Width * scaleFactor);
            }

            // Print column headers
            foreach (var column in columnsToPrint.Skip(currentPageIndex))
            {
                int colIndex = columnsToPrint.IndexOf(column);
                int colWidth = columnWidths[colIndex];
                if (remainingWidth < colWidth)
                {
                    break;
                }

                RectangleF rect = new RectangleF(x, y, colWidth, transactionsGridView.RowTemplate.Height);
                string columnHeaderText = columnHeaderMappings.ContainsKey(column.Name) ? columnHeaderMappings[column.Name] : column.HeaderText;
                e.Graphics.DrawString(columnHeaderText, transactionsGridView.Font, Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                x += colWidth + columnSpacing;
                remainingWidth -= (colWidth + columnSpacing);
                columnsPrinted++;
            }

            y += transactionsGridView.RowTemplate.Height + rowSpacing;  // Add space after header
            x = e.MarginBounds.Left;

            // Print rows
            foreach (DataGridViewRow row in transactionsGridView.Rows)
            {
                if (row.IsNewRow) continue;

                remainingWidth = printableWidth;

                foreach (var cell in row.Cells.Cast<DataGridViewCell>().Where(c => c.OwningColumn.Name != "UserID").Skip(currentPageIndex).Take(columnsPrinted))
                {
                    int cellWidth = (int)(cell.OwningColumn.Width * scaleFactor);
                    if (remainingWidth < cellWidth)
                    {
                        break;
                    }

                    RectangleF rect = new RectangleF(x, y, cellWidth, transactionsGridView.RowTemplate.Height);
                    e.Graphics.DrawString(cell.Value?.ToString(), transactionsGridView.Font, Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    x += cellWidth + columnSpacing;
                    remainingWidth -= (cellWidth + columnSpacing);
                }

                y += transactionsGridView.RowTemplate.Height + rowSpacing;  // Add space after each row
                x = e.MarginBounds.Left;

                if (y >= e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    currentPageIndex += columnsPrinted;
                    return;
                }
            }

            e.HasMorePages = false;
            currentPageIndex = 0;
        }




        private void backButton_Click_1(object sender, EventArgs e)
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

        private void transactionsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void printButton_Click_1(object sender, EventArgs e)
        {
            currentPageIndex = 0;
            columnsToPrint = transactionsGridView.Columns.Cast<DataGridViewColumn>()
                .Where(col => col.Visible && col.Name != "UserID").ToList();  // Exclude UserID column from printing

            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;


            printDocument.DefaultPageSettings.Landscape = true;

            // Set wider margins
            printDocument.DefaultPageSettings.Margins = new Margins(100, 100, 100, 100);

            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void CustomerReport_Load_1(object sender, EventArgs e)
        {

        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
           

            nametxt.Clear();
            phonenumbertxt.Clear();
            Categorytxt.Clear();
            profileimg.Image= null;
            membershipIDtxt.Clear();
            transactionsGridView.Columns.Clear();
           
        }

        private void CustomerReport_Load_2(object sender, EventArgs e)
        {

        }

        private void transactionsGridView_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
