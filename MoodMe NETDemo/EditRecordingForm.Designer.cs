namespace MoodMe_NETDemo
{
    partial class EditRecordingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditRecordingForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BTNCancel = new System.Windows.Forms.Button();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.LBLSelectedRecording = new System.Windows.Forms.Label();
            this.BTNSubmitRecording = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TagTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BTNCancel);
            this.groupBox1.Controls.Add(this.axWindowsMediaPlayer1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.LBLSelectedRecording);
            this.groupBox1.Controls.Add(this.BTNSubmitRecording);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.TagTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 426);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit Recording";
            // 
            // BTNCancel
            // 
            this.BTNCancel.Location = new System.Drawing.Point(289, 393);
            this.BTNCancel.Margin = new System.Windows.Forms.Padding(2);
            this.BTNCancel.Name = "BTNCancel";
            this.BTNCancel.Size = new System.Drawing.Size(56, 28);
            this.BTNCancel.TabIndex = 26;
            this.BTNCancel.Text = "Cancel";
            this.BTNCancel.UseVisualStyleBackColor = true;
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(6, 37);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(320, 221);
            this.axWindowsMediaPlayer1.TabIndex = 25;
            // 
            // textBox1
            // 
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBox1.Enabled = false;
            this.textBox1.ForeColor = System.Drawing.Color.IndianRed;
            this.textBox1.Location = new System.Drawing.Point(81, 305);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(125, 20);
            this.textBox1.TabIndex = 23;
            this.textBox1.Text = "Field cannot be empty";
            // 
            // LBLSelectedRecording
            // 
            this.LBLSelectedRecording.AutoSize = true;
            this.LBLSelectedRecording.Location = new System.Drawing.Point(80, 276);
            this.LBLSelectedRecording.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LBLSelectedRecording.Name = "LBLSelectedRecording";
            this.LBLSelectedRecording.Size = new System.Drawing.Size(78, 13);
            this.LBLSelectedRecording.TabIndex = 22;
            this.LBLSelectedRecording.Text = "None Selected";
            // 
            // BTNSubmitRecording
            // 
            this.BTNSubmitRecording.Enabled = false;
            this.BTNSubmitRecording.Location = new System.Drawing.Point(229, 393);
            this.BTNSubmitRecording.Margin = new System.Windows.Forms.Padding(2);
            this.BTNSubmitRecording.Name = "BTNSubmitRecording";
            this.BTNSubmitRecording.Size = new System.Drawing.Size(56, 28);
            this.BTNSubmitRecording.TabIndex = 21;
            this.BTNSubmitRecording.Text = "Submit";
            this.BTNSubmitRecording.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 294);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Tag:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 276);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Selected Item:";
            // 
            // TagTextBox
            // 
            this.TagTextBox.Location = new System.Drawing.Point(81, 291);
            this.TagTextBox.Name = "TagTextBox";
            this.TagTextBox.Size = new System.Drawing.Size(125, 20);
            this.TagTextBox.TabIndex = 18;
            // 
            // EditRecordingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 450);
            this.Controls.Add(this.groupBox1);
            this.Name = "EditRecordingForm";
            this.Text = "EditRecordingForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label LBLSelectedRecording;
        private System.Windows.Forms.Button BTNSubmitRecording;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TagTextBox;
        private System.Windows.Forms.Button BTNCancel;
    }
}