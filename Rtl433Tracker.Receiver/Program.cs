using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Rtl433Tracker.Receiver
{
    class Program
    {
        private static bool _keepRunning = true;

        private static HttpClient _httpClient = new HttpClient();

        private static string _rtl433Path;

        private static string _trackerPostEndpoint;

        static void Main(string[] args)
        {
            LoadSettings();

            Console.CancelKeyPress += (a, b) => _keepRunning = false;

            while (_keepRunning)
            {
                Console.WriteLine("STARTING PROCESS");
                using (var process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.StartInfo.FileName = _rtl433Path;
                    process.StartInfo.Arguments = "-F json -U -G -q";
                    process.OutputDataReceived += (sender, argss) =>
                    {
                        Console.WriteLine("received output: {0}", argss.Data);
                        if (string.IsNullOrWhiteSpace(argss.Data) == false)
                        {
                            var content = new StringContent(argss.Data, Encoding.Default, "application/json");
                            var result = _httpClient.PostAsync(_trackerPostEndpoint, content).Result;
                            Console.WriteLine($"Request finished: Result code: {result.StatusCode}");
                        }
                    };
                    process.ErrorDataReceived += (sender, argss) => Console.WriteLine("received error: {0}", argss.Data);
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                }
                Console.WriteLine("PROCESS ENDED Restarting in a moment...");
                Thread.Sleep(3000);
            }
        }

        private static void LoadSettings()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .Build();

            _rtl433Path = configuration["rtl433Path"];
            _trackerPostEndpoint = configuration["trackerPostEndpoint"];
        }
    }
}
