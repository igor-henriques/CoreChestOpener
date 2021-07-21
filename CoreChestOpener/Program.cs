using CoreChestOpener.License;
using CoreChestOpener.Model;
using CoreChestOpener.Server;
using CoreChestOpener.Watcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PWToolKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreChestOpener
{
    class Program
    {
        static int maxDecimalCases;
        static Model.Random rnd;

        static List<Drop> items;
        static LicenseControl license = new LicenseControl();

        static ServerConnection ServerConnection;
        static Definitions definitions;

        static ManualResetEvent quitEvent = new ManualResetEvent(false);

        static async Task Main()
        {
            await InitializePrefs();            

            Run();
            Stop();
        }

        static void Run()
        {
            LogWatcher logWatcher = new LogWatcher(ServerConnection, definitions);
        }

        private static async Task InitializePrefs()
        {            
            Console.WriteLine("INICIALIZANDO SISTEMA DE LICENÇA\n");
            CoreLicense licenseConfigs = JsonConvert.DeserializeObject<CoreLicense>(await File.ReadAllTextAsync("./Configurations/License.json"));
            await license.Start(licenseConfigs.User, licenseConfigs.Licensekey, licenseConfigs.Product);

            Console.WriteLine("INICIALIZANDO ITENS\n");
            JArray jsonItems = (JArray)JsonConvert.DeserializeObject(await File.ReadAllTextAsync("./Configurations/serializedObjects.json"));
            items = await InitializeItems(jsonItems);

            Console.WriteLine("INICIALIZANDO CONFIGURAÇÕES DE SERVIDOR\n");
            JObject jsonServerConfig = (JObject)JsonConvert.DeserializeObject(await File.ReadAllTextAsync("./Configurations/ServerConnection.json"));
            ServerConnection = await LoadServerConfig(jsonServerConfig);

            Console.WriteLine("INICIALIZANDO CONFIGURAÇÕES DA FERRAMENTA\n\n");
            JObject jsonAppConfig = (JObject)JsonConvert.DeserializeObject(await File.ReadAllTextAsync("./Configurations/Definitions.json"));
            definitions = await LoadAppConfig(jsonAppConfig);

            Console.WriteLine("GERANDO NÚMEROS DE SORTEIO");
            DefineRandomizer(definitions);
            DefineNumbers();

            Console.WriteLine("PROGRAMADO POR IRONSIDE\nBOM USO!\nDiscord para Report Bug: Ironside#3862\n=============================================================");
        }
        static async Task<List<Drop>> InitializeItems(JArray jsonNodes)
        {
            List<Drop> itens = new List<Drop>();

            foreach (var node in jsonNodes)
            {
                itens.Add(JsonConvert.DeserializeObject<Drop>(node.ToString()));
            }

            return itens;
        }
        static async Task<ServerConnection> LoadServerConfig(JObject jsonNodes)
        {
            ServerConnection ServerConnection = new ServerConnection
            (
                jsonNodes["GDELIVERYD"]["HOST"].ToObject<string>(),
                jsonNodes["GDELIVERYD"]["PORT"].ToObject<int>(),
                jsonNodes["GPROVIDER"]["HOST"].ToObject<string>(),
                jsonNodes["GPROVIDER"]["PORT"].ToObject<int>(),
                jsonNodes["GAMEDBD"]["HOST"].ToObject<string>(),
                jsonNodes["GAMEDBD"]["PORT"].ToObject<int>(),
                (PwVersion)jsonNodes["PW_VERSION"].ToObject<int>(),
                jsonNodes["LOGS_PATH"].ToObject<string>()
            );

            return ServerConnection;
        }

        static void Stop()
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                quitEvent.Set();
                eArgs.Cancel = true;
            };

            quitEvent.WaitOne();
        }

        static async Task<Definitions> LoadAppConfig(JObject jsonNodes)
        {
            Definitions defs = new Definitions(items)
            {
                ChestId = jsonNodes["ID DO BAÚ"].ToObject<int>(),
                ChestName = jsonNodes["NOME DO BAÚ"].ToObject<string>()
            };

            return defs;
        }
        private static void DefineNumbers()
        {
            Parallel.ForEach(definitions.Items, item =>
            {
                List<int> sortValues = new List<int>();

                Parallel.For(0, (int)IntTransform((double)item.Probability), x =>
                {
                    int randomNumber = Model.Random.Generate();

                    sortValues.Add(randomNumber);
                    Console.WriteLine($"GERANDO NÚMEROS PARA {item.Name}({sortValues.Count}): {randomNumber}");

                    item.SortNumbers = sortValues;
                });
            });
        }
        private static double IntTransform(double source)
        {
            for (int i = 0; i < maxDecimalCases; i++)
            {
                source *= 10;
            }

            return source;
        }
        private static void DefineRandomizer(Definitions defs)
        {
            List<decimal> probabilites = defs.Items.Select(x => x.Probability).ToList();
            List<string> textProbabilities = new List<string>();

            foreach (var prob in probabilites)
            {
                textProbabilities.Add(prob.ToString().Split('.', ',').Last());
            }

            maxDecimalCases = textProbabilities.Select(x => x.Length).Max();

            rnd = new Model.Random(maxDecimalCases);
        }        
    }
}