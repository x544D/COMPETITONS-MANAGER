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
        public PictureBox icon;
        public Label menuItem_title;
      

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
            


            icon = new PictureBox()
            {
                Image = img,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 26,
                Height = 26,
                BackColor = cl,
                Parent = this,
                Location = new Point( Width / 2 - 13 , Height / 2 - 26),
                Name = "icon",
                
            };

            menuItem_title = new Label()
            {
                Text = title,
                Parent = this,
                ForeColor = Color.Silver,
                
                Font = new Font("tahoma", 11F, FontStyle.Regular, GraphicsUnit.Pixel),
                Location = new Point(0 , Height / 2 + 5),
                Name = "title",
                Width = Width,
                TextAlign = ContentAlignment.MiddleCenter,
                

            };

            //divider = new Panel()
            //{
            //    Size = new Size(w, 1),
            //    Dock = DockStyle.Bottom,
            //    BackColor = Color.DimGray,
            //    Parent = this,
            //    Name = "divider"

            //};

            #region Events


            // event dyal this = usercontrol
            this.Click += myClick;
            this.MouseEnter += myEnter;
            this.MouseLeave += myLeave;
            //this.MouseMove += myMove;


            // events dyal icon | picturebox
            icon.Click += myClick;
            icon.MouseEnter += myEnter;
            icon.MouseLeave += myLeave;
            //icon.MouseMove += myMove;

            // events dyal Label | 
            menuItem_title.Click += myClick;
            menuItem_title.MouseEnter += myEnter;
            menuItem_title.MouseLeave += myLeave;
            //menuItem_title.MouseMove += myMove;

            // events dyal Panel > divider
            //divider.Click += myClick;
            //divider.MouseEnter += myEnter;
            //divider.MouseLeave += myLeave;
            //divider.MouseMove += myMove;




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
                    ctr.Parent.BackColor = Color.FromArgb(255, 20, 20, 20);
                    icon.BackColor = Color.Transparent;
                }
            }
            else
            {
                resetMenu_state(ctr);
                if (ctr.Tag == null)
                {
                    ctr.Tag = "clicked";
                    ctr.BackColor = Color.FromArgb(255, 20 ,20, 20);
                    icon.BackColor = Color.Transparent;

                }
            }
        }

        private void myEnter(object o, EventArgs e)
        {
            //if (!Properties.Settings.Default.isHovered) MyHoverHandler?.Invoke(this, new EventArgs());

        


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
            //if (Properties.Settings.Default.isHovered) MyLeaveHandler?.Invoke(this, new EventArgs());

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
