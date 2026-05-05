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
            this.lstResults = new System.Windows.Forms.ListBox();
            this.btnWybierzXLSX = new System.Windows.Forms.Button();
            this.btnSprawdz = new System.Windows.Forms.Button();
            this.lblStatusFiles = new System.Windows.Forms.Label();
            this.panelDrop = new System.Windows.Forms.Panel();
            this.lblDropHint = new System.Windows.Forms.Label();
            this.panelDrop.SuspendLayout();
            this.lblOd = new System.Windows.Forms.Label();
            this.dtpOd = new System.Windows.Forms.DateTimePicker();
            this.lblDo = new System.Windows.Forms.Label();
            this.dtpDo = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            
            // 
            // panelDrop
            // 
            this.panelDrop.AllowDrop = true;
            this.panelDrop.BackColor = System.Drawing.Color.White;
            this.panelDrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDrop.Controls.Add(this.lblDropHint);
            this.panelDrop.Location = new System.Drawing.Point(12, 12);
            this.panelDrop.Name = "panelDrop";
            this.panelDrop.Size = new System.Drawing.Size(400, 85);
            this.panelDrop.TabIndex = 3;
            // 
            // lblDropHint
            // 
            this.lblDropHint.BackColor = System.Drawing.Color.White;
            this.lblDropHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDropHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblDropHint.ForeColor = System.Drawing.Color.Gray;
            this.lblDropHint.Location = new System.Drawing.Point(0, 0);
            this.lblDropHint.Name = "lblDropHint";
            this.lblDropHint.Size = new System.Drawing.Size(398, 83);
            this.lblDropHint.TabIndex = 0;
            this.lblDropHint.Text = "Przeciągnij pliki .cs tutaj";
            this.lblDropHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // 
            // lstResults
            // 
            this.lstResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstResults.BackColor = System.Drawing.Color.White;
            this.lstResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstResults.Font = new System.Drawing.Font("Consolas", 9F);
            this.lstResults.ForeColor = System.Drawing.Color.Black;
            this.lstResults.FormattingEnabled = true;
            this.lstResults.HorizontalScrollbar = true;
            this.lstResults.IntegralHeight = false;
            this.lstResults.ItemHeight = 18;
            this.lstResults.Location = new System.Drawing.Point(16, 160);
            this.lstResults.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(906, 400);
            this.lstResults.TabIndex = 0;
            this.lstResults.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // btnWybierzXLSX
            // 
            this.btnWybierzXLSX.BackColor = System.Drawing.Color.White;
            this.btnWybierzXLSX.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnWybierzXLSX.FlatAppearance.BorderSize = 2;
            this.btnWybierzXLSX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWybierzXLSX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnWybierzXLSX.ForeColor = System.Drawing.Color.Black;
            this.btnWybierzXLSX.Location = new System.Drawing.Point(567, 25);
            this.btnWybierzXLSX.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnWybierzXLSX.Name = "btnWybierzXLSX";
            this.btnWybierzXLSX.Size = new System.Drawing.Size(213, 43);
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
            this.btnSprawdz.Location = new System.Drawing.Point(800, 25);
            this.btnSprawdz.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSprawdz.Name = "btnSprawdz";
            this.btnSprawdz.Size = new System.Drawing.Size(133, 43);
            this.btnSprawdz.TabIndex = 1;
            this.btnSprawdz.Text = "Sprawdź";
            this.btnSprawdz.UseVisualStyleBackColor = false;
            // 
            // lblStatusFiles
            // 
            this.lblStatusFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblStatusFiles.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatusFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblStatusFiles.Location = new System.Drawing.Point(567, 74);
            this.lblStatusFiles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusFiles.Name = "lblStatusFiles";
            this.lblStatusFiles.Size = new System.Drawing.Size(253, 18);
            this.lblStatusFiles.TabIndex = 2;
            this.lblStatusFiles.Text = "Załadowane pliki: 0  |  XLSX: (brak)";
            this.lblStatusFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatusFiles.Click += new System.EventHandler(this.lblStatusFiles_Click);
            // 
            // lblOd
            // 
            this.lblOd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOd.AutoSize = true;
            this.lblOd.Location = new System.Drawing.Point(12, 470);
            this.lblOd.Name = "lblOd";
            this.lblOd.Size = new System.Drawing.Size(28, 16);
            this.lblOd.TabIndex = 3;
            this.lblOd.Text = "Od:";
            this.lblOd.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dtpOd
            // 
            this.dtpOd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dtpOd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOd.Location = new System.Drawing.Point(38, 467);
            this.dtpOd.Name = "dtpOd";
            this.dtpOd.Size = new System.Drawing.Size(110, 22);
            this.dtpOd.TabIndex = 4;
            this.dtpOd.Value = new System.DateTime(2026, 4, 1, 0, 0, 0, 0);
            // 
            // lblDo
            // 
            this.lblDo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDo.AutoSize = true;
            this.lblDo.Location = new System.Drawing.Point(158, 470);
            this.lblDo.Name = "lblDo";
            this.lblDo.Size = new System.Drawing.Size(28, 16);
            this.lblDo.TabIndex = 5;
            this.lblDo.Text = "Do:";
            // 
            // dtpDo
            // 
            this.dtpDo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dtpDo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDo.Location = new System.Drawing.Point(184, 467);
            this.dtpDo.Name = "dtpDo";
            this.dtpDo.Size = new System.Drawing.Size(110, 22);
            this.dtpDo.TabIndex = 6;
            this.dtpDo.Value = new System.DateTime(2026, 4, 30, 0, 0, 0, 0);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 580);
            this.Controls.Add(this.dtpDo);
            this.Controls.Add(this.lblDo);
            this.Controls.Add(this.dtpOd);
            this.Controls.Add(this.lblOd);
            this.Controls.Add(this.lstResults);
            this.Controls.Add(this.lblStatusFiles);
            this.Controls.Add(this.btnSprawdz);
            this.Controls.Add(this.panelDrop);
            this.Controls.Add(this.btnWybierzXLSX);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "Sprawdzanie Kolokwiów";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstResults;
        private System.Windows.Forms.Panel panelDrop;
        private System.Windows.Forms.Label lblDropHint;
        private System.Windows.Forms.Button btnWybierzXLSX;
        private System.Windows.Forms.Button btnSprawdz;
        private System.Windows.Forms.Label lblStatusFiles;
        private System.Windows.Forms.Label lblOd;
        private System.Windows.Forms.DateTimePicker dtpOd;
        private System.Windows.Forms.Label lblDo;
        private System.Windows.Forms.DateTimePicker dtpDo;
    }
}

