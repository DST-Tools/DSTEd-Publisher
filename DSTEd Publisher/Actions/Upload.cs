using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher.Actions {
    class Upload : ActionClass {
        public Upload() {
            this.Name           = "upload";
            this.Description    = "Uploads a new Steam-WorkShop item.";
            this.Arguments      = "<Directory>";
        }

        public override int Run(string[] arguments) {
            Console.WriteLine("--upload <Directory> is currently not implemented.");
            return 0;
        }
    }
}
