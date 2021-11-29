using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace list_j_svrs
{

    public class JamulusServers
    {
        public long numip { get; set; }
        public long port { get; set; }
        public string country { get; set; }
        public long maxclients { get; set; }
        public long perm { get; set; }
        public string name { get; set; }
        public string ipaddrs { get; set; }
        public string city { get; set; }
        public string ip { get; set; }
        public long ping { get; set; }
        public Os ps { get; set; }
        public string version { get; set; }
        public string versionsort { get; set; }
        public long nclients { get; set; }
        public long index { get; set; }
        public Client[] clients { get; set; }
        public long? port2 { get; set; }
    }

    public class Client
    {
        public long chanid { get; set; }
        public string country { get; set; }
        public string instrument { get; set; }
        public string skill { get; set; }
        public string name { get; set; }
        public string city { get; set; }
    }

    public enum Os { Linux, MacOs, Windows };

    class ListJSvrs
    {
        static Dictionary<string, string> JamulusListURLs = new Dictionary<string, string>()
        {
            {"Any Genre 1", "https://jamulus.softins.co.uk/servers.php?central=anygenre1.jamulus.io:22124" }
            ,{"Any Genre 2", "https://jamulus.softins.co.uk/servers.php?central=anygenre2.jamulus.io:22224" }
            ,{"Any Genre 3", "https://jamulus.softins.co.uk/servers.php?central=anygenre3.jamulus.io:22624" }
            ,{"Genre Rock",  "https://jamulus.softins.co.uk/servers.php?central=rock.jamulus.io:22424" }
            ,{"Genre Jazz",  "https://jamulus.softins.co.uk/servers.php?central=jazz.jamulus.io:22324" }
            ,{"Genre Classical/Folk",  "https://jamulus.softins.co.uk/servers.php?central=classical.jamulus.io:22524" }
            ,{"Genre Choral/BBShop",  "https://jamulus.softins.co.uk/servers.php?central=choral.jamulus.io:22724" }
        };

        static Dictionary<string, string> LastReportedList = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                (message, cert, chain, ssl) =>
                {
                    return true;
                };

            using var client = new HttpClient(httpClientHandler);

            var serverStates = new Dictionary<string, Task<string>>();

            foreach (var key in JamulusListURLs.Keys)
            {
                serverStates.Add(key, client.GetStringAsync(JamulusListURLs[key]));
            }

            foreach (var key in JamulusListURLs.Keys)
            {
                LastReportedList[key] = serverStates[key].Result;
            }

            // i've loaded the 6 tables. now list out each server in every table.
            foreach (var key in LastReportedList.Keys)
            {
                var serversOnList = System.Text.Json.JsonSerializer.Deserialize<List<JamulusServers>>(LastReportedList[key]);
                foreach (var server in serversOnList)
                {
                    Console.WriteLine(server.ip + ":" + server.port);
                }
            }
        }
    }
}
