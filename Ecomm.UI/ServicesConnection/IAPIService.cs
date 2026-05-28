namespace Ecomm.UI.ServicesConnection
{
    public interface IAPIService
    {
        Task<T> GetAsync<T>(string url, string token = null);
        Task<T> PostAsync<T>(string url, object data, string token = null);
        Task<T> PutAsync<T>(string url, object data, string token = null);
        Task PatchAsync(string url,object? data = null,string? token = null);
        Task<bool> deleteAsync(string url, string token = null);
        Task DeleteAsync(string url, string token = null);
        Task<bool> PutMultipartAsync(string url, object dto, string token = null);
        Task<T?> PostMultipartAsync<T>(string url, object dto, string token = null);
    }
}
