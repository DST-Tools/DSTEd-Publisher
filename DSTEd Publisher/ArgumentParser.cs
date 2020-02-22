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
        List<ActionClass> handlers = new List<ActionClass>(4);

        internal void AddHandler(ActionClass handler) {
            handlers.Add(handler);
        }

        internal int Parse(string[]? args) {
            for(int position = 0; position < args.Length; ++position) {
                string value    = args[position];
                string command  = null;

                if(!value.StartsWith("--") && (value[0] == '-' || value[0] == '/')) {
                    command = value.Substring(1);
                } else {
                    if(value.StartsWith("--")) {
                        command = value.Substring(2);
                    } else {
                        continue;
                    }
                }

                foreach(var pair in handlers) {
                    if(string.Compare(pair.Name, command, true) == 0) {
                        string[] arguments = new string[args.Length];

                        for(int index = 0; position < arguments.Length; position++) {
                            arguments[index] = args[index + position];
                        }

                        return pair.Run(arguments);
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
                Console.WriteLine("\n\t--" + pair.Name + " " + pair.Arguments);
                Console.WriteLine("\n\t\t" + pair.Description);
            }
        }
    }
}
