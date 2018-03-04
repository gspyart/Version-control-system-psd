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
using System.IO;

namespace dp
{

    public partial class MainWin : Form
    {
        OpenFileDialog dir = new OpenFileDialog();
        List<PSDFile> projects = new List<PSDFile>(); //список проектов

        public void update()
        {
            listBox2.Items.Clear(); //очистка, перед обновлением списка2
            var o = (PSDFile)(listBox1.SelectedItem); //выбранный элемент в списке1
            Saves buffer = o.commit;
            foreach (info t in buffer.commits) //проход по всем коммитам проекта
            {
                listBox2.Items.Add(t);
            }
        }

        public MainWin()
        {
            InitializeComponent();
            int i = 0;
            listBox1.DisplayMember = "name";
            listBox2.DisplayMember = "t";
            dir.Filter = "Psd file (*.psd)|*.psd";
            dir.FileOk += (a, b) =>
            {
                string name = dir.SafeFileName.Remove(dir.SafeFileName.Length - 4, 4);


                var p1 = new PSDFile(name, dir.FileName.Remove(dir.FileName.Length - dir.SafeFileName.Length, dir.SafeFileName.Length), Convert.ToString(i));

                p1.looks.Changed += new FileSystemEventHandler(delegate
                {
                    p1.AddCommit(new info("descr", "name"));
                    p1.looks.EnableRaisingEvents = false;
                    p1.looks.EnableRaisingEvents = true;
                    update();
                });

                projects.Add(p1);
                listBox1.Items.Add(p1);
                i++;
            };
        }

        private async void MainWin_Load(object sender, EventArgs e) //загрузка программы
        {
            try
            {
                Enabled = false;
                if (Program.id.data.client != null) //если есть авторизированный пользователь
                {      
                    var k = await Program.id.data.client.Users.GetCurrentAccountAsync(); //проверка токена на актуальность
                    label5.Text = k.Email;
                    label1.Text = k.Name.DisplayName;
                }
                else
                {
                    Form1 auth = new Form1();
                    auth.Show();
                }
                Enabled = true;
            }
            catch (Dropbox.Api.DropboxException a) //если токен недействителен
            {
                Program.id.DeleteUser(Program.id.data.sender);
                Program.id.LogOut();
                MessageBox.Show(a.Message);
                Application.Exit();
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Program.id.LogOut();
            Form1 k = new Form1();
            label1.Text = label5.Text = "none";
            
            k.Show();
            this.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dir.ShowDialog();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            update();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
