using System;
using DSTEd.Publisher.SteamWorkshop;

namespace DSTEd.Publisher.Actions {
    class Download : ActionClass {
        public Download() {
            this.Name           = "download";
            this.Description    = "Download a Steam-WorkShop item.";
            this.Arguments      = "<ID>";
        }

        public override int Run(string[] arguments) {
            if(!Steam.Start()) {
                Console.WriteLine("Steam is not running...");
                return -1;
            }

            Console.WriteLine("--download <ID> is currently not implemented.");

            Steam.Stop();

            return 0;
        }
    }
}
