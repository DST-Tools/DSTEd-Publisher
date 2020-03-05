using System;
using System.Collections.Generic;
using DSTEd.Publisher.SteamWorkshop;

namespace DSTEd.Publisher.Actions {
    class List : ActionClass {
        public List() {
            this.Name             = "list";
            this.Description      = "Displays a list of all published Steam-WorkShop items.";
            this.Arguments        = "<Begin page (optional)>";
        }

        public override int Run(string[] arguments) {
            uint page = 1;

            if(!Steam.Start(Steam.APP_ID)) {
                Console.WriteLine("Steam is not running...");
                return -1;
            }

            if(arguments.Length >= 1) {
                page = uint.Parse(arguments[0]);
            }

            Console.WriteLine("Page: " + page);

            Steam.GetWorkShopItems(page, delegate(Steam.ExitCodes error, List<WorkshopItem> results, uint count, uint total) {
#if DEBUG
                Console.WriteLine("Callback");
#endif

                if (error != 0) {
                    Console.WriteLine($"Some UGC Error! (Code: {error}");
                    return;
                }

                Console.WriteLine($"You have published {total} mod(s) in total.\nThis is page {page} with {count} results (max. 50 per page)");

                foreach(WorkshopItem entry in results) {
                    Console.WriteLine(entry.ToString());
                }

                // @ToDo create output
            });

            Steam.Stop();

            return 0;
        }
    }
}
