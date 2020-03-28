using System;
using System.Drawing;
using System.Windows.Forms;


namespace sjProj
{


    public partial class Form1 : Form
    {
        SAAD_BUILDER SB;
        Form1 main_form;

        
         

        public Form1()
        {
            //Check if connected !

            if (!_Configs.isConnected())
            {
                MessageBox.Show(_Configs.GetJsonValueByKey("disconnected"));
                Environment.Exit(-2);
            }

            // check if in morocco

            

            //Check Version ---- jdida

            _Configs.CheckForUpdates(Version.Parse(ProductVersion));

            // download the settings file

            _Configs.setup_settingsFile();

            // Initializing Lform dyalna !



            SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(912, 534);
            Load += new EventHandler(Form1_Load);

            ResumeLayout(false);

        }

       

        public void Form1_Load(object sender, EventArgs e)
        {
            #region Screen&WindowState

            Screen screen = Screen.PrimaryScreen;

            WindowState = FormWindowState.Normal;
            MaximizeBox = false;
            MinimumSize = new Size(screen.Bounds.Width, screen.Bounds.Height);

            int x = screen.Bounds.X;
            int y = screen.Bounds.Y;
            int w = screen.Bounds.Width;
            int h = screen.Bounds.Height;


            #endregion

            main_form = this;
            SB = new SAAD_BUILDER(ref main_form, x, y, w, h);
            SB.builder();
  




        }
    }
}
