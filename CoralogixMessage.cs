using Newtonsoft.Json;

namespace GoodFillingTargets
{
    public class CoralogixMessage
    {
        [JsonProperty("privateKey")]
        public string PrivateKey { get; set; }

        [JsonProperty("applicationName")]
        public string ApplicationName { get; set; }

        [JsonProperty("subsystemName")]
        public string SubsystemName { get; set; }

        [JsonProperty("computerName")]
        public string ComputerName { get; set; }

        [JsonProperty("logEntries")]
        public CoralogixLogEntries[] LogEntries { get; set; }
    }

    public class CoralogixLogEntries
    {
        [JsonProperty("timeStamp")]
        public long TimeStamp { get; set; }

        [JsonProperty("severity")]
        public int Severity { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        [JsonProperty("threadId")]
        public string ThreadId { get; set; }
    }
}