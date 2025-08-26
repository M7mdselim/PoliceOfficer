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
    public partial class UpdateTransaction : Form
    {



        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;
        private string _username;
        private string ConnectionString;
        private BindingSource bindingSource;



        public UpdateTransaction(string username)
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
            usersDataGridView.CellValueChanged += usersDataGridView_CellValueChanged;
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
        {
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

            // Initialize the DataGridView and add it to the form if it is not already added in the designer
            if (usersDataGridView == null)
            {
                usersDataGridView = new DataGridView();
                usersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                usersDataGridView.Height = 400; // Adjust as needed
                usersDataGridView.Top = 10;
                usersDataGridView.Left = 10;
                usersDataGridView.Width = this.ClientSize.Width - 20; // Leave some padding
                usersDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                usersDataGridView.DataSource = bindingSource;
                this.Controls.Add(usersDataGridView);
            }

            // Note: Load and Update buttons should be automatically wired up in the designer
            // Ensure that the buttons are connected to their event handlers
        }

        private void loadbtn_Click(object sender, EventArgs e)
        {
            SearchTransactionsByName(nametxt.Text);
        }

        private void updatebtn_Click(object sender, EventArgs e)
        {
            UpdateData();
        }
        private void SearchTransactionsByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    // If name is null or whitespace, load the last 1000 rows
                    LoadRecentTransactions();
                }
                else
                {
                    string normalizedFilter = NormalizeArabicText(name);

                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        string userQuery = "SELECT UserID FROM Users WHERE Name = @Name";
                        using (SqlCommand userCommand = new SqlCommand(userQuery, connection))
                        {
                            userCommand.Parameters.AddWithValue("@Name", normalizedFilter);
                            connection.Open();

                            List<int> userIds = new List<int>();

                            using (SqlDataReader reader = userCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    userIds.Add(reader.GetInt32(0));
                                }
                            }

                            if (userIds.Count > 0)
                            {
                                // Load transactions for all matching UserIDs
                                LoadData(userIds);
                            }
                            else
                            {
                                MessageBox.Show("User not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching for transactions: " + ex.Message);
            }
        }

        private void LoadRecentTransactions()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    string query = @"
                SELECT TOP 1000 * 
                FROM Transactions 
                ORDER BY DateAndTime DESC"; // or any other relevant sorting column

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        bindingSource.DataSource = dataTable;
                        usersDataGridView.DataSource = bindingSource;

                        // Remove the existing SportName column if it exists
                        if (usersDataGridView.Columns.Contains("SportName"))
                        {
                            usersDataGridView.Columns.Remove("SportName");
                        }

                        // Add the DataGridViewComboBoxColumn for SportName
                        DataGridViewComboBoxColumn sportNameColumn = new DataGridViewComboBoxColumn
                        {
                            Name = "SportName",
                            DataSource = LoadSports(), // Load the sports data
                            DisplayMember = "SportName",
                            ValueMember = "SportID",
                            DataPropertyName = "SportID"
                        };

                        // Insert the column after the UserID column
                        int userIdColumnIndex = usersDataGridView.Columns["UserID"].Index;
                        usersDataGridView.Columns.Add(sportNameColumn);
                        usersDataGridView.Columns["SportName"].DisplayIndex = userIdColumnIndex + 1;

                        usersDataGridView.Columns["TransactionID"].ReadOnly = true;
                        usersDataGridView.Columns["UserID"].ReadOnly = true;
                        usersDataGridView.Columns["DateAndTime"].ReadOnly = true;
                        usersDataGridView.Columns["CashierName"].ReadOnly = true;
                        usersDataGridView.Columns["RemainingAmount"].ReadOnly = true;

                        // Optionally, hide the SportID column if needed
                        if (usersDataGridView.Columns.Contains("SportID"))
                        {
                            usersDataGridView.Columns["SportID"].Visible = false;
                        }
                    }
                }
                MessageBox.Show("Recent transactions loaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading recent transactions: " + ex.Message);
            }
        }




        private void LoadData(List<int> userIds)
        {
            try
            {
                if (userIds == null || userIds.Count == 0)
                {
                    MessageBox.Show("No user IDs provided.");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    // Create a comma-separated string of user IDs
                    string userIdList = string.Join(",", userIds);

                    string query = $"SELECT * FROM Transactions WHERE UserID IN ({userIdList})";

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        bindingSource.DataSource = dataTable;
                        usersDataGridView.DataSource = bindingSource;

                        // Remove the existing SportName column if it exists
                        if (usersDataGridView.Columns.Contains("SportName"))
                        {
                            usersDataGridView.Columns.Remove("SportName");
                        }

                        // Add the DataGridViewComboBoxColumn for SportName
                        DataGridViewComboBoxColumn sportNameColumn = new DataGridViewComboBoxColumn
                        {
                            Name = "SportName",
                            DataSource = LoadSports(), // Load the sports data
                            DisplayMember = "SportName",
                            ValueMember = "SportID",
                            DataPropertyName = "SportID"
                        };

                        // Insert the column after the UserID column
                        int userIdColumnIndex = usersDataGridView.Columns["UserID"].Index;
                        usersDataGridView.Columns.Add(sportNameColumn);
                        usersDataGridView.Columns["SportName"].DisplayIndex = userIdColumnIndex + 1;

                        // Set specific columns to read-only
                        usersDataGridView.Columns["TransactionID"].ReadOnly = true;
                        usersDataGridView.Columns["UserID"].ReadOnly = true;
                        usersDataGridView.Columns["DateAndTime"].ReadOnly = true;
                        usersDataGridView.Columns["CashierName"].ReadOnly = true;
                        usersDataGridView.Columns["RemainingAmount"].ReadOnly = true;

                        // Optionally, hide the SportID column if needed
                        if (usersDataGridView.Columns.Contains("SportID"))
                        {
                            usersDataGridView.Columns["SportID"].Visible = false;
                        }
                    }
                }
                MessageBox.Show("Data loaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading transactions: " + ex.Message);
            }
        }







        private void usersDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the event is triggered for valid row and column indices
            if (e.RowIndex >= 0)
            {
                // Handling SportName changes
                if (e.ColumnIndex == usersDataGridView.Columns["SportName"].Index)
                {
                    // Ensure ComboBox has a valid selection
                    if (sportsComboBox.SelectedValue != null)
                    {
                        int selectedSportID;
                        if (int.TryParse(sportsComboBox.SelectedValue.ToString(), out selectedSportID))
                        {
                            // Update the SportID cell in the current row
                            usersDataGridView.Rows[e.RowIndex].Cells["SportID"].Value = selectedSportID;

                            // Recalculate RemainingAmount after changing SportID
                            RecalculateRemainingAmount(e.RowIndex);
                        }
                        else
                        {
                            MessageBox.Show("Invalid SportID selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                   
                }

                // Handling AmountPaid changes
                if (e.ColumnIndex == usersDataGridView.Columns["AmountPaid"].Index ||
                    e.ColumnIndex == usersDataGridView.Columns["SportName"].Index 
                    || e.ColumnIndex == usersDataGridView.Columns["DiscountPercentage"].Index)
                {
                    // Recalculate RemainingAmount if AmountPaid changes
                    RecalculateRemainingAmount(e.RowIndex);
                }
            }
        }

        private void RecalculateRemainingAmount(int rowIndex)
        {
            try
            {
                // Get the updated value of AmountPaid
                var amountPaidCell = usersDataGridView.Rows[rowIndex].Cells["AmountPaid"];
                decimal amountPaid;
                if (decimal.TryParse(amountPaidCell.Value?.ToString(), out amountPaid))
                {
                    // Get the discount percentage
                    var discountCell = usersDataGridView.Rows[rowIndex].Cells["DiscountPercentage"];
                    decimal discountPercentage;
                    if (decimal.TryParse(discountCell.Value?.ToString(), out discountPercentage))
                    {
                        // Get the SportID
                        var sportIDCell = usersDataGridView.Rows[rowIndex].Cells["SportID"];
                        int sportID;
                        if (int.TryParse(sportIDCell.Value?.ToString(), out sportID))
                        {
                            // Get the UserID
                            var userIDCell = usersDataGridView.Rows[rowIndex].Cells["UserID"];
                            int userID;
                            if (int.TryParse(userIDCell.Value?.ToString(), out userID))
                            {
                                // Retrieve the user's category and sport price based on category
                                string category = GetUserCategory(userID);
                                decimal price = GetSportPriceByCategory(sportID, category);

                                // Calculate the remaining amount
                                decimal discountAmount = (price * discountPercentage) / 100;
                                decimal discountedPrice = (price * 1.14m) - discountAmount;
                                decimal remainingAmount = discountedPrice - amountPaid;

                                // Update the RemainingAmount cell
                                var remainingAmountCell = usersDataGridView.Rows[rowIndex].Cells["RemainingAmount"];
                                remainingAmountCell.Value = remainingAmount;
                            }
                            else
                            {
                                MessageBox.Show("Invalid UserID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid SportID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid discount percentage.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid amount paid value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to get user category from database
        private string GetUserCategory(int userID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Category FROM Users WHERE UserID = @UserID", connection))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
        }

        // Method to get sport price based on category from database
        private decimal GetSportPriceByCategory(int sportID, string category)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT " +
                    "CASE WHEN @Category = 'عضو' THEN MemberPrice " +
                    "WHEN @Category = 'مدني' THEN CivilianPrice " +
                    "WHEN @Category = 'درجة 1' THEN Degree1Price " +
                    "WHEN @Category = 'جيش' THEN MilitaryPrice " +
                    "ELSE NULL END AS Price " +
                    "FROM Sports WHERE SportID = @SportID", connection))
                {
                    cmd.Parameters.AddWithValue("@SportID", sportID);
                    cmd.Parameters.AddWithValue("@Category", category);
                    return (decimal)cmd.ExecuteScalar();
                }
            }
        }




        private decimal GetSportPrice(int sportID)
        {
            // Implement logic to retrieve the sport price based on the sportID
            // This might involve querying the database to get the price for the given sportID
            decimal price = 0;
            // Example code (replace with actual implementation)
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT MemberPrice FROM Sports WHERE SportID = @SportID", connection);
                command.Parameters.AddWithValue("@SportID", sportID);
                connection.Open();
                price = (decimal)command.ExecuteScalar();
            }
            return price;
        }



        private void UpdateData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    // Update the SQL query to select data from the Transactions table
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Transactions", connection);
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
            UserUpdate home = new UserUpdate(_username);
            home.ShowDialog();
            this.Close();
        }







        private DataTable LoadSports()
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT SportID, SportName FROM Sports", connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataTable);
            }
            return dataTable;
        }





        private void updatetransbtn_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


       



        private void sportsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usersDataGridView.CurrentRow != null)
            {
                // Get the selected SportID from ComboBox
                int selectedSportID = (int)sportsComboBox.SelectedValue;

                // Update the SportID cell in the current row
                usersDataGridView.CurrentRow.Cells["SportID"].Value = selectedSportID;
            }
        }

        private void nametxt_TextChanged(object sender, EventArgs e)
        {

        }
        private void nametxt_Leave(object sender, EventArgs e)
        {
            SearchTransactionsByName(nametxt.Text);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }



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

    }



}
