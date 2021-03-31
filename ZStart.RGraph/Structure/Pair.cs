namespace ZStart.RGraph.Structure
{
    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T iFirst, U iSecond)
        {
            this.first = iFirst;
            this.second = iSecond;
        }

        public T first { get; set; }
        public U second { get; set; }
    }
}
