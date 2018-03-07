using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Web;
using System.Windows.Controls;
using Dropbox.Api;
using Dropbox.Api.Users;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using ImageMagick;

namespace DropbBoxLogIn
{
    public class User
    {
        public string username { get; set; }
        public string token { get; set; }
        public User(string u, string t)
        {

            username = u;
            token = t;
        }
    }

    public delegate void none();

    public class Authorization
    {

        public event none SenderChanged;

        public class UserData
        {

            protected internal class Jsondata
            {
                public User activeuser;
                public List<User> allusers;
            }
            public User _sender { get { return sender; } set { sender = value; } }
            public User sender; // активный пользователь
            public DropboxClient client; //api of active user
            private List<User> activeUsers = new List<User>(); //list of active users
            public void AddUser(User person)
            {
                bool l = true;
                foreach (User o in activeUsers)
                {
                    if (o.token == person.token || o.username == person.username)
                    {
                        l = false;
                    }
                }

                if (l == true)
                {
                    activeUsers.Add(person);
                }
            } //add new active user (class User)
            public void DeleteUser(User person)
            {
                activeUsers.Remove(person);
            }
            public User UsersLoad() //ok
            {
                if (File.Exists("userlist"))
                {
                    StreamReader tklist = new StreamReader("userlist");
                    Jsondata info = new Jsondata();
                    info = JsonConvert.DeserializeObject<Jsondata>(tklist.ReadToEnd());
                    User buff = info.activeuser;
                    activeUsers = info.allusers;
                    tklist.Close();
                    if (buff != null) { client = new DropboxClient(buff.token); return buff; }
                }
                return null;


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

        public void DeleteUser(User o)
        {
            data.DeleteUser(o);
            data.UsersSave();
        }


        // public event none ELogOut;
        public async void Choose(User a)
        {
            try
            {
                if (a != null)
                {
                    data.sender = a;
                    data.client = new DropboxClient(a.token);
                    var k = await data.client.Users.GetCurrentAccountAsync(); //проверка токена на актуальность
                    data.UsersSave();
                    SenderChanged?.Invoke();
                }
            }
            catch (Dropbox.Api.DropboxException v) //если токен недействителен
            {
                MessageBox.Show(v.Message);
                DeleteUser(data.sender);
                LogOut();

            }



        } //выбор активного пользователя со списка
        public async void Logined(Uri wlink) //ok
        {

            Uri uri_token = wlink;
            OAuth2Response s_Token = DropboxOAuth2Helper.ParseTokenFragment(uri_token);
            data.client = new DropboxClient(s_Token.AccessToken);
            var inf = await data.client.Users.GetCurrentAccountAsync();
            User newuser = new User(inf.Name.DisplayName, s_Token.AccessToken);
            data.AddUser(newuser);
            Choose(newuser);
            data.UsersSave();
        }

        public bool IsLogged()
        {

            if (data.sender != null)
            {
                return true;
            }
            return false;
        }
        public Uri Login()
        {
            return link;
        } //добавить нового пользователя
        public void LogOut()
        {
            data.sender = null;
            data.client = null;
            //ELogOut();
            data.UsersSave();
        }

        //Dropbox Api
        public void UploadAll()
        {
            data.client.Files.BeginCreateFolderV2("/PSDGit");
        }
        public void Start()
        {
            Choose(data.UsersLoad());
        }

    }


}