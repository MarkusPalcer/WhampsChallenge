namespace WhampsChallenge.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNot<T>(this object me)
        {
            return !(me is T);
        }
    }
}