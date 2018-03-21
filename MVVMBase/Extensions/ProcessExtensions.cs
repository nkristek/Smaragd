using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace nkristek.MVVMBase.Extensions
{
    public static class ProcessExtensions
    {
        /// <summary>
        /// Waits asynchronously for the <see cref="Process"/> to exit.
        /// https://stackoverflow.com/a/19104345
        /// </summary>
        /// <param name="process">The <see cref="Process"/> to wait on.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>. If invoked, the task will return immediately as canceled.</param>
        /// <returns>A <see cref="Task"/> representing waiting for the <see cref="Process"/> to end.</returns>
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(tcs.SetCanceled);

            return tcs.Task;
        }
    }
}
