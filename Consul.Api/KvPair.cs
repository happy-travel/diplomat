using Newtonsoft.Json;

namespace Diplomat.Consul.Api
{
    public class KvPair
    {
        /// <summary>
        ///     CreateIndex holds the index corresponding the creation of this KVPair. This is a read-only field.
        /// </summary>
        [JsonProperty("CreateIndex")]
        public ulong CreateIndex { get; set; }

        /// <summary>
        ///     Flags are any user-defined flags on the key. It is up to the implementer to check these values, since Consul does
        ///     not treat them specially.
        /// </summary>
        [JsonProperty("Flags")]
        public ulong Flags { get; set; }

        /// <summary>
        ///     Key is the name of the key. It is also part of the URL path when accessed via the API.
        /// </summary>
        [JsonProperty("Key")]
        public string Key { get; set; } = null!;

        /// <summary>
        ///     LockIndex holds the index corresponding to a lock on this key, if any. This is a read-only field.
        /// </summary>
        [JsonProperty("LockIndex")]
        public ulong LockIndex { get; set; }

        /// <summary>
        ///     ModifyIndex is used for the Check-And-Set operations and can also be fed back into the WaitIndex of the
        ///     QueryOptions in order to perform blocking queries.
        /// </summary>
        [JsonProperty("ModifyIndex")]
        public ulong ModifyIndex { get; set; }

        /// <summary>
        ///     Namespace is the namespace the KVPair is associated with Namespacing is a Consul Enterprise feature.
        /// </summary>
        [JsonProperty("Namespace")]
        public string? Namespace { get; set; }

        /// <summary>
        ///     Session is a string representing the ID of the session. Any other interactions with this key over the same session
        ///     must specify the same session ID.
        /// </summary>
        [JsonProperty("Session")]
        public string? Session { get; set; }

        /// <summary>
        ///     Value is the value for the key. This can be any value, but it will be base64 encoded upon transport.
        /// </summary>
        [JsonProperty("Value")]
        public byte[]? Value { get; set; }
    }
}