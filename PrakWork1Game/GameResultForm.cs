using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrakWork1Game
{
    public partial class GameResultForm : Form
    {
        public GameResultForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.OpenForms["Form2"].Close();
            this.Close();
            Application.OpenForms["Form1"].Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 oldF = (Form2)Application.OpenForms["Form2"];
            oldF.closedByAnoterForm = true;
            oldF.Close();
            this.Close();
            Form2 f2 = new Form2();
            f2.Show();
        }
    }
}
