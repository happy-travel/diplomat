namespace HappyTravel.ConsulKeyValueClient.ConsulClient
{
    public class QueryOptions
    {
        public bool IsRecursive { get; set; }


        public static QueryOptions Default 
            => new QueryOptions();
    }
}
