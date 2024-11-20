namespace Degree.Services.Interfaces
{
    public interface ISessionService
    {
        void SetObjectAsJson(string key, object value);
        T? GetObjectFromJson<T>(string key);
    }
}
