using System.Collections.Generic;

namespace Diplomat.Abstractions
{
    public interface ISettingsReader
    {
        Dictionary<string, object> Read();
    }
}