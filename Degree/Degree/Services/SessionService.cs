using Degree.Services.Interfaces;
using Newtonsoft.Json;

namespace Degree.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        private ISession Session => _httpContextAccessor.HttpContext?.Session
            ?? throw new InvalidOperationException("Session is not available");

        public void SetObjectAsJson(string key, object value) => Session.SetString(key, JsonConvert.SerializeObject(value));

        public T? GetObjectFromJson<T>(string key)
        {
            var value = Session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
