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
    class Data
    {
        public ObservableCollection<PSDProject> UserProjects { get; set; }

        public void AddProject(PSDProject b)
        {
            UserProjects.Add(b);
     
        }
        public Data()
        {
            UserProjects = new ObservableCollection<PSDProject>();
            
        }

    }



    class PSDProject //проект
    {
        public FileSystemWatcher looks = new FileSystemWatcher(); //отслеживание файла
        public string name { get; set; } //название проекта
        public string dir { get; set; } //путь проекта
        public string owner { get; set; } //уникальный id проекта
        public ObservableCollection<Save> Commits { get; set; } //список коммитов проекта
        public PSDProject(string n, string d, string v)
        {
            Commits = new ObservableCollection<Save>();
            name = n;
            dir = d;
            owner = v;
            Directory.CreateDirectory("data/" + owner + "/" + name);
            //   looks.Path = dir;
            //   looks.EnableRaisingEvents = true;

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
            Commits.Add(b);


        }  //добавить коммит

        public void SortCommits() //сортировать коммиты
        {

        }
    }

    class Save //коммит
    {
        public string message { get; set; }
        public int number { get; set; }
        public Save(string m)
        {
            message = m;
            number = 0;
        }
        public Save()
        {
            message = "commit_test";

        }
    }


}

