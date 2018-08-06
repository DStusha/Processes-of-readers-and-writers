using System;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;

namespace Reader
{
    class Read
    {
        public static Mutex m = Mutex.OpenExisting("myMutex");
        static void Main(string[] args)
        { 
            MemoryMappedFile map = MemoryMappedFile.CreateOrOpen("myMap", 1024 * 1024);
            MemoryMappedViewAccessor a = map.CreateViewAccessor();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\file.txt";
            while (true)
            {
                m.WaitOne();
                int countRec = a.ReadInt32(0);
                if (countRec != 0)
                {
                    int finalValue = a.ReadInt32(sizeof(int) + (countRec - 1) * sizeof(Int32));
                    Console.WriteLine(" Читатель считал запись № " + finalValue);
                    Console.WriteLine();
                    string text = " Читатель считал запись № " + finalValue + Environment.NewLine;
                    File.AppendAllText(path, text, Encoding.UTF8);
                    countRec--;
                    a.Write(0, countRec);
                }
                m.ReleaseMutex();
            }
        }
        ~Read()
        {
            m.Dispose();
        }
    }
}
