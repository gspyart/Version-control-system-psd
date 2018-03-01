using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dropbox.Api;
using Newtonsoft.Json;
using System.IO;

namespace GitLog
{

    class User
    {
        public string username;
        public string token;
        public User(string u, string t)
        {
            username = u;
            token = t;
        }
    }
    class UserData
    {
        private List<User> activeUsers = new List<User>(); //list of active users
        public void AddUser(User person)
        {
            foreach (User o in activeUsers)
            {
                if (o.token == person.token || o.username == person.username)
                {
                    throw new Exception("same user");
                }
                activeUsers.Add(person);
            }

        } //add new active user (class User)
        public void UsersLoad()
        {
            if (File.Exists("tklist"))
            {
                StreamReader tklist = new StreamReader("tklist");
                activeUsers = JsonConvert.DeserializeObject<List<User>>(tklist.ReadToEnd());
                tklist.Close();
            }
            else { new StreamWriter("tklist"); }
        }  //upload file of active users
        public void UsersSave()
        {

            if (File.Exists("tklist"))
            {
                StreamReader tklist = new StreamReader("tklist");
                List<User> buffus = new List<User>();
                buffus = JsonConvert.DeserializeObject<List<User>>(tklist.ReadToEnd());
                tklist.Close();
                foreach (User b in activeUsers)
                {
                    buffus.Add(b);
                }
                StreamWriter tklist_w = new StreamWriter("tklist");
                tklist_w.Write(JsonConvert.SerializeObject(buffus));
                tklist_w.Close();

            }
            else
            {
                StreamWriter tklist_w = new StreamWriter("tklist");
                tklist_w.Write(JsonConvert.SerializeObject(activeUsers));
                tklist_w.Close();
            }
        } //save file of active users
    }
    class Auth
    {
        Uri link = new Uri("https://www.dropbox.com/oauth2/authorize?response_type=token&client_id=yqkotqxyx2w2v4l&redirect_uri=https://localhost/authorize");
        UserData data = new UserData(); 
        User sender; // активный пользователь
        public void Choose(User a)
        {
            if (a.token.Length != 0 || a.username.Length != 0) sender = a;
            else throw new Exception("Error user");

        } //выбор активного пользователя со списка

        public void AddNew(WebBrowser explorer)
        {
            void AddNew_Next(WebBrowser ex)
            {
                Uri uri_token = ex.Url;
                OAuth2Response s_Token = DropboxOAuth2Helper.ParseTokenFragment(uri_token);
                data.AddUser(new User(s_Token.Uid, s_Token.AccessToken));
            }
            explorer.Navigate(link);
            explorer.Size = new Size(500, 500);
            explorer.Top = 0;
            explorer.Left = 100;

            WebBrowserNavigatedEventHandler a = (ab, ba) => { ba.Url.AbsoluteUri.Contains("https://localhost/"); AddNew_Next(explorer); };
            explorer.Navigated += a;
        }
    }

}
