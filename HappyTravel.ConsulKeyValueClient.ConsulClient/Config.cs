namespace HappyTravel.ConsulKeyValueClient.ConsulClient
{
    public class Config
    {
        /// <summary>
        /// Address is the address of the Consul server.
        /// </summary>
        public string Address { get; set; } = "127.0.0.1:8500";

        /// <summary>
        /// Scheme is the URI scheme for the Consul server.
        /// </summary>
        public string Scheme { get; set; } = "http";

        /// <summary>
        /// Token is used to provide a per-request ACL token which overrides the agent's default token.
        /// </summary>
        public string? Token { get; set; }

        
        /*public ILogger Logger { get; }

        /// <summary>
        /// Datacenter to use. If not provided, the default agent datacenter is used.
        /// </summary>
        public string DataCenter { get; set; }*/
    }
}
