using System.IO;
using System.Windows.Forms;

namespace PuppetMaster {
	partial class PuppetMasterForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.logTextBox = new System.Windows.Forms.TextBox();
			this.commandTextBox = new System.Windows.Forms.TextBox();
			this.loadConfigFileButton = new System.Windows.Forms.Button();
			this.startAllButton = new System.Windows.Forms.Button();
			this.startOneButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.commandButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.crashAllButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// logTextBox
			// 
			this.logTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.logTextBox.Location = new System.Drawing.Point(13, 13);
			this.logTextBox.Multiline = true;
			this.logTextBox.Name = "logTextBox";
			this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logTextBox.Size = new System.Drawing.Size(681, 294);
			this.logTextBox.TabIndex = 0;
			// 
			// commandTextBox
			// 
			this.commandTextBox.Location = new System.Drawing.Point(13, 313);
			this.commandTextBox.Name = "commandTextBox";
			this.commandTextBox.Size = new System.Drawing.Size(681, 20);
			this.commandTextBox.TabIndex = 1;
			// 
			// loadConfigFileButton
			// 
			this.loadConfigFileButton.Location = new System.Drawing.Point(700, 11);
			this.loadConfigFileButton.Name = "loadConfigFileButton";
			this.loadConfigFileButton.Size = new System.Drawing.Size(118, 45);
			this.loadConfigFileButton.TabIndex = 2;
			this.loadConfigFileButton.Text = "Load Config File";
			this.loadConfigFileButton.UseVisualStyleBackColor = true;
			this.loadConfigFileButton.Click += new System.EventHandler(this.loadConfigFileButton_Click);
			// 
			// startAllButton
			// 
			this.startAllButton.Location = new System.Drawing.Point(700, 89);
			this.startAllButton.Name = "startAllButton";
			this.startAllButton.Size = new System.Drawing.Size(118, 23);
			this.startAllButton.TabIndex = 3;
			this.startAllButton.Text = "Start All";
			this.startAllButton.UseVisualStyleBackColor = true;
			// 
			// startOneButton
			// 
			this.startOneButton.Location = new System.Drawing.Point(700, 118);
			this.startOneButton.Name = "startOneButton";
			this.startOneButton.Size = new System.Drawing.Size(118, 23);
			this.startOneButton.TabIndex = 4;
			this.startOneButton.Text = "Start One";
			this.startOneButton.UseVisualStyleBackColor = true;
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog1";
			this.openFileDialog.InitialDirectory = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE";
			this.openFileDialog.Title = "Choose Config";
			// 
			// commandButton
			// 
			this.commandButton.Location = new System.Drawing.Point(700, 311);
			this.commandButton.Name = "commandButton";
			this.commandButton.Size = new System.Drawing.Size(118, 23);
			this.commandButton.TabIndex = 5;
			this.commandButton.Text = "Enter";
			this.commandButton.UseVisualStyleBackColor = true;
			this.commandButton.Click += new System.EventHandler(this.commandButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(700, 68);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 18);
			this.label1.TabIndex = 6;
			this.label1.Text = "Operators:";
			// 
			// crashAllButton
			// 
			this.crashAllButton.Location = new System.Drawing.Point(700, 147);
			this.crashAllButton.Name = "crashAllButton";
			this.crashAllButton.Size = new System.Drawing.Size(118, 23);
			this.crashAllButton.TabIndex = 7;
			this.crashAllButton.Text = "Crash All";
			this.crashAllButton.UseVisualStyleBackColor = true;
			this.crashAllButton.Click += new System.EventHandler(this.crashAllButton_Click);
			// 
			// PuppetMasterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(830, 345);
			this.Controls.Add(this.crashAllButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.commandButton);
			this.Controls.Add(this.startOneButton);
			this.Controls.Add(this.startAllButton);
			this.Controls.Add(this.loadConfigFileButton);
			this.Controls.Add(this.commandTextBox);
			this.Controls.Add(this.logTextBox);
			this.Name = "PuppetMasterForm";
			this.Text = "Puppet Master - DADStorm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox logTextBox;
		private System.Windows.Forms.TextBox commandTextBox;
		private System.Windows.Forms.Button loadConfigFileButton;
		private System.Windows.Forms.Button startAllButton;
		private System.Windows.Forms.Button startOneButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private Button commandButton;
		private Label label1;
		private Button crashAllButton;
	}
}

