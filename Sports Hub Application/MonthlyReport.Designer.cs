using System;
using System.Windows.Forms;

namespace Mixed_Gym_Application
{
    partial class MonthlyReport
    {
        private System.ComponentModel.IContainer components = null;
        private DateTimePicker datePicker;
        private Button loadReportButton;
        private DataGridView transactionsGridView;
        private System.Windows.Forms.Timer timer;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonthlyReport));
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.loadReportButton = new System.Windows.Forms.Button();
            this.transactionsGridView = new System.Windows.Forms.DataGridView();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.titleLabel = new System.Windows.Forms.Label();
            this.PrintButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.ExportToExcelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.transactionsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // datePicker
            // 
            this.datePicker.CustomFormat = "MMMM yyyy";
            this.datePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datePicker.Location = new System.Drawing.Point(355, 518);
            this.datePicker.Margin = new System.Windows.Forms.Padding(2);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(244, 20);
            this.datePicker.TabIndex = 0;
            this.datePicker.ValueChanged += new System.EventHandler(this.datePicker_ValueChanged);
            // 
            // loadReportButton
            // 
            this.loadReportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.loadReportButton.Font = new System.Drawing.Font("PT Bold Heading", 15.75F);
            this.loadReportButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.loadReportButton.Location = new System.Drawing.Point(669, 491);
            this.loadReportButton.Margin = new System.Windows.Forms.Padding(2);
            this.loadReportButton.Name = "loadReportButton";
            this.loadReportButton.Size = new System.Drawing.Size(180, 68);
            this.loadReportButton.TabIndex = 1;
            this.loadReportButton.Text = "بحث";
            this.loadReportButton.UseVisualStyleBackColor = false;
            this.loadReportButton.Click += new System.EventHandler(this.loadReportButton_Click);
            // 
            // transactionsGridView
            // 
            this.transactionsGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.transactionsGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.transactionsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.transactionsGridView.Location = new System.Drawing.Point(12, 95);
            this.transactionsGridView.Margin = new System.Windows.Forms.Padding(2);
            this.transactionsGridView.Name = "transactionsGridView";
            this.transactionsGridView.RowTemplate.Height = 24;
            this.transactionsGridView.Size = new System.Drawing.Size(837, 363);
            this.transactionsGridView.TabIndex = 2;
            this.transactionsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.transactionsGridView_CellContentClick);
            this.transactionsGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.transactionsGridView_CellDoubleClick);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.Font = new System.Drawing.Font("PT Bold Heading", 21.75F);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.titleLabel.Location = new System.Drawing.Point(283, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(290, 52);
            this.titleLabel.TabIndex = 4;
            this.titleLabel.Text = "تقرير شهري جيم ميكس";
            // 
            // PrintButton
            // 
            this.PrintButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.PrintButton.Font = new System.Drawing.Font("PT Bold Heading", 15.75F);
            this.PrintButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.PrintButton.Location = new System.Drawing.Point(9, 494);
            this.PrintButton.Name = "PrintButton";
            this.PrintButton.Size = new System.Drawing.Size(177, 65);
            this.PrintButton.TabIndex = 5;
            this.PrintButton.Text = "طباعة";
            this.PrintButton.UseVisualStyleBackColor = false;
            this.PrintButton.Click += new System.EventHandler(this.PrintButton_Click);
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.White;
            this.backButton.BackgroundImage = global::Mixed_Gym_Application.Properties.Resources.icons8_back_button_502;
            this.backButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.backButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.backButton.ForeColor = System.Drawing.Color.IndianRed;
            this.backButton.Location = new System.Drawing.Point(12, 12);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(70, 72);
            this.backButton.TabIndex = 23;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // ExportToExcelButton
            // 
            this.ExportToExcelButton.Location = new System.Drawing.Point(252, 518);
            this.ExportToExcelButton.Name = "ExportToExcelButton";
            this.ExportToExcelButton.Size = new System.Drawing.Size(70, 23);
            this.ExportToExcelButton.TabIndex = 24;
            this.ExportToExcelButton.Text = "استخراج ";
            this.ExportToExcelButton.UseVisualStyleBackColor = true;
            this.ExportToExcelButton.Click += new System.EventHandler(this.ExportToExcelButton_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Century Schoolbook", 21.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.label1.Location = new System.Drawing.Point(337, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 34);
            this.label1.TabIndex = 47;
            this.label1.Text = "Mixed GYM";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MonthlyReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 583);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExportToExcelButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.PrintButton);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.transactionsGridView);
            this.Controls.Add(this.loadReportButton);
            this.Controls.Add(this.datePicker);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MonthlyReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "تقرير شهري";
            this.Load += new System.EventHandler(this.MonthlyReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.transactionsGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label titleLabel;
        private Button PrintButton;
        private Button backButton;
        private Button ExportToExcelButton;
        private Label label1;
    }
}
