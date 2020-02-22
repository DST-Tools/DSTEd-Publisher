using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher {
     abstract class ActionClass {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Arguments { get; set; }

        public virtual int Run(string[] args) {
            /* Override Me */
            return -1;
        }
    }

    internal class ArgumentParser {
        internal delegate int CommandHandler(string[] commandArgs);

        Dictionary<string, ActionClass> handlers = new Dictionary<string, ActionClass>(4);

        internal void AddHandler(string command, ActionClass handler) {
            handlers.Add(command, handler);
        }

        internal int Parse(string[]? args) {
            for(int position = 0; position < args.Length; ++position) {
                string value    = args[position];
                string command  = null;
                char firstChar  = value[0];

                if(!value.StartsWith("--") && (firstChar == '-' || firstChar == '/')) {
                    command = value.Substring(1);
                } else {
                    if(value.StartsWith("--")) {
                        command = value.Substring(2);
                    } else {
                        continue;
                    }
                }

                foreach(var pair in handlers) {
                    if(string.Compare(pair.Key, command, true) == 0) {
                        string[] commandArgs = new string[args.Length];

                        for(int index = 0; position < commandArgs.Length; position++) {
                            commandArgs[index] = args[index + position];
                        }

                        return pair.Value.Run(commandArgs);
                    }
                }

                Help();
                return 0;
            }

            Help();
            return 0;
        }

        private void Help() {
            Console.WriteLine("DSTEd Publisher version " + typeof(Program).Assembly.GetName().Version.ToString()  + "\n");
            Console.WriteLine("Usage:\n");
            Console.WriteLine("Publisher.exe");

            foreach (var pair in handlers) {
                Console.WriteLine("\n\t--" + pair.Value.Name + " " + pair.Value.Arguments);
                Console.WriteLine("\n\t\t" + pair.Value.Description);
            }
        }
    }
}
