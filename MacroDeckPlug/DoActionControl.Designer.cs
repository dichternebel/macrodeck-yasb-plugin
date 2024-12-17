namespace dichternebel.YaSB.MacroDeckPlug
{
    partial class DoActionControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            roundedPanel1 = new SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel();
            label3 = new Label();
            label2 = new Label();
            comboBox2 = new ComboBox();
            comboBox1 = new ComboBox();
            buttonPrimary1 = new SuchByte.MacroDeck.GUI.CustomControls.ButtonPrimary();
            label1 = new Label();
            roundedPanel2 = new SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel();
            label4 = new Label();
            labelArgument = new Label();
            labelActionName = new Label();
            labelActionId = new Label();
            roundedTextBoxActionId = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            roundedTextBoxActionName = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            roundedTextBoxArgument = new SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox();
            roundedPanel1.SuspendLayout();
            roundedPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // roundedPanel1
            // 
            roundedPanel1.BackColor = Color.FromArgb(36, 36, 36);
            roundedPanel1.Controls.Add(label3);
            roundedPanel1.Controls.Add(label2);
            roundedPanel1.Controls.Add(comboBox2);
            roundedPanel1.Controls.Add(comboBox1);
            roundedPanel1.Controls.Add(buttonPrimary1);
            roundedPanel1.Location = new Point(30, 41);
            roundedPanel1.Name = "roundedPanel1";
            roundedPanel1.Size = new Size(799, 89);
            roundedPanel1.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Tahoma", 9F);
            label3.Location = new Point(348, 13);
            label3.Name = "label3";
            label3.Size = new Size(47, 14);
            label3.TabIndex = 5;
            label3.Text = "Actions";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Tahoma", 9F);
            label2.Location = new Point(20, 13);
            label2.Name = "label2";
            label2.Size = new Size(45, 14);
            label2.TabIndex = 4;
            label2.Text = "Groups";
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(349, 34);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(310, 31);
            comboBox2.TabIndex = 3;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(19, 34);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(310, 31);
            comboBox1.TabIndex = 2;
            // 
            // buttonPrimary1
            // 
            buttonPrimary1.BorderRadius = 8;
            buttonPrimary1.FlatAppearance.BorderSize = 0;
            buttonPrimary1.FlatStyle = FlatStyle.Flat;
            buttonPrimary1.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonPrimary1.ForeColor = Color.White;
            buttonPrimary1.HoverColor = Color.Empty;
            buttonPrimary1.Icon = null;
            buttonPrimary1.Location = new Point(677, 34);
            buttonPrimary1.Name = "buttonPrimary1";
            buttonPrimary1.Progress = 0;
            buttonPrimary1.ProgressColor = Color.FromArgb(0, 103, 205);
            buttonPrimary1.Size = new Size(99, 31);
            buttonPrimary1.TabIndex = 1;
            buttonPrimary1.Text = "Use";
            buttonPrimary1.UseVisualStyleBackColor = true;
            buttonPrimary1.UseWindowsAccentColor = true;
            buttonPrimary1.WriteProgress = true;
            buttonPrimary1.Click += buttonPrimary1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(49, 13);
            label1.Name = "label1";
            label1.Size = new Size(211, 23);
            label1.TabIndex = 1;
            label1.Text = "Select action to be used";
            // 
            // roundedPanel2
            // 
            roundedPanel2.BackColor = Color.FromArgb(36, 36, 36);
            roundedPanel2.Controls.Add(roundedTextBoxArgument);
            roundedPanel2.Controls.Add(roundedTextBoxActionName);
            roundedPanel2.Controls.Add(roundedTextBoxActionId);
            roundedPanel2.Controls.Add(label4);
            roundedPanel2.Controls.Add(labelArgument);
            roundedPanel2.Controls.Add(labelActionName);
            roundedPanel2.Controls.Add(labelActionId);
            roundedPanel2.Location = new Point(30, 174);
            roundedPanel2.Name = "roundedPanel2";
            roundedPanel2.Size = new Size(799, 237);
            roundedPanel2.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Tahoma", 8F);
            label4.Location = new Point(20, 217);
            label4.Name = "label4";
            label4.Size = new Size(65, 13);
            label4.TabIndex = 3;
            label4.Text = "*mandatory";
            // 
            // labelArgument
            // 
            labelArgument.AutoSize = true;
            labelArgument.Location = new Point(20, 137);
            labelArgument.Name = "labelArgument";
            labelArgument.Size = new Size(93, 23);
            labelArgument.TabIndex = 2;
            labelArgument.Text = "Argument";
            // 
            // labelActionName
            // 
            labelActionName.AutoSize = true;
            labelActionName.Location = new Point(19, 79);
            labelActionName.Name = "labelActionName";
            labelActionName.Size = new Size(126, 23);
            labelActionName.TabIndex = 1;
            labelActionName.Text = "Action Name*";
            // 
            // labelActionId
            // 
            labelActionId.AutoSize = true;
            labelActionId.Location = new Point(19, 24);
            labelActionId.Name = "labelActionId";
            labelActionId.Size = new Size(97, 23);
            labelActionId.TabIndex = 0;
            labelActionId.Text = "Action ID*";
            // 
            // roundedTextBoxActionId
            // 
            roundedTextBoxActionId.BackColor = Color.FromArgb(65, 65, 65);
            roundedTextBoxActionId.Font = new Font("Tahoma", 9F);
            roundedTextBoxActionId.Icon = null;
            roundedTextBoxActionId.Location = new Point(162, 22);
            roundedTextBoxActionId.MaxCharacters = 32767;
            roundedTextBoxActionId.Multiline = false;
            roundedTextBoxActionId.Name = "roundedTextBoxActionId";
            roundedTextBoxActionId.Padding = new Padding(8, 5, 8, 5);
            roundedTextBoxActionId.PasswordChar = false;
            roundedTextBoxActionId.PlaceHolderColor = Color.Gray;
            roundedTextBoxActionId.PlaceHolderText = "";
            roundedTextBoxActionId.ReadOnly = false;
            roundedTextBoxActionId.ScrollBars = ScrollBars.None;
            roundedTextBoxActionId.SelectionStart = 0;
            roundedTextBoxActionId.Size = new Size(497, 25);
            roundedTextBoxActionId.TabIndex = 4;
            roundedTextBoxActionId.TextAlignment = HorizontalAlignment.Left;
            // 
            // roundedTextBoxActionName
            // 
            roundedTextBoxActionName.BackColor = Color.FromArgb(65, 65, 65);
            roundedTextBoxActionName.Font = new Font("Tahoma", 9F);
            roundedTextBoxActionName.Icon = null;
            roundedTextBoxActionName.Location = new Point(162, 77);
            roundedTextBoxActionName.MaxCharacters = 32767;
            roundedTextBoxActionName.Multiline = false;
            roundedTextBoxActionName.Name = "roundedTextBoxActionName";
            roundedTextBoxActionName.Padding = new Padding(8, 5, 8, 5);
            roundedTextBoxActionName.PasswordChar = false;
            roundedTextBoxActionName.PlaceHolderColor = Color.Gray;
            roundedTextBoxActionName.PlaceHolderText = "";
            roundedTextBoxActionName.ReadOnly = false;
            roundedTextBoxActionName.ScrollBars = ScrollBars.None;
            roundedTextBoxActionName.SelectionStart = 0;
            roundedTextBoxActionName.Size = new Size(497, 25);
            roundedTextBoxActionName.TabIndex = 5;
            roundedTextBoxActionName.TextAlignment = HorizontalAlignment.Left;
            // 
            // roundedTextBoxArgument
            // 
            roundedTextBoxArgument.BackColor = Color.FromArgb(65, 65, 65);
            roundedTextBoxArgument.Font = new Font("Tahoma", 9F);
            roundedTextBoxArgument.Icon = null;
            roundedTextBoxArgument.Location = new Point(162, 137);
            roundedTextBoxArgument.MaxCharacters = 32767;
            roundedTextBoxArgument.Multiline = false;
            roundedTextBoxArgument.Name = "roundedTextBoxArgument";
            roundedTextBoxArgument.Padding = new Padding(8, 5, 8, 5);
            roundedTextBoxArgument.PasswordChar = false;
            roundedTextBoxArgument.PlaceHolderColor = Color.Gray;
            roundedTextBoxArgument.PlaceHolderText = "";
            roundedTextBoxArgument.ReadOnly = false;
            roundedTextBoxArgument.ScrollBars = ScrollBars.None;
            roundedTextBoxArgument.SelectionStart = 0;
            roundedTextBoxArgument.Size = new Size(497, 25);
            roundedTextBoxArgument.TabIndex = 6;
            roundedTextBoxArgument.TextAlignment = HorizontalAlignment.Left;
            // 
            // DoActionControl
            // 
            AutoScaleDimensions = new SizeF(10F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(roundedPanel2);
            Controls.Add(label1);
            Controls.Add(roundedPanel1);
            Name = "DoActionControl";
            roundedPanel1.ResumeLayout(false);
            roundedPanel1.PerformLayout();
            roundedPanel2.ResumeLayout(false);
            roundedPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel roundedPanel1;
        private SuchByte.MacroDeck.GUI.CustomControls.ButtonPrimary buttonPrimary1;
        private Label label1;
        private ComboBox comboBox1;
        private Label label3;
        private Label label2;
        private ComboBox comboBox2;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedPanel roundedPanel2;
        private Label labelActionId;
        private Label labelActionName;
        private Label label4;
        private Label labelArgument;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox roundedTextBoxArgument;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox roundedTextBoxActionName;
        private SuchByte.MacroDeck.GUI.CustomControls.RoundedTextBox roundedTextBoxActionId;
    }
}
