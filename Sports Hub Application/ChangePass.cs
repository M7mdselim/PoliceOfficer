using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mixed_Gym_Application
{
    public partial class Changepass : Form
    {
        private string _username;
        public Changepass(string username)
        {

            _username = username;
            InitializeComponent();
        }

        private void ChangePassbtn_Click_1(object sender, EventArgs e)
        {
            // Retrieve values from input fields
            string currentUsername = usertxt.Text; // TextBox for current username     
            string newPassword = passwordtxt.Text;
            string confirmPassword = confirmpasstxt.Text; // TextBox for new password


            if (string.IsNullOrEmpty(currentUsername) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Username and password fields cannot be empty.");
                return;
            }
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.connectionString))
            {
                try
                {
                    connection.Open();

                    // SQL query to update username and password
                    string query = @"
                UPDATE PoliceOfficerDB.dbo.CashierDetails
                SET Username = @CurrentUsername,
                    PasswordHash = @NewPassword
                WHERE Username = @CurrentUsername";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@CurrentUsername", currentUsername);
                        command.Parameters.AddWithValue("@NewPassword", newPassword);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User details updated successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Update failed. Please check the username.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void homebackbtn_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Home home = new Home(_username);
            home.ShowDialog();
            this.Close();
        }

       
    }
}
