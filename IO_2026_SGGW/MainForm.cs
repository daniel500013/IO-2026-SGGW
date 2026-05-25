using IO_2026_SGGW.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IO_2026_SGGW
{
    public partial class MainForm : Form
    {
        private readonly List<StudentSolution> studentSolutions = new
        List<StudentSolution>();
        private string xlsxPath;
        private AnswerKey answerKey;

        public MainForm()
        {
            InitializeComponent();
            SetupGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Excel files (*.xlsx)|*.xlsx", Title = "Wybierz plik z kluczem odpowiedzi" })
            {
                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    answerKey = new AnswerKeyLoader().Load(ofd.FileName);
                    xlsxPath = ofd.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd wczytywania klucza:\n" + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    answerKey = null;
                    xlsxPath = null;
                }

                UpdateStatusBar(); // metoda z Zadania 1
            }
        }

        private void lblStatusFiles_Click(object sender, EventArgs e)
        {
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        
        private void panelDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                bool hasCsFile = false;
                foreach (var path in paths)
                {
                    if (path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    {
                        hasCsFile = true;
                        break;
                    }
                }
                if (hasCsFile)
                {
                    e.Effect = DragDropEffects.Copy;
                    panelDrop.BackColor = Color.LightGreen;
                    lblDropHint.BackColor = Color.LightGreen;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
            panelDrop.BackColor = Color.White;
            lblDropHint.BackColor = Color.White;
        }
        private void panelDrop_DragLeave(object sender, EventArgs e)
        {
            panelDrop.BackColor = Color.White;
            lblDropHint.BackColor = Color.White;
        }
        private void panelDrop_DragDrop(object sender, DragEventArgs e)
        {
            panelDrop.BackColor = Color.White;
            lblDropHint.BackColor = Color.White;
            var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            AddCsFiles(paths);
        }
        private void AddCsFiles(string[] paths)
        {
            foreach (var path in paths)
            {
                if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (studentSolutions.Exists(s => s.FilePath == path)) continue;
                studentSolutions.Add(new StudentSolution
                {
                    StudentId = Path.GetFileNameWithoutExtension(path),
                    FilePath = path,
                    SourceCode = File.ReadAllText(path),
                    LastModified = File.GetLastWriteTime(path)
                });
            }
            UpdateStatusBar();
        }


        private void btnWybierzPlikiCs_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog
            {
                Filter = "C# files (*.cs)|*.cs",
                Multiselect = true
            })
            {
                if (ofd.ShowDialog() != DialogResult.OK) return;
                AddCsFiles(ofd.FileNames);
            }
        }

        private void btnFiltruj_Click(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }
        private List<StudentSolution> GetInRangeSolutions()
        {
            var from = dtpOd.Value.Date;
            var to = dtpDo.Value.Date;
            var result = new List<StudentSolution>();
            foreach (var s in studentSolutions)
            {
                var d = s.LastModified.Date;
                if (d >= from && d <= to) result.Add(s);
            }
            return result;
        }
        private void UpdateStatusBar()
        {
            if (lblStatusFiles.InvokeRequired)
            {
                lblStatusFiles.Invoke(new Action(UpdateStatusBar));
                return;
            }

            string xlsxName = string.IsNullOrEmpty(xlsxPath) ? "(brak)" : Path.GetFileName(xlsxPath);
            lblStatusFiles.Text = $"Załadowane: {studentSolutions.Count} | W zakresie: {GetInRangeSolutions().Count} | XLSX: {xlsxName}";
        }
        private void btnEksportuj_Click(object sender, EventArgs e)
        {
            if (resultsList.Count == 0)
            {
                MessageBox.Show("Brak wyników do eksportu", "Brak danych");
                return;
            }
            using (var sfd = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = "wyniki_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") +
            ".xlsx"
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                try
                {
                    new ResultsExporter().Export(resultsList, sfd.FileName);
                    if (MessageBox.Show("Zapisano. Otworzyć folder?", "Sukces",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                        Process.Start("explorer.exe", "/select,\"" + sfd.FileName +
                        "\"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd zapisu:\n" + ex.Message, "Błąd");
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void SetupCustomUI()
        {
            // ToolTipy dla kontrolek
            ToolTip toolTip = new ToolTip();

            // Przyciski
            toolTip.SetToolTip(btnWybierzXLSX, "Wybierz plik referencyjny z rozszerzeniem .xlsx");
            toolTip.SetToolTip(btnSprawdz, "Uruchom weryfikację załadowanych plików .cs");
            toolTip.SetToolTip(btnFiltruj, "Zastosuj filtr czasowy do listy wyników");
            toolTip.SetToolTip(btnEksportuj, "Zapisz obecne wyniki do nowego pliku Excel");

            // Pickery dat i Drop Zone
            toolTip.SetToolTip(dtpOd, "Wybierz datę początkową");
            toolTip.SetToolTip(dtpDo, "Wybierz datę końcową");
            toolTip.SetToolTip(panelDrop, "Przeciągnij i upuść pliki studentów (.cs) tutaj");

            // Menu kontekstowe dla lstResults
            ContextMenuStrip resultsMenu = new ContextMenuStrip();


            // Kopiuj zaznaczone (Skrót: Ctrl+C)
            ToolStripMenuItem itemCopy = new ToolStripMenuItem("Kopiuj zaznaczone");
            itemCopy.ShortcutKeys = Keys.Control | Keys.C;
            itemCopy.Click += (sender, e) => { };

            // Zaznacz wszystko (Skrót: Ctrl+A)
            ToolStripMenuItem itemSelectAll = new ToolStripMenuItem("Zaznacz wszystko");
            itemSelectAll.ShortcutKeys = Keys.Control | Keys.A;
            itemSelectAll.Click += (sender, e) =>
            {
                // Zaznacza wszystko tylko, jeśli właściwość SelectionMode listy pozwala na wielokrotny wybór
                //USUNIĘTE LST RESULTS
            };

            // 3. Separator (pozioma linia oddzielająca)
            ToolStripSeparator separator = new ToolStripSeparator();

            // 4. Wyczyść listę
            ToolStripMenuItem itemClear = new ToolStripMenuItem("Wyczyść listę");
            itemClear.Click += (sender, e) =>
            {
                // Czyści listę. Uwaga od Gemini: jeśli lista używa DataSource, to wyrzuci wyjątek, 
                // wtedy trzeba by wyczyścić samo źródło (np. listę w tle).
            };

            // Dodanie elementów do menu w odpowiedniej kolejności
            resultsMenu.Items.Add(itemCopy);
            resultsMenu.Items.Add(itemSelectAll);
            resultsMenu.Items.Add(separator);
            resultsMenu.Items.Add(itemClear);

            // Przypięcie gotowego menu do kontrolki ListBox
        }

        private readonly BindingList<ResultRow> resultsList = new
            BindingList<ResultRow>();


        private void SetupGrid()
        {
            dgvResults.AutoGenerateColumns = false;
            dgvResults.Columns.Clear();
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName =
                    "Student",
                HeaderText = "Student", FillWeight = 15
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName =
                    "Zadanie",
                HeaderText = "Zadanie", FillWeight = 12
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName =
                    "Parametry",
                HeaderText = "Parametry", FillWeight = 15
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName =
                    "Oczekiwany",
                HeaderText = "Oczekiwany", FillWeight = 12
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName =
                    "Uzyskany",
                HeaderText = "Uzyskany", FillWeight = 25
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName =
                    "Punkty",
                HeaderText = "Pkt", FillWeight = 6
            });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName =
                    "Status",
                HeaderText = "Status", FillWeight = 15
            });
            dgvResults.DataSource = resultsList;
        }

        private void dgvResults_CellFormatting(object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= resultsList.Count) return;
            var row = resultsList[e.RowIndex];
            Color color;
            switch (row.Status)
            {
                case RunStatus.Ok: color = Color.LightGreen; break;
                case RunStatus.Bledny: color = Color.LightCoral; break;
                case RunStatus.Timeout: color = Color.Orange; break;
                case RunStatus.BrakMetody: color = Color.LightGray; break;
                case RunStatus.Wyjatek: color = Color.LightYellow; break;
                case RunStatus.BladKompilacji: color = Color.DarkGray; break;
                case RunStatus.ZlyFormatParametrow: color = Color.LightGray; break;
                default: color = Color.White; break;
            }

            dgvResults.Rows[e.RowIndex].DefaultCellStyle.BackColor = color;
        }

        
    }
}