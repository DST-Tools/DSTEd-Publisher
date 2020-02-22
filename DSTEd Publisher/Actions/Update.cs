using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher.Actions {
    class Update : ActionClass {
        public Update() {
            this.Name           = "update";
            this.Description    = "Update a local Steam-WorkShop item.";
            this.Arguments      = "<Directory|ID>";
        }

        public override int Run(string[] args) {
            Console.WriteLine("--update <Directory|ID> is currently not implemented.");
            return 0;
        }
    }
}
