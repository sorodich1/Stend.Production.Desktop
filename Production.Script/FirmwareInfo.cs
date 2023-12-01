using Ankom.Common;
using Newtonsoft.Json;
using System;

namespace Production.Script
{
    public class FirmwareInfo
    {
        public int ID { get; set; }

        public string ExecutionID { get; set; }

        public int Controller1_Device1 { get; set; }

        public int Controller1_Device2 { get; set; }

        public int Controller2_Device1 { get; set; }

        public int Controller2_Device2 { get; set; }

        public int Controller2_Device3 { get; set; }

        [JsonProperty("FirmwareKey")]
        public string FirmwareKeyHS { get; set; }

        [JsonIgnore]
        public byte[] FirmwareKey
        {
            get => HexUtils.HexStrToBytes(this.FirmwareKeyHS);
            set => this.FirmwareKeyHS = HexUtils.BytesToHexStr(value);
        }

        [JsonIgnore]
        public DateTime DateTime { get; set; }

        public string Description { get; set; }
    }
}
