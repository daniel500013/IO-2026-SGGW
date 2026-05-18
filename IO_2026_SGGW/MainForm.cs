using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using IO_2026_SGGW.Core;

namespace IO_2026_SGGW
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            SetupGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void lblStatusFiles_Click(object sender, EventArgs e)
        {
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnFiltruj_Click(object sender, EventArgs e)
        {
        }

        private void btnEksportuj_Click(object sender, EventArgs e)
        {
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