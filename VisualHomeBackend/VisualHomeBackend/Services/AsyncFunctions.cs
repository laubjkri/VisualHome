namespace VisualHomeBackend.Services
{
    public static class AsyncFunctions
    {
        public static async Task<(bool timeout, T? result)> ExecuteWithTimeout<T>(Task<T> task, TimeSpan timeoutSpan)
        {
            var delayTask = Task.Delay(timeoutSpan);
            var completedTask = await Task.WhenAny(task, delayTask);

            if (completedTask == delayTask)
            {                
                return (true, default(T)); // Timeout
            }

            return (false, await task); // Success
        }
    }
}
