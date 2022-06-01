using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace HpgViewer
{
    public partial class Form1 : Form
    {
        public static HPG hpg = new HPG();
        public const int margin = 10;
        

        Point _mousePt = new Point();
        bool _tracking = false;

        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
        }

        private void pictureBox1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {                        
            if ((e.Delta > 0) && (hpg.scale < 1.9F)) { hpg.scale += 0.1F; }
            if ((e.Delta < 0) && (hpg.scale > 1.0F)) { hpg.scale -= 0.1F; }
            pictureBox1_redraw();
            
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "HPG Files|*.hpg";
            openFileDialog1.Title = "Select a HPG File";            
                        
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                hpg = new HPG();
                F_userdisplay fud = new F_userdisplay();
                fud.feladat = 0;
                fud.filename = openFileDialog1.FileName;
                this.Text = "HPG Viewer  - Filename: " + fud.filename;
                fud.ShowDialog(this);
                pictureBox1_redraw();
            }  
        }

 
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

 

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hpg.bm != null) 
            { 
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "JPEG Image (.jpeg)|*.jpeg";
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    var bmp = new Bitmap((int)297 * 5, (int)210 * 5);
                    var graph = Graphics.FromImage(bmp);

                    graph.DrawImage(hpg.bm, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    bmp.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    graph.Dispose();
                }                
            }
        }
      

        private void Form1_Resize(object sender, EventArgs e)
        {
            panel1.Height = this.ClientRectangle.Height - menuStrip1.Height;
            panel1.Width  = (int)((2.97F / 2.1F) * this.ClientRectangle.Height);
            panel1.Left   = (this.Width - panel1.Width) / 2;
            hpg.scale = 1;
            pictureBox1.Width = panel1.ClientRectangle.Width;
            pictureBox1.Height = panel1.ClientRectangle.Height;
            pictureBox1_redraw();
            
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {  

            printDocument1.DefaultPageSettings.Landscape = true;
            Margins margins = new Margins(50, 50, 50, 50);
            printDocument1.DefaultPageSettings.Margins = margins;
            if (printDialog1.ShowDialog() == DialogResult.OK) printDocument1.Print();
            
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.PageSettings.Landscape = true;            
            e.Graphics.DrawImage(hpg.bm, e.MarginBounds);
        }

        private void pensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hpg.drawingpen != null)
            {
                Pensetup ps = new Pensetup();
                int i = 0;
                foreach (myPens h in hpg.drawingpen)
                {
                    UC_pens newpen = new UC_pens();
                    newpen.panel1.BackColor = h.szin;
                    newpen.numericUpDown1.Value = h.width;
                    newpen.label4.Text = h.id.ToString();
                    newpen.Top = i * newpen.Height;
                    i++;
                    ps.panel1.Controls.Add(newpen);
                }
                if (DialogResult.OK == ps.ShowDialog(this)) 
                {
                    hpg.draw();
                    pictureBox1_redraw();
                }
            }
        }

 

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _mousePt = e.Location;
                _tracking = true;
            }
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }



        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _tracking = false;
            pictureBox1.Refresh();
        }

        private void pictureBox1_redraw()
        {
            if (hpg.bm != null)
            {
                if (panel1.ClientRectangle.Width > 0)
                {
                    int pw = (int)(hpg.scale * (float)panel1.ClientRectangle.Width - 10.0F);
                    int ph = (int)(hpg.scale * (float)panel1.ClientRectangle.Height - 10.0F);
                    Bitmap nbitmap = new Bitmap(pw, ph);
                    Graphics g = Graphics.FromImage(nbitmap);
                    g.DrawImage(hpg.bm, new Rectangle(0, 0, pw, ph));
                    pictureBox1.Image = nbitmap;
                    pictureBox1.Refresh();
                }
            }      
        } 

       

        private void pictureBox1_MouseMove_1(object sender, MouseEventArgs e)
        {            
            if (_tracking)
            {
                this.panel1.AutoScrollPosition = new Point(-this.panel1.AutoScrollPosition.X + (_mousePt.X - e.X),
                 -this.panel1.AutoScrollPosition.Y + (_mousePt.Y - e.Y));     
            }                
        }  
  
    }
}
