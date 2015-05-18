using System;

namespace BoomerangX.DataFetcher
{
    public static class Logger
    {
        public static bool showlog = false;

        public static void Log(string s)
        {
            if (showlog)
                Console.WriteLine(s);
        }


    }
}
