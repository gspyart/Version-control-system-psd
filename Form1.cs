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
           // this.FormClosing += (o, p) => { Application.Exit(); };
            WebBrowserNavigatedEventHandler a = (ab, ba) => {

                if (ba.Url.AbsoluteUri.Contains("https://localhost/authorize") && !ba.Url.AbsoluteUri.Contains("dropbox.com"))
                {
                    var k = Task.Run(() => Program.id.Logined(explorer));
                    k.Wait();
                    button1.Enabled = true;
                    explorer.Navigate("http://0.0.0.0/");
                    Controls.Remove(explorer);
                    this.Close();
                }
            };
            explorer.Navigated += a;


            listBox1.DisplayMember = "username";
            foreach (User o in Program.id.data.GetList())
            {
                listBox1.Items.Add(o);
            }

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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var o = (User)(listBox1.SelectedItem);
            Program.id.Choose(o);
            this.Close();


        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
