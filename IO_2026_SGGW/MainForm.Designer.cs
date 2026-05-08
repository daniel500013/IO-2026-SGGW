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
            this.lblOd = new System.Windows.Forms.Label();
            this.dtpOd = new System.Windows.Forms.DateTimePicker();
            this.lblDo = new System.Windows.Forms.Label();
            this.dtpDo = new System.Windows.Forms.DateTimePicker();
            this.btnFiltruj = new System.Windows.Forms.Button();
            this.btnEksportuj = new System.Windows.Forms.Button();
            this.mainProgressBar = new System.Windows.Forms.ProgressBar();
            this.panelDrop.SuspendLayout();
            this.SuspendLayout();
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
            this.lstResults.ItemHeight = 22;
            this.lstResults.Location = new System.Drawing.Point(18, 200);
            this.lstResults.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(1019, 375);
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
            // 
            // lblDropHint
            // 
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
            // 
            // lblOd
            // 
            this.lblOd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOd.AutoSize = true;
            this.lblOd.Location = new System.Drawing.Point(14, 588);
            this.lblOd.Name = "lblOd";
            this.lblOd.Size = new System.Drawing.Size(34, 20);
            this.lblOd.TabIndex = 3;
            this.lblOd.Text = "Od:";
            this.lblOd.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dtpOd
            // 
            this.dtpOd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dtpOd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOd.Location = new System.Drawing.Point(43, 584);
            this.dtpOd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dtpOd.Name = "dtpOd";
            this.dtpOd.Size = new System.Drawing.Size(123, 26);
            this.dtpOd.TabIndex = 4;
            this.dtpOd.Value = new System.DateTime(2026, 4, 1, 0, 0, 0, 0);
            // 
            // lblDo
            // 
            this.lblDo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDo.AutoSize = true;
            this.lblDo.Location = new System.Drawing.Point(178, 588);
            this.lblDo.Name = "lblDo";
            this.lblDo.Size = new System.Drawing.Size(34, 20);
            this.lblDo.TabIndex = 5;
            this.lblDo.Text = "Do:";
            // 
            // dtpDo
            // 
            this.dtpDo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dtpDo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDo.Location = new System.Drawing.Point(207, 584);
            this.dtpDo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dtpDo.Name = "dtpDo";
            this.dtpDo.Size = new System.Drawing.Size(123, 26);
            this.dtpDo.TabIndex = 6;
            this.dtpDo.Value = new System.DateTime(2026, 4, 30, 0, 0, 0, 0);
            // 
            // btnFiltruj
            // 
            this.btnFiltruj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFiltruj.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnFiltruj.FlatAppearance.BorderSize = 0;
            this.btnFiltruj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFiltruj.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFiltruj.ForeColor = System.Drawing.Color.White;
            this.btnFiltruj.Location = new System.Drawing.Point(681, 581);
            this.btnFiltruj.Name = "btnFiltruj";
            this.btnFiltruj.Size = new System.Drawing.Size(125, 35);
            this.btnFiltruj.TabIndex = 7;
            this.btnFiltruj.Text = "Filtruj";
            this.btnFiltruj.UseVisualStyleBackColor = false;
            this.btnFiltruj.Click += new System.EventHandler(this.btnFiltruj_Click);
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
            this.mainProgressBar.Name = "mainProgressBar";
            this.mainProgressBar.Size = new System.Drawing.Size(1032, 15);
            this.mainProgressBar.TabIndex = 9;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 725);
            this.Controls.Add(this.mainProgressBar);
            this.Controls.Add(this.btnEksportuj);
            this.Controls.Add(this.btnFiltruj);
            this.Controls.Add(this.dtpDo);
            this.Controls.Add(this.lblDo);
            this.Controls.Add(this.dtpOd);
            this.Controls.Add(this.lblOd);
            this.Controls.Add(this.lstResults);
            this.Controls.Add(this.lblStatusFiles);
            this.Controls.Add(this.btnSprawdz);
            this.Controls.Add(this.panelDrop);
            this.Controls.Add(this.btnWybierzXLSX);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Sprawdzanie Kolokwiów";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelDrop.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnFiltruj;
        private System.Windows.Forms.Button btnEksportuj;
        private System.Windows.Forms.ProgressBar mainProgressBar;
    }
}

