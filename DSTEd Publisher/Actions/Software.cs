using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher.Actions {
    class Software : ActionClass {
        public Software() {
            this.Name           = "version"; 
            this.Description    = "Displays the version information for the Publisher.";
        }

        public override int Run(string[] arguments) {
            Console.WriteLine("Version " + typeof(Program).Assembly.GetName().Version.ToString()  + "\n");
            return 0;
        }
    }
}
