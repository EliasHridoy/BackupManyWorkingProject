using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Compressing
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to "+
            @"            
   ___                   _ _      _      _                _                
  /___\_ __   ___    ___| (_) ___| | __ | |__   __ _  ___| | ___   _ _ __  
 //  // '_ \ / _ \  / __| | |/ __| |/ / | '_ \ / _` |/ __| |/ / | | | '_ \ 
/ \_//| | | |  __/ | (__| | | (__|   <  | |_) | (_| | (__|   <| |_| | |_) |
\___/ |_| |_|\___|  \___|_|_|\___|_|\_\ |_.__/ \__,_|\___|_|\_\\__,_| .__/ 
                                                                    |_|    
"
            );
            var a  =  Compress(); 
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        class PathOfFile
        {
            public string FileName { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public bool WillCompressed { get; set; }
            public int HowManyBackupFileKeep {get;set;}
        }

        static bool Compress()
        {
             if(!IsFileExist()) return false;   
            string Lines = File.ReadAllText("./FilePath.txt");
            var filePath = new List<PathOfFile>();
            try{
             filePath = JsonConvert.DeserializeObject<List<PathOfFile>>(Lines);
            }
            catch(Exception)
            {
                Console.WriteLine("FilePath.txt <== This file is not in JSON formate. It's corrupted");
                return false;
            }
            foreach (var obj in filePath)
            {
                if (!obj.WillCompressed) continue;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Your Project ===> " + obj.FileName + " is compressing. Please wait... ");
                Console.ResetColor();
                string FromLocation = Path.GetFullPath(obj.From);
                string ToLocation = Path.GetFullPath(obj.To);
                if (!Directory.Exists(FromLocation))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FromLocation of ===> " + obj.FileName + " this project is not found!!!");
                    Console.ResetColor();
                    continue;
                }
                if (!Directory.Exists(ToLocation))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ToLocation of ===> " + obj.FileName + " this project is not found!!!");
                    Console.ResetColor();
                    continue;
                }
                int Done = 5;
                string fileName = obj.FileName + " " + DateTime.Now.ToString("dd-MMM-yyyy hh-mm-s tt");
                while (Done-- > 0)
                {
                    ToLocation = Path.Combine(ToLocation, fileName + ".zip");
                    try
                    {
                        var work = new Task (()=>{
                            ZipFile.CreateFromDirectory(FromLocation, ToLocation);
                            Thread.Sleep(4000);
                        });
                        work.Start();

                        while (!work.IsCompleted)
                        {
                            
                            Console.Write('.');
                            Thread.Sleep(500);
                        }
                        Console.WriteLine();
                        Done = 0;
                    }
                    catch (IOException)
                    {
                        fileName += "(1)";
                    }
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(obj.FileName + " is compressed.");
                Console.ResetColor();

                if(obj.HowManyBackupFileKeep>0){
                    DeletePreviousBackup(obj.To, obj.FileName, obj.HowManyBackupFileKeep);
                }
            }
            Console.WriteLine("" +
            @"          
 ██████  ██████  ███    ███ ██████  ██      ███████ ████████ ███████ ██████  
██      ██    ██ ████  ████ ██   ██ ██      ██         ██    ██      ██   ██ 
██      ██    ██ ██ ████ ██ ██████  ██      █████      ██    █████   ██   ██ 
██      ██    ██ ██  ██  ██ ██      ██      ██         ██    ██      ██   ██ 
 ██████  ██████  ██      ██ ██      ███████ ███████    ██    ███████ ██████                                                                                                     
");


            return true;
        }
        static bool IsFileExist(){
            string FullPath = Path.GetFullPath("FilePath.txt");

            var isFileExist = File.Exists(FullPath);
            if (!isFileExist)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This \"FilePath.txt\" file is not exist. Please restore it.");
                Console.ResetColor();
                return false;
            }
            return true;
        }
        static void DeletePreviousBackup(string path, string FileName, int KeepItem)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fiArray = di.GetFiles();
            Array.Sort(fiArray, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.CreationTime, y.CreationTime));
            fiArray = Array.FindAll(fiArray, a => a.FullName.Contains(FileName) == true);
            int deleteItem = fiArray.Length - KeepItem - 1;
            for (int i = 0; i < deleteItem; i++)
            {
                Console.WriteLine("Deleting " + fiArray[i].FullName + " this file");
                File.Delete(fiArray[i].FullName);
            }
        }
 
    }
}
