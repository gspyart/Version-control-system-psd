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
    
    public partial class MainWin : Form
    {
        
        public MainWin()
        {
            InitializeComponent();
           

        }
        
        
        private  async void MainWin_Load(object sender, EventArgs e)
        {
           
            if (Program.id.data.client != null)
            {
                label1.Text = Program.id.data.sender.username;
                var k = await Program.id.data.client.Users.GetCurrentAccountAsync();
                label5.Text = k.Email;
            }
            
          
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.id.LogOut();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
