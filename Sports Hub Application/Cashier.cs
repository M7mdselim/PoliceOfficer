using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Mixed_Gym_Application
{
    public partial class Cashier : KryptonForm
    {
        private string connectionString = DatabaseConfig.connectionString;
        private string currentUsername;

        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;

        public Cashier(string username)
        {
            InitializeComponent();
            currentUsername = username;
            this.nidnumber.Leave += new System.EventHandler(this.nidnumber_Leave);


            this.Text = $"Prison Management System - Welcome {username}";
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


        private void addprisonerbtn_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        int prisonerInfoId;

                        // 1) Check if PrisonerInfo already exists by NIDNumber
                        string checkQuery = "SELECT PrisonerInfoID FROM PrisonerInfo WHERE NIDNumber = @NIDNumber";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                        {
                            checkCmd.Parameters.AddWithValue("@NIDNumber", nidnumber.Text.Trim());
                            object existingId = checkCmd.ExecuteScalar();

                            if (existingId != null) // already exists
                            {
                                prisonerInfoId = Convert.ToInt32(existingId);
                            }
                            else
                            {
                                // Insert new PrisonerInfo
                                string insertPrisonerInfo = @"
                            INSERT INTO PrisonerInfo (FullName, NIDNumber, DangerousLevel, PrisonerStatus, CreatedBy , DepositPlace)
                            VALUES (@FullName, @NIDNumber, @DangerousLevel, @PrisonerStatus, @CreatedBy , @DepositPlace);
                            SELECT SCOPE_IDENTITY();";

                                using (SqlCommand cmdInfo = new SqlCommand(insertPrisonerInfo, connection))
                                {
                                    cmdInfo.Parameters.AddWithValue("@FullName", fullNametxt.Text.Trim());
                                    cmdInfo.Parameters.AddWithValue("@NIDNumber", nidnumber.Text.Trim());
                                    cmdInfo.Parameters.AddWithValue("@DangerousLevel", dangerouslevelcombo.SelectedItem?.ToString() ?? "");
                                    cmdInfo.Parameters.AddWithValue("@PrisonerStatus", prisonerstatus.SelectedItem?.ToString() ?? "");
                                    cmdInfo.Parameters.AddWithValue("@CreatedBy", currentUsername);
                                    cmdInfo.Parameters.AddWithValue("@DepositPlace", Deposittxt.Text.Trim());

                                    prisonerInfoId = Convert.ToInt32(cmdInfo.ExecuteScalar());
                                }
                            }
                        }

                        // 2) Insert into Prisoner table
                        string insertPrisoner = @"
                    INSERT INTO Prisoner 
                    (PrisonerInfoID, FullName, ReservationNumber, CaseID, DangerousLevel, PrisonerStatus, 
                     Accused, PrinciplesType, ServiceTime, HospitalDate, LeaveDate, 
                     NIDNumber, CriminalRecord, ImprisonmentDetails, SecurityRevealed, 
                     CensorshipInfo, Notes, CreatedBy , DepositPlace , NextSession , diseasestatus) 
                    VALUES 
                    (@PrisonerInfoID, @FullName, @ReservationNumber, @CaseID, @DangerousLevel, @PrisonerStatus, 
                     @Accused, @PrinciplesType, @ServiceTime, @HospitalDate, @LeaveDate, 
                     @NIDNumber, @CriminalRecord, @ImprisonmentDetails, @SecurityRevealed, 
                     @CensorshipInfo, @Notes, @CreatedBy , @DepositPlace , @NextSession ,@diseasestatus)";

                        using (SqlCommand cmdPrisoner = new SqlCommand(insertPrisoner, connection))
                        {
                            cmdPrisoner.Parameters.AddWithValue("@PrisonerInfoID", prisonerInfoId);
                            cmdPrisoner.Parameters.AddWithValue("@FullName", fullNametxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@ReservationNumber", reservationnumber.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@CaseID", caseid.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@DangerousLevel", dangerouslevelcombo.SelectedItem?.ToString() ?? "");
                            cmdPrisoner.Parameters.AddWithValue("@PrisonerStatus", prisonerstatus.SelectedItem?.ToString() ?? "");
                            cmdPrisoner.Parameters.AddWithValue("@Accused", accusedtxt.Text.Trim());
                            if (principlestxt.CustomFormat == " ")
                                cmdPrisoner.Parameters.AddWithValue("@PrinciplesType", DBNull.Value);
                            else
                                cmdPrisoner.Parameters.AddWithValue("@PrinciplesType", principlestxt.Value);
                            //cmdPrisoner.Parameters.AddWithValue("@PrinciplesType", principlestxts.Text);
                            cmdPrisoner.Parameters.AddWithValue("@ServiceTime", servicetimetxt.Text);
                            // HospitalDate
                            if (hospitaldate.CustomFormat == " ")
                                cmdPrisoner.Parameters.AddWithValue("@HospitalDate", DBNull.Value);
                            else
                                cmdPrisoner.Parameters.AddWithValue("@HospitalDate", hospitaldate.Value);

                            // LeaveDate
                            if (leavedate.CustomFormat == " ")
                                cmdPrisoner.Parameters.AddWithValue("@LeaveDate", DBNull.Value);
                            else
                                cmdPrisoner.Parameters.AddWithValue("@LeaveDate", leavedate.Value);


                            if (nextsessiondate.CustomFormat == " ")
                                cmdPrisoner.Parameters.AddWithValue("@NextSession", DBNull.Value);
                            else
                                cmdPrisoner.Parameters.AddWithValue("@NextSession", nextsessiondate.Value);

                            cmdPrisoner.Parameters.AddWithValue("@NIDNumber", nidnumber.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@CriminalRecord", criminalrecordtxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@ImprisonmentDetails", Imprisonmenttxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@SecurityRevealed", securityrevealedtxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@CensorshipInfo", Censorshiptxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@Notes", notestxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@CreatedBy", currentUsername);
                            cmdPrisoner.Parameters.AddWithValue("@DepositPlace", Deposittxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@diseasestatus", diseasestatus.Text.Trim());

                            int result = cmdPrisoner.ExecuteNonQuery();
                            if (result > 0)
                            {
                                MessageBox.Show("Prisoner added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                               
                                ResetForm();
                            }
                            else
                            {
                                MessageBox.Show("Failed to add prisoner.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(fullNametxt.Text))
            {
                MessageBox.Show("Please enter full name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fullNametxt.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(reservationnumber.Text))
            {
                MessageBox.Show("Please enter reservation number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                reservationnumber.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(caseid.Text))
            {
                MessageBox.Show("Please enter case ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                caseid.Focus();
                return false;
            }

            if (dangerouslevelcombo.SelectedItem == null)
            {
                MessageBox.Show("Please select dangerous level.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dangerouslevelcombo.Focus();
                return false;
            }

            if (prisonerstatus.SelectedItem == null)
            {
                MessageBox.Show("Please select prisoner status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                prisonerstatus.Focus();
                return false;
            }

           

            return true;
        }

        private void resetformbtn_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            fullNametxt.Clear();
            reservationnumber.Clear();
            caseid.Clear();
            dangerouslevelcombo.SelectedIndex = -1;
            prisonerstatus.SelectedIndex = -1;
            accusedtxt.Clear();
           
            servicetimetxt.Clear();
            hospitaldate.Value = DateTime.Now;
            leavedate.Value = DateTime.Now;
            nidnumber.Clear();
            criminalrecordtxt.Clear();
            Imprisonmenttxt.Clear();
            securityrevealedtxt.Clear();
            Censorshiptxt.Clear();
            notestxt.Clear();
            diseasestatus.Clear();
            Deposittxt.SelectedIndex = -1;
            hospitaldate.Format = DateTimePickerFormat.Custom;
            hospitaldate.CustomFormat = " ";

            leavedate.Format = DateTimePickerFormat.Custom;
            leavedate.CustomFormat = " ";

            principlestxt.Format = DateTimePickerFormat.Custom;
            principlestxt.CustomFormat = " ";

            nextsessiondate.Format = DateTimePickerFormat.Custom;
            nextsessiondate.CustomFormat = " ";

            fullNametxt.Focus();

            fullNametxt.ReadOnly = false;
            dangerouslevelcombo.Enabled = true;
            prisonerstatus.Enabled = true;
            Deposittxt.Enabled = true;
        }

        private void dailyreportbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            CashierDailyReport cashierDailyReport = new CashierDailyReport(cashiernamelabel.Text);
            cashierDailyReport.ShowDialog();
            this.Close();
        }

        private void exitbtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Text validation for numeric fields
        private void servicetimetxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void nidnumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Cashier_Load(object sender, EventArgs e)
        {
            SetButtonVisibilityBasedOnRole();
            cashiernamelabel.Text= currentUsername;
            hospitaldate.Format = DateTimePickerFormat.Custom;
            hospitaldate.CustomFormat = " ";

            leavedate.Format = DateTimePickerFormat.Custom;
            leavedate.CustomFormat = " ";


            nextsessiondate.Format = DateTimePickerFormat.Custom;
            nextsessiondate.CustomFormat = " ";

            principlestxt.Format = DateTimePickerFormat.Custom;
            principlestxt.CustomFormat = " ";


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT DISTINCT NIDNumber FROM PrisonerInfo WHERE NIDNumber IS NOT NULL";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            AutoCompleteStringCollection nidList = new AutoCompleteStringCollection();

                            while (reader.Read())
                            {
                                nidList.Add(reader["NIDNumber"].ToString());
                            }

                            nidnumber.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                            nidnumber.AutoCompleteSource = AutoCompleteSource.CustomSource;
                            nidnumber.AutoCompleteCustomSource = nidList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading NID suggestions: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void nidnumber_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nidnumber.Text))
                return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT FullName, DangerousLevel, PrisonerStatus ,DepositPlace
                             FROM PrisonerInfo 
                             WHERE NIDNumber = @NIDNumber";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@NIDNumber", nidnumber.Text.Trim());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Fill fields
                                fullNametxt.Text = reader["FullName"].ToString();
                                dangerouslevelcombo.SelectedItem = reader["DangerousLevel"].ToString();
                                prisonerstatus.SelectedItem = reader["PrisonerStatus"].ToString();
                                Deposittxt.Text = reader["DepositPlace"].ToString();

                                // Lock them
                                fullNametxt.ReadOnly = true;
                                dangerouslevelcombo.Enabled = false;
                                prisonerstatus.Enabled = false;
                                Deposittxt.Enabled = false;
                            }
                            else
                            {
                                // No existing NID → unlock fields to allow new input
                                fullNametxt.ReadOnly = false;
                                dangerouslevelcombo.Enabled = true;
                                prisonerstatus.Enabled = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving prisoner info: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nidnumber_TextChanged(object sender, EventArgs e)
        {

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

        private void SetButtonVisibilityBasedOnRole()
        {
            int roleID = GetRoleIdForCurrentUser();

            if (roleID == 1)
            {
                // Hide the button if role ID is 1
                backbtn.Visible = false;
              


            }
            else if (roleID == 2)
            {
                // Show the button if role ID is 2
                backbtn.Visible = false;
               

            }
            else if (roleID == 3)
            {
                // Show the button if role ID is 2
                backbtn.Visible = true;
               

            }
            else if (roleID == 4)
            {
                // Show the button if role ID is 2
                backbtn.Visible = true;
                

            }
        }

        private void backbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            Home home = new Home(cashiernamelabel.Text);
            home.ShowDialog();
            this.Close();

        }

        // Make it look empty
       
            

        private void hospitaldate_ValueChanged(object sender, EventArgs e)
        {
            hospitaldate.Format = DateTimePickerFormat.Short;
            hospitaldate.CustomFormat = null; // reset
        }

        private void leavedate_ValueChanged(object sender, EventArgs e)
        {
            leavedate.Format = DateTimePickerFormat.Short;
            leavedate.CustomFormat = null; // reset
        }

        private void hospitaldatecancel_Click(object sender, EventArgs e)
        {
            hospitaldate.Format = DateTimePickerFormat.Custom;
            hospitaldate.CustomFormat = " ";
        }

        private void leavedatecancel_Click(object sender, EventArgs e)
        {
            leavedate.Format = DateTimePickerFormat.Custom;
            leavedate.CustomFormat = " ";
        }

        private void principlestxt_ValueChanged(object sender, EventArgs e)
        {

            principlestxt.Format = DateTimePickerFormat.Short;
            principlestxt.CustomFormat = null; // reset

        }

        private void principlescancel_Click(object sender, EventArgs e)
        {
            principlestxt.Format = DateTimePickerFormat.Custom;
            principlestxt.CustomFormat = " ";
        }

        private void nextsessiondate_ValueChanged(object sender, EventArgs e)
        {
            nextsessiondate.Format = DateTimePickerFormat.Short;
            nextsessiondate .CustomFormat = null; // reset
        }

        private void nextsessiondatecancel_Click(object sender, EventArgs e)
        {
            nextsessiondate.Format = DateTimePickerFormat.Custom;
            nextsessiondate.CustomFormat = " ";
        }

        private void Deposittxt_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}