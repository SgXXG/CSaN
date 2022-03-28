
namespace Pop3Client
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textField = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.textFieldServerAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textFieldCommand = new System.Windows.Forms.TextBox();
            this.btnSendCommand = new System.Windows.Forms.Button();
            this.textFieldPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textField
            // 
            this.textField.Location = new System.Drawing.Point(12, 234);
            this.textField.Multiline = true;
            this.textField.Name = "textField";
            this.textField.ReadOnly = true;
            this.textField.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textField.Size = new System.Drawing.Size(542, 393);
            this.textField.TabIndex = 0;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 149);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(542, 29);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect to server";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // textFieldServerAddress
            // 
            this.textFieldServerAddress.Location = new System.Drawing.Point(174, 27);
            this.textFieldServerAddress.Name = "textFieldServerAddress";
            this.textFieldServerAddress.Size = new System.Drawing.Size(380, 27);
            this.textFieldServerAddress.TabIndex = 0;
            this.textFieldServerAddress.TextChanged += new System.EventHandler(this.textFieldServerAddress_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "POP3-server address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Command:";
            // 
            // textFieldCommand
            // 
            this.textFieldCommand.Location = new System.Drawing.Point(174, 93);
            this.textFieldCommand.Name = "textFieldCommand";
            this.textFieldCommand.Size = new System.Drawing.Size(380, 27);
            this.textFieldCommand.TabIndex = 1;
            // 
            // btnSendCommand
            // 
            this.btnSendCommand.Location = new System.Drawing.Point(12, 184);
            this.btnSendCommand.Name = "btnSendCommand";
            this.btnSendCommand.Size = new System.Drawing.Size(542, 29);
            this.btnSendCommand.TabIndex = 3;
            this.btnSendCommand.Text = "Send command";
            this.btnSendCommand.UseVisualStyleBackColor = true;
            this.btnSendCommand.Click += new System.EventHandler(this.btnSendCommand_Click);
            // 
            // textFieldPort
            // 
            this.textFieldPort.Location = new System.Drawing.Point(174, 60);
            this.textFieldPort.Name = "textFieldPort";
            this.textFieldPort.Size = new System.Drawing.Size(380, 27);
            this.textFieldPort.TabIndex = 6;
            this.textFieldPort.TextChanged += new System.EventHandler(this.textFieldPort_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Port:";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(528, 219);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(26, 15);
            this.btnClear.TabIndex = 8;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 634);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.textFieldPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSendCommand);
            this.Controls.Add(this.textFieldCommand);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textFieldServerAddress);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.textField);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pop3Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textField;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox textFieldServerAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textFieldCommand;
        private System.Windows.Forms.Button btnSendCommand;
        private System.Windows.Forms.TextBox textFieldPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnClear;
    }
}

