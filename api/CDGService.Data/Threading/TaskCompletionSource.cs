 

using System;
using System.Threading.Tasks;

namespace CDGService.Data.Threading
{
    /// <summary>
    /// Task completion source
    /// </summary>
    public abstract class TaskCompletionSource
    {
        /// <summary>
        /// Gets the task.
        /// </summary>
        /// <value>
        /// The task.
        /// </value>
        public abstract Task Task { get; }

        /// <summary>
        ///     Sets the result.
        /// </summary>
        /// <param name="result">The result.</param>
        public abstract void SetResult(object result);

        /// <summary>
        ///     Sets the exception.
        /// </summary>
        /// <param name="e">The e.</param>
        public abstract void SetException(Exception e);

        /// <summary>
        /// Creates a TaskCompletionSource for the given.
        /// </summary>
        /// <param name="taskType">Type of the task 
        /// (may be void or null, in which case the result parameter to SetResult() is ignored).</param>
        /// <returns></returns>
        public static TaskCompletionSource Create(Type taskType)
        {
            var tcsArgumentType = taskType == typeof (void) || taskType == null ? typeof (object) : taskType;
            var tcsType = typeof (TaskCompletionSourceImplementation<>).MakeGenericType(tcsArgumentType);
            var source = (TaskCompletionSource) Activator.CreateInstance(tcsType);
            return source;
        }
    }
}