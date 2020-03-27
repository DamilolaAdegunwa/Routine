using BlockSms.Core;
using BlockSms.Core.Helper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
namespace BlockSms.Mobile.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        public static string IP;
        /// <summary>
        /// 
        /// </summary>
        public static int Port;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            var bud = new ConfigurationBuilder();
            var basepath = AppContext.BaseDirectory;
            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                basepath = Path.GetDirectoryName(pathToExe);
            }
            bud.SetBasePath(basepath);
            var config = bud.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();
            IP = config["IP"];
            Port = Convert.ToInt32(config["Port"]);
            var builder = CreateWebHostBuilder(basepath, args.Where(arg => arg != "--console").ToArray());
            if (isService) builder.UseContentRoot(basepath);
            var host = builder.Build();

            var _logger = host.Services.GetRequiredService<ILogger<Program>>();
            _logger.LogInformation($"{config["Name"]}({config["Version"]})服务已启动，当前地址：http://{IP}:{Port}");
            host.Run();

        }
        /// <summary>
        /// 
        /// </summary>
        public static IWebHostBuilder CreateWebHostBuilder(string basePath, string[] args)
        {
            if (string.IsNullOrEmpty(IP)) IP = NetworkHelper.LocalIPAddress;
            if (Port == 0) Port = NetworkHelper.GetRandomAvaliablePort();
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://{IP}:{Port}")
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddLog4Net(basePath + "/log4net.config", true, CommonConsts.LogRepository);
                })
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", false, true);
                });
        }
    }
}
