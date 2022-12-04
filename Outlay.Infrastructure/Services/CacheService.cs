// using Newtonsoft.Json;
// using Outlay.Infrastructure.Interfaces;
// using StackExchange.Redis;
//
// namespace Outlay.Infrastructure.Services;
//
// public class CacheService : ICacheService
// {
//     private IDatabase _db;
//
//     public CacheService()
//     {
//         ConfigureRedis();
//     }
//
//     private void ConfigureRedis()
//     {
//         _db = ConnectionHelper.Connection.GetDatabase();
//     }
//
//     public T GetData<T>(string key)
//     {
//         var value = _db.StringGet(key);
//         return !string.IsNullOrEmpty(value) ? JsonConvert.DeserializeObject<T>(value!)! : default!;
//     }
//
//     public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
//     {
//         TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
//         var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
//         return isSet;
//     }
//
//     public object RemoveData(string key)
//     {
//         bool _isKeyExist = _db.KeyExists(key);
//         if (_isKeyExist == true)
//         {
//             return _db.KeyDelete(key);
//         }
//
//         return false;
//     }
// }