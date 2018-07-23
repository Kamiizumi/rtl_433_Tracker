using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Rtl433Tracker.Receiver
{
    /// <summary>
    /// Console wrapper for rtl_433 which posts receiver readings to the tracker server.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Client to use when posting data to tracker.
        /// </summary>
        private static HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Full path to the rtl_433 executable.
        /// </summary>
        private static string _rtl433Path;

        /// <summary>
        /// URL to the tracker endpoint to post readings to.
        /// </summary>
        private static string _trackerPostEndpoint;

        /// <summary>
        /// Console application entry point.
        /// </summary>
        static void Main()
        {
            LoadSettings();

            // Loop to restart rtl_433 should it fail.
            while (true)
            {
                Console.WriteLine("STARTING PROCESS");

                using (var process = new Process())
                {
                    // Configure the rtl_433 executable / arguments to start.
                    process.StartInfo.FileName = _rtl433Path;

                    // -F json: Format as JSON (the tracker server expects this format).
                    // -U: Use UTC timestamps.
                    // -G: Enable all decoders so we can capture as much data as possible.
                    // -q: Quiet mode, to suppress unwanted output.
                    process.StartInfo.Arguments = "-F json -U -G -q";

                    // We want to handle the process internally so don't start via the OS.
                    process.StartInfo.UseShellExecute = false;

                    // Redirect output so we can process readings / errors.
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

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

                    // Begin the process and start listening to output.
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Wait here for the process to end.
                    process.WaitForExit();
                }

                Console.WriteLine("PROCESS ENDED Restarting in a moment...");
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// Load settings from appsettings.json.
        /// </summary>
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
