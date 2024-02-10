using Grpc.Core;
using Grpc.Core.Interceptors;
using Newtonsoft.Json;

namespace Server.Interceptors
{
    public class DelayInterceptor : Interceptor
    {
        private readonly ILogger<DelayInterceptor> _logger;

        public DelayInterceptor(ILogger<DelayInterceptor> logger)
        {
            _logger = logger;
        }
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
       TRequest request,
       ServerCallContext context,
       UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                if (request is not null)
                {
                    dynamic? person = JsonConvert.DeserializeObject(request.ToString());

                    if (person?.delay != null)
                    {
                        Thread.Sleep((int)person.delay * 1000);
                    }
                }

                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error thrown by {context.Method}.");
                throw;
            }
        }
    }
}