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


            actions.Add(new DSTEdActions.List());

            foreach (DSTEdActions.ActionBase action in actions)
            {
                if (action.Name == arguments[0])
                {
                    Console.InputEncoding = Encoding.Unicode;
                    Console.OutputEncoding = Encoding.Unicode;

                    string datajson;
                    if (arguments.Length >= 2)
                        datajson = Console.ReadLine();//for debug.
                    else
                        datajson = ReadLine('\0');
                    object data = JsonConvert.DeserializeObject(datajson, action.DataType);

                    int ret = (int)action.Do(data);

                    string outJson;
                    if (arguments.Length >= 2)
                        outJson = JsonConvert.SerializeObject(action.ResultObject, Formatting.Indented);//debug
                    else
                        outJson = JsonConvert.SerializeObject(action.ResultObject, Formatting.None);
                    Console.Write(outJson);
                    //Console.Write('\0');
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
