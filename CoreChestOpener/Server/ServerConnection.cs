using PWToolKit;

namespace CoreChestOpener.Server
{
    public class ServerConnection
    {
        public string logsPath { get; set; }
        public PwVersion PwVersion { get; set; }
        public GDeliveryd GDeliveryd { get; set; }
        public GProvider GProvider { get; set; }
        public Gamedbd Gamedbd { get; set; }

        public ServerConnection(string GDeliverydHost, int GDeliverydPort, string GProviderHost, int GProviderPort, string GamedbdHost, int GamedbdPort, PwVersion PwVersion, string logsPath)
        {
            this.logsPath = logsPath;
            this.PwVersion = PwVersion;
            this.GDeliveryd = new GDeliveryd(GDeliverydHost, GDeliverydPort);
            this.GProvider = new GProvider(GProviderHost, GProviderPort);
            this.Gamedbd = new Gamedbd(GamedbdHost, GamedbdPort);
        }
    }
}
