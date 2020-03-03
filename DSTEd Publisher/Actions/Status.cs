using System;
using DSTEd.Publisher.SteamWorkshop;

namespace DSTEd.Publisher.Actions {
    class Status : ActionClass {
        public Status() {
            this.Name           = "status";
            this.Description    = "Displays the complete status of all uploaded Steam-WorkShop items and local projects.";
        }

        public override int Run(string[] arguments) {
            if(!Steam.Start()) {
                Console.WriteLine("Steam is not running...");
                return -1;
            }

            // @ToDo implementation

            Steam.Stop();

            Console.WriteLine("--status is currently not implemented.");
            return 0;
        }
    }
}
