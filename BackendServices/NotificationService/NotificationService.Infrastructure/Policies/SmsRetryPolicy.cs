using Polly;
using Polly.Extensions.Http;

namespace NotificationService.Infrastructure.Policies
{
    public static class SmsRetryPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> GetPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(response =>(int)response.StatusCode == 429)
                .WaitAndRetryAsync(3,retryAttempt =>TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}