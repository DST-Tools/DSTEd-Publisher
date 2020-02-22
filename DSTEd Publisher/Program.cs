using System;
namespace DSTEd.Publisher
{
    public static class Program
    {
        static int Update(string[] commandArgs)
        {
            return 0;
        }
        static string Help_enUS = "";

        public static int Main(string[] args)
        {
            try
            {
                var parser = new ArgumentParser();
                string language = System.Globalization.CultureInfo.CurrentCulture.EnglishName;
                try
                {
                    using var reader = new System.IO.StreamReader(".\\help-" + language);
                    parser.HelpMessage = reader.ReadToEnd();
                }
                catch (System.IO.FileNotFoundException)
                {
                    parser.HelpMessage = Help_enUS;
                    throw;
                }
                parser.AddHandler("update", Update);


                return parser.Parse(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ex.HResult;
            }
        }
    }
}
