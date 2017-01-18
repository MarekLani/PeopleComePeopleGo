using StackExchange.Redis;
using System.Text;

private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
{
    string redisCacheName = ConfigurationManager.AppSettings["RedisCacheName"].ToString();
    string redisCachePassword = ConfigurationManager.AppSettings["RedisCachePassword"].ToString();
    return ConnectionMultiplexer.Connect(redisCacheName + ",abortConnect=false,ssl=true,password=" + redisCachePassword);
});

public static ConnectionMultiplexer Connection
{
    get
    {
        return lazyConnection.Value;
    }
}

static string insertKeyValuePair()
{
    IDatabase cache = Connection.GetDatabase();
    //string depersvalue = $"{account_id}:{field}:{input}";
    //string value = $"{account_id}{Guid.NewGuid()}";
    bool created = cache.StringSet("key", "value", when: When.NotExists);

    //cache.StringGet();
    //cache.KeyDelete();
    string ret = created ? value : cache.StringGet("key").ToString();
    return (ret);
}