using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Threading;

namespace Processes
{
    class Program
    {
        public static Mutex myMutex = new Mutex(true, "myMutex");
        static void Main(string[] args)
        {
            using (MemoryMappedFile myMap = MemoryMappedFile.CreateNew("myMap", 1024 * 1024))
            {
                int writersCount = 4;
                int i = 0;

                // в свойствах проектов Writer и Reader в выходной путь сборки прописать Debug проекта Processes
                string pathProcessRead = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Reader.exe";
                string pathProcessWrite = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Writer.exe";

                using (var myMap2 = MemoryMappedFile.CreateOrOpen("myMap", 1024 * 1024))
                {
                    var accessor = myMap2.CreateViewAccessor();
                    accessor.Write(0, 0);
                }
                myMutex.ReleaseMutex();
                Process processRead = new Process();
                processRead.StartInfo.FileName = pathProcessRead;
                processRead.StartInfo.UseShellExecute = false;
                processRead.Start();
                while (i < writersCount)
                {
                    Thread.Sleep(1000);
                    Process processWrite = new Process();
                    processWrite.StartInfo.FileName = pathProcessWrite;
                    processWrite.StartInfo.Arguments = Convert.ToString(i);
                    processWrite.StartInfo.UseShellExecute = false;
                    processWrite.Start();
                    i++;
                }
            }
        }
        ~Program()
        {
            myMutex.Dispose();
        }
    }
}
