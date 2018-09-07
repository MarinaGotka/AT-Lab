using System;

namespace Task_FindRussianSymbolsInText
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            try
            {
                FileWatcher watcher = new FileWatcher();
                watcher.Initialization();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
