using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HpgViewer
{
    public partial class UC_pens : UserControl
    {
        public UC_pens()
        {
            InitializeComponent();
        }

       
        private void panel1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.panel1.BackColor = colorDialog1.Color;
            }

        }

 
    }
}
