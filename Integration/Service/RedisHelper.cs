using StackExchange.Redis;

namespace Integration.Service;
public static class RedisHelper
{
    private static Lazy<ConnectionMultiplexer> RedisConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
        ConfigurationOptions conf = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            ConnectTimeout = 100,
        };

        conf.EndPoints.Add("localhost", 6379);

        return ConnectionMultiplexer.Connect(conf.ToString());
    });
    public static ConnectionMultiplexer Connection => RedisConnection.Value;

    public static bool RedisLock(string key, string value, TimeSpan expireTime)
    {
        bool isLock = false;

        try
        {
            isLock = Connection.GetDatabase().StringSet(key, value, expireTime, When.NotExists);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Lock: {ex.Message}");
            isLock = true;
        }

        return isLock;
    }
    public static void RedisLockFree(string lockKey)
    {
        Connection.GetDatabase().KeyDelete(lockKey);
    }
}
