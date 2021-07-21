using PWToolKit.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreChestOpener.Server
{
    public class Gamedbd : IPwDaemonConfig
    {
        public string Host { get; }
        public int Port { get; }

        public Gamedbd(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
