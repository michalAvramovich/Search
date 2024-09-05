namespace SearchApi.Services
{
    public interface ICacheService
    {
        public T? GetItem<T>(string key);
        public void SaveItem<T>(string key, T item);
    }
}
