using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HpgViewer
{
    public partial class F_userdisplay : Form
    {
        public F_userdisplay()
        {
            InitializeComponent();
        }
        public int feladat;
        public string filename="";

        private void F_userdisplay_Shown(object sender, EventArgs e)
        {
            this.Refresh();
            switch (feladat)
            {
                case 0:
                    label1.Text = "Betöltés folyamatban....";
                    label1.Refresh();
                    if (0 == Form1.hpg.load(filename))
                    {
                        label1.Text = "Rajzolás folyamatban....";
                        label1.Refresh();
                        Form1.hpg.draw();                        
                    }
                    else { MessageBox.Show("Érvénytelen file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    break;

                

                default:
                    label1.Text = "";
                    this.Refresh();
                    break;
            }
            this.Close();
        }
    }
}
