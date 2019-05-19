using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Rtl433Tracker.Receiver
{
    /// <summary>
    /// Console wrapper for rtl_433 which posts receiver readings to the tracker server.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Client to use when posting data to tracker.
        /// </summary>
        private static HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Logger to write messages to.
        /// </summary>
        private static ILogger _logger;

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
        private static void Main()
        {
            // Perform initialisation.
            InitLogging();
            LoadSettings();

            // Loop to restart rtl_433 should it fail.
            while (true)
            {
                using (var rtl433Process = new Process())
                {
                    // Configure the rtl_433 executable / arguments to start.
                    rtl433Process.StartInfo.FileName = _rtl433Path;

                    // -F json: Format as JSON (the tracker server expects this format).
                    // -U: Use UTC timestamps.
                    // -G: Enable all decoders so we can capture as much data as possible.
                    // -q: Quiet mode, to suppress unwanted output.
                    rtl433Process.StartInfo.Arguments = "-F json -M utc -G -q";

                    // We want to handle the process internally so don't start via the OS.
                    rtl433Process.StartInfo.UseShellExecute = false;

                    // Redirect output so we can process readings / errors.
                    rtl433Process.StartInfo.RedirectStandardOutput = true;
                    rtl433Process.OutputDataReceived += OnOutputDataReceived;

                    rtl433Process.StartInfo.RedirectStandardError = true;
                    rtl433Process.ErrorDataReceived += OnErrorDataReceived;

                    // Begin the process and start listening to output.
                    _logger.Information("Starting rtl_433 process...");
                    rtl433Process.Start();
                    rtl433Process.BeginOutputReadLine();
                    rtl433Process.BeginErrorReadLine();

                    // Wait here for the process to end.
                    rtl433Process.WaitForExit();
                }

                // rtl_433 ended. Restart the process after a few seconds.
                const int restartSeconds = 3;
                _logger.Error("rtl_433 process ended. Restarting in {restartSeconds} seconds...", restartSeconds);
                Thread.Sleep(restartSeconds * 1000);
            }
        }

        /// <summary>
        /// Initialise the logger.
        /// </summary>
        private static void InitLogging()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _logger.Information("Logging initialised");
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

            _logger.Information("Path to rtl_433 executable: {rtl433Path}", _rtl433Path);
            _logger.Information("URL to tracker POST endpoint: {trackerPostEndpoint}", _trackerPostEndpoint);
        }

        /// <summary>
        /// Event handler for when output is received from rtl_433.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">Data received.</param>
        private static void OnOutputDataReceived(object sender, DataReceivedEventArgs eventArgs)
        {
            _logger.Information("Received output: {data}", eventArgs.Data);

            if (string.IsNullOrWhiteSpace(eventArgs.Data) == false)
            {
                // Wrap in try / catch so if something goes wrong with the request the application doesn't crash out.
                try
                {
                    _logger.Information("Posting output to endpoint...");

                    var content = new StringContent(eventArgs.Data, Encoding.Default, "application/json");
                    var result = _httpClient.PostAsync(_trackerPostEndpoint, content).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        _logger.Information("Post successful. Result code: {resultCode}", result.StatusCode);
                    }
                    else
                    {
                        _logger.Error("Post failed. Result code: {resultCode}", result.StatusCode);
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(exception, "Exception occurred while posting to endpoint");
                }
            }
        }

        /// <summary>
        /// Event handler for when errors are received from rtl_433.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">Error received.</param>
        private static void OnErrorDataReceived(object sender, DataReceivedEventArgs eventArgs)
        {
            _logger.Warning("Received error: {data}", eventArgs.Data);
        }
    }
}
