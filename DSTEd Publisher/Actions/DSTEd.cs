using System;
using System.Collections.Generic;
using System.Text;
using static DSTEd.Publisher.SteamWorkshop.Steam;
using Newtonsoft.Json;
namespace DSTEd.Publisher.Actions
{
    /// <summary>
    /// This will be used, when DSTEd runs publisher.
    /// </summary>
    class DSTEd : ActionClass
    {
        int Exitcode = 0;//exit with nothing wrong.
        List<DSTEdActions.ActionBase> actions = new List<DSTEdActions.ActionBase>(10);
        public DSTEd()
        {
            base.Name = "DSTEdInternal";
            base.Description = "For DSTEd internal use. Read command and data from standard input.";
            base.Arguments = "No arguments";


            //Steamworks.SteamUGC.SetAllowLegacyUpload
        }

        public override int Run(string[] arguments)
        {
            if(arguments.Length < 1)
            {
                Console.WriteLine("At least 1 argument is needed.");
                return (int)ExitCodes.ArgumentsMissing;
            }

            foreach (DSTEdActions.ActionBase action in actions)
            {
                if (string.Compare(action.Name, arguments[0], true) == 0)
                {
                    Console.InputEncoding = Encoding.UTF8;
                    Console.OutputEncoding = Encoding.UTF8;

                    string datajson = ReadLine('\0');
                    object data = JsonConvert.DeserializeObject(datajson, action.DataType);

                    int ret = (int)action.Do(data);
                    Console.WriteLine(JsonConvert.SerializeObject(action.ResultObject, Formatting.None));

                    return ret;
                }
            }

            Console.WriteLine("No Command Found. Avaliable commands are\n");
            foreach (DSTEdActions.ActionBase action in actions)
                Console.WriteLine(action.Name);

            return (int)ExitCodes.GenericError;//nothing done
        }

        /// <summary>
        /// C++ cin::getline like console read function.
        /// </summary>
        /// <param name="delim"></param>
        /// <returns>line readed</returns>
        private static string ReadLine(char delim)
        {
            var data = new StringBuilder(80);
            for (char ch = (char)Console.Read(); ch != delim; ch = (char)Console.Read())
                data.Append(ch);

            return data.ToString();
        }
    }
}
