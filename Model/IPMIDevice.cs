using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Fetta.Dtos;

namespace Fetta.Model
{
    public class IPMIDevice
    {
        private string _hostname;
        private string _username;
        private string _password;

        private string _connectionString;
        
        public static IPMIDevice Create(string hostname, string username, string password)
        {
            var obj = new IPMIDevice();
            
            obj._hostname = hostname;
            obj._username = username;
            obj._password = password;

            obj._connectionString = " -I lanplus" +
                                    " -H " + obj._hostname +
                                    " -U " + obj._username + 
                                    " -P " + obj._password;
            
            return obj;

        }

        public List<Metric> Refresh()
        {
            var command = _connectionString + " sensor";

            var metrics = new List<Metric>();
            
            using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "ipmitool",
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }))
            {
                process.WaitForExit();
                
                var result = process.StandardOutput.ReadToEnd();

                using (StringReader reader = new StringReader(result))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var splitString = line.Split("|");

                        var metric = new Metric();
                        metric.Name = splitString[0].Trim();

                        if (float.TryParse(splitString[1].Trim(), out var testNumber))
                        {
                            metric.value = testNumber;
                            metric.Datatype = ValueTypeEnum.float_value;
                        }
                        else
                        {
                            metric.value = splitString[1].Trim();
                            metric.Datatype = ValueTypeEnum.string_value;
                        }
                        
                        metrics.Add(metric);
                        
                    }
                }
                
            }

            return metrics;
        }

    }
}