using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace Mixed_Gym_Application
{
    public partial class UserUpdate : Form
    {
        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;
        private string _username;
        private string ConnectionString;
        private BindingSource bindingSource;

        public UserUpdate(string username)
        {

            InitializeComponent();

            _username = username;
            ConnectionString = DatabaseConfig.connectionString;

            // Initialize controls only after form is fully initialized
            InitializeControls();

            // Store initial size and location of all controls after the form is loaded
            _initialFormWidth = this.Width;
            _initialFormHeight = this.Height;

            _controlsInfo = new ControlInfo[this.Controls.Count];
            for (int i = 0; i < this.Controls.Count; i++)
            {
                Control c = this.Controls[i];
                _controlsInfo[i] = new ControlInfo(c.Left, c.Top, c.Width, c.Height, c.Font.Size);
            }

            // Set event handler for form resize
            this.Resize += Home_Resize;
            usersDataGridView.CellDoubleClick += usersDataGridView_CellDoubleClick;
            nametxt.Leave += nametxt_Leave;
            ConfigureNameAutoComplete();
            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.Name = "deleteColumn";
            deleteButtonColumn.HeaderText = "";
            deleteButtonColumn.Text = "مسح";
            deleteButtonColumn.UseColumnTextForButtonValue = true;
            usersDataGridView.Columns.Insert(0, deleteButtonColumn);






        }
        private void usersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {  // Handle cell content click event here
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = usersDataGridView[e.ColumnIndex, e.RowIndex];

            }

            if (e.RowIndex >= 0 && usersDataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                // Ask for confirmation
                DialogResult result = MessageBox.Show("Are you sure you want to delete this user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Remove the row from the DataGridView
                    usersDataGridView.Rows.RemoveAt(e.RowIndex);

                    // Save changes to the database
                    UpdateData();
                }
            }
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

        private void InitializeControls()
        {
            // Initialize BindingSource
            bindingSource = new BindingSource();

            if (usersDataGridView == null)
            {
                usersDataGridView = new DataGridView();
                usersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                usersDataGridView.Height = 400;
                usersDataGridView.Top = 10;
                usersDataGridView.Left = 10;
                usersDataGridView.Width = this.ClientSize.Width - 20;
                usersDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                usersDataGridView.DataSource = bindingSource;
                this.Controls.Add(usersDataGridView);
            }
        }


        // Other methods remain unchanged


        private void ConfigureNameAutoComplete()
        {
            AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();
            string query = "SELECT Name FROM Users";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = reader.GetString(0);
                                // Add both normalized and reverse-normalized versions to the collection
                                autoCompleteCollection.Add(NormalizeArabicText(name));
                                autoCompleteCollection.Add(ReverseNormalizeArabicText(name));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }

            nametxt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            nametxt.AutoCompleteSource = AutoCompleteSource.CustomSource;
            nametxt.AutoCompleteCustomSource = autoCompleteCollection;

            nametxt.KeyDown += Nametxt_KeyDown;
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
              
                .Replace('ة', 'ه')  // Normalize 'ة' to 'ه'
                .Replace('ؤ', 'و'); // Normalize 'ؤ' to 'و'
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
            
                .Replace('ه', 'ة')  // Reverse normalize 'ه' to 'ة'
                .Replace('و', 'ؤ'); // Reverse normalize 'و' to 'ؤ'
        }








        private async Task SearchUserByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                
                return;
            }

            // Normalize the input name
            string normalizedInput = NormalizeArabicText(name);
            Debug.WriteLine($"Normalized Input: {normalizedInput}");

            string query = @"
    SELECT ID, Mobilenumber, Name, Category, ProfileImage 
    FROM Users 
    WHERE dbo.NormalizeArabicText(Name) LIKE '%' + dbo.NormalizeArabicText(@Name) + '%'
    ORDER BY DateUpdated DESC";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
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
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()) ;
                        
                           
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







        private async void Nametxt_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the Enter key was pressed
            if (e.KeyCode == Keys.Enter)
            {
                // Avoid processing when the suggestions are not visible
                if (nametxt.AutoCompleteMode != AutoCompleteMode.None)
                {
                    e.SuppressKeyPress = true; // Prevent the default behavior of the Enter key
                    await SearchUserByNameAsync(nametxt.Text);
                }
            }
        }






        private void loadbtn_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void updatebtn_Click_1(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void LoadData(string nameFilter = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = "SELECT TOP 100 * FROM Users";

                    if (!string.IsNullOrEmpty(nameFilter))
                    {
                        // Normalize the search filter
                        string normalizedFilter = NormalizeArabicText(nameFilter);
                        query += " WHERE Name LIKE @NameFilter";
                    }

                    query += " ORDER BY DateUpdated DESC";

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                    if (!string.IsNullOrEmpty(nameFilter))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@NameFilter", "%" + NormalizeArabicText(nameFilter) + "%");
                    }

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    if (usersDataGridView != null)
                    {
                        bindingSource.DataSource = dataTable;
                        usersDataGridView.DataSource = bindingSource;

                        // Set specific columns to read-only
                        usersDataGridView.Columns["UserID"].ReadOnly = true;
                        usersDataGridView.Columns["DateUpdated"].ReadOnly = true;

                        // Auto size columns to fit data
                        usersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        usersDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
                    }
                }
                MessageBox.Show("Data loaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data: " + ex.Message);
            }
        }





        private void UpdateData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT  * From Users", connection);
                    SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                    DataTable dataTable = (DataTable)bindingSource?.DataSource;

                    if (dataTable != null)
                    {
                        dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                        dataAdapter.Update(dataTable);
                    }
                }
                MessageBox.Show("Data updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating data: " + ex.Message);
            }
        }


        private void usersDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Check if the double-clicked column is ProfileImage
                if (usersDataGridView.Columns[e.ColumnIndex].Name == "ProfileImage")
                {
                    // Get the image data from the cell
                    object cellValue = usersDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    if (cellValue != null && cellValue is byte[] imageData && imageData.Length > 0)
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                Image image = Image.FromStream(ms);

                                using (Form imageForm = new Form())
                                {
                                    PictureBox pictureBox = new PictureBox
                                    {
                                        Image = image,
                                        Dock = DockStyle.Fill,
                                        SizeMode = PictureBoxSizeMode.Zoom
                                    };
                                    imageForm.Controls.Add(pictureBox);
                                    imageForm.Width = 800;  // Adjust size as needed
                                    imageForm.Height = 600; // Adjust size as needed
                                    imageForm.Text = "Profile Image";
                                    imageForm.ShowDialog();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred while displaying the image: " + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No image data available or image data is invalid.");
                    }
                }
            }
        }




        private void Loginform_Load(object sender, EventArgs e)
        {




            // Ensure that the DataGridView and buttons are correctly wired up if not done in the designer
            if (usersDataGridView != null)
            {
                usersDataGridView.CellContentClick += usersDataGridView_CellContentClick;



            }
            else
            {
                MessageBox.Show("DataGridView not found. Ensure it is placed correctly in the form.");
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Home home = new Home(_username);
            home.ShowDialog();
            this.Close();
        }






        private void updatetransbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            UpdateTransaction trans = new UpdateTransaction(_username);
            trans.ShowDialog();
            this.Close();
        }

        private void nametxt_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void nametxt_Leave(object sender, EventArgs e)
        {
            LoadData(nametxt.Text);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
