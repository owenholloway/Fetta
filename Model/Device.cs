using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fetta.Dtos;
using Google.Protobuf;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Fetta.Model
{
    public class Device
    {
        private IMqttClientOptions _options;
        private IMqttClient _mqttClient;

        private string _groupId;
        private string _edgeNodeId;
        private string _deviceId;
        private ulong _seq;

        private string _commandTopic;

        private List<Metric> _metrics { get; set; }
        
        private Payload _lastPayload { get; set; }

        public static Device Create(IMqttClientOptions options, string groupId, string edgeNodeId, string deviceId)
        {
            var obj = new Device();
            
            obj._options = options;
            obj._deviceId = deviceId;
            obj._groupId = groupId;
            obj._edgeNodeId = edgeNodeId;
            
            obj._metrics = new List<Metric>();
            
            obj.CreateMetric(ValueTypeEnum.string_value, "lanceolata.com.au","Properties/Hardware Make");
            obj.CreateMetric(ValueTypeEnum.string_value, "FETTA","Properties/Hardware Make");
            obj.CreateMetric(ValueTypeEnum.string_value, "0.0.1","Properties/Hardware Make");
            obj.CreateMetric(ValueTypeEnum.boolean_value, false, "Node Control/Next Server");
            obj.CreateMetric(ValueTypeEnum.boolean_value, false, "Node Control/Rebirth");
            //obj.CreateMetric(ValueTypeEnum.string_value, "4.0.10", "Node Info/Transmission Version");
            obj.CreateMetric(ValueTypeEnum.ulong_value, (ulong)1, "bdSeq");

            return obj;
        }
        
        public async Task StartDevice()
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
            
            await _mqttClient.ConnectAsync(_options, CancellationToken.None);

            _commandTopic = "spBv1.0/" + _groupId + "/NCMD/" + _edgeNodeId;

            await _mqttClient.SubscribeAsync(_commandTopic);

            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                var message = e.ApplicationMessage;
                ProcessMessage(message);
            });

            BirthDevice();

        }

        private async Task BirthDevice()
        {
            var birthMessage = Model.Payload.Create();
            _seq = 0;

            foreach (var metric in _metrics)
            {
                birthMessage.CreateMetric(metric.datatype, metric.value, metric.name);
            }
            
            var birthPayloadMessage = birthMessage.CreateDto();

            var birthMessageJson = Newtonsoft.Json.JsonConvert.SerializeObject(birthPayloadMessage);
            
            Console.WriteLine(birthMessageJson);
            
            var birthPayload = Lib.Tahu.Payload.Parser.ParseJson(birthMessageJson);

            var nBirthTopic = "spBv1.0/" + _groupId + "/NBIRTH/" + _edgeNodeId;
            
            var nMessageBuilt = new MqttApplicationMessageBuilder()
                .WithTopic(nBirthTopic)
                .WithPayload(birthPayload.ToByteString())
                .Build();
            
            await _mqttClient.PublishAsync(nMessageBuilt, CancellationToken.None);
            _seq += 1;
            _lastPayload = birthMessage;

        }

        public async Task UpdateMetric(string metricName, object value)
        {
            if (_metrics.Any(m => m.name.Equals(metricName)))
            {
                var dataMessage = Model.Payload.Create(_lastPayload);
                var metric = _metrics.Find(m => m.name.Equals(metricName));
                
                //Asserted not null due to if statement above
                Debug.Assert(metric != null, nameof(metric) + " != null");
                dataMessage.CreateMetric(metric.datatype, value, metric.name);
                dataMessage.CreateMetric(ValueTypeEnum.ulong_value, _seq, "bdSeq");
                
                var dataMessageDto = dataMessage.CreateDto();
                var dataMessageJson = Newtonsoft.Json.JsonConvert.SerializeObject(dataMessageDto);
                Console.WriteLine(dataMessageJson);
                var payloadData = Lib.Tahu.Payload.Parser.ParseJson(dataMessageJson);

                var ddataTopic = "spBv1.0/" + _groupId + "/NDATA/" + _edgeNodeId;
                
                var messageBuilt = new MqttApplicationMessageBuilder()
                    .WithTopic(ddataTopic)
                    .WithPayload(payloadData.ToByteString())
                    .Build();
            
                await _mqttClient.PublishAsync(messageBuilt, CancellationToken.None);
                _seq += 1;

                _lastPayload = dataMessage;

            }
            
        }

        private void ProcessMessage(MqttApplicationMessage message)
        {
            Console.WriteLine(message.Topic);
            
            if (true)
            {
                var messageJson = Lib.Tahu.Payload.Parser.ParseFrom(message.Payload);
                Console.WriteLine(messageJson);
                if (messageJson.ToString().Contains("Node Control/Rebirth"))
                {
                    BirthDevice();
                }

            }
        }
        
        public void CreateMetric(ValueTypeEnum datatype, object value, string name)
        {
            var metric = new Metric();

            metric.datatype = datatype;
            metric.value = value;
            metric.name = name;
            
            _metrics.Add(metric);
        }

        public bool IsConnected()
        {
            return _mqttClient.IsConnected;
        }

    }
}