namespace suffix_tree
{
    using System.Text;

    class EndIndex
    {
        public int End { get; private set; }

        public EndIndex(int end)
        {
            this.End = end;
        }

        public void Increase()
        {
            this.End++;
        }
    }

    class SuffixNode
    {
        // Only support alphabet numbers, so 256 is enough.
        // TODO: we can do some improvement on space usage and support more characters.
        private const int TOTAL_CHARS = 256;
        public int Start;
        public EndIndex EndIndex;
        public SuffixNode SuffixLink;
        
        // Use character as children index directly.
        public SuffixNode[] Children = new SuffixNode[TOTAL_CHARS];

        // The suffix index. -1 indicates internal node.
        public int Index;
        
        public static SuffixNode CreateNode(int start, EndIndex endIndex)
        {
            SuffixNode node = new SuffixNode();
            node.Start = start;
            node.EndIndex = endIndex;
            return node;
        }

        public int Length
        {
            get
            {
                return this.EndIndex.End - this.Start + 1;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (var node in this.Children)
            {
                if (node != null)
                {
                    sb.Append((char)i + " ");
                }
                i++;
            }
            return $"SuffixNode [start={this.Start}] {sb}";
        }
    }
}
