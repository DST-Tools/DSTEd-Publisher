using System;

namespace DSTEd.Publisher.Actions {
    class Software : ActionClass {
        public Software() {
            this.Name           = "version"; 
            this.Description    = "Displays the version information for the Publisher.";
        }

        public override int Run(string[] arguments) {
            Console.WriteLine("Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()  + "\n");
            return 0;
        }
    }
}
