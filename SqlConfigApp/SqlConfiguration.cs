using System;
using System.IO; // Add this line
using System.Windows.Forms;

namespace SqlConfigApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Example saving to a config file
            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyApp", "config.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(configPath)); // Ensure the directory exists
            File.WriteAllLines(configPath, new[] {
                txtServerName.Text,
                txtUsername.Text,
                txtPassword.Text
            });

            MessageBox.Show("Configuration saved.");
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
