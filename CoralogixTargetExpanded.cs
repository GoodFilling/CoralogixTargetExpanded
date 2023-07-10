using CoralogixCoreSDK;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using PumpCommunication.Models;

namespace CoralogixTargetExpanded
{
    [Target("ImprovedCoralogix")]
    public class CoralogixTargetExpanded : TargetWithLayout
    {
        private readonly string Host;
        private readonly string PrivateKey;
        private readonly string ApplicationName;
        private readonly string SubsystemName;
        private readonly string ComputerName;

        private Dictionary<LogLevel, Severity> SeverityDictionary = new Dictionary<LogLevel, Severity>();

        public CoralogixTargetExpanded(string hostUrl, string privateKey, string applicationName, string subsystemName, string computerName)
        {
            Host = hostUrl;
            PrivateKey = privateKey;
            ApplicationName = applicationName;
            SubsystemName = subsystemName;
            ComputerName = computerName;

            //build severity map
            SeverityDictionary.Add(LogLevel.Debug, Severity.Debug);
            SeverityDictionary.Add(LogLevel.Trace, Severity.Verbose);
            SeverityDictionary.Add(LogLevel.Info, Severity.Info);
            SeverityDictionary.Add(LogLevel.Warn, Severity.Warning);
            SeverityDictionary.Add(LogLevel.Error, Severity.Error);
            SeverityDictionary.Add(LogLevel.Fatal, Severity.Critical);
        }

        //see https://github.com/NLog/NLog/wiki/How-to-write-a-custom-target
        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = RenderLogEvent(this.Layout, logEvent);
            SendTheMessageToRemoteHost(logMessage, logEvent);
        }

        private async void SendTheMessageToRemoteHost(string message, LogEventInfo logEvent)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, this.Host);

                var requestBody = new CoralogixMessage()
                {
                    ApplicationName = ApplicationName,
                    SubsystemName = SubsystemName,
                    PrivateKey = PrivateKey,
                    ComputerName = ComputerName,
                    LogEntries = new CoralogixLogEntries[]{
                        new CoralogixLogEntries()
                        {
                            Text = $"Line {logEvent.CallerLineNumber}:{message}",
                            TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            Severity = DecodeLevel(logEvent.Level),
                            ClassName = logEvent.CallerClassName,
                            MethodName = logEvent.CallerMemberName,
                            ThreadId = logEvent.UserStackFrameNumber.ToString(),
                            Category = String.Empty
                        }
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), null, "application/json");
                request.Content = content;
                await httpClient.SendAsync(request);
            }
        }

        private int DecodeLevel(LogLevel level)
        {
            return (int)SeverityDictionary.GetValueOrDefault(level, Severity.Info);
        }
    }
}