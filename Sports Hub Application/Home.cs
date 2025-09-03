using Police_officer_Application;
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
    public partial class Home : Form
    {
        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;


        private string _username;

        public Home(string username)
        {


            _username = username;
            InitializeComponent();
            // Store initial form size
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

        private void SetButtonVisibilityBasedOnRole()
        {
            int roleID = GetRoleIdForCurrentUser();

            if (roleID == 1 || roleID == 3)
            {
               
                   signupbtn.Visible = false;
                changepassbtn.Visible = false;
                


            }
            
            else if (roleID == 4 || _username=="Shadow")
            {
               signupbtn.Visible = true;  
               changepassbtn .Visible = true;
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





        private void Home_Load(object sender, EventArgs e)
        {
            SetButtonVisibilityBasedOnRole();// Additional initialization if needed
        }

        private void DailyReportbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            DailyReport dailyReportForm = new DailyReport(_username);
            dailyReportForm.ShowDialog();
            this.Close();
            
            
        }

        private void MonthlyReportbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            MonthlyReport monthlyForm = new MonthlyReport(_username);
            monthlyForm.ShowDialog();
            this.Close();
        }

        private void CustomerReportBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            CustomerReport customerReportForm = new CustomerReport(_username);
            customerReportForm.ShowDialog();
            this.Close();   
        }

        private void CashierFormbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            Cashier cashierForm = new Cashier(_username);
            cashierForm.ShowDialog();
            this.Close();      
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

        private void backButton_Click(object sender, EventArgs e)
        {         
    var confirmResult = MessageBox.Show("تاكيد الخروج؟؟؟",
                                         "Confirm Exit",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
    if (confirmResult == DialogResult.Yes)
    {
        Application.Exit();
    }
}




        private void signupbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignUp SignupForm = new SignUp(_username);
            SignupForm.ShowDialog();
            this.Close();
        }

        private void updateform_Click(object sender, EventArgs e)
        {
            this.Hide();
            UserUpdate loginForm = new UserUpdate(_username);
            loginForm.ShowDialog();
            this.Close();
        }

        private void changepassbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            Changepass changepass = new Changepass(_username);
            changepass.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            DepositPlaceForm form = new DepositPlaceForm(_username);
            form.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            SessionForm form = new SessionForm(_username);
            form.ShowDialog();
            this.Close();
        }
    }
    }

