using System;
using System.Collections.Generic;

namespace Fetta.Dtos
{
    public class PayloadDto
    {
        public ulong timestamp { get; set; } = (ulong) DateTimeOffset.Now.ToUnixTimeSeconds();
        public ulong seq { get; set; } = 0;
        //public Guid uuid { get; set; } = Guid.Empty;
        public List<object> metrics { get; set; } = new List<object>();
        //public byte[] body { get; set; }
    }
}