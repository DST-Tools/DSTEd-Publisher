using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher.Actions {
    class Status : ActionClass {
        public Status() {
            this.Name           = "status";
            this.Description    = "Displays the complete status of all uploaded Steam-WorkShop items and local projects.";
        }

        public override int Run(string[] arguments) {
            Console.WriteLine("--status is currently not implemented.");
            return 0;
        }
    }
}
