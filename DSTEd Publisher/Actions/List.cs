using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher.Actions {
    class List : ActionClass {
        public List() {
            this.Name           = "list";
            this.Description    = "Displays a list of all published Steam-WorkShop items.";
        }

        public override int Run(string[] arguments) {
            Console.WriteLine("--list is currently not implemented.");
            return 0;
        }
    }
}
