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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace Mixed_Gym_Application
{
    public partial class SignUp : KryptonForm
    {
        private string ConnectionString;
        public static string LoggedInUsername { get; private set; }
        public static int LoggedInUserRole { get; private set; }


        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;
        private string _username;


          


            public SignUp(string username)
        {
            _username = username;
            InitializeComponent();
            ConnectionString = DatabaseConfig.connectionString;
            this.AcceptButton = signupbtn; // Set the AcceptButton property








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


        private void signupbtn_Click(object sender, EventArgs e)
        {
            // Check if any of the required fields are empty
            if (string.IsNullOrWhiteSpace(usertxt.Text) || string.IsNullOrWhiteSpace(passwordtxt.Text) || string.IsNullOrWhiteSpace(confirmpasstxt.Text) || rolecombo.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the password and confirm password fields match
            if (passwordtxt.Text != confirmpasstxt.Text)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Map role names to role IDs
            Dictionary<string, int> roleMapping = new Dictionary<string, int>
    {
        { "امين", 1 },
        { "امين+", 2 },
        { "ادمن", 3 },
        { "كنترول", 4 }
    };

            // Get the selected role name
            string selectedRole = rolecombo.SelectedItem.ToString();

            // Validate the selected role and get its corresponding ID
            if (!roleMapping.TryGetValue(selectedRole, out int roleID))
            {
                MessageBox.Show("Invalid role selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Replace with your actual connection string
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if the username already exists
                    string checkUserQuery = "SELECT COUNT(*) FROM CashierDetails WHERE Username = @Username";
                    using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection))
                    {
                        checkUserCommand.Parameters.AddWithValue("@Username", usertxt.Text);
                        int userCount = (int)checkUserCommand.ExecuteScalar();

                        if (userCount > 0)
                        {
                            MessageBox.Show("Username already exists. Please choose a different username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Insert the new user into the database
                    string query = "INSERT INTO CashierDetails (Username, PasswordHash, RoleID) VALUES (@Username, @PasswordHash, @RoleID)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        string password = passwordtxt.Text; // Consider hashing the password here
                        command.Parameters.AddWithValue("@Username", usertxt.Text);
                        command.Parameters.AddWithValue("@PasswordHash", password);
                        command.Parameters.AddWithValue("@RoleID", roleID); // Use the roleID from the dictionary

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Sign up successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            usertxt.Text = null;
                            passwordtxt.Text = null;
                            confirmpasstxt.Text = null;
                            rolecombo.SelectedIndex = -1; // Clear the role combo box selection
                        }
                        else
                        {
                            MessageBox.Show("Sign up failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }







        private void kryptonPalette1_PalettePaint(object sender, PaletteLayoutEventArgs e)
        {

        }

        private void kryptonLabel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Login login = new Login();  
            login.ShowDialog();
            this.Close();
        }

        private void SignUp_Load(object sender, EventArgs e)
        {

        }

        private void homebackbtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Home home = new Home(_username);
            home.ShowDialog();
            this.Close();
        }

        private void usertxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void rolecombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
