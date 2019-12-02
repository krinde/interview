namespace suffix_array
{
    using System;

    public class SuffixArray
    {
        private char[] _input;
        private Suffix[] _su;

        public int[] Sa;
        public int[] Rank;
        
        public SuffixArray(char[] input)
        {
            this._input = input;
            this._su = new Suffix[input.Length];
            this.Sa = new int[input.Length];
            this.Rank = new int[input.Length];
        }

        public void Build()
        {
            for (var i = 0; i < this._input.Length; i++)
            {
                this._su[i] = new Suffix(i, this._input[i], 0);
                this.Sa[i] = this._su[i].Index;
                this.Rank[this._su[i].Index] = i;
            }

            for (var i = 0; i < this._input.Length; i++)
            {
                this._su[i].NextRank = i + 1 < this._input.Length
                    ? this._su[i + 1].Rank
                    : -1;
            }

            for (var len = 4; ;len *= 2)
            {
                QuickSort(this._su);

                var toIncrease = 0;
                // Reassign the rank value
                for (var i = 0; i < this._input.Length; i++)
                {
                    var r = i > 0
                        ? this._su[i-1].Rank + toIncrease
                        : 0;
                    this.Sa[i] = this._su[i].Index;
                    this.Rank[this._su[i].Index] = i;
                    if (i < this._input.Length - 1)
                    {
                        toIncrease = Compare(this._su[i + 1], this._su[i]);
                    }
                    this._su[i].Rank = r;
                }

                // judge if all suffix are sorted with differnt rank
                if (this._su[this._su.Length - 1].Rank == this._su.Length - 1)
                {
                    break;
                }

                // Reassign the next rank value for next iteration
                for (var i = 0; i < this._input.Length; i++)
                {
                    var nextIndex = this._su[i].Index + len / 2;
                    this._su[i].NextRank = nextIndex < this._su.Length
                        ? this._su[this.Rank[nextIndex]].Rank
                        : -1;
                }
            }
        }

        public int[] BuildLCP()
        {
            var lcp = new int[this._input.Length];
            var k = 0;
            for (var i = 0; i < this._input.Length; i++)
            {
                if (this.Rank[i] == this._input.Length - 1)
                {
                    k = 0;
                    continue;
                }

                var nextCharIndex = this.Sa[this.Rank[i] + 1];
                while (i + k < this._input.Length
                    && nextCharIndex + k < this._input.Length
                    && this._input[i + k] == this._input[nextCharIndex + k])
                {
                    k++;
                }

                lcp[this.Rank[i]] = k;
                
                if (k > 0)
                {
                    k--;
                }
            }

            return lcp;
        }

        public void Print()
        {
            for (var i = 0; i < this.Sa.Length; i++)
            {
                Console.Write($"{i}: ");
                for (var j = this.Sa[i]; j < this._input.Length; j++)
                {
                    Console.Write(this._input[j]);
                }
                Console.WriteLine();
            }
        }

        private static void QuickSort(Suffix[] su)
        {
            QuickSort(su, 0, su.Length - 1);
        }

        private static void QuickSort(Suffix[] su, int p, int r)
        {
            if (p < r)
            {
                var q = Partition(su, p, r);
                QuickSort(su, p, q - 1);
                QuickSort(su, q + 1, r);
            }
        }

        private static int Partition(Suffix[] su, int p, int r)
        {
            var x = su[r];
            var i = p - 1;
            for (var j = p; j < r; j++)
            {
                if (Compare(su[j], x) <= 0)
                {
                    i++;
                    Swap(su, i, j);
                }
            }
            Swap(su, i + 1, r);
            return i + 1;
        }

        private static int Compare(Suffix s1, Suffix s2)
        {
            if (s1.Rank > s2.Rank)
            {
                return 1;
            }
            else if (s1.Rank < s2.Rank)
            {
                return -1;
            }

            if (s1.NextRank > s2.NextRank)
            {
                return 1;
            }
            else if (s1.NextRank < s2.NextRank)
            {
                return -1;
            }

            return 0;
        }

        private static void Swap(Suffix[] su, int i, int j)
        {
            if (i == j)
            {
                return;
            }

            var tmp = su[j];
            su[j] = su[i];
            su[i] = tmp;
        }
    }
}