using System.Windows.Forms;

namespace Mixed_Gym_Application
{
    public partial class DailyReport : Form
    {
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DailyReport));
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.loadReportButton = new System.Windows.Forms.Button();
            this.transactionsGridView = new System.Windows.Forms.DataGridView();
            this.titleLabel = new System.Windows.Forms.Label();
            this.PrintButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.ExportToExcelButton = new System.Windows.Forms.Button();
            this.columnnamecombobox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.searchtxt = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.transactionsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // datePicker
            // 
            this.datePicker.Location = new System.Drawing.Point(389, 521);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(223, 20);
            this.datePicker.TabIndex = 0;
            // 
            // loadReportButton
            // 
            this.loadReportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.loadReportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.loadReportButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.loadReportButton.Location = new System.Drawing.Point(722, 529);
            this.loadReportButton.Name = "loadReportButton";
            this.loadReportButton.Size = new System.Drawing.Size(177, 65);
            this.loadReportButton.TabIndex = 1;
            this.loadReportButton.Text = "بحث";
            this.loadReportButton.UseVisualStyleBackColor = false;
            this.loadReportButton.Click += new System.EventHandler(this.loadReportButton_Click);
            // 
            // transactionsGridView
            // 
            this.transactionsGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.transactionsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.transactionsGridView.Location = new System.Drawing.Point(10, 86);
            this.transactionsGridView.Name = "transactionsGridView";
            this.transactionsGridView.RowTemplate.Height = 24;
            this.transactionsGridView.Size = new System.Drawing.Size(889, 407);
            this.transactionsGridView.TabIndex = 2;
            this.transactionsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.transactionsGridView_CellContentClick);
            this.transactionsGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.transactionsGridView_CellDoubleClick);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.titleLabel.Location = new System.Drawing.Point(333, 25);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(246, 33);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "تقرير يومي قسم الشرطه";
            // 
            // PrintButton
            // 
            this.PrintButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.PrintButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.PrintButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.PrintButton.Location = new System.Drawing.Point(12, 529);
            this.PrintButton.Name = "PrintButton";
            this.PrintButton.Size = new System.Drawing.Size(177, 65);
            this.PrintButton.TabIndex = 4;
            this.PrintButton.Text = "طباعة";
            this.PrintButton.UseVisualStyleBackColor = false;
            this.PrintButton.Click += new System.EventHandler(this.PrintButton_Click);
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.Transparent;
            this.backButton.BackgroundImage = global::Police_officer_Application.Properties.Resources.icons8_back_button_502;
            this.backButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.backButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.backButton.ForeColor = System.Drawing.Color.IndianRed;
            this.backButton.Location = new System.Drawing.Point(10, 12);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(70, 68);
            this.backButton.TabIndex = 21;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // ExportToExcelButton
            // 
            this.ExportToExcelButton.Location = new System.Drawing.Point(205, 553);
            this.ExportToExcelButton.Name = "ExportToExcelButton";
            this.ExportToExcelButton.Size = new System.Drawing.Size(70, 23);
            this.ExportToExcelButton.TabIndex = 22;
            this.ExportToExcelButton.Text = "استخراج ";
            this.ExportToExcelButton.UseVisualStyleBackColor = true;
            this.ExportToExcelButton.Click += new System.EventHandler(this.ExportToExcelButton_Click);
            // 
            // columnnamecombobox
            // 
            this.columnnamecombobox.FormattingEnabled = true;
            this.columnnamecombobox.Location = new System.Drawing.Point(553, 555);
            this.columnnamecombobox.Name = "columnnamecombobox";
            this.columnnamecombobox.Size = new System.Drawing.Size(163, 21);
            this.columnnamecombobox.TabIndex = 42;
            this.columnnamecombobox.SelectedIndexChanged += new System.EventHandler(this.columnnamecombobox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.label1.Location = new System.Drawing.Point(630, 520);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 20);
            this.label1.TabIndex = 41;
            this.label1.Text = "بحث عن طريق";
            // 
            // searchtxt
            // 
            this.searchtxt.Location = new System.Drawing.Point(315, 556);
            this.searchtxt.Name = "searchtxt";
            this.searchtxt.Size = new System.Drawing.Size(162, 20);
            this.searchtxt.TabIndex = 40;
            this.searchtxt.TextChanged += new System.EventHandler(this.searchtxt_TextChanged);
            // 
            // DailyReport
            // 
            this.ClientSize = new System.Drawing.Size(911, 617);
            this.Controls.Add(this.columnnamecombobox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.searchtxt);
            this.Controls.Add(this.ExportToExcelButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.PrintButton);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.transactionsGridView);
            this.Controls.Add(this.loadReportButton);
            this.Controls.Add(this.datePicker);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DailyReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "تقرير يومي";
            this.Load += new System.EventHandler(this.DailyReport_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.transactionsGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.DateTimePicker datePicker;
        private System.Windows.Forms.Button loadReportButton;
        private System.Windows.Forms.DataGridView transactionsGridView;
        private Label titleLabel;
        private Button PrintButton;
        private Button backButton;
        private Button ExportToExcelButton;
        private ComboBox columnnamecombobox;
        private Label label1;
        private TextBox searchtxt;
    }
}
