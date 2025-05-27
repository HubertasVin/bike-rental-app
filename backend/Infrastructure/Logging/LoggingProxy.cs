using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BikeRentalApp.Infrastructure.Logging
{
    public class LoggingProxy<T> : DispatchProxy
    {
        public T Decorated { get; set; } = default!;
        public ILogger<T> Logger { get; set; } = default!;
        public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
        public ILoggingToggleService ToggleService { get; set; } = default!;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine($"Logging enabled: {ToggleService.Enabled}");
            if (ToggleService.Enabled)
            {
                var user = HttpContextAccessor.HttpContext?.User;
                var userName = user?.Identity?.Name ?? "<anonymous>";
                var roles =
                    user?.Claims.Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .DefaultIfEmpty("<none>")
                        .Aggregate((a, b) => $"{a},{b}") ?? "<none>";

                Logger.LogInformation(
                    "User={User}; Roles={Roles}; Time={Time:u}; Method={Type}.{Method}",
                    userName,
                    roles,
                    DateTime.UtcNow,
                    typeof(T).Name,
                    targetMethod.Name
                );
            }

            object? result;
            try
            {
                result = targetMethod.Invoke(Decorated, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException!;
            }

            if (result is not Task task)
                return result!;

            if (targetMethod.ReturnType == typeof(Task))
                return InterceptAsync(task);

            var resultType = targetMethod.ReturnType.GetGenericArguments()[0];
            var interceptorMethod = typeof(LoggingProxy<T>)
                .GetMethod(
                    nameof(InterceptAsyncGeneric),
                    BindingFlags.Instance | BindingFlags.NonPublic
                )!
                .MakeGenericMethod(resultType);

            return interceptorMethod.Invoke(this, new object[] { task, targetMethod })!;
        }

        private async Task InterceptAsync(Task task)
        {
            await task.ConfigureAwait(false);
        }

        private async Task<TResult> InterceptAsyncGeneric<TResult>(
            Task<TResult> task,
            MethodInfo method
        )
        {
            return await task.ConfigureAwait(false);
        }
    }
}
