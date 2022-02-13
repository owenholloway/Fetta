using System;
using System.Threading;
using System.Threading.Tasks;
using Fetta.Dtos;
using Fetta.Model;
using MQTTnet;
using MQTTnet.Client.Options;


namespace Fetta
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("FettaTest")
                .WithTcpServer("172.16.40.200")
                .WithCredentials("FettaTest", "FettaTest")
                .Build();

            var device = Device.Create(options, "testGroup", "testNode", "testDevice");

            device.CreateMetric(ValueTypeEnum.float_value,(float) 100,"test value");
            
            await device.StartDevice();

            while (device.IsConnected())
            {
                Thread.Sleep(100);
                var value = (float) Math.PI * 20 * ((float)DateTime.Now.Millisecond/10000);
                await device.UpdateMetric("test value", value);
            }

        }
    }
}