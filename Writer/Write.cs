using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Writer
{
    class Write
    {
        public static Mutex m = Mutex.OpenExisting("myMutex");
        static void Main(string[] args)
        {
            int capacityMap = 1024 * 1024;
            MemoryMappedFile map = MemoryMappedFile.CreateOrOpen("myMap", capacityMap);
            MemoryMappedViewAccessor a = map.CreateViewAccessor();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\file.txt";
            int numRec = 0;
            bool flag = true;
            int capacityWrite = 0;
            while (flag)
            {
                m.WaitOne();
                int countRec = a.ReadInt32(0);
                int pos = a.ReadInt32(4);
                if (capacityWrite >= capacityMap)
                {
                    flag = false;
                    Console.WriteLine(" Писатель № " + args[0] + " уничтожен");
                    Console.WriteLine();
                    string text = " Писатель № " + args[0] + " уничтожен" + Environment.NewLine; ;
                    File.AppendAllText(path, text, Encoding.UTF8);
                }
                else
                {
                    a.Write(sizeof(int) + countRec * sizeof(Int32), numRec);
					capacityWrite = capacityWrite + sizeof(int);
                    Console.WriteLine(" Писатель № " + args[0] + " запись № " + numRec.ToString());
                    Console.WriteLine();
                    string text1 = countRec + " Писатель № " + args[0] + " запись № " + numRec + Environment.NewLine;
                    capacityWrite = capacityWrite + sizeof(Int32) + sizeof(char) * 12 + sizeof(char) + sizeof(char) * 10;
                    File.AppendAllText(path, text1, Encoding.UTF8);
                    numRec++;
                    countRec++;
                    a.Write(0, countRec);
                }
                 m.ReleaseMutex();
            }
        }
        ~Write()
        {
            m.Dispose();
        }
    }
}
