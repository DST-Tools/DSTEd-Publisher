using DSTEd.Publisher.Actions;
using System;

namespace DSTEd.Publisher {
    public static class Program {
        public static int Main(string[] args) {
            try {
                var parser      = new ArgumentParser();
               
                parser.AddHandler("update",     new Update());
                parser.AddHandler("upload",     new Upload());
                parser.AddHandler("download",   new Download());
                parser.AddHandler("status",     new Status());
                parser.AddHandler("list",       new List());
                parser.AddHandler("version",    new Software());

                return parser.Parse(args);
            } catch(Exception ex) {
                /* Do Nothing */
            }

            return 0;
        }
    }
}
