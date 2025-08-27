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

        public Cashier(string username)
        {
            InitializeComponent();
            currentUsername = username;
            this.nidnumber.Leave += new System.EventHandler(this.nidnumber_Leave);


            this.Text = $"Prison Management System - Welcome {username}";
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
                            INSERT INTO PrisonerInfo (FullName, NIDNumber, DangerousLevel, PrisonerStatus, CreatedBy)
                            VALUES (@FullName, @NIDNumber, @DangerousLevel, @PrisonerStatus, @CreatedBy);
                            SELECT SCOPE_IDENTITY();";

                                using (SqlCommand cmdInfo = new SqlCommand(insertPrisonerInfo, connection))
                                {
                                    cmdInfo.Parameters.AddWithValue("@FullName", fullNametxt.Text.Trim());
                                    cmdInfo.Parameters.AddWithValue("@NIDNumber", nidnumber.Text.Trim());
                                    cmdInfo.Parameters.AddWithValue("@DangerousLevel", dangerouslevelcombo.SelectedItem?.ToString() ?? "");
                                    cmdInfo.Parameters.AddWithValue("@PrisonerStatus", prisonerstatus.SelectedItem?.ToString() ?? "");
                                    cmdInfo.Parameters.AddWithValue("@CreatedBy", currentUsername);

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
                     CensorshipInfo, Notes, CreatedBy) 
                    VALUES 
                    (@PrisonerInfoID, @FullName, @ReservationNumber, @CaseID, @DangerousLevel, @PrisonerStatus, 
                     @Accused, @PrinciplesType, @ServiceTime, @HospitalDate, @LeaveDate, 
                     @NIDNumber, @CriminalRecord, @ImprisonmentDetails, @SecurityRevealed, 
                     @CensorshipInfo, @Notes, @CreatedBy)";

                        using (SqlCommand cmdPrisoner = new SqlCommand(insertPrisoner, connection))
                        {
                            cmdPrisoner.Parameters.AddWithValue("@PrisonerInfoID", prisonerInfoId);
                            cmdPrisoner.Parameters.AddWithValue("@FullName", fullNametxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@ReservationNumber", reservationnumber.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@CaseID", caseid.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@DangerousLevel", dangerouslevelcombo.SelectedItem?.ToString() ?? "");
                            cmdPrisoner.Parameters.AddWithValue("@PrisonerStatus", prisonerstatus.SelectedItem?.ToString() ?? "");
                            cmdPrisoner.Parameters.AddWithValue("@Accused", accusedtxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@PrinciplesType", principlestxt.Text);
                            cmdPrisoner.Parameters.AddWithValue("@ServiceTime", servicetimetxt.Text);
                            cmdPrisoner.Parameters.AddWithValue("@HospitalDate", hospitaldate.Value);
                            cmdPrisoner.Parameters.AddWithValue("@LeaveDate", leavedate.Value);
                            cmdPrisoner.Parameters.AddWithValue("@NIDNumber", nidnumber.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@CriminalRecord", criminalrecordtxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@ImprisonmentDetails", Imprisonmenttxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@SecurityRevealed", securityrevealedtxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@CensorshipInfo", Censorshiptxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@Notes", notestxt.Text.Trim());
                            cmdPrisoner.Parameters.AddWithValue("@CreatedBy", currentUsername);

                            int result = cmdPrisoner.ExecuteNonQuery();
                            if (result > 0)
                            {
                                MessageBox.Show("Prisoner added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                fullNametxt.ReadOnly = false;
                                dangerouslevelcombo.Enabled = true;
                                prisonerstatus.Enabled = true;
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
            principlestxt.Text = "";
            servicetimetxt.Clear();
            hospitaldate.Value = DateTime.Now;
            leavedate.Value = DateTime.Now;
            nidnumber.Clear();
            criminalrecordtxt.Clear();
            Imprisonmenttxt.Clear();
            securityrevealedtxt.Clear();
            Censorshiptxt.Clear();
            notestxt.Clear();

            fullNametxt.Focus();
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
            cashiernamelabel.Text= currentUsername;


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
                    string query = @"SELECT FullName, DangerousLevel, PrisonerStatus 
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

                                // Lock them
                                fullNametxt.ReadOnly = true;
                                dangerouslevelcombo.Enabled = false;
                                prisonerstatus.Enabled = false;
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
    }
}