using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher
{
    internal class ArgumentParser
    {
        /// <summary>
        /// Command handler
        /// </summary>
        /// <param name="commandArgs">Arguments for this command</param>
        /// <returns>Returns a value as exit code</returns>
        internal delegate int CommandHandler(string[] commandArgs);

        Dictionary<string, CommandHandler> handlers = new Dictionary<string, CommandHandler>(4);
        internal string HelpMessage { get; set; }

        internal void AddHandler(string command, CommandHandler handler)
        {
            handlers.Add(command, handler);
        }

        /// <summary>
        /// Parser
        /// </summary>
        /// <param name="args">"string[] args" from Main</param>
        internal int Parse(string[]? args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                string str = args[i];
                string command = null;
                char firstChar = str[0];

                if (firstChar == '-' || firstChar == '/')
                    command = str.Substring(1);
                else
                {
                    if (str.StartsWith("--"))
                        command = str.Substring(2);
                    else
                        continue;
                }

                foreach (var kvPair in handlers)
                    if (string.Compare(kvPair.Key, command, true) == 0)  
                    {
                        var commandArgs = new string[args.Length];

                        for (int i2 = 0; i < commandArgs.Length; i++)
                            commandArgs[i2] = args[i2 + i];

                        return kvPair.Value(commandArgs);
                    }

                Console.WriteLine("Command not found, here is the help.\n");
                Console.WriteLine(HelpMessage);
                return 0;
            }

            Console.WriteLine("No Command provided, here is the help.\n");
            Console.WriteLine(HelpMessage);
            return 0;
        }
    }
}
