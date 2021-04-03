namespace HappyTravel.ConsulKeyValueClient.Abstractions
{
    public class DiplomatOptions
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

        public string? LocalSettingsPath { get; set; } = null;

        public string KeyPrefix { get; set; } = string.Empty;
    }
}
