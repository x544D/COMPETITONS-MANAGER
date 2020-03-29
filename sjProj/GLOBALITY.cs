using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using sjProj.UC;
using System.IO;

using System.Net;
using System.Net.Sockets;

using System.Globalization;
using Newtonsoft.Json.Linq;


namespace sjProj
{
    
    static class _Configs
    {


        private static string settings_File_path = Application.LocalUserAppDataPath + @"\settings.json";
        private static string strings_File_path = Application.LocalUserAppDataPath + @"\strings.json";

        //Global Exit App Proc
        private static void Exit_(string isMsg = null)
        {
            if (isMsg != null) MessageBox.Show(isMsg);
            Environment.Exit(-1);
        }


        //CHECK THE CONNECTIVITY
        public static bool isConnected()
        {
            try
            {
                using (var client = new WebClient())
                using (Stream stream = client.OpenRead("https://x544d.github.io"))
                    return true;
            }
            catch (Exception) { return false; }
        }

        //get the windows 2 letter language
        public static string get_default_lang()
        {
            return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        }

        // download string content of Settings File
        public static async Task<string> get_settings(string uri)
        {
            try
            {
                HttpWebRequest req = WebRequest.Create(uri) as HttpWebRequest;

                using (HttpWebResponse resp = await req.GetResponseAsync() as HttpWebResponse)
                using (Stream stream = resp.GetResponseStream())
                using (StreamReader rd = new StreamReader(stream))
                {
                    return await rd.ReadToEndAsync();
                }

            }
            catch (Exception) { return null; }
        }

        // Check if settings exists in local appdata , if not downlaod it 
        public static async void setup_settingsFile()
        {
            try
            {

                // download the settings file
                if (!File.Exists(settings_File_path))
                {
                    // get the json string
                    string result = await get_settings(Properties.Settings.Default.settings_link);

                    // more check
                    if (result is null)
                    {
                        MessageBox.Show(GetJsonValueByKey("invalid_settings_file"));
                        // to do in Exit()
                        if (File.Exists(settings_File_path)) File.Delete(settings_File_path);
                        Environment.Exit(-2);
                    }

                    // set language  f awal isti3maaal
                    if (get_default_lang() == "fr")
                    {
                        // yla kant fr khasna nbadlo string "result" li telecharginah w nraj3o en = fr
                        // cle  : app_lang

                        JObject ob = JObject.Parse(result);
                        if (ob.ContainsKey("app_lang"))
                        {
                            ob["app_lang"] = "fr";
                            result = ob.ToString();
                            Properties.Settings.Default.current_lang = "fr";

                        }
                    }

                    File.WriteAllText(settings_File_path, result, Encoding.ASCII);
                }
                else
                {
                    JObject obj = JObject.Parse(File.ReadAllText(settings_File_path));
                    Properties.Settings.Default.current_lang = obj["app_lang"].ToString();
                }
            }
            catch
            {
                // delete the  old invalid setting file to re-download after
                MessageBox.Show(GetJsonValueByKey(new List<string> { "invalid_settings_file", "please_restart" } , "\n"));
                if (File.Exists(settings_File_path)) File.Delete(settings_File_path);
                Environment.Exit(0);
            }
        }

        // To Check Updates
        public static void CheckForUpdates(Version currentV)
        {
            try
            {
                WebClient client = new WebClient();
                string strv = client.DownloadString(new Uri(Properties.Settings.Default.app_version_link));
                Version v = Version.Parse(strv.Trim());


                if (currentV.CompareTo(v) == -1)
                {
                    string updateMsg = GetJsonValueByKey("update_msg");
                    updateMsg = updateMsg.Replace("%a", currentV.ToString()).Replace("%b", strv);


                    DialogResult dr =  MessageBox.Show(updateMsg, GetJsonValueByKey("updateTitle"), MessageBoxButtons.YesNo);



                    if (dr == DialogResult.Yes)
                    {
                        // khasna  : TELECHARGER NOUVELLE VERSION DYAL HAD LAPPLICATION !
                        // updater 3d party.
                    }

                }

            }
            catch (Exception)
            {
                //
            }
        }

        // Get string value by Key from strings json file
        public static string GetJsonValueByKey(string key)
        {
            try
            {
                string lang = Properties.Settings.Default.current_lang;
                JObject jo = JObject.Parse(File.ReadAllText(strings_File_path));
                jo = JObject.Parse(jo[lang].ToString());

                return jo[key].ToString();

            }
            catch (Exception)
            {
                MessageBox.Show(GetJsonValueByKey("invalid_strings_file"));
                Environment.Exit(0);
                return null;
            }


        }
        public static string GetJsonValueByKey(List<string> keys , string separator = "" )
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                string lang = Properties.Settings.Default.current_lang;
                JObject jo = JObject.Parse(File.ReadAllText(strings_File_path));
                jo = JObject.Parse(jo[lang].ToString());

                // sb = null = ""
                
                keys.ForEach(key =>
                {
                    if (sb.ToString() != string.Empty) sb.Append(separator);
                    sb.Append(jo[key].ToString());
                });

                return sb.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Strings file !");
                Environment.Exit(0);
                return null;
            }


        }

        // Get IpV4 INFOS
        public static List<IPAddress> Get_Addresses()
        {
            // http://icanhazip.com
            try
            {
                WebClient client = new WebClient();

                IPAddress localAddr = Dns.GetHostEntry("").AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                IPAddress externalAddr = IPAddress.Parse(client.DownloadString("http://icanhazip.com"));

                return new List<IPAddress> { localAddr , externalAddr };

            }
            catch (Exception)
            {
                return null;
            }
        }

        //Check if it's running from Morocco
        public static void isInMorocco()
        {
            try
            {
                WebClient w = new WebClient();
                string jo = JObject.Parse(w.DownloadString("http://www.geoplugin.net/json.gp"))["geoplugin_countryName"].ToString();

                if (jo.ToLower() != "morocco")
                {
                    Exit_(GetJsonValueByKey("only_in_morocco"));
                }

            }
            catch (Exception) { Exit_(GetJsonValueByKey("disconnected"));}

        }

    }

    static class SAAD_NATIVE
    {
        public enum MSG_BOX_TYPES : int
        {
            SAAD_MB_OK  = 1
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);

    }

    class SAAD_BUILDER  
    {

        /// <summary>
        ///            -> GENERAL _ GLOBAL _ FORM _ SETTINGS
        /// </summary>

        #region GlobalStuff

        string FormTitle = "0 x 5 4 4 D => R E G I O N A L | 2 0 2 0";
        string FormName = "mainForm";

        Properties.Settings settings = new Properties.Settings();

        int leftPanelFullWidth = 200; // when the mouse enters
        int leftPanelMiniWidth = 100; // when the mouse is out of the leftPanel


        #endregion 



        /// <summary>
        ///         -> Attrs | Controls
        /// </summary>

        #region Control&Declarations

        private Rect screen;
        
        private Form1 main_form;

        private Panel TitlePanel;
        private Panel LeftSidePanel;
        public static Panel ContainerPanel;

        private Label title;

        private PictureBox icon;
        private PictureBox closeBtn;
        private PictureBox minBtn;
        private PictureBox pinTopBtn;

        private _MenuItem TestItem;
        private _MenuItem TestItem2;
        private _MenuItem TestItem3;
        private _MenuItem TestItem4;

        private PictureBox MenuIcon;

        private Panel SearchInputContainer;
        private TextBox SearchInput;


        struct Vector2
        {
            public int x;
            public int y;
        };

        struct Rect
        {
            public int x;
            public int y;
            public int w;
            public int h;
        };
        #endregion


        /// <summary>
        ///             -> Global EVENTS !
        /// </summary>

        #region TitleBarPanel

        bool isMoving = false;
        Point LocOnclick;


        private void md(object o, MouseEventArgs ev)
        {
            if (!settings.PinnedTop)
            {
                isMoving = true;
                LocOnclick = ev.Location;
            }
        }
        private void mu(object o, MouseEventArgs ev)
        {
            isMoving = false;
        }
        private void mm(object o, MouseEventArgs ev)
        {
            if (!settings.PinnedTop)
            {
                if (isMoving)
                {
                    Control c = ((Panel)o).Parent;

                    int dx = LocOnclick.X - ev.Location.X;
                    int dy = LocOnclick.Y - ev.Location.Y;

                    c.Location = new Point(c.Location.X - dx, c.Location.Y - dy);
                }
            }
        }

        #endregion

        /// <summary>
        ///             user controls 1
        /// </summary>



        ///////


        public SAAD_BUILDER(ref Form1 main_form, int x, int y, int w, int h)
        {
            screen.x = x;
            screen.y = y;
            screen.w = w;
            screen.h = h;

            this.main_form = main_form;

        }

        

        public void CloseForm()
        {

            //if (MessageBox.Show("Voulez vous quittez ?","confirmation",MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    Environment.Exit(0);
            //}

            string[] _ = _Configs.GetJsonValueByKey("exit_confirmation").Split('|');

            int ch = SAAD_NATIVE.MessageBox(main_form.Handle, _[0], _[1], 4);

            // 6 == yes
            // 7 == no
            if (ch == 6) Environment.Exit(1);

        }

        public void PinToTop()
        {
            if (!settings.PinnedTop)
            {
                main_form.MinimumSize = new Size(screen.w, TitlePanel.Height);
                main_form.Height = TitlePanel.Height;
                main_form.TopMost = true;
                main_form.Location = new Point(0, 0);
                settings.PinnedTop = true;
                MenuIcon.Enabled = false;
            }
            else
            {
                main_form.MinimumSize = new Size(screen.w, screen.h);
                main_form.Height = screen.h;
                main_form.TopMost = false;
                main_form.Location = new Point(0, 0);
                settings.PinnedTop = false;
                MenuIcon.Enabled = true;
            }
        }


        public void builder()
        {
            if (main_form != null && !main_form.IsDisposed)
            {
                // tracking stacked controls fel main form :
                Vector2 _Tracking = new Vector2();
                _Tracking.x = 0;
                _Tracking.y = 0;

                //// Form 
                
                main_form.Name = FormName;
                main_form.Text = FormTitle;
                main_form.Location = new Point(0, 0);
                main_form.FormBorderStyle = FormBorderStyle.None;

                //// Top_TitleBAR

                TitlePanel = new Panel()
                {
                    Bounds = new Rectangle(_Tracking.x, _Tracking.y, screen.w, 70),
                    BackColor = settings.TitleBarColor,
                    Parent = main_form,
                };

                TitlePanel.MouseDown += md;
                TitlePanel.MouseUp   += mu;
                TitlePanel.MouseMove += mm;

                TitlePanel.DoubleClick += (o, a) =>  main_form.Location = new Point(0, 0); 
                //TitlePanel.MouseDoubleClick += (o, a) => main_form.Location = new Point(0, 0);
             
                _Tracking.y += 70;


                // Title bar sub COntrols region

                #region Controls_TitlePanel

                MenuIcon = new PictureBox
                {
                    Parent = TitlePanel,
                    Width = 20,
                    Height = 20,
                    Location = new Point( leftPanelMiniWidth / 2 - 10 , TitlePanel.Height / 2 - (25 / 2)),
                    Image = Properties.Resources.Menu2,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Cursor = Cursors.Hand,
                };


                // _working // MenuIcon Click Event __x
                MenuIcon.Click += (o, a) =>
                {
                    try
                    {
                        if (!settings.PinnedTop && !settings.isStretched)
                        {
                            LeftSidePanel.Width = leftPanelFullWidth;
                            settings.isStretched = true;

                            foreach (Control ctrl in LeftSidePanel.Controls)
                            {
                                if (ctrl is _MenuItem)
                                {
                                    _MenuItem menu = ctrl as _MenuItem;
                                    menu.Height = 50;
                                    menu.icon.Bounds = new Rectangle(new Point(10, 50 / 2 - 10), new Size(20, 20));
                                    menu.menuItem_title.Bounds = new Rectangle(new Point(15,50/2 - 12 / 2),
                                        new Size(menu.Width,12));
                                }
                            }

                        }
                        else if (settings.isStretched)
                        {
                            LeftSidePanel.Width = leftPanelMiniWidth;
                            settings.isStretched = false;

                            foreach (Control ctrl in LeftSidePanel.Controls)
                            {
                                if (ctrl is _MenuItem)
                                {
                                    _MenuItem menu = ctrl as _MenuItem;
                                    menu.Height = 100;
                                    menu.icon.Bounds = new Rectangle(new Point(menu.Width / 2 - 13, menu.Height / 2 - 26),
                                        new Size(26, 26));

                                    menu.menuItem_title.Bounds = new Rectangle(new Point(0, menu.Height / 2 + 5),
                                        new Size(menu.Width, 12));
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                };


                icon = new PictureBox()
                {
                    Image = Properties.Resources.icon,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 25,
                    Height = 25,
                    Parent = TitlePanel,
                    Location = new Point(100 + MenuIcon.Width , TitlePanel.Height / 2 - (25/2))
                };

                title = new Label()
                {
                    Width = 200,
                    Text = settings.app_title,
                    ForeColor = Color.FromArgb(255, 157, 157, 157),
                    BackColor = settings.TitleBarColor,
                    Location = new Point(110 + MenuIcon.Width + icon.Width , TitlePanel.Height / 2 - ( 12 / 2)),
                    Parent = TitlePanel
                };

                closeBtn = new PictureBox()
                {
                    Cursor = Cursors.Hand,
                    BackColor = Color.Red,
                    Width = 20,
                    Height = 20,
                    Parent = TitlePanel,
                    Location = new Point(main_form.Width - 50, TitlePanel.Height / 2 - 10),

                };

                closeBtn.Click += (o, ev) => CloseForm(); 
                
                
                pinTopBtn = new PictureBox()
                {

                    Cursor = Cursors.Hand,
                    BackColor = Color.YellowGreen,
                    Width = 20,
                    Height = 20,
                    Parent = TitlePanel,
                    Location = new Point(main_form.Width - 80, TitlePanel.Height / 2 - 10),
                };

                pinTopBtn.Click += (e, o) => PinToTop();

                minBtn = new PictureBox()
                {
                    Cursor = Cursors.Hand,
                    BackColor = Color.Green,
                    Width = 20,
                    Height = 20,
                    Parent = TitlePanel,
                    Location = new Point(main_form.Width - 110, TitlePanel.Height / 2 - 10),
                };

                minBtn.Click += (e, o) => main_form.WindowState = FormWindowState.Minimized;

                #endregion


                // Search TextBox

                SearchInputContainer = new Panel {
                    Parent = TitlePanel,
                    Bounds = new Rectangle(new Point(400, TitlePanel.Height / 2 - 15), new Size(500, 30)),
                    BackColor = settings.ContainerBgColor,
                };

                SearchInput = new TextBox {

                    Parent = SearchInputContainer,
                    BorderStyle = BorderStyle.None,
                    Font = new Font("tahoma", 25f, FontStyle.Regular, GraphicsUnit.Pixel),
                    BackColor = settings.ContainerBgColor,
                    ForeColor = Color.FromArgb(255 ,80, 80, 80),
                    Dock = DockStyle.Fill

                };



                //// LEFT PANEL !

                LeftSidePanel = new Panel()
                {
                    Bounds      = new Rectangle(_Tracking.x, _Tracking.y, leftPanelMiniWidth , screen.h),
                    BackColor   = settings.TitleBarColor,
                    Parent      = main_form,
                    Name        = "LeftPanel"
                };



                _Tracking.x += 200;
                _Tracking.y += 0;

                // adding LEFT panel sub controls :


                #region LeftPanel_menuItems
               
                TestItem = new _MenuItem(100, leftPanelMiniWidth,/* Color.FromArgb(255, 60, 60, 60)*/ Color.Transparent, _Configs.GetJsonValueByKey("menu4").ToString(), Properties.Resources.settings)
                {
                    Parent = LeftSidePanel,
                    Dock = DockStyle.Top,
                };



                TestItem2 = new _MenuItem(100, leftPanelMiniWidth,/* Color.FromArgb(255, 60, 60, 60)*/ Color.Transparent, _Configs.GetJsonValueByKey("menu3").ToString(), Properties.Resources.cameras)
                {
                    Parent = LeftSidePanel,
                    Dock = DockStyle.Top,
                };



                TestItem3 = new _MenuItem(100, leftPanelMiniWidth, /* Color.FromArgb(255, 60, 60, 60)*/ Color.Transparent, _Configs.GetJsonValueByKey("menu2").ToString(), Properties.Resources.users)
                {
                    Parent = LeftSidePanel,
                    Dock = DockStyle.Top,
                };



                TestItem4 = new _MenuItem(100, leftPanelMiniWidth,/* Color.FromArgb(255, 60, 60, 60)*/ Color.Transparent, _Configs.GetJsonValueByKey("menu1").ToString(), Properties.Resources.comps)
                {
                    Parent = LeftSidePanel,
                    Dock = DockStyle.Top,
                };

                //MenuIconPanel = new Panel
                //{
                //    Parent = TitlePanel,
                //    Dock = DockStyle.Left,
                //    Height = 30,
                //    BackColor = Color.Red,
                //};




                #endregion

                //// Container_panel

                ContainerPanel = new Panel()
                {
                    Bounds = new Rectangle(_Tracking.x, _Tracking.y, screen.w - _Tracking.y, screen.h),
                    BackColor = settings.ContainerBgColor,
                    Parent = main_form,
                };

                _Tracking.x += screen.w - _Tracking.y;
                _Tracking.y += 0;

            }
            else
            {
                string[] _ = _Configs.GetJsonValueByKey("unknown_err1").Split('|');
                SAAD_NATIVE.MessageBox(main_form.Handle, _[0], _[1], 0);
                Environment.Exit(-1);
            }

            
            
            

        }



    }

}
