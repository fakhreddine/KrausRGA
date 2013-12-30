using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace KrausRGA.VersionCheck
{
    public static class GetFileNames
    {
        public static string Url = "";

        public static string GetDirectoryListingRegexForUrl(string url)
        {
            if (url.Equals(Url))
            {
                return "\\\"([^\"]*)\\\"";
            }
            throw new NotSupportedException();
        }

        public static List<string> ListDiractory()
        {
            List<string> lsFiles = new List<string>();
            string url = Url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();

                    Regex regex = new Regex(GetDirectoryListingRegexForUrl(url));
                    MatchCollection matches = regex.Matches(html);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Success && match.ToString().Contains('.'))
                            {

                                String s = match.ToString();
                                s = s.Replace('"', ' ').Trim(' ');
                                s = s.Replace('/', ' ').Trim(' ');
                                s = Removelast(s);
                                lsFiles.Add(s);
                            }
                        }
                    }
                }
            }
            return lsFiles;
        }

        /// <summary>
        /// Best Of My Logic.
        /// Recursive Fucntion call .
        /// </summary>
        /// <param name="Filename">
        /// File Name Count
        /// </param>
        /// <returns></returns>
        public static String Removelast(String Filename)
        {
            String _return = Filename;
            int i = Filename.IndexOf(' ');
            _return = Filename.Remove(0, i + 1);
            if (Filename.Contains(' '))
                _return = Removelast(_return);
            return _return;
        }

        /// <summary>
        /// Downloads Files From HTTP server.
        /// and Move that files from current diracotry To another Location.
        /// </summary>
        /// <param name="FileName">
        /// String File Name to Downlad
        /// </param>
        /// <param name="Url">
        /// string Url To download files
        /// </param>
        /// <param name="CopyToLocation">
        /// Copy that files to which Filder. (Move Files From Current Folder to another.)
        /// </param>
        public static void downloadFromFTP(String FileName, String CopyToLocation)
        {
            try
            {
                string remoteUri = Url;
                string fileName = FileName, myStringWebResource = null;
                // Create a new WebClient instance.
                WebClient myWebClient = new WebClient();

                // Concatenate the domain with the Web resource filename.
                myStringWebResource = remoteUri + fileName;

                // Download the Web resource and save it into the current filesystem folder.
                myWebClient.DownloadFile(myStringWebResource, fileName);
                try
                {
                    File.Move(Environment.CurrentDirectory + "\\" + fileName, CopyToLocation + fileName);
                    Console.WriteLine(DateTime.Now.ToString("hh:mm:ss tt ") + " Downloading File : " + fileName);

                }
                catch (DirectoryNotFoundException)
                {
                    Directory.CreateDirectory(CopyToLocation);
                }
                catch (Exception)
                { }
            }
            catch (Exception)
            { }

        }
    }
}
