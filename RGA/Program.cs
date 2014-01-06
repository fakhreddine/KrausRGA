using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGA;
using RGA.VersionCheck;
namespace RGA
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Update Version

            try
            {
                Console.WriteLine("New updates are available for Application, Please wait downloading updates...");
                Console.WriteLine("*[Warning : Please do not interrupt while updating. ]");
                GetFileNames.Url = "http://192.168.1.16/FTPRGA/";
                if (GetFileNames.ListDiractory().Count > 0)
                {
                    foreach (String Sitem in Directory.GetFiles(Environment.CurrentDirectory))
                    {
                        if (Sitem != Environment.CurrentDirectory + "\\RGA.pdb" && Sitem != Environment.CurrentDirectory + "\\RGA.exe" && Sitem != Environment.CurrentDirectory + "\\RGA.exe.config" && Sitem != Environment.CurrentDirectory + "\\RGA.vshost.exe.config" && Sitem != Environment.CurrentDirectory + "\\RGA.vshost.exe")
                        {
                            try
                            {
                                File.Delete(Sitem);
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                        }
                    }
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\NewFiles\\");

                    foreach (String Sitem in GetFileNames.ListDiractory())
                    {
                        try
                        {

                            GetFileNames.downloadFromFTP(Sitem, Environment.CurrentDirectory + "\\NewFiles\\");
                            if (Sitem.Contains(".txt") && !Sitem.Contains("VersionNumber.txt"))
                                File.Move(Environment.CurrentDirectory + "\\NewFiles\\" + Sitem, Environment.CurrentDirectory + "\\" + Sitem.Replace(".txt", ""));
                            else
                                File.Move(Environment.CurrentDirectory + "\\NewFiles\\" + Sitem, Environment.CurrentDirectory + "\\" + Sitem);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception)
            { }
            finally
            {
                try
                {
                    Directory.Delete(Environment.CurrentDirectory + "\\NewFiles\\", true);
                    String DirPath = Environment.CurrentDirectory;
                    System.Diagnostics.ProcessStartInfo RgaApplication = new System.Diagnostics.ProcessStartInfo();
                    RgaApplication.FileName = DirPath + "\\KrausRGA.exe";
                    RgaApplication.Verb = "runas";
                    RgaApplication.WorkingDirectory = DirPath;
                    RgaApplication.UseShellExecute = true;
                    System.Diagnostics.Process.Start(RgaApplication);
                }
                catch (Exception)
                {
                    //Directory.Delete(Environment.CurrentDirectory , true);
                    String DirPath = Environment.CurrentDirectory;
                    System.Diagnostics.ProcessStartInfo RgaApplication = new System.Diagnostics.ProcessStartInfo();
                    RgaApplication.FileName = DirPath + "\\KrausRGA.exe";
                    RgaApplication.Verb = "runas";
                    RgaApplication.WorkingDirectory = DirPath;
                    RgaApplication.UseShellExecute = true;
                    System.Diagnostics.Process.Start(RgaApplication);
                }
                
            }
            #endregion
        }
    }
}
