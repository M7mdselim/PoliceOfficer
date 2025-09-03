namespace Police_officer_Application
{
    partial class DepositPlaceForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvPrisoners;
        private System.Windows.Forms.ComboBox cmbDepositPlaces;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label lblDepositPlace;
        private System.Windows.Forms.Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvPrisoners = new System.Windows.Forms.DataGridView();
            this.cmbDepositPlaces = new System.Windows.Forms.ComboBox();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.lblDepositPlace = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.PrintButton = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrisoners)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPrisoners
            // 
            this.dgvPrisoners.AllowUserToAddRows = false;
            this.dgvPrisoners.AllowUserToDeleteRows = false;
            this.dgvPrisoners.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPrisoners.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrisoners.Location = new System.Drawing.Point(12, 63);
            this.dgvPrisoners.Name = "dgvPrisoners";
            this.dgvPrisoners.ReadOnly = true;
            this.dgvPrisoners.RowHeadersVisible = false;
            this.dgvPrisoners.Size = new System.Drawing.Size(957, 355);
            this.dgvPrisoners.TabIndex = 0;
            // 
            // cmbDepositPlaces
            // 
            this.cmbDepositPlaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepositPlaces.Font = new System.Drawing.Font("Tahoma", 10F);
            this.cmbDepositPlaces.FormattingEnabled = true;
            this.cmbDepositPlaces.Location = new System.Drawing.Point(266, 457);
            this.cmbDepositPlaces.Name = "cmbDepositPlaces";
            this.cmbDepositPlaces.Size = new System.Drawing.Size(220, 24);
            this.cmbDepositPlaces.TabIndex = 1;
            this.cmbDepositPlaces.SelectedIndexChanged += new System.EventHandler(this.cmbDepositPlaces_SelectedIndexChanged);
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Font = new System.Drawing.Font("Tahoma", 10F);
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Items.AddRange(new object[] {
            "حبس احتياطي",
            "حكم عليه",
            "اخلاء سبيل"});
            this.cmbStatus.Location = new System.Drawing.Point(600, 456);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(150, 24);
            this.cmbStatus.TabIndex = 2;
            // 
            // lblDepositPlace
            // 
            this.lblDepositPlace.AutoSize = true;
            this.lblDepositPlace.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblDepositPlace.Location = new System.Drawing.Point(328, 437);
            this.lblDepositPlace.Name = "lblDepositPlace";
            this.lblDepositPlace.Size = new System.Drawing.Size(90, 17);
            this.lblDepositPlace.TabIndex = 4;
            this.lblDepositPlace.Text = "مكان الإيداع:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(624, 436);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 17);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "حالة السجين:";
            // 
            // PrintButton
            // 
            this.PrintButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.PrintButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.PrintButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.PrintButton.Location = new System.Drawing.Point(26, 424);
            this.PrintButton.Name = "PrintButton";
            this.PrintButton.Size = new System.Drawing.Size(177, 65);
            this.PrintButton.TabIndex = 6;
            this.PrintButton.Text = "طباعة";
            this.PrintButton.UseVisualStyleBackColor = false;
            this.PrintButton.Click += new System.EventHandler(this.PrintButton_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.btnSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.btnSearch.Location = new System.Drawing.Point(774, 424);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(177, 65);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "بحث";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.Transparent;
            this.backButton.BackgroundImage = global::Police_officer_Application.Properties.Resources.icons8_back_button_502;
            this.backButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.backButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.backButton.ForeColor = System.Drawing.Color.IndianRed;
            this.backButton.Location = new System.Drawing.Point(12, 5);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(70, 52);
            this.backButton.TabIndex = 22;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // DepositPlaceForm
            // 
            this.ClientSize = new System.Drawing.Size(973, 498);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.PrintButton);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblDepositPlace);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.cmbDepositPlaces);
            this.Controls.Add(this.dgvPrisoners);
            this.Name = "DepositPlaceForm";
            this.Text = "عرض السجناء حسب مكان الإيداع والحالة";
            this.Load += new System.EventHandler(this.FormDepositPlaces_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrisoners)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button PrintButton;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button backButton;
    }
}