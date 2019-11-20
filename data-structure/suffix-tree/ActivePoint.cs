namespace suffix_tree
{
    class ActivePoint
    {
        public SuffixNode ActiveNode;
        public int ActiveEdge;
        public int ActiveLength;

        public ActivePoint(SuffixNode node)
        {
            this.ActiveNode = node;
            this.ActiveEdge = -1;
            this.ActiveLength = 0;
        }

        public override string ToString()
        {
            return $"ActivePoint [activeNode={this.ActiveNode}, activeIndex={this.ActiveEdge}, activeLength={this.ActiveLength}]";
        }
    }
}