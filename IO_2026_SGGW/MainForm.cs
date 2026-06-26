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
    /// <summary>
    /// Główne okno aplikacji do sprawdzania kolokwiów. Łączy interfejs użytkownika z logiką oceniania:
    /// wczytanie klucza XLSX, dodawanie plików <c>.cs</c> studentów (przeciąganie lub okno dialogowe),
    /// uruchomienie sprawdzania oraz eksport wyników do Excela.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>Lista wczytanych rozwiązań studentów (plików <c>.cs</c>) oczekujących na sprawdzenie.</summary>
        private readonly List<StudentSolution> studentSolutions = new
        List<StudentSolution>();

        /// <summary>Ścieżka aktualnie wczytanego pliku klucza XLSX; <c>null</c>, gdy nie wczytano klucza.</summary>
        private string xlsxPath;

        /// <summary>Aktualnie wczytany klucz odpowiedzi; <c>null</c>, gdy nie wczytano klucza.</summary>
        private AnswerKey answerKey;

        /// <summary>
        /// Inicjalizuje okno: buduje kontrolki (<c>InitializeComponent</c>) i konfiguruje tabelę wyników
        /// (<see cref="SetupGrid"/>).
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            SetupGrid();

        }
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, uint action, IntPtr str);
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                ChangeWindowMessageFilterEx(this.Handle, 0x0233, 1, IntPtr.Zero); // WM_DROPFILES
                ChangeWindowMessageFilterEx(this.Handle, 0x004A, 1, IntPtr.Zero); // WM_COPYDATA
                ChangeWindowMessageFilterEx(this.Handle, 0x0049, 1, IntPtr.Zero); // WM_COPYGLOBALDATA
            }
            catch { }
        }


        /// <summary>
        /// Obsługuje przycisk wyboru klucza XLSX: otwiera okno dialogowe, wczytuje wybrany plik przez
        /// <see cref="AnswerKeyLoader"/> i aktualizuje pasek statusu. Błędy wczytywania są logowane
        /// i pokazywane użytkownikowi w oknie komunikatu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
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
                    DebugLog($"OK Load: {ofd.FileName} | zadan={answerKey.Tasks.Count}");
                    UpdateStatusBar();
                    MessageBox.Show($"Wczytano klucz XLSX: {Path.GetFileName(ofd.FileName)} ({answerKey.Tasks.Count} zadań).",
                        "Wczytywanie klucza", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    DebugLog($"FAIL Load: {ofd.FileName}\n{ex.GetType().FullName}: {ex.Message}\nInner: {ex.InnerException?.GetType().FullName}: {ex.InnerException?.Message}\n{ex.StackTrace}");
                    MessageBox.Show("Błąd wczytywania klucza:\n" + ex.GetType().Name + ": " + ex.Message +
                        (ex.InnerException != null ? "\n\n" + ex.InnerException.GetType().Name + ": " + ex.InnerException.Message : ""),
                        "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    answerKey = null;
                    xlsxPath = null;
                }

                UpdateStatusBar(); // metoda z Zadania 1
            }
        }

        /// <summary>
        /// Dopisuje wpis diagnostyczny (ze znacznikiem czasu) do pliku <c>io_debug.log</c> na pulpicie.
        /// Ewentualne błędy zapisu są celowo ignorowane.
        /// </summary>
        /// <param name="msg">Treść komunikatu do zapisania.</param>
        private static void DebugLog(string msg)
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "io_debug.log");
                File.AppendAllText(path, DateTime.Now.ToString("HH:mm:ss") + " " + msg + Environment.NewLine);
            }
            catch { }
        }

        /// <summary>
        /// Obsługa kliknięcia etykiety statusu plików. Obecnie pusta (brak akcji).
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void lblStatusFiles_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Pozostałość po wcześniejszej kontrolce listy, obecnie pusta obsługa zdarzenia (bez akcji).
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        
        /// <summary>
        /// Obsługuje wejście przeciąganych plików nad panel upuszczania. Jeśli wśród przeciąganych elementów
        /// jest plik <c>.cs</c>, dopuszcza kopiowanie i podświetla panel na zielono; w przeciwnym razie
        /// blokuje upuszczenie.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia przeciągania (lista ścieżek oraz ustawiany efekt).</param>
        private void panelDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                bool hasCsFile = false;
                foreach (var path in paths)
                {
                    if (File.Exists(path) && path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
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
        /// <summary>
        /// Przywraca białe tło panelu upuszczania po opuszczeniu go przez kursor przeciągania.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void panelDrop_DragLeave(object sender, EventArgs e)
        {
            panelDrop.BackColor = Color.White;
            lblDropHint.BackColor = Color.White;
        }
        /// <summary>
        /// Obsługuje upuszczenie plików na panel: przywraca tło i przekazuje upuszczone ścieżki
        /// do <see cref="AddCsFiles"/>.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia przeciągania zawierające upuszczone ścieżki plików.</param>
        private void panelDrop_DragDrop(object sender, DragEventArgs e)
        {
            panelDrop.BackColor = Color.White;
            lblDropHint.BackColor = Color.White;
            var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            AddCsFiles(paths);
        }
        /// <summary>
        /// Dodaje wskazane pliki <c>.cs</c> do listy rozwiązań studentów, pomijając pliki o innym rozszerzeniu
        /// oraz już wcześniej dodane (rozpoznawane po ścieżce). Po dodaniu odświeża pasek statusu.
        /// </summary>
        /// <param name="paths">Ścieżki plików do rozważenia.</param>
        private void AddCsFiles(string[] paths)
        {
            int added = 0, skipped = 0;
            foreach (var path in paths)
            {
                try
                {
                    // T1-11: odrzuć katalogi - folder "Testy.CS" też kończy się na ".cs"
                    if (Directory.Exists(path)) { skipped++; continue; }
                    if (!File.Exists(path)) { skipped++; continue; }
                    if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) { skipped++; continue; }
                    if (studentSolutions.Exists(s => s.FilePath == path)) continue;
                    studentSolutions.Add(new StudentSolution
                    {
                        StudentId = Path.GetFileNameWithoutExtension(path),
                        FilePath = path,
                        SourceCode = File.ReadAllText(path),
                        LastModified = File.GetLastWriteTime(path)
                    });
                    added++;
                }
                catch (Exception ex)
                {
                    // T1-15: błąd jednego pliku NIE może przerwać całej operacji multi-drop
                    skipped++;
                    DebugLog($"Pominięto plik '{path}': {ex.GetType().Name}: {ex.Message}");
                }
            }
            UpdateStatusBar();
            if (skipped > 0)
                MessageBox.Show($"Dodano {added} plik(ów). Pominięto {skipped} (foldery / pliki zablokowane / nie.cs).",
        "Dodawanie plików", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (added > 0)
                MessageBox.Show($"Wczytano {added} plik(ów) .cs.",
        "Dodawanie plików", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>
        /// Obsługuje wybór plików <c>.cs</c> z okna dialogowego (z możliwością wielokrotnego wyboru)
        /// i przekazuje je do <see cref="AddCsFiles"/>.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
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

        /// <summary>
        /// Uruchamia sprawdzanie: waliduje, czy wczytano pliki i klucz, czyści poprzednie wyniki, blokuje UI,
        /// wywołuje <see cref="GradingService.RunAsync"/>, a po zakończeniu pokazuje łączną liczbę punktów
        /// i z powrotem odblokowuje kontrolki.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private async void btnSprawdz_Click(object sender, EventArgs e)
        {
            if (studentSolutions.Count == 0) { MessageBox.Show("Brak plików .cs"); return; }
            if (answerKey == null) { MessageBox.Show("Wybierz klucz XLSX"); return; }

            resultsList.Clear();
            mainProgressBar.Value = 0;
            EnableUiControls(false);

            int timeoutMs = (int)numTimeout.Value * 1000;
            var progress = new Progress<int>(p => mainProgressBar.Value = Math.Min(100, p));

            try
            {
                await new GradingService().RunAsync(studentSolutions, answerKey, timeoutMs, resultsList, progress);
                int total = 0;
                foreach (var r in resultsList) total += r.Punkty;
                MessageBox.Show($"Sprawdzono. Łącznie {total} punktów.", "Gotowe");
            }
            finally
            {
                EnableUiControls(true);
            }
        }

        /// <summary>
        /// Włącza lub wyłącza kontrolki interfejsu na czas trwania sprawdzania, aby zapobiec równoległym akcjom.
        /// </summary>
        /// <param name="enabled"><c>true</c>, aby włączyć kontrolki; <c>false</c>, aby je zablokować.</param>
        private void EnableUiControls(bool enabled)
        {
            btnSprawdz.Enabled = enabled;
            btnEksportuj.Enabled = enabled;
            btnWybierzXLSX.Enabled = enabled;
            numTimeout.Enabled = enabled;
            panelDrop.Enabled = enabled;
        }

        /// <summary>
        /// Aktualizuje pasek statusu liczbą wczytanych plików i nazwą wybranego klucza XLSX.
        /// W razie potrzeby przełącza wykonanie na wątek UI (<see cref="Control.InvokeRequired"/>).
        /// </summary>
        private void UpdateStatusBar()
        {
            if (lblStatusFiles.InvokeRequired)
            {
                lblStatusFiles.Invoke(new Action(UpdateStatusBar));
                return;
            }

            string xlsxName = string.IsNullOrEmpty(xlsxPath) ? "(brak)" : Path.GetFileName(xlsxPath);
            lblStatusFiles.Text = $"Załadowane: {studentSolutions.Count} | XLSX: {xlsxName}";
        }
        /// <summary>
        /// Obsługuje eksport wyników: jeśli istnieją jakiekolwiek wyniki, otwiera okno zapisu i zapisuje je
        /// przez <see cref="ResultsExporter"/>, a następnie opcjonalnie otwiera folder z plikiem w Eksploratorze.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
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

        /// <summary>
        /// Obsługa zdarzenia załadowania okna. Obecnie pusta (brak dodatkowej inicjalizacji).
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia.</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Przygotowuje dodatkowe elementy interfejsu: podpowiedzi (tooltipy) dla przycisków i panelu
        /// oraz menu kontekstowe wyników. Metoda jest niekompletna i obecnie nigdzie nie jest wywoływana
        /// (pozostałość po wcześniejszej wersji UI z listą wyników).
        /// </summary>
        private void SetupCustomUI()
        {
            // ToolTipy dla kontrolek
            ToolTip toolTip = new ToolTip();

            // Przyciski
            toolTip.SetToolTip(btnWybierzXLSX, "Wybierz plik referencyjny z rozszerzeniem .xlsx");
            toolTip.SetToolTip(btnSprawdz, "Uruchom weryfikację załadowanych plików .cs");
            toolTip.SetToolTip(btnEksportuj, "Zapisz obecne wyniki do nowego pliku Excel");

            // Drop Zone
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

        /// <summary>
        /// Kolekcja wyników powiązana z tabelą (<c>DataGridView</c>) jako źródło danych; uzupełniana na bieżąco
        /// w trakcie sprawdzania przez <see cref="GradingService"/>.
        /// </summary>
        private readonly BindingList<ResultRow> resultsList = new
            BindingList<ResultRow>();


        /// <summary>
        /// Konfiguruje tabelę wyników (<c>DataGridView</c>): definiuje kolumny powiązane z właściwościami
        /// <see cref="ResultRow"/> i ustawia źródło danych na <see cref="resultsList"/>.
        /// </summary>
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

        /// <summary>
        /// Koloruje wiersze tabeli wyników w zależności od statusu (<see cref="RunStatus"/>): np. zielony dla
        /// poprawnych, czerwony dla błędnych, pomarańczowy dla przekroczenia limitu czasu.
        /// </summary>
        /// <param name="sender">Źródło zdarzenia.</param>
        /// <param name="e">Dane zdarzenia formatowania komórki (zawierają indeks wiersza).</param>
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