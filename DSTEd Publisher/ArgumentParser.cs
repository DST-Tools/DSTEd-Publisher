using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher {
     abstract class ActionClass {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Arguments { get; set; }

        public virtual int Run(string[] arguments) {
            /* Override Me */
            return -1;
        }
    }

    internal class ArgumentParser {
        List<ActionClass> handlers = new List<ActionClass>(900);

        internal void AddHandler(ActionClass handler) {
            handlers.Add(handler);
        }

        internal int Parse(string[]? args) {
            for(int position = 0; position < args.Length; ++position) {
                string value    = args[position];
                string command  = null;

                if(!value.StartsWith("--") && (value[0] == '-' || value[0] == '/')) {
                    command = value.Substring(1);
                } else if(value.StartsWith("--")) {
                    command = value.Substring(2);
                } else {
                    continue;
                }

                foreach(var entry in handlers) {
                    if(string.Compare(entry.Name, command, true) == 0) {
                        string[] arguments = new string[args.Length];

                        for(int index = 0; position < arguments.Length; position++) {
                            arguments[index] = args[index + position];
                        }

                        return entry.Run(arguments);
                    }
                }

                Help();
                return 0;
            }

            Help();
            return 0;
        }

        private void Help() {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("DSTEd Publisher version " + typeof(Program).Assembly.GetName().Version.ToString()  + "\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Usage:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Publisher.exe");

            foreach(var entry in handlers) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("\n\t--" + entry.Name);
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\t" + entry.Arguments);
                
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("\n\t\t\t" + entry.Description);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n");
        }
    }
}
