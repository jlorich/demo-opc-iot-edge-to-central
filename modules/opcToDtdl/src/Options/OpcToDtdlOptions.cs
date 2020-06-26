namespace MicrosoftSolutions.IoT.Edge.OpcToDtdl.Options
{
    /// <summary>
    ///  Configuration options for this function
    /// </summary>
    public class OpcToDtdlOptions
    {
        public string NodeIdRegex { get; set; } = "#s=([a-zA-Z0-9_\\.]+)";
        public string ApplicationUriRegex { get; set; } = "(.*)";
        public string DefaultApplicationUri { get; set; } = "";
    }
}
