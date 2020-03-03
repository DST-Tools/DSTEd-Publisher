using System;
using DSTEd.Publisher.SteamWorkshop;

namespace DSTEd.Publisher.Actions {
    class Update : ActionClass {
        public Update() {
            this.Name           = "update";
            this.Description    = "Update a local Steam-WorkShop item.";
            this.Arguments      = "<Directory|ID>";
        }

        public override int Run(string[] arguments) {
            if(!Steam.Start()) {
                Console.WriteLine("Steam is not running...");
                return -1;
            }

            Console.WriteLine("--update <Directory|ID> is currently not implemented.");

            Steam.Stop();

            return 0;
        }
    }
}
