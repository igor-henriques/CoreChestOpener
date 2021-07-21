using CoreChestOpener.Data;
using CoreChestOpener.Server;
using PWToolKit.API.GDeliveryd;
using PWToolKit.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreChestOpener.Model
{
    public class MailService
    {
        private readonly Definitions defs;
        private static ServerConnection server { get; set; }

        public MailService(Definitions _defs)
        {
            defs = _defs;
        }
        public async Task Send(List<SortOrder> orders, ServerConnection serverConnection, Definitions definitions)
        {
            try
            {
                server = serverConnection;

                foreach (var order in orders)
                {
                    var drops = GetFrequencies(order.Drops.Select(x => x.Id).ToList());

                    foreach (var drop in drops)
                    {
                        EmailBuilder(drop, order.RoleID);
                    }

                    PrivateChat.Send(server.GDeliveryd, order.RoleID, "Confira no seu correio o(s) prêmio(s) do(s) baú(s)!");                    
                }
            }
            catch (System.Exception ex)
            {
                LogWriter.Write(ex.ToString());
            }            
        }
        private void EmailBuilder(KeyValuePair<int, int> dropData, int RoleID)
        {
            try
            {
                Drop curDrop = defs.Items.Where(x => x.Id.Equals(dropData.Key)).FirstOrDefault();                
                int count = curDrop.Count * dropData.Value;

                if (curDrop.Stack > count)
                {
                    SysSendMail.Send(server.GDeliveryd, RoleID, $"{defs.ChestName}", "Que sorte!", GenerateItem(curDrop, count));
                }
                else if (curDrop.Stack is 1)
                {
                    for (int i = 0; i < count; i++)
                    {
                        SysSendMail.Send(server.GDeliveryd, RoleID, $"{defs.ChestName}", "Que sorte!", GenerateItem(curDrop, 1));
                    }
                }
                else if (curDrop.Stack < count)
                {
                    int originalCount = count;
                    int amountSent = 0;

                    while (count > curDrop.Stack)
                    {
                        count = curDrop.Stack;

                        SysSendMail.Send(server.GDeliveryd, RoleID, $"{defs.ChestName}", "Que sorte!", GenerateItem(curDrop, count));
                        amountSent += count;

                        count = originalCount - amountSent;
                    }

                    if (count > 0)
                    {
                        SysSendMail.Send(server.GDeliveryd, RoleID, $"{defs.ChestName}", "Que sorte!", GenerateItem(curDrop, count));
                        count = originalCount;
                    }
                }

                LogWriter.Write($"Foi enviado {count}x {curDrop.Name}({curDrop.Id}) ao jogador {RoleID}");
            }
            catch (System.Exception ex)
            {
                LogWriter.Write(ex.ToString());
            }            
        }
        static Dictionary<int, int> GetFrequencies(List<int> values)
        {
            var result = new Dictionary<int, int>();

            foreach (int value in values)
            {
                if (result.TryGetValue(value, out int count))
                {
                    result[value] = count + 1;
                }
                else
                {
                    result.Add(value, 1);
                }
            }

            return result;
        }
        private static GRoleInventory GenerateItem(Drop order, int correctCount)
        {
            GRoleInventory item = new GRoleInventory()
            {
                Id = order.Id,
                MaxCount = order.Stack,
                Proctype = 0,
                Mask = order.Mask,
                Octet = order.Octet,
                Count = correctCount,
            };

            return item;
        }
    }
}
