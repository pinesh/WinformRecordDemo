
using System;
using System.IO;

namespace MoodMe_NETDemo
{
    partial class BaseForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.DataGridViewRecordings = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BTNNewRecording = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewRecordings)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            // 
            // DataGridViewRecordings
            // 
            this.DataGridViewRecordings.AllowUserToAddRows = false;
            this.DataGridViewRecordings.AllowUserToDeleteRows = false;
            this.DataGridViewRecordings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewRecordings.Location = new System.Drawing.Point(4, 17);
            this.DataGridViewRecordings.Margin = new System.Windows.Forms.Padding(2);
            this.DataGridViewRecordings.MultiSelect = false;
            this.DataGridViewRecordings.Name = "DataGridViewRecordings";
            this.DataGridViewRecordings.ReadOnly = true;
            this.DataGridViewRecordings.RowHeadersVisible = false;
            this.DataGridViewRecordings.RowHeadersWidth = 82;
            this.DataGridViewRecordings.RowTemplate.Height = 33;
            this.DataGridViewRecordings.Size = new System.Drawing.Size(454, 292);
            this.DataGridViewRecordings.TabIndex = 3;
            this.DataGridViewRecordings.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BTNNewRecording);
            this.groupBox1.Controls.Add(this.DataGridViewRecordings);
            this.groupBox1.Location = new System.Drawing.Point(16, 24);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(462, 379);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Recording Manager";
            // 
            // BTNNewRecording
            // 
            this.BTNNewRecording.Location = new System.Drawing.Point(4, 313);
            this.BTNNewRecording.Margin = new System.Windows.Forms.Padding(2);
            this.BTNNewRecording.Name = "BTNNewRecording";
            this.BTNNewRecording.Size = new System.Drawing.Size(454, 53);
            this.BTNNewRecording.TabIndex = 25;
            this.BTNNewRecording.Text = "Create";
            this.BTNNewRecording.UseVisualStyleBackColor = true;
            this.BTNNewRecording.Click += new System.EventHandler(this.BTNNewRecording_Click_1);
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 407);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "BaseForm";
            this.Text = "MoodMe Demo";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewRecordings)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

      

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView DataGridViewRecordings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BTNNewRecording;
    }
}

