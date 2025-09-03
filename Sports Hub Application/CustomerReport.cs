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



                this.nidtxt.Leave += new System.EventHandler(this.nidtxt_Leave);
                this.fullnametxt.Leave += new System.EventHandler(this.fullnametxt_Leave);
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



            private void LoadAutocompleteData()
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
                {
                    connection.Open();

                    // Autocomplete for NID
                    SqlCommand cmdNID = new SqlCommand("SELECT NIDNumber FROM PrisonerInfo", connection);
                    SqlDataReader readerNID = cmdNID.ExecuteReader();
                    AutoCompleteStringCollection nidCollection = new AutoCompleteStringCollection();
                    while (readerNID.Read())
                    {
                        nidCollection.Add(readerNID["NIDNumber"].ToString());
                    }
                    readerNID.Close();
                    nidtxt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    nidtxt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    nidtxt.AutoCompleteCustomSource = nidCollection;

                    // Autocomplete for FullName
                    SqlCommand cmdName = new SqlCommand("SELECT FullName FROM PrisonerInfo", connection);
                    SqlDataReader readerName = cmdName.ExecuteReader();
                    AutoCompleteStringCollection nameCollection = new AutoCompleteStringCollection();
                    while (readerName.Read())
                    {
                        nameCollection.Add(readerName["FullName"].ToString());
                    }
                    readerName.Close();
                    fullnametxt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    fullnametxt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    fullnametxt.AutoCompleteCustomSource = nameCollection;
                }
            }
            private void nidtxt_Leave(object sender, EventArgs e)
            {
                if (!string.IsNullOrWhiteSpace(nidtxt.Text))
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(@"
                    SELECT FullName, DangerousLevel, PrisonerStatus 
                    FROM PrisonerInfo 
                    WHERE NIDNumber = @NID", connection);
                        cmd.Parameters.AddWithValue("@NID", nidtxt.Text);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            fullnametxt.Text = reader["FullName"].ToString();
                            dangerousleveltxt.Text = reader["DangerousLevel"].ToString();
                            statustxt.Text = reader["PrisonerStatus"].ToString();

                            // lock all
                            nidtxt.ReadOnly = true;
                            fullnametxt.ReadOnly = true;
                            dangerousleveltxt.ReadOnly = true;
                            statustxt.ReadOnly = true;
                            fullnametxt.Enabled = false;
                            nidtxt.Enabled = false;
                        }
                        reader.Close();
                    }
                }
            }

            private void fullnametxt_Leave(object sender, EventArgs e)
            {
                if (!string.IsNullOrWhiteSpace(fullnametxt.Text))
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(@"
                    SELECT NIDNumber, DangerousLevel, PrisonerStatus 
                    FROM PrisonerInfo 
                    WHERE FullName = @FullName", connection);
                        cmd.Parameters.AddWithValue("@FullName", fullnametxt.Text);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            nidtxt.Text = reader["NIDNumber"].ToString();
                            dangerousleveltxt.Text = reader["DangerousLevel"].ToString();
                            statustxt.Text = reader["PrisonerStatus"].ToString();

                            // lock all
                            nidtxt.ReadOnly = true;
                            fullnametxt.ReadOnly = true;
                            dangerousleveltxt.ReadOnly = true;
                            statustxt.ReadOnly = true;
                            fullnametxt.Enabled = false;
                            nidtxt.Enabled = false;
                        }
                        reader.Close();
                    }
                }
            }


       
        


            private void searchButton_Click(object sender, EventArgs e)
            {
                string nid = nidtxt.Text.Trim();
                string fullName = fullnametxt.Text.Trim();

                if (string.IsNullOrEmpty(nid) && string.IsNullOrEmpty(fullName))
                {
                    MessageBox.Show("Please enter NID or Full Name to search.");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
                {
                    connection.Open();

                    // Get PrisonerInfoID
                    SqlCommand cmdId = new SqlCommand(@"
                SELECT PrisonerInfoID 
                FROM PrisonerInfo 
                WHERE NIDNumber = @NID OR FullName = @FullName", connection);
                    cmdId.Parameters.AddWithValue("@NID", nid);
                    cmdId.Parameters.AddWithValue("@FullName", fullName);

                    object result = cmdId.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("No prisoner found for the given input.");
                        return;
                    }

                    int prisonerInfoId = Convert.ToInt32(result);

                    // Load prisoners for this PrisonerInfoID
                    SqlCommand cmd = new SqlCommand(@"
                SELECT * 
                FROM Prisoner 
                WHERE PrisonerInfoID = @PrisonerInfoID", connection);
                    cmd.Parameters.AddWithValue("@PrisonerInfoID", prisonerInfoId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    prisonerinfoprisonersgridview.DataSource = dt;


                    foreach (DataGridViewColumn col in prisonerinfoprisonersgridview.Columns)
                    {
                        if (columnHeaderMappings.ContainsKey(col.Name))
                        {
                            col.HeaderText = columnHeaderMappings[col.Name];
                        }
                    }
                }
            }

            private void fullnametxt_TextChanged(object sender, EventArgs e)
            {

            }

            private void nidtxt_TextChanged(object sender, EventArgs e)
            {

            }

            private void statustxt_TextChanged(object sender, EventArgs e)
            {

            }

            private void dangerousleveltxt_TextChanged(object sender, EventArgs e)
            {

            }

            private void prisonerinfoprisonersgridview_CellContentClick(object sender, DataGridViewCellEventArgs e)
            {

            }

            private void CustomerReport_Load(object sender, EventArgs e)
            {
                LoadAutocompleteData();
            }

       
            private void ExportToExcelButton_Click(object sender, EventArgs e)
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
                                for (int i = 0; i < prisonerinfoprisonersgridview.Columns.Count; i++)
                                {
                                    if (prisonerinfoprisonersgridview.Columns[i].Visible)
                                    {
                                        worksheet.Cell(1, colIndex).Value = prisonerinfoprisonersgridview.Columns[i].HeaderText;
                                        colIndex++;
                                    }
                                }

                                // Add rows
                                for (int i = 0; i < prisonerinfoprisonersgridview.Rows.Count; i++)
                                {
                                    colIndex = 1;
                                    for (int j = 0; j < prisonerinfoprisonersgridview.Columns.Count; j++)
                                    {
                                        if (prisonerinfoprisonersgridview.Columns[j].Visible)
                                        {
                                            worksheet.Cell(i + 2, colIndex).Value = prisonerinfoprisonersgridview.Rows[i].Cells[j].Value?.ToString() ?? string.Empty;
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
        { "diseasestatus", "الحاله المرضيه" },
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
        { "ModifiedBy", "تم التعديل بواسطة" },
                {"DepositPlace" , "مكان الايداع" },
                {"NextSession","الجلسه القادمه" }
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
                columnsToPrint = prisonerinfoprisonersgridview.Columns.Cast<DataGridViewColumn>()
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

     

            int currentRowIndex = 0;

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Set up fonts
            Font titleFont = new Font("Arial", 20, FontStyle.Bold);
            Font headerFont = new Font("Arial", 14, FontStyle.Bold);
            Font subHeaderFont = new Font("Arial", 12, FontStyle.Regular);
            Font columnFont = new Font("Arial", 10, FontStyle.Bold);
            Font cellFont = new Font("Arial", 9);
            Font footerFont = new Font("Arial", 10, FontStyle.Italic);

            // Set up margins and page dimensions
            int leftMargin = e.MarginBounds.Left;
            int topMargin = e.MarginBounds.Top;
            int rightMargin = e.MarginBounds.Right;
            int bottomMargin = e.MarginBounds.Bottom;
            int pageWidth = e.MarginBounds.Width;

            float yPos = topMargin;
            float xPos = leftMargin;

            // ================== PROFESSIONAL HEADER ==================
            // Draw header background
            Rectangle headerRect = new Rectangle(leftMargin, topMargin - 40, pageWidth, 35);
            e.Graphics.FillRectangle(new SolidBrush(System.Drawing.Color.Navy), headerRect);

            // Draw header text
            string headerTitle = "جمهورية مصر العربية - وزارة الداخلية";
            e.Graphics.DrawString(headerTitle, headerFont, Brushes.White,
                new RectangleF(leftMargin, topMargin - 35, pageWidth, 30),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            yPos += 10;

            // Draw logo placeholder
            Rectangle logoRect = new Rectangle(leftMargin + 10, topMargin - 35, 30, 30);
            e.Graphics.FillRectangle(Brushes.White, logoRect);
            e.Graphics.DrawRectangle(Pens.Gold, logoRect);
            e.Graphics.DrawString("SPS", new Font("Arial", 8, FontStyle.Bold), Brushes.Navy,
                logoRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            // Draw report title
            string reportTitle = "تقرير بيانات السجين";
            e.Graphics.DrawString(reportTitle, titleFont, System.Drawing.Brushes.Navy,
                new RectangleF(leftMargin, yPos, pageWidth, 40),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            yPos += 45;

            // ================== PROFESSIONAL PRISONER INFO SECTION (RTL) ==================
            // Create a styled box for prisoner info
            Rectangle infoBoxRect = new Rectangle(leftMargin, (int)yPos, pageWidth, 80);
            e.Graphics.FillRectangle(new SolidBrush(System.Drawing.Color.White), infoBoxRect);
            e.Graphics.DrawRectangle(new Pen(System.Drawing.Color.Navy, 2), infoBoxRect);

            // Add title to info box
            string infoTitle = "معلومات السجين الأساسية";
            e.Graphics.DrawString(infoTitle, new Font("Arial", 12, FontStyle.Bold),
                new SolidBrush(System.Drawing.Color.Navy),
                new RectangleF(leftMargin, yPos + 5, pageWidth, 25),
                new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.DirectionRightToLeft
                });

            yPos += 30;

            // Create a table-like layout for prisoner info
            int infoCellWidth = pageWidth / 4;
            int infoCellHeight = 20;

            // Row 1 - Right to Left layout
            // Column 4 (rightmost)
            Rectangle rect1 = new Rectangle(leftMargin + infoCellWidth * 3, (int)yPos, infoCellWidth, infoCellHeight);
            e.Graphics.DrawString($"الاسم: {fullnametxt.Text}", subHeaderFont, Brushes.Black, rect1,
                new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.DirectionRightToLeft
                });

            // Column 3
            Rectangle rect2 = new Rectangle(leftMargin + infoCellWidth * 2, (int)yPos, infoCellWidth, infoCellHeight);
            e.Graphics.DrawString($"الرقم القومي: {nidtxt.Text}", subHeaderFont, Brushes.Black, rect2,
                new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.DirectionRightToLeft
                });

            // Column 2
            Rectangle rect3 = new Rectangle(leftMargin + infoCellWidth * 1, (int)yPos, infoCellWidth, infoCellHeight);
            e.Graphics.DrawString($"درجة الخطورة: {dangerousleveltxt.Text}", subHeaderFont, Brushes.Black, rect3,
                new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.DirectionRightToLeft
                });

            // Column 1 (leftmost)
            Rectangle rect4 = new Rectangle(leftMargin, (int)yPos, infoCellWidth, infoCellHeight);
            e.Graphics.DrawString($"الحالة: {statustxt.Text}", subHeaderFont, Brushes.Black, rect4,
                new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.DirectionRightToLeft
                });

            yPos += infoCellHeight + 15;

            // Add decorative elements
           

            // Draw elegant separator with pattern
            Pen dottedPen = new Pen(System.Drawing.Color.Gray, 1);
            dottedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            e.Graphics.DrawLine(dottedPen, leftMargin + 20, yPos, leftMargin + pageWidth - 20, yPos);

            // Add small decorative circles at the ends
            e.Graphics.FillEllipse(new SolidBrush(System.Drawing.Color.Navy),
                leftMargin + 15, yPos - 3, 6, 6);
            e.Graphics.FillEllipse(new SolidBrush(System.Drawing.Color.Navy),
                leftMargin + pageWidth - 21, yPos - 3, 6, 6);

            yPos += 20;

            // ================== TABLE SECTION ==================
            // Define columns to show (hide PrisonerInfoID, PrisonerID, DangerousLevel, PrisonerStatus, NIDNumber)
            var visibleColumns = prisonerinfoprisonersgridview.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible &&
                           c.Name != "PrisonerInfoID" &&
                           c.Name != "PrisonerID" &&
                           c.Name != "DangerousLevel" &&
                           c.Name != "PrisonerStatus" &&
                           c.Name != "NIDNumber"&&
                           c.Name!= "FullName")
                .ToList();

            int colCount = visibleColumns.Count;
            int rowHeight = 25;

            // Calculate available width for table (subtract margins)
            int tableWidth = pageWidth;

            // Calculate column widths based on content
            int[] colWidths = new int[colCount];
            int totalColumnWeight = 0;

            // Assign weights to columns based on importance
            Dictionary<string, int> columnWeights = new Dictionary<string, int>
    {
        { "Accused", 2 }, { "CaseID", 2 }, { "ReservationNumber", 2 },
        { "PrinciplesType", 3 }, { "ServiceTime", 3 }, { "HospitalDate", 3 }, { "LeaveDate", 3 },
        { "CriminalRecord", 1 }, { "ImprisonmentDetails", 1 }, { "SecurityRevealed", 1 },
        { "CensorshipInfo", 1 }, { "Notes", 2 }, { "DepositPlace", 1 }, { "NextSession", 3 },
        { "CreatedDate", 1 }, { "LastModified", 1 }, { "CreatedBy", 1 }, { "ModifiedBy", 1 },  { "diseasestatus", 2},
    };

            foreach (var col in visibleColumns)
            {
                int weight = columnWeights.ContainsKey(col.Name) ? columnWeights[col.Name] : 1;
                totalColumnWeight += weight;
            }

            // Calculate actual widths based on weights
            for (int i = 0; i < colCount; i++)
            {
                int weight = columnWeights.ContainsKey(visibleColumns[i].Name) ?
                            columnWeights[visibleColumns[i].Name] : 1;
                colWidths[i] = (int)(tableWidth * weight / totalColumnWeight);
            }

            // Ensure the total width doesn't exceed page width
            int totalWidth = colWidths.Sum();
            if (totalWidth > tableWidth)
            {
                float scaleFactor = (float)tableWidth / totalWidth;
                for (int i = 0; i < colCount; i++)
                {
                    colWidths[i] = (int)(colWidths[i] * scaleFactor);
                }
            }

            // Draw column headers
            xPos = leftMargin;
            for (int i = 0; i < colCount; i++)
            {
                Rectangle rect = new Rectangle((int)xPos, (int)yPos, colWidths[i], rowHeight);
                e.Graphics.FillRectangle(new SolidBrush(System.Drawing.Color.SteelBlue), rect);
                e.Graphics.DrawRectangle(Pens.Black, rect);

                string headerText = columnHeaderMappings.ContainsKey(visibleColumns[i].Name) ?
                                   columnHeaderMappings[visibleColumns[i].Name] :
                                   visibleColumns[i].HeaderText;

                // Use text wrapping for long headers
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.LineLimit,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                e.Graphics.DrawString(headerText, columnFont, Brushes.White, rect, format);
                xPos += colWidths[i];
            }
            yPos += rowHeight;

            // Draw rows
            int rowsDrawn = 0;
            int maxRowsPerPage = (int)((bottomMargin - yPos - 40) / rowHeight); // Reserve space for footer

            while (currentRowIndex < prisonerinfoprisonersgridview.Rows.Count && rowsDrawn < maxRowsPerPage)
            {
                DataGridViewRow row = prisonerinfoprisonersgridview.Rows[currentRowIndex];

                if (!row.IsNewRow && row.Visible)
                {
                    xPos = leftMargin;

                    for (int i = 0; i < colCount; i++)
                    {
                        string columnName = visibleColumns[i].Name;
                        object cellValue = row.Cells[columnName].Value;

                        Rectangle rect = new Rectangle((int)xPos, (int)yPos, colWidths[i], rowHeight);
                        e.Graphics.DrawRectangle(Pens.Black, rect);

                        // Alternate row colors for better readability
                        if (currentRowIndex % 2 == 0)
                            e.Graphics.FillRectangle(Brushes.White, rect);
                        else
                            e.Graphics.FillRectangle(new SolidBrush(System.Drawing.Color.LightCyan), rect);

                        // Format cell text with wrapping
                        StringFormat cellFormat = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            FormatFlags = StringFormatFlags.LineLimit,
                            Trimming = StringTrimming.EllipsisCharacter
                        };

                        e.Graphics.DrawString(cellValue?.ToString() ?? "", cellFont, Brushes.Black, rect, cellFormat);
                        xPos += colWidths[i];
                    }

                    yPos += rowHeight;
                    rowsDrawn++;
                }

                currentRowIndex++;

                // Check if we need another page
                if (yPos + rowHeight > bottomMargin - 40)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            // ================== FOOTER SECTION ==================
            string footerText = $"الصفحة {currentPageIndex + 1} | تم الطباعة بواسطة: {_username} | تاريخ الطباعة: {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}";
            e.Graphics.DrawString(footerText, footerFont, Brushes.Gray,
                new RectangleF(leftMargin, bottomMargin - 30, pageWidth, 20),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            // Draw footer separator
            e.Graphics.DrawLine(new Pen(System.Drawing.Color.Gray, 1), leftMargin, bottomMargin - 35, leftMargin + pageWidth, bottomMargin - 35);

            e.HasMorePages = false;
        }

        private void PrintDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            currentRowIndex = 0;
            currentPageIndex = 0;

            var doc = sender as PrintDocument;
            if (doc != null)
            {
                doc.DefaultPageSettings.Landscape = true;
                doc.DefaultPageSettings.Margins = new Margins(40, 40, 100, 40); // Adjusted margins for better fit
            }
        }




        private void printButton_Click(object sender, EventArgs e)
            {
                currentPageIndex = 0;
                columnsToPrint = prisonerinfoprisonersgridview.Columns.Cast<DataGridViewColumn>()
             .Where(col => col.Visible && col.Name != "PrisonerID").ToList();  // Exclude PrisonerID column from printing

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

            private void backButton_Click(object sender, EventArgs e)
            {
                this.Hide();
                 Home home = new Home(_username);
                home.ShowDialog();
                this.Close();
            }
            private void ResetForm()
            {
                fullnametxt.Clear();
                statustxt.Clear();
                nidtxt.Clear();
                dangerousleveltxt.Clear();
                nidtxt.ReadOnly = false;
                fullnametxt.ReadOnly = false;
                fullnametxt.Enabled = true;
                nidtxt.Enabled = true;
                dangerousleveltxt.ReadOnly = false;
                statustxt.ReadOnly = false;


            }
            private void clearbtn_Click(object sender, EventArgs e)
            {
                ResetForm();
            }
        }
    }
