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
            private List<User> activeUsers = new List<User>(); //list of active users
            public void AddUser(User person)
            {
                foreach (User o in activeUsers)
                {
                    if (o.token == person.token || o.username == person.username)
                    {
                        MessageBox.Show("same user");
                        throw new Exception("same user");
                    }
                   
                }
                activeUsers.Add(person);

            } //add new active user (class User)
            public void UsersLoad()
            {
                if (File.Exists("userlist"))
                {
                    StreamReader tklist = new StreamReader("userlist");
                    activeUsers = JsonConvert.DeserializeObject<List<User>>(tklist.ReadToEnd());
                    tklist.Close();
                }
                
            }  //upload file of active users
            public void UsersSave()
            {

                if (File.Exists("userlist"))
                {
                    File.Delete("userlist");
                    StreamWriter tklist_w = new StreamWriter("userlist");
                    tklist_w.Write(JsonConvert.SerializeObject(activeUsers));
                    tklist_w.Close();
                }
                else
                {
                    StreamWriter tklist_w = new StreamWriter("userlist");
                    tklist_w.Write(JsonConvert.SerializeObject(activeUsers.ToArray()));
                    tklist_w.Close();
                } 
            } //save file of active users
            public List<User> GetList()
            {
                return activeUsers;
            }
        }
        //g9k6M7MsLsAAAAAAAAAAGICpJg05VVzBHf7TLMNJXUtd28EJ45qRbmrTMyGoowMJ
        Uri link = new Uri("https://www.dropbox.com/oauth2/authorize?response_type=token&client_id=yqkotqxyx2w2v4l&redirect_uri=https://localhost/authorize");
        public UserData data = new UserData();
        User sender; // активный пользователь
        public void Choose(User a)
        {
            if (a.token.Length != 0 || a.username.Length != 0) sender = a;
            else throw new Exception("Error user");

        } //выбор активного пользователя со списка
        async public Task Logined(WebBrowser ex)
        {
            Uri uri_token = ex.Url;
            OAuth2Response s_Token = DropboxOAuth2Helper.ParseTokenFragment(uri_token);

            DropboxClient client = new DropboxClient(s_Token.AccessToken);
            var inf = await client.Users.GetCurrentAccountAsync();
            sender = new User(inf.Name.DisplayName, s_Token.AccessToken);
            data.AddUser(sender);
            data.UsersSave();

        }
        public void Login(WebBrowser explorer)
        {
            explorer.Navigate(link);
        } //добавить нового пользователя
        public Auth()
        {
           data.UsersLoad();
        }
    }


}
