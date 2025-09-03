namespace Mixed_Gym_Application
{
    partial class UpdateTransaction
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateTransaction));
            this.loadtbtn = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.backButton = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.updatebtn = new System.Windows.Forms.Button();
            this.usersDataGridView = new System.Windows.Forms.DataGridView();
            this.sportsComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.searchtxt = new System.Windows.Forms.TextBox();
            this.columnnamecombobox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.usersDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // loadtbtn
            // 
            this.loadtbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.loadtbtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.loadtbtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.loadtbtn.Location = new System.Drawing.Point(662, 489);
            this.loadtbtn.Margin = new System.Windows.Forms.Padding(2);
            this.loadtbtn.Name = "loadtbtn";
            this.loadtbtn.Size = new System.Drawing.Size(180, 68);
            this.loadtbtn.TabIndex = 26;
            this.loadtbtn.Text = "بحث";
            this.loadtbtn.UseVisualStyleBackColor = false;
            this.loadtbtn.Click += new System.EventHandler(this.loadtbtn_Click);
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.Transparent;
            this.backButton.BackgroundImage = global::Police_officer_Application.Properties.Resources.icons8_back_button_502;
            this.backButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.backButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.backButton.ForeColor = System.Drawing.Color.IndianRed;
            this.backButton.Location = new System.Drawing.Point(12, 10);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(70, 69);
            this.backButton.TabIndex = 30;
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.titleLabel.Location = new System.Drawing.Point(368, 16);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(151, 39);
            this.titleLabel.TabIndex = 28;
            this.titleLabel.Text = "تعديل عمليه";
            // 
            // updatebtn
            // 
            this.updatebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.updatebtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.updatebtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(219)))), ((int)(((byte)(200)))));
            this.updatebtn.Location = new System.Drawing.Point(21, 489);
            this.updatebtn.Margin = new System.Windows.Forms.Padding(2);
            this.updatebtn.Name = "updatebtn";
            this.updatebtn.Size = new System.Drawing.Size(180, 68);
            this.updatebtn.TabIndex = 32;
            this.updatebtn.Text = "تعديل";
            this.updatebtn.UseVisualStyleBackColor = false;
            this.updatebtn.Click += new System.EventHandler(this.updatebtn_Click);
            // 
            // usersDataGridView
            // 
            this.usersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.usersDataGridView.Location = new System.Drawing.Point(12, 102);
            this.usersDataGridView.Name = "usersDataGridView";
            this.usersDataGridView.Size = new System.Drawing.Size(837, 353);
            this.usersDataGridView.TabIndex = 33;
            this.usersDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.usersDataGridView_CellContentClick);
            // 
            // sportsComboBox
            // 
            this.sportsComboBox.FormattingEnabled = true;
            this.sportsComboBox.Location = new System.Drawing.Point(824, 12);
            this.sportsComboBox.Name = "sportsComboBox";
            this.sportsComboBox.Size = new System.Drawing.Size(25, 21);
            this.sportsComboBox.TabIndex = 34;
            this.sportsComboBox.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(44)))), ((int)(((byte)(87)))));
            this.label1.Location = new System.Drawing.Point(544, 489);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 20);
            this.label1.TabIndex = 38;
            this.label1.Text = "بحث عن طريق";
            // 
            // searchtxt
            // 
            this.searchtxt.Location = new System.Drawing.Point(229, 516);
            this.searchtxt.Name = "searchtxt";
            this.searchtxt.Size = new System.Drawing.Size(128, 20);
            this.searchtxt.TabIndex = 37;
            this.searchtxt.TextChanged += new System.EventHandler(this.searchtxt_TextChanged);
            // 
            // columnnamecombobox
            // 
            this.columnnamecombobox.FormattingEnabled = true;
            this.columnnamecombobox.Location = new System.Drawing.Point(467, 516);
            this.columnnamecombobox.Name = "columnnamecombobox";
            this.columnnamecombobox.Size = new System.Drawing.Size(163, 21);
            this.columnnamecombobox.TabIndex = 39;
            this.columnnamecombobox.SelectedIndexChanged += new System.EventHandler(this.columnnamecombobox_SelectedIndexChanged);
            // 
            // UpdateTransaction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(861, 598);
            this.Controls.Add(this.columnnamecombobox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.searchtxt);
            this.Controls.Add(this.sportsComboBox);
            this.Controls.Add(this.usersDataGridView);
            this.Controls.Add(this.updatebtn);
            this.Controls.Add(this.loadtbtn);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.titleLabel);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpdateTransaction";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "تعديل ملف";
            this.Load += new System.EventHandler(this.UpdateTransaction_Load);
            ((System.ComponentModel.ISupportInitialize)(this.usersDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button loadtbtn;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button updatebtn;
        private System.Windows.Forms.DataGridView usersDataGridView;
        private System.Windows.Forms.ComboBox sportsComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox searchtxt;
        private System.Windows.Forms.ComboBox columnnamecombobox;
    }
}