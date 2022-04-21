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
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();

            if (Options.playModeAgainstPc)
            {
                radioButton1.Checked = true;
            }
            else
            {
                radioButton2.Checked = true;
            }

            if (Options.firstPlyerToScoreLargerThanTwo)
            {
                radioButton3.Checked = true;
            }
            else
            {
                radioButton4.Checked = true;
            }

            if (Options.firstPlayerMovesFirst)
            {
                radioButton5.Checked = true;
            }
            else
            {
                radioButton6.Checked = true;
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Options.playModeAgainstPc = true;
            }
            else
            {
                Options.playModeAgainstPc = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                Options.firstPlyerToScoreLargerThanTwo = true;
            }
            else
            {
                Options.firstPlyerToScoreLargerThanTwo = false;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                Options.firstPlayerMovesFirst = true;
            }
            else
            {
                Options.firstPlayerMovesFirst = false;
            }
        }
    }
}
