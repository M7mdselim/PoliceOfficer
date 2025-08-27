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

        private int currentPage = 0; // Track the current page number
        private int rowsPerPage; // Number of rows per page
        private int totalRows; // Total number of rows

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
            Font headerFont = new Font(prisonerinfoprisonersgridview.Font, FontStyle.Bold);






            // Draw header items with spacing

            e.Graphics.DrawString($" {nidtxt.Text} : رقم القومي", headerFont, Brushes.Black, new PointF(x, y));
            e.Graphics.DrawString($" {dangerousleveltxt.Text} : درجه الخطوره", headerFont, Brushes.Black, new PointF(x + headerItemSpacing, y));
            e.Graphics.DrawString($"{statustxt.Text} : الحاله", headerFont, Brushes.Black, new PointF(x + 2 * headerItemSpacing, y));
            e.Graphics.DrawString($"{fullnametxt.Text} :  الاسم", headerFont, Brushes.Black, new PointF(x + 3 * headerItemSpacing, y));




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

                RectangleF rect = new RectangleF(x, y, colWidth, prisonerinfoprisonersgridview.RowTemplate.Height);
                string columnHeaderText = columnHeaderMappings.ContainsKey(column.Name) ? columnHeaderMappings[column.Name] : column.HeaderText;
                e.Graphics.DrawString(columnHeaderText, prisonerinfoprisonersgridview.Font, Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                x += colWidth + columnSpacing;
                remainingWidth -= (colWidth + columnSpacing);
                columnsPrinted++;
            }

            y += prisonerinfoprisonersgridview.RowTemplate.Height + rowSpacing;  // Add space after header
            x = e.MarginBounds.Left;

            // Print rows
            foreach (DataGridViewRow row in prisonerinfoprisonersgridview.Rows)
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

                    RectangleF rect = new RectangleF(x, y, cellWidth, prisonerinfoprisonersgridview.RowTemplate.Height);
                    e.Graphics.DrawString(cell.Value?.ToString(), prisonerinfoprisonersgridview.Font, Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    x += cellWidth + columnSpacing;
                    remainingWidth -= (cellWidth + columnSpacing);
                }

                y += prisonerinfoprisonersgridview.RowTemplate.Height + rowSpacing;  // Add space after each row
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
            
        }
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ResetForm();
        }
    }
}
