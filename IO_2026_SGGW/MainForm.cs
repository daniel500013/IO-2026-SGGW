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
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
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
        private ProgressBar mainProgressBar;


        private void MainForm_Load(object sender, EventArgs e)
        {
            mainProgressBar = new ProgressBar();
            
            //Ustawienia wizualne (DoD: wysokość 15px)
            mainProgressBar.Height = 15;
            mainProgressBar.Value = 0;
            
            //Marginesy 12px (X = 12, Szerokość = Całość - 2 * 12)
            mainProgressBar.Location = new Point(12, 150); // Przykładowe Y - dostosuj wg potrzeby
            mainProgressBar.Width = this.ClientSize.Width - 24;

            //DoD: Anchor ustawiony na Top, Left, Right
            mainProgressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            //Dodanie do formy
            this.Controls.Add(mainProgressBar);
        }

    }
}
