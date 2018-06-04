using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.IO;
using Dianoga.ImageMagick;
using System.Threading;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using System.IO.Packaging;
using DropbBoxLogIn;
using System.Drawing;
using System.IO.Compression;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
using Dropbox.Api.Users;
using Newtonsoft.Json;
using System.Data.Sql;
using System.ComponentModel;
using System.Data;
using ImageMagick;
using PSDGitFinal;
namespace dp    

{
    class RemoteProject //проекты на dropbox
    {
        public bool fresh = false;
        public MemoryStream image { get; set; }
        public string name { get; set; }
        public List<Save> commits;
    }

    class Data // главный класс с данными
    {
        static int load = 0;
        public Data()
        {
            UserProjects = new ObservableCollection<PSDProject>();
        }
        public delegate Task Tu(PSDProject a, Dropbox.Api.DropboxClient k);
        public ObservableCollection<PSDProject> UserProjects { get; set; }

        public static MemoryStream Decompress(FileStream e)
        {
            var k = new MemoryStream();
            var t = new GZipStream(e, CompressionMode.Decompress);
            t.CopyTo(k);
            t.Close();
            return k;
        }

        public bool ProjectExist(string name, string owner)
        {
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "Select * from Projects where project_name = '" + name + "' and owner= '" + owner + "'";
            var data = m_sqlCmd.ExecuteReader();

            if (data.HasRows)
            {
                return true;
            }
            return false;
        }

        public void AddProject(PSDProject b) //добавить проект
        {
            UserProjects.Add(b);
        }
        public void DeleteProject(PSDProject proj)
        {
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "Delete from Commits where Project_id = " + proj.id;
            m_sqlCmd.ExecuteNonQuery();
            m_sqlCmd.CommandText = "Delete from Projects where id = " + proj.id + " and owner = '" + proj.owner_id + "'";
            m_sqlCmd.ExecuteNonQuery();
            UserProjects.Remove(proj);
            var filelist = new DirectoryInfo(@"data/" + proj.owner_id.Replace(':', '-') + "/" + proj.name.Remove(proj.name.Length - 4));
            FileInfo[] Files = filelist.GetFiles();
            foreach (var item in Files)
            {
                item.Delete();
            }
            Directory.Delete("data/" + proj.owner_id.Replace(':', '-') + "/" + proj.name.Remove(proj.name.Length - 4));
        }
        public static void metadata(PSDProject a, FileStream k) // формирование методанных о проекте
        {
            List<Save> infojson = a.Commits.ToList(); //сериализация метаданных
            var inf = JsonConvert.SerializeObject(infojson);
            foreach (byte u in inf.ToArray())
            {
                k.WriteByte(u);
            }
            k.Close();
        }
        public static List<Save> getmetadata(PSDProject psd)
        {
            try
            {
                var file = File.Open("data/" + psd.owner_id.Replace(':', '-') + "/" + psd.name.Remove(psd.name.Length - 4) + "/metadata", FileMode.Open);
                StreamReader sr = new StreamReader(file);
                string meta = sr.ReadToEnd();
                sr.Close();
                List<Save> eq = JsonConvert.DeserializeObject<List<Save>>(meta); //данные о коммитах в бд
                return eq;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения metadata файла с диска: " + ex.Message);
            }
            return new List<Save>();
        }
        //БД 
        public void DatabaseDBLoad(User user)
        {
            try
            {
                SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
                m_dbConn.Open();
                SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
                m_sqlCmd.CommandText = "Select * from Projects";
                SQLiteDataReader data = m_sqlCmd.ExecuteReader();
                var list = new List<PSDProjectOffline>();
                while (data.Read())
                {
                    if (data.GetString(3) == user.id)
                    {
                        PSDProjectOffline m = new PSDProjectOffline(data.GetInt32(0), data.GetString(1), data.GetString(2), data.GetString(3));
                        list.Add(m);
                    }
                }
                m_dbConn.Close();
                //получаем список проектов из Dropbox
                var rp = Task.Run(() => GetProjects(App.Authorization.data.client));//костыль 
                rp.Wait();

                //Добавление несуществующих проектов в БД
                foreach (var item in rp.Result)
                {
                    bool exist = false;
                    foreach (var item2 in list)
                    {
                        if (item.name + ".psd" == item2.name)
                        {
                            exist = true;
                        }

                    }
                    if (exist == false)
                    {
                        PSDProjectOffline m = new PSDProjectOffline(-1, item.name + ".psd", "", user.id);
                        AddProject(m);
                        DatabaseInsert(m);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } //данные из Dropbox о проектах
        public void DatabaseLoad(User user) //данные из БД о проектах
        {
            try
            {
                foreach (PSDProject o in UserProjects)
                {
                    o.off();
                }
                UserProjects.Clear();
                SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
                m_dbConn.Open();
                SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
                m_sqlCmd.CommandText = "Select * from Projects";
                SQLiteDataReader data = m_sqlCmd.ExecuteReader();
                SQLiteCommand m_sqlCmd2 = m_dbConn.CreateCommand();
                m_sqlCmd2.CommandText = "Select * from Commits";
                SQLiteDataReader data2 = m_sqlCmd2.ExecuteReader();

                while (data.Read())
                {
                    data2.Close();
                    data2 = m_sqlCmd2.ExecuteReader();
                    if (data.GetString(3) == user.id)
                    {
                        PSDProject m = new PSDProject(data.GetInt32(0), data.GetString(1), data.GetString(2), data.GetString(3));
                        AddProject(m);
                        while (data2.Read())
                        {
                            Save o = new Save(data2.GetString(3), data2.GetInt32(2), data2.GetInt32(1));
                            if (data.GetInt32(0) == data2.GetInt32(2))
                            {

                                var ms = Decompress(File.Open("data/" + m.owner_id.Replace(':', '-') + "/" + m.name.Remove(m.name.Length - 4) + "/commit" + o.que, FileMode.Open));
                                var t = File.Create("data/" + m.owner_id.Replace(':', '-') + "/" + m.name.Remove(m.name.Length - 4) + "/img");
                                foreach (var item in ms.ToArray())
                                {
                                    t.WriteByte(item);
                                }
                                ms.Close();
                                t.Close();

                                //var of = File.Open("data/" + m.owner_id.Replace(':', '-') + "/" + m.name.Remove(m.name.Length - 4) + "/img", FileMode.Open);
                                var image = new MagickImage("data/" + m.owner_id.Replace(':', '-') + "/" + m.name.Remove(m.name.Length - 4) + "/img");
                                var mem = new MemoryStream();
                                image.ToBitmap().Save(mem, System.Drawing.Imaging.ImageFormat.Bmp);       //ломается
                                o.preview = mem.ToArray();
                                mem.Close();

                                m.AddCommit(o);
                                ms.Close();
                            }

                        }
                    }
                }
                data.Close();
                data2.Close();
                m_dbConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DatabaseLoad error: " + ex.Message);
            }
        }
        public void DatabaseInsert(PSDProject a)  //Добавить проект в БД
        {
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "INSERT INTO Projects (project_name, dir, owner) values ('" + a.name + "','" + a.dir + "','" + a.owner_id + "')";
            m_sqlCmd.ExecuteNonQuery();
            m_dbConn.Close();
        }
        public void DatabaseDelete(int id)
        {

        } //удалить проект с БД
        //dropbox api
        public async static void TryUpload(Tu cb, PSDProject a, Dropbox.Api.DropboxClient k, User user) //Синхронизация
        {
            try
            {
                ListFolderResult z = new ListFolderResult();
                void innerupload()
                {
                    MessageBox.Show("попытка загрузки");
                    if (load < 1)
                    {
                        load++;
                        Thread th = new Thread(new ThreadStart(() => { var t = Task.Run(() => cb(a, k)); t.Wait(); load--; }));
                        th.Start();
                    }
                }
                void innerdownload(List<Save> saves)
                {
                    MessageBox.Show("попытка скачивания");
                    if (load < 1)
                    {
                        load++;
                        Thread th = new Thread(new ThreadStart(() => { var t = Task.Run(() => Download(k, a, user, saves)); load--; }));
                        th.Start();
                    }
                }

                //начало
                try
                {
                    //z = await k.Files.ListFolderAsync("/" + a.name.Remove(a.name.Length - 4));
                    var h = Task.Run(() => k.Files.DownloadAsync(path: "/" + a.name.Remove(a.name.Length - 4) + "/metadata"));
                    h.Wait();
                    var str = await h.Result.GetContentAsStringAsync();
                    List<Save> first = getmetadata(a);
                    List<Save> second = JsonConvert.DeserializeObject<List<Save>>(str);
                    string path = "data/" + a.owner_id.Replace(':', '-') + "/" + a.name.Remove(a.name.Length - 4);
                    if (first.Count > second.Count)
                    {
                        MessageBox.Show("Синхронизация: загрузка на dropbox");
                        innerupload();
                    }
                    else if (first.Count < second.Count)
                    {
                        MessageBox.Show("Синхронизация: скачивание");
                        innerdownload(second);
                        // RemoteProject rp = new RemoteProject() { name = a.name.Remove(a.name.Length - 4), fresh = true };
                        // await Download(k,rp, user);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("создается новая директория на dropbox: " + ex.Message);
                    innerupload();
                }

            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.Message);
            }


        }
        public async Task ProjectLoad(PSDProject t, DropboxClient user) //Залить проект на dropbox  [ОК]
        {
            string dirpath = "/" + t.name.Remove(t.name.Length - 4); //назвние проекта
            async Task insert()
            {
                foreach (Save o in t.Commits)
                {
                    var file = File.Open("data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/commit" + o.que, FileMode.Open);
                    await user.Files.UploadAsync(dirpath + "/commit" + o.que, WriteMode.Overwrite.Instance, body: file);
                }
                string path = "data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/preview.jpeg";
                //НЕ РАБОТАЕТ В ПРОЕКТАХ БЕЗ ПУТИ 
                var image = new MagickImage(t.dir + t.name);
                var fc = File.Create(path);
                image.ToBitmap().Save(fc, System.Drawing.Imaging.ImageFormat.Jpeg);
                fc.Close();
                var fo = File.Open(path, FileMode.Open);
                await user.Files.UploadAsync(dirpath + "/preview.jpeg", WriteMode.Overwrite.Instance, body: fo);
                File.Delete(path);
                fo.Close();

                using (var infons = File.Create("data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/metadata"))
                {
                    metadata(t, infons);
                }
                using (var infons = File.Open("data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/metadata", FileMode.Open))
                {
                    await user.Files.UploadAsync(dirpath + "/metadata", WriteMode.Overwrite.Instance, body: infons);
                    infons.Close();
                }
            }
            try
            {
                await user.Files.CreateFolderV2Async(dirpath);
                var u = Task.Run(() => insert());
                u.Wait();
            }
            catch (DropboxException e)
            {
                var u = Task.Run(() => insert());
                u.Wait();
            }

        }
        public static async Task Download(DropboxClient user, PSDProject projname, User myuser, List<Save> saves)
        {
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "select id from Projects where project_name = " + "'" + projname.name + "'";
            SQLiteDataReader data = m_sqlCmd.ExecuteReader();
            if (data.HasRows)
            {
                MessageBox.Show("Проект существует");
                data.Read();
                int id = data.GetInt32(0); //айди проекта
                SQLiteCommand m_sqlCmd2 = m_dbConn.CreateCommand();
                try
                {
                    foreach (var s in saves)
                    {
                        m_sqlCmd2.CommandText = "select commit_number from Commits where commit_number = " + s.que; //есть ли такой коммит
                        SQLiteDataReader data2 = m_sqlCmd2.ExecuteReader();
                        if (!data2.HasRows) //если такого коммита нет, то..
                        {
                            MessageBox.Show(s.message);
                            SQLiteCommand m_sqlCmd3 = m_dbConn.CreateCommand();
                            m_sqlCmd3.CommandText = "INSERT into Commits (commit_number, Project_id, message) values (" + s.que + "," + id + ",'" + s.message + "')";
                            m_sqlCmd3.ExecuteNonQuery();

                            var md = await user.Files.DownloadAsync(path: "/" + projname.name.Remove(projname.name.Length - 4) + "/commit" + s.que);
                            var file = await md.GetContentAsStreamAsync();

                            var t = File.Create("data/" + myuser.id.Replace(':', '-') + "/" + projname.name.Remove(projname.name.Length - 4) + "/commit" + s.que);
                            file.CopyTo(t);
                            file.Close();
                            t.Close();
                            projname.AddCommit(s);
                        }
                        data2.Close();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }

            m_dbConn.Close();
        } //Скачать проект с Dropbox
        public async Task<List<RemoteProject>> GetProjects(DropboxClient user)
        {
            RemoteProject y = new RemoteProject();
            List<RemoteProject> list = new List<RemoteProject>(); //список проектов с Dropbox
            var lst = await user.Files.ListFolderAsync(string.Empty);
            foreach (var item in lst.Entries) //проход по всем проектам
            {
                try
                {
                    var md = await user.Files.DownloadAsync(path: "/" + item.Name + "/metadata"); //проверка на наличие metadata файла на дропбоксе
                    string file = await md.GetContentAsStringAsync();
                    List<Save> eq = JsonConvert.DeserializeObject<List<Save>>(file); //данные о коммитах в Dropbox
                    y.commits = eq;
                    foreach (PSDProject u in UserProjects) //просмотр всех проектов на компе 
                    {
                        List<Save> mydate = getmetadata(u); //данные о коммитах на компе
                        if (eq.Count < mydate.Count) y.fresh = false;
                        else y.fresh = true;
                    }
                }
                catch (DropboxException e)
                {
                    MessageBox.Show("Ошибка получения файла metadata с Dropbox: " + e.Message);
                    continue;
                }
                try
                {
                    var k = await user.Files.DownloadAsync(path: "/" + item.Name + "/preview.jpeg");
                    var stream = await k.GetContentAsStreamAsync();
                    MemoryStream t = new MemoryStream();
                    stream.CopyTo(t);
                    y.name = item.Name;
                    y.image = t;
                    list.Add(y);
                }
                catch (DropboxException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return list;
        } //Получить список проектов с Dropbox
    }

    class PSDProject //проект
    {
        public bool infofile = false;
        public static event EventHandler okno;
        public FileSystemWatcher looks = new FileSystemWatcher(); //отслеживание файла
        public int id { get; set; }
        public string name { get; set; } //полное название файла
        public string dir { get; set; } //путь проекта, на конце "/"
        public string owner_id { get; set; } //имя пользователя проекта
        public void off()
        {
            this.looks.Dispose();
        }
        public ObservableCollection<Save> Commits { get; set; } //список коммитов проекта
        public void txt(string txt)
        {
            try
            {
                Save ns;
                ns = txt.Length == 0 ? new Save("autocommit", id, Commits.Count) : new Save(txt, id, Commits.Count);
                SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
                m_dbConn.Open();
                SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
                m_sqlCmd.CommandText = "INSERT INTO Commits (commit_number, Project_id, message) values (" + ns.que + "," + this.id + ",'" + ns.message + "')";
                m_sqlCmd.ExecuteNonQuery();
                m_dbConn.Close();



                AddCommit(ns);
                var t = File.Create("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/commit" + (Commits.Count - 1));
                var tr = File.Open(dir + name, FileMode.Open);
                GZipStream o = new GZipStream(t, CompressionMode.Compress);
                tr.CopyTo(o);
                o.Close();
                tr.Close();
                t.Close();

                //дублирование кода
                var ms = Data.Decompress(File.Open("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/commit" + ns.que, FileMode.Open));
                var tt = File.Create("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/img");
                foreach (var item in ms.ToArray())
                {
                    tt.WriteByte(item);
                }
                ms.Close();
                tt.Close();
                var image = new MagickImage("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/img");
                var mem = new MemoryStream();
                image.ToBitmap().Save(mem, System.Drawing.Imaging.ImageFormat.Bmp);       
                ns.preview = mem.ToArray();
                mem.Close();
                ms.Close();
                //======================
             
                // var ms = Data.Decompress(File.Open("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/commit" + ns.que, FileMode.Open));
                // ns.preview = new Bitmap(ms);

                using (var infons = File.Create("data/" + this.owner_id.Replace(':', '-') + "/" + this.name.Remove(this.name.Length - 4) + "/metadata"))
                {
                    Data.metadata(this, infons);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления нового коммита: " + ex.Message);
            }
        } //добавление нового коммита
        public PSDProject(int i, string n, string d, string v)
        {
            id = i;
            Commits = new ObservableCollection<Save>();
            name = n;
            dir = d;
            owner_id = v;
            // d + n = full directioay
            Directory.CreateDirectory("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4)); //замена недопустимых символов в пути
            try
            {
                looks.Path = d;
                looks.EnableRaisingEvents = false;
                looks.EnableRaisingEvents = true;
                looks.Deleted += (a, b) =>
                {
                    if (b.FullPath == dir + name)
                    {
                        looks.EnableRaisingEvents = false;
                        looks.EnableRaisingEvents = true;
                        okno(this, EventArgs.Empty);
                    }
                };
            }
            catch (Exception message)
            {
                MessageBox.Show("Ошибка в конструкторе PSDProject: " + message.Message);
            }
            finally
            {
                using (var infons = File.Create("data/" + this.owner_id.Replace(':', '-') + "/" + this.name.Remove(this.name.Length - 4) + "/metadata"))
                {
                    Data.metadata(this, infons);
                }
            }
        }


        public void AddCommit(Save b)
        {
            /*
            MagickImage image = new MagickImage();
            MagickReadSettings settings = new MagickReadSettings();
            settings.Density = new Density(144);
            image.Read(dir + name + ".psd", settings);
            Directory.CreateDirectory("data/" + name + "/" + savenum.ToString());
            image.Write("data/" + name + "/" + savenum.ToString() + "/" +"preview.jpg" );
            PSDFile.savenum++;
            */
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Commits.Add(b);
            });

        }  //добавить коммит

        public void SortCommits() //сортировать коммиты
        {
        }
        public static explicit operator RemoteProject(PSDProject proj)
        {
            RemoteProject k = new RemoteProject();
            k.name = proj.name.Remove(proj.name.Length - 4);
            return k;
        }
    }

    class PSDProjectOffline : PSDProject
    {
        public PSDProjectOffline(int i, string n, string d, string v) : base(i, n, d, v)
        {
            this.looks.EnableRaisingEvents = false;
        }
    }
    class Save //коммит
    {
        public string message { get; set; } //описание
        public int number { get; set; } //айди проекта
        public int que { get; set; } //айди коммита
        public byte[] preview { get; set; } //превью изображениия
        public Save(string m, int n, int q)
        {
            que = q;
            message = m;
            number = n;
        }
        public Save()
        {
            message = "commit_test";
            number = 0;
        }
        public void Open(PSDProject a)
        {
            string path = a.dir + this.message + this.que + ".psd";   //сохранить файл в дериктории исходного проекта f
            if (!File.Exists(path))
            {
                var file = File.Open("data/" + a.owner_id.Replace(':', '-') + "/" + a.name.Remove(a.name.Length - 4) + "/commit" + que, FileMode.Open);
                var k = File.Create(path);
                var t = new GZipStream(file, CompressionMode.Decompress);
                t.CopyTo(k);
                t.Close();
                k.Close();
                file.Close();
            }
            System.Diagnostics.Process.Start(path);
        }
    }


}

