using Grpc.Core;
using Grpc.Core.Interceptors;
using Newtonsoft.Json;
using System.Dynamic;
using System.Reflection.PortableExecutable;

namespace Server.Interceptors
{
    public class ErrorHandlingInterceptor : Interceptor
    {
        private readonly ILogger<ErrorHandlingInterceptor> _logger;

        public ErrorHandlingInterceptor(ILogger<ErrorHandlingInterceptor> logger)
        {
            _logger = logger;
        }
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
       TRequest request,
       ServerCallContext context,
       UnaryServerMethod<TRequest, TResponse> continuation)
        {
            //_logger.LogInformation("kwadratowanie");
            try
            {
                //request.
                dynamic person = JsonConvert.DeserializeObject(request.ToString());
                //Console.WriteLine(request);
                Console.WriteLine(person?.delay != null);

                Console.WriteLine(context.RequestHeaders.GetValue("Filename"));
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
