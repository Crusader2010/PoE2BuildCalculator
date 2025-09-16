using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Item
    {
        public int Id { get; set; }

        public ItemStats ItemStats { get; set; }

        public string Name { get; set; } = "Placeholder name";

        public string Class { get; set; } = "Placeholder class";

        public bool IsMine { get; set; } = false;
    }
}
