using System;

namespace Fetta.Dtos
{
    public class MetricDto
    {
        public string name { get; set; } = String.Empty;
       // public ulong alias { get; set; } = 0;
        public ulong timestamp { get; set; } = (ulong) DateTimeOffset.Now.ToUnixTimeSeconds();
        public uint datatype { get; set; } = 3;
        //public bool isHistorical { get; set; } = false;
        //public bool is_transient { get; set; } = true;
        //public bool isNull { get; set; } = false;
    }
}