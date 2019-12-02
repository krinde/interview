namespace suffix_array
{
    public class Suffix
    {
        public int Index;
        public int Rank;
        public int NextRank;

        public Suffix(int index, int rank, int nextRank)
        {
            this.Index = index;
            this.Rank = rank;
            this.NextRank = rank;
        }
    }
}