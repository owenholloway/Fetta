using System;
using System.Collections.Generic;
using Fetta.Dtos;

namespace Fetta.Model
{
    public class Payload
    {
        public ulong timestamp { get; private set; } = (ulong) (DateTimeOffset.Now.ToUnixTimeSeconds()*1000 + DateTime.Now.Millisecond);
        public ulong seq { get; private set; } = 0;
        //public Guid uuid { get; private set; }
        public List<Metric> metrics { get; private set; } = new List<Metric>();
        //public byte[] body { get; private set; }

        public static Payload Create(Payload lastPayload = null)
        {
            var obj = new Payload();

            if (lastPayload is null)
            {
                obj.seq = 0;
            }
            else
            {
                obj.seq = (lastPayload.seq + 1) % 256;
            }

            return obj;
        }

        public void CreateMetric(ValueTypeEnum datatype, object value, string name)
        {
            var metric = new Metric();

            metric.Datatype = datatype;
            metric.value = value;
            metric.Name = name;
            
            metrics.Add(metric);
        }
        
        public PayloadDto CreateDto()
        {
            var payloadDto = new PayloadDto();
            

            payloadDto.timestamp = timestamp;
            payloadDto.seq = seq;
            //payloadDto.uuid = uuid;
            //payloadDto.body = body;

            foreach (var metric in metrics)
            {

                switch (metric.Datatype)
                {
                    
                    case ValueTypeEnum.int_value:
                        var intMetricDto = new MetricIntDto
                        {
                            intValue = (uint)metric.value,
                            timestamp = metric.Timestamp,
                            name = metric.Name,
                            datatype = (uint)metric.Datatype 
                        };
                        payloadDto.metrics.Add(intMetricDto);
                        break;
                    
                    
                    case ValueTypeEnum.uint_value:
                        var uIntMetricDto = new MetricUIntDto
                        {
                            intValue = (uint)metric.value,
                            timestamp = metric.Timestamp,
                            name = metric.Name,
                            datatype = (uint)metric.Datatype 
                        };
                        payloadDto.metrics.Add(uIntMetricDto);
                        break;
                    
                    
                    case ValueTypeEnum.ulong_value:
                        var uLongMetricDto = new MetricULongDto()
                        {
                            longValue = (ulong)metric.value,
                            timestamp = metric.Timestamp,
                            name = metric.Name,
                            datatype = (uint)metric.Datatype 
                        };
                        payloadDto.metrics.Add(uLongMetricDto);
                        break;
                    
                    
                    case ValueTypeEnum.float_value:
                        var floatMetricDto = new MetricFloatDto
                        {
                            floatValue = (float) metric.value,
                            timestamp = metric.Timestamp,
                            name = metric.Name,
                            datatype = (uint)metric.Datatype 
                        };
                        payloadDto.metrics.Add(floatMetricDto);
                        break;
                    
                    
                    case ValueTypeEnum.string_value:
                        var stringMetricDto = new MetricStringDto
                        {
                            stringValue = (string) metric.value,
                            timestamp = metric.Timestamp,
                            name = metric.Name,
                            datatype = (uint)metric.Datatype 
                        };
                        payloadDto.metrics.Add(stringMetricDto);
                        break;
                    
                    
                    case ValueTypeEnum.boolean_value:
                        var booleanMetricDto = new MetricBooleanDto()
                        {
                            booleanValue = (bool) metric.value,
                            timestamp = metric.Timestamp,
                            name = metric.Name,
                            datatype = (uint)metric.Datatype 
                        };
                        payloadDto.metrics.Add(booleanMetricDto);
                        break;

                }

            }
            
            return payloadDto;
            
        }
    }
}