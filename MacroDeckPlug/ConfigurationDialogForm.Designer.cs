namespace dichternebel.YaSB.MacroDeckPlug
{
    partial class ConfigurationDialogForm
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
            verticalTabControl1 = new SuchByte.MacroDeck.GUI.CustomControls.VerticalTabControl();
            tabPage1 = new TabPage();
            roundedPanel3 = new SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel();
            checkBox3 = new CheckBox();
            buttonPrimary1 = new SuchByte.MacroDeck.GUI.CustomControls.ButtonPrimary();
            checkBox2 = new CheckBox();
            label8 = new Label();
            roundedPanel2 = new SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel();
            textBoxPassword = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            checkBox1 = new CheckBox();
            label7 = new Label();
            label2 = new Label();
            label1 = new Label();
            roundedPanel1 = new SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel();
            textBoxEndpoint = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            textBoxPort = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            textBoxAddress = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            tabPage2 = new TabPage();
            label6 = new Label();
            treeView1 = new TreeView();
            verticalTabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            roundedPanel3.SuspendLayout();
            roundedPanel2.SuspendLayout();
            roundedPanel1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // verticalTabControl1
            // 
            verticalTabControl1.Alignment = TabAlignment.Left;
            verticalTabControl1.Controls.Add(tabPage1);
            verticalTabControl1.Controls.Add(tabPage2);
            verticalTabControl1.Dock = DockStyle.Fill;
            verticalTabControl1.ItemSize = new Size(221, 60);
            verticalTabControl1.Location = new Point(1, 1);
            verticalTabControl1.Multiline = true;
            verticalTabControl1.Name = "verticalTabControl1";
            verticalTabControl1.SelectedIndex = 0;
            verticalTabControl1.SelectedTabColor = Color.FromArgb(80, 80, 80);
            verticalTabControl1.Size = new Size(798, 448);
            verticalTabControl1.SizeMode = TabSizeMode.Fixed;
            verticalTabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = Color.Transparent;
            tabPage1.Controls.Add(roundedPanel3);
            tabPage1.Controls.Add(label8);
            tabPage1.Controls.Add(roundedPanel2);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(roundedPanel1);
            tabPage1.Location = new Point(64, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(730, 440);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Config";
            // 
            // roundedPanel3
            // 
            roundedPanel3.BackColor = Color.FromArgb(33, 33, 33);
            roundedPanel3.Controls.Add(checkBox3);
            roundedPanel3.Controls.Add(buttonPrimary1);
            roundedPanel3.Controls.Add(checkBox2);
            roundedPanel3.Location = new Point(67, 304);
            roundedPanel3.Name = "roundedPanel3";
            roundedPanel3.Size = new Size(600, 100);
            roundedPanel3.TabIndex = 5;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(20, 25);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(159, 20);
            checkBox3.TabIndex = 2;
            checkBox3.Text = "Delete variables on exit";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // buttonPrimary1
            // 
            buttonPrimary1.BorderRadius = 8;
            buttonPrimary1.Enabled = false;
            buttonPrimary1.FlatAppearance.BorderSize = 0;
            buttonPrimary1.FlatStyle = FlatStyle.Flat;
            buttonPrimary1.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonPrimary1.ForeColor = Color.DarkGray;
            buttonPrimary1.HoverColor = Color.Empty;
            buttonPrimary1.Icon = null;
            buttonPrimary1.Location = new Point(37, 51);
            buttonPrimary1.Name = "buttonPrimary1";
            buttonPrimary1.Progress = 0;
            buttonPrimary1.ProgressColor = Color.FromArgb(0, 103, 205);
            buttonPrimary1.Size = new Size(178, 28);
            buttonPrimary1.TabIndex = 1;
            buttonPrimary1.Text = "Reset Plugin Configuration";
            buttonPrimary1.UseVisualStyleBackColor = true;
            buttonPrimary1.UseWindowsAccentColor = true;
            buttonPrimary1.WriteProgress = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(20, 59);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(15, 14);
            checkBox2.TabIndex = 0;
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(67, 284);
            label8.Name = "label8";
            label8.Size = new Size(32, 16);
            label8.TabIndex = 4;
            label8.Text = "Misc";
            // 
            // roundedPanel2
            // 
            roundedPanel2.BackColor = Color.FromArgb(33, 33, 33);
            roundedPanel2.Controls.Add(textBoxPassword);
            roundedPanel2.Controls.Add(checkBox1);
            roundedPanel2.Controls.Add(label7);
            roundedPanel2.Location = new Point(67, 166);
            roundedPanel2.Name = "roundedPanel2";
            roundedPanel2.Size = new Size(600, 100);
            roundedPanel2.TabIndex = 3;
            // 
            // textBoxPassword
            // 
            textBoxPassword.BackColor = Color.FromArgb(65, 65, 65);
            textBoxPassword.Enabled = false;
            textBoxPassword.Font = new Font("Tahoma", 9F);
            textBoxPassword.Icon = null;
            textBoxPassword.Location = new Point(89, 54);
            textBoxPassword.MaxCharacters = 32767;
            textBoxPassword.Multiline = false;
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Padding = new Padding(8, 5, 8, 5);
            textBoxPassword.PasswordChar = true;
            textBoxPassword.PlaceHolderColor = Color.Gray;
            textBoxPassword.PlaceHolderText = "";
            textBoxPassword.ReadOnly = false;
            textBoxPassword.ScrollBars = ScrollBars.None;
            textBoxPassword.SelectionStart = 0;
            textBoxPassword.Size = new Size(250, 25);
            textBoxPassword.TabIndex = 3;
            textBoxPassword.TextAlignment = HorizontalAlignment.Left;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(20, 22);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(148, 20);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Enable authentication";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(20, 59);
            label7.Name = "label7";
            label7.Size = new Size(62, 16);
            label7.TabIndex = 1;
            label7.Text = "Password";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(67, 147);
            label2.Name = "label2";
            label2.Size = new Size(138, 16);
            label2.TabIndex = 2;
            label2.Text = "Authentication Settings";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(67, 11);
            label1.Name = "label1";
            label1.Size = new Size(161, 16);
            label1.TabIndex = 1;
            label1.Text = "Websocket Server Settings";
            // 
            // roundedPanel1
            // 
            roundedPanel1.BackColor = Color.FromArgb(33, 33, 33);
            roundedPanel1.Controls.Add(textBoxEndpoint);
            roundedPanel1.Controls.Add(textBoxPort);
            roundedPanel1.Controls.Add(textBoxAddress);
            roundedPanel1.Controls.Add(label5);
            roundedPanel1.Controls.Add(label4);
            roundedPanel1.Controls.Add(label3);
            roundedPanel1.Location = new Point(67, 30);
            roundedPanel1.Name = "roundedPanel1";
            roundedPanel1.Size = new Size(600, 100);
            roundedPanel1.TabIndex = 0;
            // 
            // textBoxEndpoint
            // 
            textBoxEndpoint.BackColor = Color.FromArgb(65, 65, 65);
            textBoxEndpoint.Font = new Font("Tahoma", 9F);
            textBoxEndpoint.Icon = null;
            textBoxEndpoint.Location = new Point(89, 69);
            textBoxEndpoint.MaxCharacters = 32767;
            textBoxEndpoint.Multiline = false;
            textBoxEndpoint.Name = "textBoxEndpoint";
            textBoxEndpoint.Padding = new Padding(8, 5, 8, 5);
            textBoxEndpoint.PasswordChar = false;
            textBoxEndpoint.PlaceHolderColor = Color.Gray;
            textBoxEndpoint.PlaceHolderText = "";
            textBoxEndpoint.ReadOnly = false;
            textBoxEndpoint.ScrollBars = ScrollBars.None;
            textBoxEndpoint.SelectionStart = 0;
            textBoxEndpoint.Size = new Size(250, 25);
            textBoxEndpoint.TabIndex = 5;
            textBoxEndpoint.TextAlignment = HorizontalAlignment.Left;
            // 
            // textBoxPort
            // 
            textBoxPort.BackColor = Color.FromArgb(65, 65, 65);
            textBoxPort.Font = new Font("Tahoma", 9F);
            textBoxPort.Icon = null;
            textBoxPort.Location = new Point(89, 39);
            textBoxPort.MaxCharacters = 32767;
            textBoxPort.Multiline = false;
            textBoxPort.Name = "textBoxPort";
            textBoxPort.Padding = new Padding(8, 5, 8, 5);
            textBoxPort.PasswordChar = false;
            textBoxPort.PlaceHolderColor = Color.Gray;
            textBoxPort.PlaceHolderText = "";
            textBoxPort.ReadOnly = false;
            textBoxPort.ScrollBars = ScrollBars.None;
            textBoxPort.SelectionStart = 0;
            textBoxPort.Size = new Size(250, 25);
            textBoxPort.TabIndex = 4;
            textBoxPort.TextAlignment = HorizontalAlignment.Left;
            // 
            // textBoxAddress
            // 
            textBoxAddress.BackColor = Color.FromArgb(65, 65, 65);
            textBoxAddress.Font = new Font("Tahoma", 9F);
            textBoxAddress.Icon = null;
            textBoxAddress.Location = new Point(89, 8);
            textBoxAddress.MaxCharacters = 32767;
            textBoxAddress.Multiline = false;
            textBoxAddress.Name = "textBoxAddress";
            textBoxAddress.Padding = new Padding(8, 5, 8, 5);
            textBoxAddress.PasswordChar = false;
            textBoxAddress.PlaceHolderColor = Color.Gray;
            textBoxAddress.PlaceHolderText = "";
            textBoxAddress.ReadOnly = false;
            textBoxAddress.ScrollBars = ScrollBars.None;
            textBoxAddress.SelectionStart = 0;
            textBoxAddress.Size = new Size(250, 25);
            textBoxAddress.TabIndex = 3;
            textBoxAddress.TextAlignment = HorizontalAlignment.Left;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(20, 73);
            label5.Name = "label5";
            label5.Size = new Size(56, 16);
            label5.TabIndex = 2;
            label5.Text = "Endpoint";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 44);
            label4.Name = "label4";
            label4.Size = new Size(30, 16);
            label4.TabIndex = 1;
            label4.Text = "Port";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(20, 14);
            label3.Name = "label3";
            label3.Size = new Size(53, 16);
            label3.TabIndex = 0;
            label3.Text = "Address";
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.Transparent;
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(treeView1);
            tabPage2.ForeColor = Color.White;
            tabPage2.Location = new Point(64, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(730, 440);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Events";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(28, 6);
            label6.Name = "label6";
            label6.Size = new Size(171, 16);
            label6.TabIndex = 1;
            label6.Text = "Select events to subscribe to";
            // 
            // treeView1
            // 
            treeView1.BackColor = Color.FromArgb(64, 64, 64);
            treeView1.Font = new Font("Tahoma", 11F);
            treeView1.ForeColor = Color.White;
            treeView1.Indent = 24;
            treeView1.ItemHeight = 24;
            treeView1.Location = new Point(29, 30);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(695, 404);
            treeView1.TabIndex = 0;
            // 
            // ConfigurationDialogForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(verticalTabControl1);
            Name = "ConfigurationDialogForm";
            Text = "Macro Deck | Yet another Streamer.Bot Plugin";
            verticalTabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            roundedPanel3.ResumeLayout(false);
            roundedPanel3.PerformLayout();
            roundedPanel2.ResumeLayout(false);
            roundedPanel2.PerformLayout();
            roundedPanel1.ResumeLayout(false);
            roundedPanel1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SuchByte.MacroDeck.GUI.CustomControls.VerticalTabControl verticalTabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel roundedPanel2;
        private Label label2;
        private Label label1;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel roundedPanel1;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label7;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox textBoxAddress;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox textBoxEndpoint;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox textBoxPort;
        private CheckBox checkBox1;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox textBoxPassword;
        private Label label6;
        private TreeView treeView1;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel roundedPanel3;
        private Label label8;
        private SuchByte.MacroDeck.GUI.CustomControls.ButtonPrimary buttonPrimary1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
    }
}