using System.Collections.Generic;

namespace CoreChestOpener.Model
{
    public class SortOrder
    {
        public int RoleID { get; set; }
        public int ChestAmount { get; set; }
        public int ChestID { get; set; }
        public List<Drop> Drops { get; set; }
    }
}