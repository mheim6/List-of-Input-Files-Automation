using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace ListInputFilesScript
{
    class Program
    {
        static void Main(string[] args)
        {
            var textLines = File.ReadAllLines(@"C:\Users\mheim\Documents\List of the Input Files Saved with Date.txt");
            string nstarUsername = "rtfcm2h";
            Console.WriteLine("Enter password: ");
            string nstarPassword = IOHelper.ReadPasswordLine();
            NStarController nstarController = new NStarController(nstarUsername, nstarPassword);
            nstarController.Login();
            nstarController.bzhao.SendKey("<PF2>");
            nstarController.bzhao.WaitReady(10, 2);

            nstarController.bzhao.SendKey("<Enter>");
            nstarController.bzhao.WaitReady(10, 1);

            nstarController.bzhao.SendKey("<Enter>");
            nstarController.bzhao.WaitReady(10, 2);

            nstarController.bzhao.SendKey("p.3.4");
            nstarController.bzhao.SendKey("<Enter>");
            nstarController.bzhao.WaitReady(10, 1);


            List<CopyCommand> needsUpdating = new List<CopyCommand>();

            foreach (var textLine in textLines)
            {

                if (nstarController.GetString(3, 30, 21) != "Data Set List Utility")
                    throw new NotImplementedException();

                var fileName = textLine.Split('\t')[0].Trim();
                var latestVersion = textLine.Split('\t')[1].Trim();
                nstarController.bzhao.SetCursor(9, 24);
                nstarController.bzhao.SendKey("<EraseEof>");
                nstarController.bzhao.SendKey(fileName);

                nstarController.bzhao.SetCursor(14, 34);
                nstarController.bzhao.SendKey("/");

                nstarController.bzhao.SetCursor(15, 34);
                nstarController.bzhao.SendKey("/");

                nstarController.bzhao.SetCursor(16, 34);
                nstarController.bzhao.SendKey("/");

                nstarController.bzhao.SetCursor(17, 34);
                nstarController.bzhao.SendKey("/");

                nstarController.bzhao.SetCursor(18, 34);
                nstarController.bzhao.SendKey(" ");

                nstarController.bzhao.SetCursor(19, 34);
                nstarController.bzhao.SendKey(" ");


                nstarController.bzhao.SendKey("<Enter>");
                nstarController.bzhao.WaitReady(10, 1);


                if (nstarController.GetString(3, 11, 18) != "Data Sets Matching")
                    throw new NotImplementedException();
                var files = GetCopyCommands(nstarController, fileName, latestVersion);
                needsUpdating.AddRange(files);
                

                nstarController.bzhao.SendKey("<PF3>");
                nstarController.bzhao.WaitReady(10, 1);


            }

            nstarController.bzhao.SendKey("<PF3>");
            nstarController.bzhao.WaitReady(10, 1);
            nstarController.bzhao.SendKey("3.3");
            nstarController.bzhao.SendKey("<Enter>");
            nstarController.bzhao.WaitReady(10, 1);


            foreach (var copyCommand in needsUpdating)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (nstarController.GetString(3, 32, 17) == "Move/Copy Utility")

                        break;
                    System.Threading.Thread.Sleep(1000);

                }


                if (nstarController.GetString(3, 32, 17) != "Move/Copy Utility")
                    throw new NotImplementedException();

                nstarController.bzhao.SetCursor(18, 25);
                nstarController.bzhao.SendKey("<EraseEof>");
                nstarController.bzhao.SendKey("'" + copyCommand.fromfile + "'");
                nstarController.bzhao.SetCursor(41, 14);
                nstarController.bzhao.SendKey("c");
                nstarController.bzhao.SendKey("<Enter>");
                nstarController.bzhao.WaitReady(10, 1);

                nstarController.bzhao.SendKey("<EraseEof>");
                nstarController.bzhao.SendKey("'" + copyCommand.tofile + "'");
                nstarController.bzhao.SendKey("<Enter>");
                nstarController.bzhao.WaitReady(10, 1);

                nstarController.bzhao.SendKey("1");
                nstarController.bzhao.SendKey("<Enter>");
                nstarController.bzhao.WaitReady(10, 1);





            }


            File.WriteAllLines(@"C:\Users\mheim\Documents\LIF_UPDATED.txt", needsUpdating.Select(x => x.fromfile + " to " + x.tofile));



            nstarController.Close();

        }

        private static List<string> GetFileList(NStarController nstarController)
        {
            List<string> files = new List<string>();

            while (true)
            {
                bool done = false;
                for (int i = 7; i <= 40; i++)
                {
                    var file = nstarController.GetString(i, 11, 63).Trim();
                    if
                       (file == "******************** End of Data Set list *********************")
                    {

                        done = true;
                        break;
                    }

                    files.Add(file);
                }

                if (done)
                {
                    break;
                }

                nstarController.bzhao.SendKey("<PF8>");
                nstarController.bzhao.WaitReady(10, 1);
            }

            return files;
        }
        private static List<CopyCommand> GetCopyCommands(NStarController nstarController, string fileName, string latestVersion)
        {
            List<CopyCommand> copyCommands = new List<CopyCommand>();

            while (true)
            {
                bool done = false;
                for (int i = 7; i <= 40; i++)
                {
                    var file = nstarController.GetString(i, 11, 63).Trim();
                    if
                       (file == "******************** End of Data Set list *********************")
                    {

                        done = true;
                        break;
                    }

                    if (!Regex.IsMatch(file, fileName.Replace(".", "\\.") + "\\.G[0-9]{4}V[0-9]{2}"))
                    {
                        continue;
                    }

                    if (string.Compare(file, fileName + "." + latestVersion) <= 0)
                    {
                        continue;

                    }

                    var copyCommand = new CopyCommand();
                    copyCommand.fromfile = file;
                    copyCommands.Add(copyCommand);

                    nstarController.bzhao.SetCursor(i, 2);
                    nstarController.bzhao.SendKey("i");
                    nstarController.bzhao.SendKey("<Enter>");
                    nstarController.bzhao.WaitReady(10, 1);
                    copyCommand.tofile = fileName + ".DT" + nstarController.GetString(17, 66, 8).Replace("/", "");
                    nstarController.bzhao.SendKey("<PF3>");
                    nstarController.bzhao.WaitReady(10, 1);
                    



                   
                }

                if (done)
                {
                    break;
                }

                nstarController.bzhao.SendKey("<PF8>");
                nstarController.bzhao.WaitReady(10, 1);
            }

            return copyCommands;
        }

        private static string GetString(int v1, int v2, int v3)
        {
            throw new NotImplementedException();
        }
    }

    internal class CopyCommand
    {
        public string fromfile;
        public string tofile;




    }
}

