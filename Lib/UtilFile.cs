using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lib
{
    public class UtilFile
    {
        public static void DeleteOldFiles(string internalPath, int maxImageAgeDays)
        {
            // Delete all files from this directory older than maxImageAgeDays ...
            // (Except for the file named "Placeholder.txt")

            DateTime oldestImageDate = DateTime.UtcNow.AddDays(-maxImageAgeDays);

            string[] files = Directory.GetFiles(internalPath);
            foreach (string f in files)
            {
                if (!f.EndsWith("Placeholder.txt"))
                {
                    try
                    {
                        DateTime d = File.GetCreationTimeUtc(f);
                        if (d < oldestImageDate)
                        {
                            string newName = f + ".bak";
                            File.Move(f, newName);

                            File.Delete(newName);
                        }
                    }
                    catch
                    {
                        // If it doesn't work, the file may have been deleted by another request in the mean time.
                    }
                }
            }
        }

        public static bool FileExists(string fullPath)
        {
            return File.Exists(fullPath);
        }

        public static string GetText(string fullPath)
        {
            var sr = File.OpenText(fullPath);
            return sr.ReadToEnd();
        }
        public static List<string> GetTextLines(string fullPath)
        {
            var sr = File.OpenText(fullPath);
            var list = new List<string>();
            while (!sr.EndOfStream)
            {
                list.Add(sr.ReadLine());
                
            }
            return list;
        }
    }
}
