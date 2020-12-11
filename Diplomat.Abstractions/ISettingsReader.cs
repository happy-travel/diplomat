using System.Collections.Generic;

namespace HappyTravel.Diplomat.Abstractions
{
    public interface ISettingsReader
    {
        Dictionary<string, object> Read();
    }
}