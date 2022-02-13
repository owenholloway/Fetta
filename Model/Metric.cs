using System;
using Fetta.Dtos;

namespace Fetta.Model
{
    public class Metric
    {
        public string name { get; set; } = String.Empty;
        public ulong alias { get; set; } = 0;
        public ulong timestamp { get; set; } = (ulong) (DateTimeOffset.Now.ToUnixTimeSeconds()*1000 + DateTime.Now.Millisecond);
        public ValueTypeEnum datatype { get; set; } = ValueTypeEnum.int_value;
        public bool is_historical { get; set; } = false;
        public bool is_transient { get; set; } = true;
        public bool is_null { get; set; } = false;
        public object value { get; set; } = 0;
    }
}