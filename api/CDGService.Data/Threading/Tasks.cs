
using System.Threading.Tasks;

namespace CDGService.Data.Threading
{
    /// <summary>
    /// Tasks helper library
    /// </summary>
    public static class Tasks
    {
        private static Task _void;

        /// <summary>
        /// Gets a default empty task.
        /// </summary>
        /// <returns>
        ///     The ok.
        /// </returns>
        public static Task Void()
        {
            if (_void == null)
            {
                var @void = new Task(() => { });
                @void.Start();
                _void = @void;
            }
            return _void;
        }
    }
}
