using IO_2026_SGGW.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IO_2026_SGGW
{
    public partial class MainForm : Form
    {
        private AnswerKey answerKey;
        private string xlsxPath;
        public MainForm()
        {
            InitializeComponent();

            SetupCustomUI();
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
            itemCopy.Click += (sender, e) =>
            {
                if (lstResults.SelectedItem != null)
                {
                    Clipboard.SetText(lstResults.SelectedItem.ToString());
                }
            };

            // Zaznacz wszystko (Skrót: Ctrl+A)
            ToolStripMenuItem itemSelectAll = new ToolStripMenuItem("Zaznacz wszystko");
            itemSelectAll.ShortcutKeys = Keys.Control | Keys.A;
            itemSelectAll.Click += (sender, e) =>
            {
                // Zaznacza wszystko tylko, jeśli właściwość SelectionMode listy pozwala na wielokrotny wybór
                if (lstResults.SelectionMode == SelectionMode.MultiSimple || lstResults.SelectionMode == SelectionMode.MultiExtended)
                {
                    for (int i = 0; i < lstResults.Items.Count; i++)
                    {
                        lstResults.SetSelected(i, true);
                    }
                }
            };

            // 3. Separator (pozioma linia oddzielająca)
            ToolStripSeparator separator = new ToolStripSeparator();

            // 4. Wyczyść listę
            ToolStripMenuItem itemClear = new ToolStripMenuItem("Wyczyść listę");
            itemClear.Click += (sender, e) =>
            {
                // Czyści listę. Uwaga od Gemini: jeśli lista używa DataSource, to wyrzuci wyjątek, 
                // wtedy trzeba by wyczyścić samo źródło (np. listę w tle).
                lstResults.Items.Clear();
            };

            // Dodanie elementów do menu w odpowiedniej kolejności
            resultsMenu.Items.Add(itemCopy);
            resultsMenu.Items.Add(itemSelectAll);
            resultsMenu.Items.Add(separator);
            resultsMenu.Items.Add(itemClear);

            // Przypięcie gotowego menu do kontrolki ListBox
            lstResults.ContextMenuStrip = resultsMenu;
        }
    }
}
