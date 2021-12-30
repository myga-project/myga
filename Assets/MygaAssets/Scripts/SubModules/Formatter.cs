namespace MygaCross
{
    public static class Formatter
    {
        public static bool OfType<T>(this object obj)
        {
            return obj is T;
        }
    }
}
