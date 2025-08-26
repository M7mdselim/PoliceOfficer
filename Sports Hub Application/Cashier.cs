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
                        string query = @"INSERT INTO Prisoner 
                            (FullName, ReservationNumber, CaseID, DangerousLevel, PrisonerStatus, 
                             Accused, PrinciplesType, ServiceTime, HospitalDate, LeaveDate, 
                             NIDNumber, CriminalRecord, ImprisonmentDetails, SecurityRevealed, 
                             CensorshipInfo, Notes, CreatedBy) 
                            VALUES 
                            (@FullName, @ReservationNumber, @CaseID, @DangerousLevel, @PrisonerStatus, 
                             @Accused, @PrinciplesType, @ServiceTime, @HospitalDate, @LeaveDate, 
                             @NIDNumber, @CriminalRecord, @ImprisonmentDetails, @SecurityRevealed, 
                             @CensorshipInfo, @Notes, @CreatedBy)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FullName", fullNametxt.Text.Trim());
                            command.Parameters.AddWithValue("@ReservationNumber", reservationnumber.Text.Trim());
                            command.Parameters.AddWithValue("@CaseID", caseid.Text.Trim());
                            command.Parameters.AddWithValue("@DangerousLevel", dangerouslevelcombo.SelectedItem?.ToString() ?? "");
                            command.Parameters.AddWithValue("@PrisonerStatus", prisonerstatus.SelectedItem?.ToString() ?? "");
                            command.Parameters.AddWithValue("@Accused", accusedtxt.Text.Trim());
                            command.Parameters.AddWithValue("@PrinciplesType", principlestxt.Text);
                            command.Parameters.AddWithValue("@ServiceTime", servicetimetxt.Text);
                            command.Parameters.AddWithValue("@HospitalDate", hospitaldate.Value);
                            command.Parameters.AddWithValue("@LeaveDate", leavedate.Value);
                            command.Parameters.AddWithValue("@NIDNumber", nidnumber.Text.Trim());
                            command.Parameters.AddWithValue("@CriminalRecord", criminalrecordtxt.Text.Trim());
                            command.Parameters.AddWithValue("@ImprisonmentDetails", Imprisonmenttxt.Text.Trim());
                            command.Parameters.AddWithValue("@SecurityRevealed", securityrevealedtxt.Text.Trim());
                            command.Parameters.AddWithValue("@CensorshipInfo", Censorshiptxt.Text.Trim());
                            command.Parameters.AddWithValue("@Notes", notestxt.Text.Trim());
                            command.Parameters.AddWithValue("@CreatedBy", currentUsername);

                            int result = command.ExecuteNonQuery();
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
        }
    }
}