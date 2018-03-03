using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DropbBoxLogIn;


namespace dp
{
 
    public partial class Form1 : Form
    {

       

        public WebBrowser explorer = new WebBrowser();
        public Form1()
        {
            InitializeComponent();

            WebBrowserNavigatedEventHandler a = (ab, ba) => {

                if (ba.Url.AbsoluteUri.Contains("https://localhost/authorize") && !ba.Url.AbsoluteUri.Contains("dropbox.com"))
                {
                    var k = Task.Run(() => Program.id.Logined(explorer));
                    k.Wait();
                    button1.Enabled = true;
                    explorer.Navigate("http://0.0.0.0/");
                    Controls.Remove(explorer);
                    var form2 = new MainWin();
                    form2.Show();
                    
                }
            };
            explorer.Navigated += a;



        }

       
        private async void  button1_Click(object sender, EventArgs e)
        {
            Controls.Add(explorer);

            button1.Enabled = false;
            explorer.Size = new Size(Size.Width, Size.Height - 100);
            explorer.Top = 0;
            explorer.Left = 0;        
            Program.id.Login(explorer);
        }
    }
}
