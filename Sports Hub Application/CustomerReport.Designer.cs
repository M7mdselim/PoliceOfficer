using System.Windows.Forms;

namespace Mixed_Gym_Application
{
    partial class CustomerReport
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox fullnametxt;
        private DataGridView prisonerinfoprisonersgridview;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerReport));
            this.fullnametxt = new System.Windows.Forms.TextBox();
            this.prisonerinfoprisonersgridview = new System.Windows.Forms.DataGridView();
            this.nameLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dangerousleveltxt = new System.Windows.Forms.TextBox();
            this.nidtxt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statustxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.searchButton = new System.Windows.Forms.Button();
            this.printButton = new System.Windows.Forms.Button();
            this.ExportToExcelButton = new System.Windows.Forms.Button();
            this.clearbtn = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.prisonerinfoprisonersgridview)).BeginInit();
            this.SuspendLayout();
            // 
            // fullnametxt
            // 
            this.fullnametxt.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fullnametxt.Location = new System.Drawing.Point(378, 189);
            this.fullnametxt.Margin = new System.Windows.Forms.Padding(2);
            this.fullnametxt.Name = "fullnametxt";
            this.fullnametxt.Size = new System.Drawing.Size(285, 33);
            this.fullnametxt.TabIndex = 0;
            this.fullnametxt.TextChanged += new System.EventHandler(this.fullnametxt_TextChanged);
            // 
            // prisonerinfoprisonersgridview
            // 
            this.prisonerinfoprisonersgridview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.prisonerinfoprisonersgridview.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.prisonerinfoprisonersgridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.prisonerinfoprisonersgridview.Location = new System.Drawing.Point(11, 315);
            this.prisonerinfoprisonersgridview.Margin = new System.Windows.Forms.Padding(2);
            this.prisonerinfoprisonersgridview.Name = "prisonerinfoprisonersgridview";
            this.prisonerinfoprisonersgridview.RowHeadersWidth = 35;
            this.prisonerinfoprisonersgridview.RowTemplate.Height = 24;
            this.prisonerinfoprisonersgridview.Size = new System.Drawing.Size(900, 346);
            this.prisonerinfoprisonersgridview.TabIndex = 2;
            this.prisonerinfoprisonersgridview.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.prisonerinfoprisonersgridview_CellContentClick);
            // 
            // nameLabel
            // 
            this.nameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.nameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.nameLabel.Location = new System.Drawing.Point(702, 184);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(157, 45);
            this.nameLabel.TabIndex = 3;
            this.nameLabel.Text = "الاسم";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // titleLabel
            // 
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.titleLabel.Location = new System.Drawing.Point(283, 30);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(411, 88);
            this.titleLabel.TabIndex = 4;
            this.titleLabel.Text = "تفرير متهم";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.label1.Location = new System.Drawing.Point(231, 193);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 29);
            this.label1.TabIndex = 5;
            this.label1.Text = "درجه الخطوره";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dangerousleveltxt
            // 
            this.dangerousleveltxt.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dangerousleveltxt.Location = new System.Drawing.Point(104, 195);
            this.dangerousleveltxt.Margin = new System.Windows.Forms.Padding(2);
            this.dangerousleveltxt.Name = "dangerousleveltxt";
            this.dangerousleveltxt.Size = new System.Drawing.Size(113, 27);
            this.dangerousleveltxt.TabIndex = 6;
            this.dangerousleveltxt.TextChanged += new System.EventHandler(this.dangerousleveltxt_TextChanged);
            // 
            // nidtxt
            // 
            this.nidtxt.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nidtxt.Location = new System.Drawing.Point(577, 259);
            this.nidtxt.Margin = new System.Windows.Forms.Padding(2);
            this.nidtxt.Name = "nidtxt";
            this.nidtxt.Size = new System.Drawing.Size(157, 27);
            this.nidtxt.TabIndex = 8;
            this.nidtxt.TextChanged += new System.EventHandler(this.nidtxt_TextChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.label2.Location = new System.Drawing.Point(748, 257);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 29);
            this.label2.TabIndex = 7;
            this.label2.Text = "رقم القومي";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statustxt
            // 
            this.statustxt.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statustxt.Location = new System.Drawing.Point(219, 259);
            this.statustxt.Margin = new System.Windows.Forms.Padding(2);
            this.statustxt.Name = "statustxt";
            this.statustxt.Size = new System.Drawing.Size(184, 27);
            this.statustxt.TabIndex = 10;
            this.statustxt.TextChanged += new System.EventHandler(this.statustxt_TextChanged);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.label3.Location = new System.Drawing.Point(424, 257);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 29);
            this.label3.TabIndex = 9;
            this.label3.Text = "الحاله الجنائيه";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // searchButton
            // 
            this.searchButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.searchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.searchButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.searchButton.Location = new System.Drawing.Point(702, 682);
            this.searchButton.Margin = new System.Windows.Forms.Padding(2);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(209, 58);
            this.searchButton.TabIndex = 1;
            this.searchButton.Text = "بحث";
            this.searchButton.UseVisualStyleBackColor = false;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // printButton
            // 
            this.printButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.printButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.printButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.printButton.Location = new System.Drawing.Point(23, 682);
            this.printButton.Margin = new System.Windows.Forms.Padding(2);
            this.printButton.Name = "printButton";
            this.printButton.Size = new System.Drawing.Size(205, 58);
            this.printButton.TabIndex = 13;
            this.printButton.Text = "طباعة";
            this.printButton.UseVisualStyleBackColor = false;
            // 
            // ExportToExcelButton
            // 
            this.ExportToExcelButton.Location = new System.Drawing.Point(319, 700);
            this.ExportToExcelButton.Name = "ExportToExcelButton";
            this.ExportToExcelButton.Size = new System.Drawing.Size(70, 23);
            this.ExportToExcelButton.TabIndex = 25;
            this.ExportToExcelButton.Text = "استخراج ";
            this.ExportToExcelButton.UseVisualStyleBackColor = true;
            // 
            // clearbtn
            // 
            this.clearbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.clearbtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.clearbtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.clearbtn.Location = new System.Drawing.Point(472, 682);
            this.clearbtn.Margin = new System.Windows.Forms.Padding(2);
            this.clearbtn.Name = "clearbtn";
            this.clearbtn.Size = new System.Drawing.Size(205, 58);
            this.clearbtn.TabIndex = 26;
            this.clearbtn.Text = "مسح ";
            this.clearbtn.UseVisualStyleBackColor = false;
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.Transparent;
            this.backButton.BackgroundImage = global::Mixed_Gym_Application.Properties.Resources.icons8_back_button_502;
            this.backButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.backButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.backButton.ForeColor = System.Drawing.Color.IndianRed;
            this.backButton.Location = new System.Drawing.Point(15, 5);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(70, 63);
            this.backButton.TabIndex = 22;
            this.backButton.UseVisualStyleBackColor = false;
            // 
            // CustomerReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(953, 749);
            this.Controls.Add(this.clearbtn);
            this.Controls.Add(this.ExportToExcelButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.printButton);
            this.Controls.Add(this.statustxt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nidtxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dangerousleveltxt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.prisonerinfoprisonersgridview);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.fullnametxt);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CustomerReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Report";
            this.Load += new System.EventHandler(this.CustomerReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.prisonerinfoprisonersgridview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label nameLabel;
        private Label titleLabel;
        private Label label1;
        private TextBox dangerousleveltxt;
        private TextBox nidtxt;
        private Label label2;
        private TextBox statustxt;
        private Label label3;
        private Button searchButton;
        private Button printButton;
        private Button backButton;
        private Button ExportToExcelButton;
        private Button clearbtn;
    }
}
