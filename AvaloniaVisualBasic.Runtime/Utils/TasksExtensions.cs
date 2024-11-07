using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AvaloniaVisualBasic.Runtime.Utils;

internal static class TaskExtensions
{
    public static void ListenErrors(this Task t,
        [CallerMemberName] string? caller = null,
        [CallerFilePath] string? callerFile = null,
        [CallerLineNumber] int? callerLineNumber = null)
    {
        if (t.IsCompleted)
        {
            if (t.IsFaulted && t.Exception is { } aggregateException)
            {
                if (aggregateException.InnerExceptions.Count == 1)
                {
                    Console.WriteLine("Error in {0} at {1}:{2}\n{3}", caller, callerFile, callerLineNumber,
                        aggregateException.InnerExceptions[0]);
                }
                else
                {
                    Console.WriteLine("Error in {0} at {1}:{2}\n{3}", caller, callerFile, callerLineNumber,
                        aggregateException);
                }
            }

            return;
        }

        t.ContinueWith(
            e =>
            {
                Console.WriteLine("Error in {0} at {1}:{2}\n{3}", caller, callerFile, callerLineNumber, e.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
    }
}