using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace archive
{

    class Program
    {

        static void Main(string[] args)
        {


            void Compress()
            {
                 byte[] byte_file = File.ReadAllBytes(@"D:\Vlodeslav\Kursach\psdarch2");
               // byte[] byte_file = { 0,0,0,0,0,0,0,0,0,1,0}
                List<byte> new_file = new List<byte>();
                List<byte> buffer = new List<byte>();
                List<byte> buffer2 = new List<byte>();
                int v = 0, pos = 0;
                byte dl = 0;
                void apart(ref List<byte> list, bool isrepeat)
                {
                    if (isrepeat == true)
                    {
                        new_file.Add((byte)(list.Count | 128));
                        new_file.Add(list[0]);
                    }
                    else
                    {
                        new_file.Add((byte)(list.Count & 127));
                        foreach (byte o in list )
                        {
                            new_file.Add(o);
                        }
                    }
                    list.Clear();
                }
                
                while (v < byte_file.Length-1)
                {
                 byte s = byte_file[v];
                    pos = v;
                    for (int i = v; i < byte_file.Length && byte_file[i] == s && dl != 127; dl++, i++)
                    {
                        buffer.Add(byte_file[i]);
                        v = i;
                    
                    }
                    dl = 0;
                   
                    if (buffer.Count == 1)
                    {
                        
                        for(int i = pos; i < byte_file.Length-1 && byte_file[i] != byte_file[i+1] && dl!=127; i++,dl++)
                        {
                         
                            buffer2.Add(byte_file[i]);
                            v = i;
                            if (i == byte_file.Length-2)
                            {
                                buffer2.Add(byte_file[i+1]);
                            }
                        }
                        
                         apart(ref buffer2, false);
                        buffer.Clear();

                        
                    }
                    else
                    {
                        apart(ref buffer, true);
                        buffer2.Clear();
                    }
                    dl = 0;
                    v++;

                }
               
                File.WriteAllBytes(@"D:\Vlodeslav\Kursach\psdarch3", new_file.ToArray());
            }
             
                try             
                    Compress();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
