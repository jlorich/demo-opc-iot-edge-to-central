using Microsoft.Azure.WebJobs;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.EdgeHub;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using MicrosoftSolutions.IoT.Edge.OpcToDtdl.Contracts;
using MicrosoftSolutions.IoT.Edge.OpcToDtdl.Options;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using System.Linq;

namespace MicrosoftSolutions.IoT.Edge.OpcToDtdl.Functions
{
    /// <summary>
    ///  This class handles conversion of OPC-UA JSON into DTDL JSON
    /// </summary>
    public class OpcToDtdlFunction {
        private OpcToDtdlOptions _Options;

        /// <summary>
        /// Constructs a new OpcToDtdlFunction class
        /// </summary>
        /// <param name="options">Configuration options for this funciton</param>
        public OpcToDtdlFunction(IOptions<OpcToDtdlOptions> options) {
            _Options = options.Value;
        }

        /// <summary>
        ///  Azure Functions entrypoint for handling OPC-UA to DTDL message conversion.
        /// </summary>
        /// <param name="message">The OPC-UA JSON Message recieved</param>
        /// <param name="output">The IoT Edge Hub output to write to</param>
        /// <param name="logger">A logger to use</param>
        [FunctionName("OpcToDtdl")]
        public async Task Run(
            [EdgeHubTrigger("opc")] Message message,
            [EdgeHub(OutputName = "dtdl")] IAsyncCollector<Message> output,
            ILogger logger
        ) {
            byte[] messageBytes = message.GetBytes();
            logger.LogInformation($"OPC-UA Message Recieved: {Encoding.UTF8.GetString(messageBytes)}");

            var opcMessage = JsonSerializer.Deserialize<OpcMessage>(messageBytes);
            var dtdlMessage = BuildDtdlMessage(opcMessage);

            var messageString = JsonSerializer.Serialize(dtdlMessage);
            var outputMessageString = Encoding.UTF8.GetBytes(messageString);

            var outputMessage = new Message(outputMessageString);
            await output.AddAsync(outputMessage);
        }

        /// <summary>
        ///  Creates a dyanmic object in the appropraite DTDL format
        /// </summary>
        /// <param name="opcMessage">The OPC-UA message to convert</param>
        private dynamic BuildDtdlMessage(OpcMessage opcMessage) {
            dynamic dtdlMessage = new ExpandoObject();
            var dict = (IDictionary<string, object>)dtdlMessage;
            
            dict.Add("NodeId", ParseNodeId(opcMessage.NodeId));
            
            if (string.IsNullOrEmpty(opcMessage.ApplicationUri)) {
                dict.Add("ApplicationUri", _Options.DefaultApplicationUri);
            } else {
                dict.Add("ApplicationUri", ParseApplicationUri(opcMessage.ApplicationUri));
            }
            
            dict.Add("Status", opcMessage.Status);
            dict.Add("SourceTimestamp", opcMessage.Value.SourceTimestamp);
            dict.Add(opcMessage.DisplayName, opcMessage.Value.Value);

            return dtdlMessage;
        }

        /// <summary>
        ///  Parses the Node id using a regular expression
        /// </summary>
        /// <param name="opcNodeId">The NodeId to parse</param>
        private string ParseNodeId(string opcNodeId) {
            Regex rgx = new Regex(_Options.NodeIdRegex);
            var match = rgx.Match(opcNodeId);
            
            return match.Groups[1].Value;
        }

        /// <summary>
        ///  Parses the Application URI using a regular expression
        /// </summary>
        /// <param name="opcApplicationUri">The ApplicationUri to parse</param>
        private string ParseApplicationUri(string opcApplicationUri) {
            Regex rgx = new Regex(_Options.ApplicationUriRegex);
            var match = rgx.Match(opcApplicationUri);
            
            return match.Groups[1].Value;
        }
    }
}
