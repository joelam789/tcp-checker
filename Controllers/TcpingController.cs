using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TcpChecker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TcpingController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        // example
        // - tcping 192.68.0.111:8080
        // - http://localhost:5000/tcping/192.168.0.111/8080
        [HttpGet("{ipAddress}/{port}/")]
        public IActionResult Get(string ipAddress, int port)
        {
            var reqIp = ipAddress;
            var reqPort = port.ToString();

            var targetAddr = reqIp + " " + reqPort;

            var tcpingBinFilePath = "tcping.exe";

            if (!System.IO.File.Exists(tcpingBinFilePath))
            {
                return NotFound(new
                {
                    msg = "Tcping binary file not found"
                });
            }

            CommonLog.Info("Tcping ... [" + targetAddr + "]");

            var cmd = '"' + tcpingBinFilePath + '"';
            var args = " -n 1 " + targetAddr;

            try
            {
                // Start the child process.
                Process p = new Process();

                // Redirect the output stream of the child process.
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;

                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = args;
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit();

                if ((!String.IsNullOrEmpty(output) && output.Contains("1 successful"))
                    || (!String.IsNullOrEmpty(error) && error.Contains("1 successful")))
                {
                    return Ok(new
                    {
                        msg = "OK"
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        msg = "Tcping returned errors"
                    });
                }

            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    msg = "Got errors when running tcping - " + ex.Message
                });
            }
        }
    }
}
