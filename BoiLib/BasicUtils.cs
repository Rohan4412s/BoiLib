using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Leaf.xNet;

namespace BoiLib
{
    public class BoiLib_Basic_Utils
    {
        public static void LoadData(string CombosPath, string ProxiesPath)
        {
            try
            {
                Variables.Combos = File.ReadAllLines(CombosPath);
                Variables.Proxies = File.ReadAllLines(ProxiesPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("An Error has Occured! - " + e.ToString());
                Console.ReadLine();
                Environment.Exit(0);
                
            }
        }

        public static void SetThreads(int input)
        {
            try
            {
                Variables.Threads = input;
            }
            catch (Exception e)
            {
                Console.WriteLine("An Error has Occured! - " + e.ToString());
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        public static void SetProxyType(ProxyType input)
        {
            try
            {
                Variables.ProxyType = input;
            }
            catch (Exception e)
            {
                Console.WriteLine("An Error has Occured! - " + e.ToString());
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        public static void SetProxySettings(HttpRequest req)
        {
            var ProxyAddress = GetRandomProxy();
            req.Proxy = ProxyClient.Parse(Variables.ProxyType, ProxyAddress);
        }

        static string GetRandomProxy()
        {
            var r = new Random();
            var number = r.Next(0, Variables.Proxies.Length - 1);
            return Variables.Proxies[number];
        }
    }
}
