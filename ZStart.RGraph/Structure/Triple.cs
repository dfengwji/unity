namespace ZStart.RGraph.Structure
{
    public class Triple<T, U, V>
    {
        public Triple()
        {
        }

        public Triple(T iFirst, U iSecond, V iThird)
        {
            this.first = iFirst;
            this.second = iSecond;
            this.third = iThird;
        }

        public T first { get; set; }
        public U second { get; set; }
        public V third { get; set; }
    }
}
