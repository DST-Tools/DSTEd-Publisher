using System;
using System.Collections.Generic;
#nullable enable

namespace DSTEd.Publisher {
     abstract class ActionClass {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;

        public abstract int Run(string[] arguments);
    }

    internal class ArgumentParser {
        List<ActionClass> handlers = new List<ActionClass>(10);

        internal void AddHandler(ActionClass handler) {
            handlers.Add(handler);
        }

        //args can't be null.
        internal int Parse(string[] args) {
            for(int position = 0; position < args.Length; ++position) {
                string value    = args[position];
                string command  = string.Empty;

                if(!value.StartsWith("--") && (value[0] == '-' || value[0] == '/')) {
                    command = value.Substring(1);
                } else if(value.StartsWith("--")) {
                    command = value.Substring(2);
                } else {
                    continue;
                }

                foreach(var entry in handlers) {
                    if(string.Compare(entry.Name, command, true) == 0) {
                        string[] arguments = new string[args.Length - position - 1];

                        //index begins with 1, so "/command" will not bring into argument
                        for(int index = 1; position < arguments.Length; position++) {
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
#pragma warning disable CS8602 // This chain will never get a null value
            Console.WriteLine("DSTEd Publisher version " + System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString()  + "\n");
#pragma warning restore CS8602 
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
