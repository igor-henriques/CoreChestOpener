using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreChestOpener.Model
{
    public class Sort
    {
        readonly Definitions definitions;

        public Sort(Definitions defs)
        {
            definitions = defs;
        }

        public async Task<List<SortOrder>> OpenChests(List<SortOrder> orders, Definitions defs)
        {
            foreach (var order in orders)
            {
                List<Drop> drops = new List<Drop>();

                for (int i = 0; i < order.ChestAmount; i++)
                {
                    int sortedNumber = Random.Generate();
                    Drop tempDrop = FindNumber(sortedNumber);

                    if (tempDrop is null)
                    {
                        while (tempDrop is null)
                        {
                            tempDrop = FindNumber(Random.Generate());
                        }
                    }                        

                    drops.Add(tempDrop);
                }

                order.Drops = drops;
            }            

            return orders;
        }                
        private Drop FindNumber(int number)
        {
            Drop newDrop = null;
            List<Drop> possibleDrops = new List<Drop>();

            foreach (var item in definitions.Items)
            {
                if (item.SortNumbers != null)
                {
                    if (item.SortNumbers.Contains(number))
                    {
                        possibleDrops.Add(item);
                    }

                    newDrop = possibleDrops.Where(x => x.Probability.Equals(possibleDrops.Min(x => x.Probability))).FirstOrDefault();
                }
            }

            return newDrop;
        }                
    }
}
