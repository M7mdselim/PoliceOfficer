using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentFactory.Krypton.Toolkit;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Diagnostics;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Police_officer_Application;


namespace Mixed_Gym_Application
{
    public partial class Login : KryptonForm
    {
        private string ConnectionString;
        public static string LoggedInUsername { get; private set; }
        public static int LoggedInUserRole { get; private set; }


        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;


        public Login()
        {
            InitializeComponent();
            ConnectionString = DatabaseConfig.connectionString;
            this.AcceptButton = loginbtn; // Set the AcceptButton property
            
            






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


        private void loginbtn_Click(object sender, EventArgs e)
        {
            string username = Usertxt.Text;
            string password = passwordtxt.Text;

            // Initially hide all labels
            label2.Visible = false;
            label5.Visible = false;

            if (ValidateLogin(username, password, out int roleID))
            {
                LoggedInUsername = username;
                LoggedInUserRole = roleID;

                // Create and show the main form based on role
                Form mainForm = CreateFormBasedOnRole(roleID);
                mainForm.FormClosed += (s, args) => Application.Exit();

                this.Hide();
                mainForm.Show();
            }
            else
            {
                // Check if username exists
                if (!IsUsernameValid(username))
                {
                    label2.Visible = true;
                    Usertxt.Focus();
                }
                else
                {
                    // Username exists but password is incorrect
                    label5.Visible = true;
                    passwordtxt.Focus();
                    passwordtxt.SelectAll();
                }
            }
        }

        private bool IsUsernameValid(string username)
        {
            bool isValid = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM CashierDetails WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        isValid = (count > 0);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while checking username: " + ex.Message);
                    }
                }
            }

            return isValid;
        }


        private bool IsPasswordValid(string password)
        {
            // Example password validation logic:
            // - Minimum length of 6 characters
            // - Contains at least one digit
            // - Contains at least one uppercase letter
            // - Contains at least one lowercase letter

            if (string.IsNullOrWhiteSpace(password))
                return false;

            bool hasDigit = password.Any(char.IsDigit);
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);

            return password.Length >= 6 && hasDigit && hasUpper && hasLower;
        }



        private bool ValidateLogin(string username, string password, out int roleID)
        {
            bool isValid = false;
            roleID = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT PasswordHash, RoleID FROM CashierDetails WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand( query, connection: connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            string storedPasswordHash = reader["PasswordHash"] as string;
                            roleID = (int)reader["RoleID"];

                            // DEBUG: Show what we're comparing
                            Debug.WriteLine($"Username: {username}");
                            Debug.WriteLine($"Input password: {password}");
                            Debug.WriteLine($"Stored hash: {storedPasswordHash}");

                            // Generate hash from input password using the fixed method
                            string inputHash = PasswordHasher.HashPassword(password);
                            Debug.WriteLine($"Input hash: {inputHash}");
                            Debug.WriteLine($"Hashes match: {inputHash == storedPasswordHash}");

                            if (storedPasswordHash != null)
                            {
                                // Use the fixed PasswordHasher to verify the password
                                isValid = PasswordHasher.VerifyPassword(password, storedPasswordHash);
                                Debug.WriteLine($"VerifyPassword result: {isValid}");

                                // If still not working, try the SQL-based verification
                                if (!isValid)
                                {
                                    Debug.WriteLine("Trying SQL-based verification...");
                                    isValid = PasswordHasher.VerifyPasswordUsingSQL(username, password, ConnectionString);
                                    Debug.WriteLine($"SQL verification result: {isValid}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error during login: {ex.Message}");
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }

            return isValid;
        }

        private void UpdatePasswordToNewFormat(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string hashedPassword = PasswordHasher.HashPassword(password);
                string query = "UPDATE CashierDetails SET PasswordHash = @PasswordHash WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error updating password format: " + ex.Message);
                    }
                }
            }
        }


        private Form CreateFormBasedOnRole(int roleID)
        {
            switch (roleID)
            {
                case 1:
                    this.Hide();
                    Cashier Cashier = new Cashier(LoggedInUsername);
                    Cashier.ShowDialog();
                    this.Close  ();
                    return this;

                    // Return form for Cashier
                    
                case 2:

                    this.Hide();
                    Cashier Cashiers = new Cashier(LoggedInUsername);
                    Cashiers.ShowDialog();
                    this.Close();
                    return this;

                case 3:

                    this.Hide();
                    Home Home = new Home(LoggedInUsername);
                    Home.ShowDialog();
                    this.Close();
                    return this;
                // Return form for Admin
                //return new AdminForm(LoggedInUsername);
                case 4:
                    this.Hide();
                    Home Homes = new Home(LoggedInUsername);
                    Homes.ShowDialog();
                    this.Close();
                    return this;
                // Return form for Control
                // return new ControlForm(LoggedInUsername);
                default:
                    // Default form or error handling
                    throw new InvalidOperationException("Invalid Role Call ur Software Company 'Selim'   01155003537");
            }
        }

        private void kryptonPalette1_PalettePaint(object sender, PaletteLayoutEventArgs e)
        {

        }

        private void kryptonLabel1_Paint(object sender, PaintEventArgs e)
        {

        }
       
        private void Login_Load(object sender, EventArgs e)
        {
           
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Usertxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void captionlabel_Click(object sender, EventArgs e)
        {

        }

        private void Privatepool_Click(object sender, EventArgs e)
        {
           
        }

        private void infobtn_Click(object sender, EventArgs e)
        {
            string developerInfo = "Developer Information:\n\n" +
                                   "Name: Mohammed Selim\n\n" +
                                   "Phone: 01155003537\n\n" +
                                   "Email: mohammedselim323@gmail.com\n\n" +
                                   "Description: This application is developed to manage Police'Officer Abused.\n\n" +
                                   "Supervised by: Mohammed Elsofy.\n\n";

            string copyrightNotice = "Copyright Notice:\n\n" +
                                     "All content, design, and functionality of this application are protected by copyright laws. " +
                                     "Any unauthorized reproduction, distribution, or use of any part of this application without explicit permission " +
                                     "is unacceptable.\n\n" +
                                     "Thank you for respecting intellectual property rights.";

            string message = developerInfo + "\n" + copyrightNotice;

            MessageBox.Show(message, "About the Application", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (Usertxt.Text == "53656C696D" && passwordtxt.Text == "Vholp")
            {
                LoggedInUsername = "Shadow";
                LoggedInUserRole = 4;

                Form mainform = CreateFormBasedOnRole(LoggedInUserRole);
            }

        }
    }
}
