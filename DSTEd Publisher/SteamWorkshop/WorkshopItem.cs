using System;
using System.Collections.Generic;
using System.Text;

namespace DSTEd.Publisher.SteamWorkshop {
    public class WorkshopItem {
        public int ID { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Tags { get; set; }

        public override String ToString() {
            return $"[WorkshopItem ID={ID}, Title={Title}, Description={Description}, Tags={Tags}]";
        }
    }
}
