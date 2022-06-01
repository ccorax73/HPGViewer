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
    public partial class Pensetup : Form
    {
        public Pensetup()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Form1.hpg.drawingpen.Clear();
            foreach (Control control in panel1.Controls)
            {
                if (control is UC_pens) 
                {
                    UC_pens p = (UC_pens)control;
                    myPens newpen = new myPens();
                    newpen.szin = p.panel1.BackColor;
                    newpen.width = Convert.ToInt32(p.numericUpDown1.Value);
                    newpen.id = Convert.ToInt32(p.label4.Text);
                    Form1.hpg.drawingpen.Add(newpen);
 
                }
                
            }   
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
