using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Task_FindRussianSymbolsInText
{
    public class FileWatcher
    {
        public static readonly string backupPath = ConfigurationManager.AppSettings["StartUrl"];
        public static readonly string path = Path.GetFullPath(@"D:\txt");
        public static readonly string infoFormatForFinding = "Ln: {0} Col: {1} Text: {2}";
        public static readonly string fileInfoFormat = "File name: {0} | Extension: {1} | Size {2} KB";
        private Regex regex;
        private string backupDirectory;
        private int lineNumber;

        public void Initialization()
        {
            regex = new Regex("[а-яА-Я]");
            backupDirectory = Path.GetDirectoryName(backupPath);
            if ((backupDirectory != null) && (!Directory.Exists(backupDirectory)))
            {
                Directory.CreateDirectory(backupDirectory);
            }
            ReadFile();
        }

        public void ReadFile()
        {
            var directory = new DirectoryInfo(path);
            var files = directory.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly).Where(x => ExtentionsList.extensions.Contains(x.Extension));
            foreach (FileInfo file in files)
            {
                Console.WriteLine(fileInfoFormat, file.Name, file.Extension, file != null ? file.Length.ToString() : "");

                var reader = file.OpenText();
                var lineNumber = 1;
                var newContentBuilder = new StringBuilder();
                while (!reader.EndOfStream)
                {
                    var line = new StringBuilder(reader.ReadLine());

                    Console.WriteLine(line);
                    Match match = regex.Match(line.ToString());

                    while (match.Success)
                    {
                        Console.WriteLine(infoFormatForFinding, lineNumber, match.Index, match.Value);
                        if (match.Value.Length == 1)
                        {
                            //line[match.Index] = ReplaceCharacter(match.Value[0]);
                        }
                        match = match.NextMatch();
                    }
                    lineNumber++;
                    newContentBuilder.AppendLine(line.ToString());
                }
                reader.Close();

                Console.WriteLine("Readed: {0} lines", lineNumber - 1);
                Console.WriteLine("Result after all changes is : ");
                Console.WriteLine(newContentBuilder);
                //Save(file, newContentBuilder);
            }
        }

    }
}