using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sjProj.UC
{
    public partial class _MenuItem : UserControl
    {
        private PictureBox icon;
        private Label menuItem_title;
        private Panel divider;
        private Color bgColor;


        public _MenuItem(int h , int w , Color cl , string title , Image img = null )
        {

            bgColor = cl;
            InitializeComponent();
            SuspendLayout();

            Name = "MenuItem";
            Load += new EventHandler(MenuItem_Load);
            Width = w;
            Height = h;
            BackColor = cl;
            Tag = null;
            Cursor = Cursors.Hand;

            //this.MouseHover += (o, ev) =>
            //{
            //    if (Tag == null)
            //    {
            //        BackColor = Color.FromArgb(255, 70, 70, 70);
            //        icon.BackColor = Color.FromArgb(255, 70, 70, 70);
            //    }
            //};

            //this.MouseLeave += (o, ev) =>
            //{

            //    if (Tag == null)
            //    {
            //        BackColor = cl;
            //        icon.BackColor = cl;
            //    }
            //};


            icon = new PictureBox()
            {
                Image = img,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 20,
                Height = 20,
                BackColor = cl,
                Parent = this,
                Location = new Point(10 , Height / 2 - 10 )
            };

            menuItem_title = new Label()
            {
                Text = title,
                Parent = this,
                ForeColor = Color.White,
                Font = new Font("tahoma", 12F, FontStyle.Bold , GraphicsUnit.Pixel),
                Location = new Point(15 + icon.Width , Height / 2 - 6 )
            };

            divider = new Panel()
            {
                Size = new Size(w, 1),
                Dock = DockStyle.Bottom,
                BackColor = Color.DimGray,
                Parent = this
            };

            #region Events


            // event dyal this = usercontrol
            this.Click += myClick;
            this.MouseHover += myHover;
            this.MouseLeave += myLeave;

            // events dyal icon | picturebox
            icon.Click += myClick;
            icon.MouseHover += myHover;
            icon.MouseLeave += myLeave;

            // events dyal Label | 
            menuItem_title.Click += myClick;
            menuItem_title.MouseHover += myHover;
            menuItem_title.MouseLeave += myLeave;

            // events dyal Panel > divider
            divider.Click += myClick;
            divider.MouseHover += myHover;
            divider.MouseLeave += myLeave;


            #endregion

            ResumeLayout(false);


        }

        private void resetMenu_state(Control clickedControl)
        {
            foreach (Control item in Parent.Controls)
            {
                if (item is _MenuItem && item != clickedControl && item.Tag != null) // "clicked"
                {
                    item.Tag = null;
                    item.BackColor = bgColor;
                    icon.BackColor = bgColor;
                }
            }
        }

        private void myClick(object o, EventArgs e)
        {
            Control ctr = o as Control;
            bool hasParent = ctr.Equals(this) ? false : true;

            if (hasParent)
            {
                resetMenu_state(ctr.Parent);
                if (ctr.Parent.Tag == null)
                {
                    ctr.Parent.Tag = "clicked";
                    ctr.Parent.BackColor = Color.DarkGray;
                    icon.BackColor = Color.Transparent;
                }
            }
            else
            {
                resetMenu_state(ctr);
                if (ctr.Tag == null)
                {
                    ctr.Tag = "clicked";
                    ctr.BackColor = Color.DarkGray;
                    icon.BackColor = Color.Transparent;

                }
            }
        }

        private void myHover(object o, EventArgs e)
        {
            Control ctr = o as Control;
            bool hasParent = ctr.Equals(this) ? false : true;

            if (hasParent)
            {
                if (ctr.Parent.Tag == null)
                {
                    BackColor = Color.FromArgb(255, 70, 70, 70);
                    icon.BackColor = Color.FromArgb(255, 70, 70, 70);
                }
            }
            else
            {
                if (ctr.Tag == null)
                {
                    BackColor = Color.FromArgb(255, 70, 70, 70);
                    icon.BackColor = Color.FromArgb(255, 70, 70, 70);
                }
            }
        }

        private void myLeave(object o, EventArgs e)
        {
            Control ctr = o as Control;
            bool hasParent = ctr.Equals(this) ? false : true;

            if (hasParent)
            {
                if (ctr.Parent.Tag == null)
                {
                    BackColor = bgColor;
                    icon.BackColor = bgColor;
                }
            }
            else
            {
                if (ctr.Tag == null)
                {
                    BackColor = bgColor;
                    icon.BackColor = bgColor;
                }
            }
        }


        private void MenuItem_Load(object sender, EventArgs e)
        {

        }



    }
}
