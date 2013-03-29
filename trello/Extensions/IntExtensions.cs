namespace trello.Extensions
{
    public static class IntExtensions
    {
        public static void Swap(ref int x, ref int y)
        {
            var tmp = x;
            x = y;
            y = tmp;
        }
    }
}
