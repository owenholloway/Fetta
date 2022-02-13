using System;
using System.Collections.Generic;
using Fetta.Dtos;

namespace Fetta.Model
{
    public class Metric
    {
        public string Name { get; set; } = string.Empty;
        public ulong Alias { get; set; } = 0;
        public ulong Timestamp { get; set; } = (ulong) (DateTimeOffset.Now.ToUnixTimeSeconds()*1000 + DateTime.Now.Millisecond);
        public ValueTypeEnum Datatype { get; set; } = ValueTypeEnum.int_value;
        public bool IsHistorical { get; set; } = false;
        public bool IsTransient { get; set; } = true;
        public bool IsNull { get; set; } = false;
        public object value { get; set; } = 0;

        public List<KeyValuePair<string, object>> Properties = new();
    }
}