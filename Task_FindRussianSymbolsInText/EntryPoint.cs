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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
