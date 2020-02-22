using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher.Actions {
    class Download : ActionClass {
        public Download() {
            this.Name           = "download";
            this.Description    = "Download a Steam-WorkShop item.";
            this.Arguments      = "<ID>";
        }

        public override int Run(string[] arguments) {
            Console.WriteLine("--download <ID> is currently not implemented.");
            return 0;
        }
    }
}
