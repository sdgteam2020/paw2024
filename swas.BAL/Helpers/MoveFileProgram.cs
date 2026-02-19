using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace swas.BAL.Helpers
{
    public class MoveFileProgram
    {
        private static string des = "";
        private static string source = "";
        private static string file = "";

        public MoveFileProgram(IConfiguration configuration)
        {
            des = configuration.GetValue<string>("destinationFolder") ?? "";
            source = configuration.GetValue<string>("sourceFolder") ?? "";
            file = configuration.GetValue<string>("filePath") ?? "";
        }
        public  void MoveFile()
        {
            try
            {
                string destinationFolder = des;
                string sourceFolder = source;
                string filePath = file;
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("MoveFile.txt not found.");
                    return;
                }
                string[] fileNames = File.ReadAllLines(filePath);

                foreach (string fileName in fileNames)
                {
                    string trimmedFileName = fileName.Trim();
                    if (string.IsNullOrEmpty(trimmedFileName)) continue;

                    string sourceFile = Path.Combine(sourceFolder, trimmedFileName);
                    string destinationFile = Path.Combine(destinationFolder, trimmedFileName);

                    if (File.Exists(sourceFile))
                    {
                        File.Move(sourceFile, destinationFile);
                        Console.WriteLine($"Moved: {trimmedFileName}");
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {trimmedFileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
