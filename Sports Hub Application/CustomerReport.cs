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
    }
}
