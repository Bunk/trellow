namespace Trellow
{
    public static class IntUtility
    {
        public static void Swap(ref int x, ref int y)
        {
            var tmp = x;
            x = y;
            y = tmp;
        }
    }
}
