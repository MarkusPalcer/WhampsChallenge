using System.Threading.Tasks;

namespace WhampsChallenge.Shared.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Makes it explicit that an asynchronous task is started but will never be awaited.
        /// </summary>
        public static void FireAndForget(this Task self)
        {
            // NOP
        }
    }
}