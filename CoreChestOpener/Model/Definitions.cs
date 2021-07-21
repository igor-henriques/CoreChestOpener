using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CoreChestOpener.Model
{
    public class Definitions
    {
        public int ChestId { get; set; }
        public string ChestName { get; set; }
        public readonly ReadOnlyCollection<Drop> Items;
        
        public Definitions(List<Drop> items)
        {
            Items = items.AsReadOnly();
        }
    }
}
