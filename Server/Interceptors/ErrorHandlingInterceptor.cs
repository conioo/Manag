﻿using Grpc.Core;
using Grpc.Core.Interceptors;
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
            _logger.LogInformation("kwadratowanie");
            try
            {
                //request.
                Console.WriteLine(request);
              
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
