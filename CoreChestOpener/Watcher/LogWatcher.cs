using CoreChestOpener.Data;
using CoreChestOpener.Model;
using CoreChestOpener.Server;
using PWToolKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CoreChestOpener.Watcher
{
    public class LogWatcher
    {
        private readonly ServerConnection server;
        private readonly Definitions defs;

        static private long lastSize;
        private static string path;
        static Timer logWatch = new Timer(500);

        public LogWatcher(ServerConnection server, Definitions definitions)
        {
            path = server.logsPath + "world2.log";
            lastSize = GetFileSize(path).Result;

            this.server = server;
            this.defs = definitions;

            PWGlobal.UsedPwVersion = server.PwVersion;

            logWatch.Elapsed += LogWatch_Elapsed;
            logWatch.Start();
        }

        private async void LogWatch_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                long fileSize = await GetFileSize(path);

                if (fileSize > lastSize)
                {
                    Sort sorter = new Sort(defs);

                    List<SortOrder> orders = await ReadTail(path, UpdateLastFileSize(fileSize));
                    orders = await sorter.OpenChests(orders, defs);

                    MailService mail = new MailService(defs);
                    await mail.Send(orders, server, defs);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Write(ex.ToString());
            }
        }
        private async Task<List<SortOrder>> ReadTail(string filename, long offset)
        {
            List<SortOrder> orders = new List<SortOrder>();

            byte[] bytes;

            using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(offset * -1, SeekOrigin.End);

                bytes = new byte[offset];
                fs.Read(bytes, 0, (int)offset);
            }

            List<string> logs = Encode.GB2312ToUtf8(bytes).Split(new string[] { "\n" }[0]).Where(x => !string.IsNullOrEmpty(x.Trim())).ToList();

            GC.Collect();

            foreach (var log in logs)
            {
                orders.Add(await DecodeMessage(log));
            }

            return orders;
        }
        private async Task<SortOrder> DecodeMessage(string encodedMessage)
        {
            SortOrder newOrder = new SortOrder();

            if (encodedMessage.Contains($"个{defs.ChestId}") && !encodedMessage.Contains("丢弃包裹") && !encodedMessage.Contains("拣起"))
            {                
                newOrder.RoleID = int.Parse(System.Text.RegularExpressions.Regex.Match(encodedMessage, @"用户([0-9]*)").Value.Replace("用户", ""));
                newOrder.ChestID = int.Parse(System.Text.RegularExpressions.Regex.Match(encodedMessage, @"个([0-9]*)").Value.Replace("个", ""));
                newOrder.ChestAmount = int.Parse(System.Text.RegularExpressions.Regex.Match(encodedMessage, @"卖店([0-9]*)").Value.Replace("卖店", ""));
            }

            return newOrder;
        }

        private static async Task<long> GetFileSize(string fileName)
        {
            return new System.IO.FileInfo(fileName).Length;
        }
        private static long UpdateLastFileSize(long fileSize)
        {
            long difference = fileSize - lastSize;
            lastSize = fileSize;

            return difference;
        }
    }
}
