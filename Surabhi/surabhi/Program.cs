using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using RestSharp.Authenticators;

namespace Surabhi
{
    class Program
    {
        public static void Main(string[] args)
        {
            Startup();
        }

        public static void Startup()
        {
            Console.Write("Starting up");
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var creds = configuration.GetSection("Credentials");
            var user = creds["Username"];
            var pw = creds["Password"];
            var apiKey = creds["ApiKey"];
            var restClient = new RestClient("https://demo-api.ig.com/");
            var authentication = new IgAuthenticator(user, pw, apiKey);
            var client = new Client("https://demo-api.ig.com/", authentication);
            var beacon = new Beacon();
            var epic = "CS.D.GBPUSD.TODAY.IP";

            Looper.Loop((o, e) =>
            {
                var market = client.GetMarket(epic);
                var signal = beacon.GetSignal(market.Snapshot);
                if (signal == "BUY")
                {
                    Console.WriteLine(DateTime.Now + "Buying");
                    client.OpenPosition(new QuickType.PositionParameters("BUY", epic, market.Snapshot.Offer, 100, 100));
                } else
                {
                    Console.WriteLine(DateTime.Now + "Selling");
                    client.OpenPosition(new QuickType.PositionParameters("SELL", epic, market.Snapshot.Bid, 100, 100));
                }
            });

            Thread.Sleep(1800000);
            //var position = new IgClient(restClient.BaseUrl.ToString(), authentication).GetMarket("KA.D.CAMLN.DAILY.IP");
        }

    }
}
