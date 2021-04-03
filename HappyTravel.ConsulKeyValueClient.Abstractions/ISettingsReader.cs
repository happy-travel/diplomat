using System.Collections.Generic;

namespace HappyTravel.ConsulKeyValueClient.Abstractions
{
    public interface ISettingsReader
    {
        Dictionary<string, object> Read();
    }
}