namespace LoLPasswordManager
{
    partial class mainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.accountPanel = new System.Windows.Forms.TableLayoutPanel();
            this.editButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.accountPanel);
            this.groupBox1.Location = new System.Drawing.Point(12, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(345, 354);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Accounts";
            // 
            // accountPanel
            // 
            this.accountPanel.AutoScroll = true;
            this.accountPanel.ColumnCount = 1;
            this.accountPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.accountPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.accountPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accountPanel.Location = new System.Drawing.Point(3, 16);
            this.accountPanel.Name = "accountPanel";
            this.accountPanel.RowCount = 1;
            this.accountPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 335F));
            this.accountPanel.Size = new System.Drawing.Size(339, 335);
            this.accountPanel.TabIndex = 0;
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.editButton.Location = new System.Drawing.Point(12, 12);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(295, 45);
            this.editButton.TabIndex = 0;
            this.editButton.Text = "Enter Edit Mode";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editModeButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsButton.BackgroundImage = global::LoLPasswordManager.Properties.Resources.ausruestung;
            this.settingsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.settingsButton.Location = new System.Drawing.Point(313, 12);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(45, 45);
            this.settingsButton.TabIndex = 1;
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 429);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.editButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(385, 468);
            this.Name = "mainForm";
            this.Text = "LoL Account Manager";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel accountPanel;
    }
}