using System;

namespace Task_FindRussianSymbolsInText
{
    public class EntryPoint
    {
        static void Main(string[] args)
        {
            try
            {
                var watcher = new FileWatcher();
                watcher.Initialization();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
