using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dianoga.ImageMagick;
using System.Collections.ObjectModel;
using System.IO;
using DropbBoxLogIn;
using System.IO.Compression;
using System.ComponentModel;
namespace dp
{
    class Data: INotifyPropertyChanged //хранимая информации о всех проектах
    {
        public ObservableCollection<PSDProject> UserProjects { get; set; }

        public void AddProject(PSDProject b)
        {
            UserProjects.Add(b);
            PropertyChanged(this, new PropertyChangedEventArgs(""));
        }
        public Data()
        {
            UserProjects = new ObservableCollection<PSDProject>();
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }



    class PSDProject //проект
    {
        public FileSystemWatcher looks = new FileSystemWatcher(); //отслеживание файла
        public string name { get; set; } //название проекта
        public string dir { get; set; } //путь проекта
        public string id { get; set; } //уникальный id проекта
        public List<Save> commits = new List<Save>(); //список коммитов проекта
        public PSDProject(string n, string d, string v)
        {
            name = n;
            dir = d;
            id = v;
         //   looks.Path = dir;
         //   looks.EnableRaisingEvents = true;
          //  Directory.CreateDirectory("data/" + id);
        }

        public void AddCommit(Save b)
        {
            commits.Add(new Save());
            /*
            MagickImage image = new MagickImage();
            MagickReadSettings settings = new MagickReadSettings();
            settings.Density = new Density(144);
            image.Read(dir + name + ".psd", settings);
            Directory.CreateDirectory("data/" + name + "/" + savenum.ToString());
            image.Write("data/" + name + "/" + savenum.ToString() + "/" +"preview.jpg" );
            PSDFile.savenum++;
            */
            commits.Add(b);


        }  //добавить коммит

        public void SortCommits() //сортировать коммиты
        {

        }
    }

    class Save //коммит
    {
        string message { get; set; }
        public Save(string m)
        {
            message = m;
        }
        public Save()
        {
            message = "commit_test";

        }
    }


}

