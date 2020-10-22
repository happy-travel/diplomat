using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Diplomat.Consul.Api
{
    public class QueryOptions
    {
        public bool IsRecursive { get; set; }


        public static QueryOptions Default 
            => new QueryOptions();
    }
}
