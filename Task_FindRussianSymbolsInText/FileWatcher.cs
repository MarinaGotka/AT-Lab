using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Task_FindRussianSymbolsInText
{
    public class FileWatcher
    {
        public static readonly string backupPath = ConfigurationManager.AppSettings["BackupPath"];
        public static readonly string path = ConfigurationManager.AppSettings["Path"];
        public static readonly string infoFormatForFinding = ConfigurationManager.AppSettings["FormatForInfoRussianSymbols"];
        public static readonly string fileInfoFormat = ConfigurationManager.AppSettings["FormatFileInfo"];
        public static readonly string regexTemplate = ConfigurationManager.AppSettings["RegexTemplate"];
        private readonly string culture = "ru-RU";
        private readonly string backupDirectory;
        private Regex regex;

        public FileWatcher()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            regex = new Regex(regexTemplate);
            backupDirectory = Path.GetDirectoryName(backupPath);
            if ((backupDirectory != null) && (!Directory.Exists(backupDirectory)))
            {
                Directory.CreateDirectory(backupDirectory);
            }
            ReadFile();
        }

        /// <summary>
        /// Метод для чтения текста из файла и замены русских символов на английские.
        /// </summary>
        public void ReadFile()
        {
            var directory = new DirectoryInfo(path);
            var files = directory.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly).Where(x => ExtentionsList.extensions.Contains(x.Extension));
            foreach (FileInfo file in files)
            {
                Console.WriteLine(fileInfoFormat, file.Name, file.Extension, file != null ? file.Length.ToString() : throw new Exception("Error.File is null"));
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
                       for(int i = 0; i< match.Value.Length; i++)
                        {
                            line[match.Index + i] = ReplaceCharacter(match.Value[i]);
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
                Save(file, newContentBuilder);
            }
        }

        /// <summary>
        /// Метод для получения разрешения от пользователя на сохранение изменений в файле.
        /// </summary>
        /// <param name="file"> файл </param>
        /// <param name="content"> измененное содержимое </param>
        public void Save(FileInfo file, StringBuilder content)
        {
            Console.WriteLine("Do you wanna replace your file with new content? Y/N");
            var answer = Console.ReadKey();

            while (true)
            {
                if (answer.Key == ConsoleKey.N)
                {
                    break;
                }
                if (answer.Key == ConsoleKey.Y)
                {
                    CopyToBackupDirectory(backupDirectory, file);
                    UpdateCurrentFileContent(file.FullName, content);

                    break;
                }
                Console.WriteLine("Error! Input Y or N.");
                answer = Console.ReadKey();
            }
            Console.WriteLine(Environment.NewLine + "Save is done.");
        }

        /// <summary>
		/// Метод для копирования содержимого файла в директорию бэкап
		/// </summary>
		/// <param name="backupDirectory"></param>
		/// <param name="file"></param>
		public void CopyToBackupDirectory(string backupDirectory, FileInfo file)
        {
            var backupFilePath = backupDirectory + "\\" + file.Name;
            file.CopyTo(Path.GetFullPath(backupFilePath), true);
        }

        /// <summary>
		///Метод для создания нового файла (или подмены существующего) с наполнением его новыми данными
		/// </summary>
		/// <param name="fullPath">Полный путь к файлу</param>
		/// <param name="content">Данные, которые будут записаны</param>
		public void UpdateCurrentFileContent(string fullPath, StringBuilder content)
        {

            FileInfo newFile = new FileInfo(fullPath);
            var writer = newFile.CreateText();
            writer.WriteLine(content.ToString());
            writer.Close();
        }

        /// <summary>
		/// Метод для замены символа на аналог английской буквы с сохранением регистра.
		/// <param name="s">Строка с одним символом</param>
		/// <returns></returns>
		public char ReplaceCharacter(char s)
        {
            var category = char.GetUnicodeCategory(s);
            var newChar = Replace(char.ToLower(s));
            return char.GetUnicodeCategory(newChar) != category ? char.ToUpper(newChar) : newChar;
        }

        /// <summary>
		/// Подменяет (частично) русские буквы на английские.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public char Replace(char value)
        {
            switch (value)
            {
                case 'б': return 'b';
                case 'в': return 'v';
                case 'г': return 'g';
                case 'д': return 'd';
                case 'ж': return 'j';
                case 'з': return 'z';
                case 'и': return 'i';
                case 'л': return 'l';
                case 'м': return 'm';
                case 'н': return 'n';
                case 'п': return 'p';
                case 'р': return 'r';
                case 'с': return 's';
                case 'т': return 't';
                case 'у': return 'u';
                default:
                    return value;
            }
        }
    }
}