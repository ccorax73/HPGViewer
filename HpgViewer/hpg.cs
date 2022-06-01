using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;


namespace HpgViewer
{
    public struct myStruct
    {
        public int x, y, pen, par2; 
        public float par1;
        public char pupd;

    }

    public struct myPens
    {
        public Color szin;        
        public int id, width;
    }
    
    public class HPG
    {
        public List<myPens> drawingpen = new List<myPens>();
        public Bitmap bm = null;        
        public List<myStruct> myList = new List<myStruct>();
        public int maxx = 0 , maxy = 0, yeltol=0, xeltol=0;        
        public HashSet<int> pens = new HashSet<int>() { };
        public float scale = 1.0F;
        public bool mirrorx = false;
        public bool mirrory = true;

        public HashSet<string> c = new HashSet<string>() { };

        public void draw()
        {
           
            bm = new Bitmap((int)((2.97F / 2.1F) * 4096), 4096);
            using (Graphics gfx = Graphics.FromImage(bm))
            gfx.Clear(Color.White);
            int i = 0;
            if (this.myList.Count > 0)
            {
                Graphics l = Graphics.FromImage(bm);                

                float mx = (bm.Width - (2.0F * Form1.margin)) / this.maxx;
                float my = (bm.Height - (2.0F * Form1.margin)) / this.maxy;

                Pen dPen = new Pen(Color.Black, 3);
                int oldpen = -1;

                PointF point1 = new PointF(0.0F, 0.0F);
                PointF point2 = new PointF(0.0F, 0.0F);

                foreach (myStruct item in this.myList) //  for each item
                {
                    
                    point1 = point2;

                    if (mirrorx) { point2.X = (float)bm.Width - (Form1.margin + mx * (float)item.x); }
                        else { point2.X = Form1.margin + mx * (float)item.x; }
                    
                    if (mirrory) { point2.Y = (float)bm.Height - (Form1.margin + my * (float)item.y); }
                        else { point2.Y = (Form1.margin + my * (float)item.y); }

                    if (item.pen != oldpen)
                    {
                        myPens f = drawingpen.FirstOrDefault(n => n.id == item.pen);
                        dPen = new Pen(f.szin, f.width);
                    }
                    oldpen = item.pen;
                    if (item.pupd == 'D') { l.DrawLine(dPen, point1, point2); }

                    if (item.pupd == 'C') 
                    { 
                        int rx = (int)(mx * (float)item.x);
                        int ry = (int)(my * (float)item.x);
                        l.DrawEllipse(dPen, new Rectangle((int)point1.X - rx, (int)point1.Y - ry, (int)2* rx, (int)2*ry));                       
                    }

                    if (item.pupd == 'A')
                    {
                        
                        int dx = (int)Math.Abs((point1.X-point2.X));
                        int dy = (int)Math.Abs((point1.Y-point2.Y));
                        int r  = (int)Math.Sqrt(dx * dx + dy * dy);

                        if (!((dx == 0) & (dy == 0)) )
                        {
                            int endangle = (point1.X >= point2.X) ? (int)((180 / Math.PI) * Math.Atan((float)dy / (float)dx))
                                : 180 - (int)((180 / Math.PI) * Math.Atan((float)dy / (float)dx));

                            int startangle = (point1.Y <= point2.Y) ? 360 - endangle - (int)item.par1
                                : endangle - (int)item.par1;

                            l.DrawArc(dPen, new Rectangle((int)point2.X - r, (int)point2.Y - r, 2 * r, 2 * r), startangle, item.par1);
                        }
                      
                    }                    
                }                

                dPen.Dispose();
                l.Dispose();                

            }            
        }

        public  string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ';' || c == ',' || c == '-' || c == '.')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public int load(string filename)
        {
            int result = 0;
            int penmax = 0;
            char oldpupd='U';
            string c = "";
            if (this.bm != null) { this.bm.Dispose(); }

            try
            {  
                using (StreamReader sr = new StreamReader(filename))
                {

                    String content = sr.ReadToEnd();
                    content = RemoveSpecialCharacters(content);

                    char[] delimiters = new char[] { ';' };

                    string[] words = content.Split(delimiters);                    
                    int pen = 0, oldx=0, oldy=0;
                    
                    foreach (string word in words)
                    {
                        

                        if (word.Length >= 2)
                        {
                            string cords = word.Substring(2, word.Length - 2);
                            string[] points = cords.Split(',');
                            myStruct newitem = new myStruct();

                            switch (word.Substring(0, 2)) 
                            {
                                case "IP":                                                                      
                                    xeltol = (Convert.ToInt32(points[2]));
                                    yeltol = (Convert.ToInt32(points[3]));    
                                    break;

                                case "SP":                                  
                                    pen = Convert.ToInt32(word.Substring(2, word.Length - 2));
                                    if (pen > penmax) { penmax = pen; }
                                    pens.Add(pen);         
                                    break;

                                case "CI":                                  // Circle
                                    newitem.pen = pen;
                                    newitem.pupd = 'C'; 
                                    newitem.x = Convert.ToInt32(points[0]);
                                    newitem.y = Convert.ToInt32(points[1]);
                                    myList.Add(newitem);
                                    break;

                                case "AA":                                  
                                   // c += word ;
                                    newitem.pen = pen;
                                    newitem.pupd = 'A';
                                    newitem.x = Math.Abs(Convert.ToInt32(points[0]) - xeltol);
                                    newitem.y = Math.Abs(Convert.ToInt32(points[1]) - yeltol);                                    
                                            
                                    newitem.par1 = float.Parse(points[2],System.Globalization.CultureInfo.InvariantCulture);
                                    
                                    myList.Add(newitem);
                                    
                                    break;


                                case "PA":                                  // Line
                                case "PU":
                                case "PD":

                                    newitem.pen = pen;
                                    if (word.Substring(0, 2) == "PU") { newitem.pupd = 'U'; }
                                    if (word.Substring(0, 2) == "PD") { newitem.pupd = 'D'; }
                                    if (word.Substring(0, 2) == "PA") { newitem.pupd = oldpupd; }
                                    

                                    if ((word.Substring(0, 2) == "PD") || (word.Substring(0, 2) == "PU") || (word.Substring(0, 2) == "PA"))
                                    { oldpupd = newitem.pupd; }

                                    
                                    if (word.Length > 2)
                                    {
                                        int i = 0;
                                        foreach (string point in points)
                                        {                                            
                                            if (i % 2 == 0)         
                                            {
                                                newitem.x = Math.Abs(Convert.ToInt32(point) - xeltol);
                                            }
                                            else
                                            {
                                                newitem.y = Math.Abs(Convert.ToInt32(point) - yeltol);

                                                if (myList.Count == 10) 
                                                {
                                                    mirrorx = (Convert.ToInt32(points[0]) < 0) ? true : false;
                                                    mirrory = (Convert.ToInt32(points[1]) < 0) ? false : true;
                                                }
                                                
                                                myList.Add(newitem);                                                

                                                oldx = newitem.x;
                                                oldy = newitem.y;


                                                if ((maxx < newitem.x) && (newitem.pupd != 'D')) { maxx = newitem.x; }
                                                if ((maxy < newitem.y) && (newitem.pupd != 'D')) { maxy = newitem.y; }
                                            }
                                            i++;
                                        }
                                    }
                                    if (word.Length == 2)   // Ha csak utasítás van, de koordináta nincs
                                    {
                                        newitem.x = oldx;
                                        newitem.y = oldy;
                                        myList.Add(newitem);                                        
                                    }
                                    break;
                            
                            }

                        }
                    }
                    
                    foreach (int p in pens)
                    {                       
                        myPens newpen = new myPens();
                        newpen.szin = Color.Black;
                        newpen.width = 3;
                        newpen.id = p;
                        drawingpen.Add(newpen);
                    }
                    drawingpen = drawingpen.OrderBy(i => i.id).ToList();                    
                }
            }
            catch 
            {
                result = -1;                
            }
            if ((myList.Count == 0) || (maxx == 0) || (maxy == 0)) 
            { result = -1; }
 
            return result;

        }
    }
}
