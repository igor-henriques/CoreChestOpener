using System.Collections.Generic;

namespace CoreChestOpener.Model
{
    public class Drop
    {
        public readonly int Id;
        public readonly string Name;
        public readonly int Count;
        public readonly int Stack;
        public readonly string Octet;
        public readonly int Proctype;
        public readonly int Mask;
        public readonly decimal Probability;
        public List<int> SortNumbers;

        public Drop(int Id, string Name, int Count, int Stack, string Octet, int Proctype, int Mask, decimal Probability, List<int> sortNumbers)
        {
            this.Id = Id;
            this.Name = Name;
            this.Count = Count;
            this.Stack = Stack;
            this.Octet = Octet;
            this.Proctype = Proctype;
            this.Mask = Mask;
            this.Probability = Probability;
            this.SortNumbers = sortNumbers;
        }
    }
}