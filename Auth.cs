using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dropbox.Api;
using Dropbox.Api.Users;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace DropbBoxLogIn
{
    public class User
    {
        public string username;
        public string token;
        public User(string u, string t)
        {
            username = u;
            token = t;
        }
    }
    class Auth
    {
      
        protected internal class UserData
        {
            protected internal class Jsondata
            {
                public User activeuser;
                public List<User> allusers;
            }
           
            public User sender; // активный пользователь
            private List<User> activeUsers = new List<User>(); //list of active users
            public DropboxClient client;
            public void AddUser(User person)
            {
                bool l = true;
                foreach (User o in activeUsers)
                {
                    if (o.token == person.token)
                    {
                        l = false;
                        
                    }
  
                   
                }

                if (l == true)
                {
                    activeUsers.Add(person);
                }
            } //add new active user (class User)
            public void UsersLoad()
            {
                if (File.Exists("userlist"))
                {
                    StreamReader tklist = new StreamReader("userlist");
                    Jsondata info = new Jsondata();
                    info = JsonConvert.DeserializeObject<Jsondata>(tklist.ReadToEnd());
                    sender = info.activeuser;
                    activeUsers = info.allusers;
                    if (sender != null) client = new DropboxClient(sender.token);
                    tklist.Close();
                }
                
            }  //upload file of active users
            public void UsersSave()
            {

                if (File.Exists("userlist"))
                {
                    File.Delete("userlist");
                    Jsondata dd = new Jsondata();
                    StreamWriter tklist_w = new StreamWriter("userlist");
                    dd.activeuser = sender;
                    dd.allusers = activeUsers;
                    tklist_w.Write(JsonConvert.SerializeObject(dd));
                    tklist_w.Close();
                }
                else
                {
                    Jsondata dd = new Jsondata();
                    dd.activeuser = sender;
                    dd.allusers = activeUsers;
                    StreamWriter tklist_w = new StreamWriter("userlist");
                    tklist_w.Write(JsonConvert.SerializeObject(dd));
                    tklist_w.Close();
                } 
            } //save file of active users
            public List<User> GetList()
            {
                return activeUsers;
            }
        }
        Uri link = new Uri("https://www.dropbox.com/oauth2/authorize?response_type=token&client_id=yqkotqxyx2w2v4l&redirect_uri=https://localhost/authorize");
        public UserData data = new UserData();
       
        public void Choose(User a)
        {
            if (a.token.Length != 0 || a.username.Length != 0) { data.sender = a;
                data.client = new DropboxClient(a.token);
            }
            else throw new Exception("Error user");

        } //выбор активного пользователя со списка
        async public Task Logined(WebBrowser ex)
        {
            Uri uri_token = ex.Url;
            OAuth2Response s_Token = DropboxOAuth2Helper.ParseTokenFragment(uri_token);

            data.client = new DropboxClient(s_Token.AccessToken);
            var inf = await data.client.Users.GetCurrentAccountAsync();
            data.sender = new User(inf.Name.DisplayName, s_Token.AccessToken);
            data.AddUser(data.sender);
            data.UsersSave();

        }
        public void Login(WebBrowser explorer)
        {
            explorer.Navigate(link);
        } //добавить нового пользователя
        public void LogOut()
        {
            data.sender = null;
            data.UsersSave();
        }
        public Auth()
        {
           data.UsersLoad();
        }
    }


}