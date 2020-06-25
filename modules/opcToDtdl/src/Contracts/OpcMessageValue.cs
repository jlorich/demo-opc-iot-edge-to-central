using System;

namespace MicrosoftSolutions.IoT.Edge.OpcToDtdl.Contracts
{
    /// <summary>
    ///  Represents an OPC-UA Json Message Value field
    /// </summary>
    internal class OpcMessageValue {
        public double Value { get; set; }

        public DateTime SourceTimestamp { get; set; }
    }
}