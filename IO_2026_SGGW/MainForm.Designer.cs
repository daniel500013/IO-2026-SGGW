using System.Drawing;
using System.Windows.Forms;

namespace IO_2026_SGGW
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnWybierzXLSX = new System.Windows.Forms.Button();
            this.btnSprawdz = new System.Windows.Forms.Button();
            this.lblStatusFiles = new System.Windows.Forms.Label();
            this.panelDrop = new System.Windows.Forms.Panel();
            this.lblDropHint = new System.Windows.Forms.Label();
            this.btnEksportuj = new System.Windows.Forms.Button();
            this.mainProgressBar = new System.Windows.Forms.ProgressBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.panelDrop.SuspendLayout();
            this.SuspendLayout();
            // 
            // dvgResults
            // 
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            this.dgvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResults.Location = new System.Drawing.Point(18, 200);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.ReadOnly = true;
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResults.Size = new System.Drawing.Size(1019, 375);
            this.dgvResults.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvResults_CellFormatting);
            //
            // lblTimeout
            //
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(638, 130);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Text = "Timeout (s):";
            //
            // numTimeout
            //
            this.numTimeout.Location = new System.Drawing.Point(733, 128);
            this.numTimeout.Minimum = 1;
            this.numTimeout.Maximum = 60;
            this.numTimeout.Value = 3;
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(60, 26);
            // 
            // btnWybierzXLSX
            // 
            this.btnWybierzXLSX.BackColor = System.Drawing.Color.White;
            this.btnWybierzXLSX.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnWybierzXLSX.FlatAppearance.BorderSize = 2;
            this.btnWybierzXLSX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWybierzXLSX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnWybierzXLSX.ForeColor = System.Drawing.Color.Black;
            this.btnWybierzXLSX.Location = new System.Drawing.Point(638, 31);
            this.btnWybierzXLSX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnWybierzXLSX.Name = "btnWybierzXLSX";
            this.btnWybierzXLSX.Size = new System.Drawing.Size(240, 54);
            this.btnWybierzXLSX.TabIndex = 0;
            this.btnWybierzXLSX.Text = "Wybierz plik XLSX";
            this.btnWybierzXLSX.UseVisualStyleBackColor = false;
            this.btnWybierzXLSX.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSprawdz
            // 
            this.btnSprawdz.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnSprawdz.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnSprawdz.FlatAppearance.BorderSize = 2;
            this.btnSprawdz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSprawdz.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnSprawdz.ForeColor = System.Drawing.Color.White;
            this.btnSprawdz.Location = new System.Drawing.Point(900, 31);
            this.btnSprawdz.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSprawdz.Name = "btnSprawdz";
            this.btnSprawdz.Size = new System.Drawing.Size(150, 54);
            this.btnSprawdz.TabIndex = 1;
            this.btnSprawdz.Text = "Sprawdź";
            this.btnSprawdz.UseVisualStyleBackColor = false;
            this.btnSprawdz.Click += new System.EventHandler(this.btnSprawdz_Click);
            //
            // lblStatusFiles
            // 
            this.lblStatusFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblStatusFiles.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatusFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStatusFiles.Location = new System.Drawing.Point(638, 92);
            this.lblStatusFiles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusFiles.Name = "lblStatusFiles";
            this.lblStatusFiles.Size = new System.Drawing.Size(285, 22);
            this.lblStatusFiles.TabIndex = 2;
            this.lblStatusFiles.Text = "Załadowane pliki: 0  |  XLSX: (brak)";
            this.lblStatusFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatusFiles.Click += new System.EventHandler(this.lblStatusFiles_Click);
            // 
            // panelDrop
            // 
            this.panelDrop.AllowDrop = true;
            this.panelDrop.BackColor = System.Drawing.Color.White;
            this.panelDrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDrop.Controls.Add(this.lblDropHint);
            this.panelDrop.Location = new System.Drawing.Point(14, 15);
            this.panelDrop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelDrop.Name = "panelDrop";
            this.panelDrop.Size = new System.Drawing.Size(450, 106);
            this.panelDrop.TabIndex = 3;
            this.panelDrop.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelDrop_DragDrop);
            this.panelDrop.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelDrop_DragEnter);
            this.panelDrop.DragLeave += new System.EventHandler(this.panelDrop_DragLeave);
            // 
            // lblDropHint
            // 
            this.lblDropHint.AllowDrop = true;
            this.lblDropHint.BackColor = System.Drawing.Color.White;
            this.lblDropHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDropHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblDropHint.ForeColor = System.Drawing.Color.Gray;
            this.lblDropHint.Location = new System.Drawing.Point(0, 0);
            this.lblDropHint.Name = "lblDropHint";
            this.lblDropHint.Size = new System.Drawing.Size(448, 104);
            this.lblDropHint.TabIndex = 0;
            this.lblDropHint.Text = "Przeciągnij pliki .cs tutaj";
            this.lblDropHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDropHint.Click += new System.EventHandler(this.btnWybierzPlikiCs_Click);
            this.lblDropHint.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelDrop_DragDrop);
            this.lblDropHint.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelDrop_DragEnter);
            this.lblDropHint.DragLeave += new System.EventHandler(this.panelDrop_DragLeave);
            //
            // btnEksportuj
            // 
            this.btnEksportuj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEksportuj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnEksportuj.FlatAppearance.BorderSize = 0;
            this.btnEksportuj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEksportuj.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEksportuj.ForeColor = System.Drawing.Color.White;
            this.btnEksportuj.Location = new System.Drawing.Point(812, 581);
            this.btnEksportuj.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEksportuj.Name = "btnEksportuj";
            this.btnEksportuj.Size = new System.Drawing.Size(225, 35);
            this.btnEksportuj.TabIndex = 8;
            this.btnEksportuj.Text = "Eksportuj do .xlsx";
            this.btnEksportuj.UseVisualStyleBackColor = false;
            this.btnEksportuj.Click += new System.EventHandler(this.btnEksportuj_Click);
            // 
            // mainProgressBar
            // 
            this.mainProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainProgressBar.Location = new System.Drawing.Point(12, 165);
            this.mainProgressBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.mainProgressBar.Name = "mainProgressBar";
            this.mainProgressBar.Size = new System.Drawing.Size(1032, 15);
            this.mainProgressBar.TabIndex = 9;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(939, 25);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
           // this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClick 
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 725);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.lblTimeout);
            this.Controls.Add(this.numTimeout);
            this.Controls.Add(this.mainProgressBar);
            this.Controls.Add(this.btnEksportuj);
            
            this.Controls.Add(this.lblStatusFiles);
            this.Controls.Add(this.btnSprawdz);
            this.Controls.Add(this.panelDrop);
            this.Controls.Add(this.btnWybierzXLSX);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Sprawdzanie Kolokwiów";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            this.panelDrop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ToolStrip toolStrip1;

        #endregion

        private System.Windows.Forms.DataGridView dgvResults; 
        private System.Windows.Forms.Label lblTimeout; 
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.Panel panelDrop;
        private System.Windows.Forms.Label lblDropHint;
        private System.Windows.Forms.Button btnWybierzXLSX;
        private System.Windows.Forms.Button btnSprawdz;
        private System.Windows.Forms.Label lblStatusFiles;
        private System.Windows.Forms.Button btnEksportuj;
        private System.Windows.Forms.ProgressBar mainProgressBar;
    }
}

