/**
 * Strongly recommend a tour on https://stackoverflow.com/questions/9452701/ukkonens-suffix-tree-algorithm-in-plain-english/9513423#9513423.
 * 
 * This a C# version of https://github.com/mission-peace/interview/blob/master/src/com/interview/suffixprefix/SuffixTree.java
 * 
 * https://www.youtube.com/watch?v=aPRqocoBsFQ is a good explanation of this implementation. 
 * https://brenden.github.io/ukkonen-animation/ is a good playgroud of Ukkonen's algo.
*/

/**
 * Construct suffix tree using Ukkonen's algo
 * 
 * Solution
 * Rule 1: For phase i+1 if S[j..i] ends at last character of leaf edge then add S[i+1] at the end.
 * Rule 2: For phase i+1 if S[j..i] ends somewhere in middle of edge and next character is not S[i+1]
 * then a new leaf edge with label S[i+1] should be created.
 * Rule 3: For phase i+1 if S[j..i] ends somewhere in middle of edge and next character is S[i+1]
 * then do nothing (resulting in implicit tree)
 * 
 * Suffix Link:
 * For every node with label x@ where x is a single character and @ is possibly empty substring, there
 * is another node with label @. This node with @ is suffix link of the node with x@. If @ is empty,
 * then suffix link is root.
 * 
 * Trick 1
 * Skip/Count trick
 * While travelling down, if number of characters on edge is less than number of characters to traverse,
 * then skip directly to the end of the edge. If number of characters on label is more than number of
 * characters to traverse, then go directly to that character we care about.
 * 
 * Edge-label compression
 * Instead of storing actual characters on the path, store only start and end indices on the path.
 * 
 * Trick 2 - Stop process as soon as we hit rule 3. Rule 3 as show stopper.
 * 
 * Trick 3 - Keep a global end on leaf to do rule 1 extension.
 * 
 * Active Point - It is the point from which traversal starts for next extension or next phrase.
 * Active point always starts from root. Other extension will get active point set up correctly
 * by last extension.
 * 
 * Active Node - Node from which active point will start
 * Active Edge - It is used to choose the edge from active node. It has index of character.
 * Active Length - How far to go on active edge.
 * 
 * Active point rules
 * 1) If active length is 0, then always start looking for the character from root.
 * 2) If rule 3 extension is applied and active length is no greater than the length of path on edge,
 *    then active length will increment by 1.
 * 3) If rule 3 extension is applied and active length is greater than the length of path on edge,
 *    then change active node, active edge and active length.
 * 4) If rule 2 extension is applied and active node is not root, then follow suffix link and set
 *    suffix link as active node, do not change active edge or active length.
 * 5) If rule 2 extension is applied and active node is root, then increase active edge by 1 and decrease
 *    active length by 1.
*/

/**
 * Further, we can do some space usage optimization. 
 * Eg. for leaf node, we don't have to store children.
 * Eg. for a-z character set, we can use ASCII number - 'a' as index.
*/

namespace suffix_tree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SuffixTree
    {
        private const char UNIQUE_CHAR = '$';
        private SuffixNode _root;
        private ActivePoint _activePoint;
        private EndIndex _globalEndIndex;
        private char[] _input;
        private int _remainingSuffixCount;

        public SuffixTree(char[] input)
        {
            this._input = new char[input.Length + 1];
            for (var i = 0; i < input.Length; i++)
            {
                this._input[i] = input[i];
            }
            this._input[input.Length] = UNIQUE_CHAR;
        }

        public void Build()
        {
            this._root = SuffixNode.CreateNode(1, new EndIndex(0));
            this._root.Index = -1;
            this._activePoint = new ActivePoint(this._root);
            this._globalEndIndex = new EndIndex(-1);
            this._remainingSuffixCount = 0;
            for (var i = 0; i < this._input.Length; i++)
            {
                this.StartPhase(i);
            }

            if (this._remainingSuffixCount != 0)
            {
                Console.WriteLine("Something went wrong!");
            }

            this.SetIndexUsingDfs(this._root, 0, this._input.Length);
        }

        // DFS traversal of the suffix tree.
        public void DfsTraversal()
        {
            var result = new List<char>();
            foreach (var child in this._root.Children)
            {
                if (child != null)
                {
                    this.DfsTraversal(child, result);
                }
            }
        }

        private void DfsTraversal(SuffixNode root, List<char> result)
        {
            var suffix = result.ToList();
            for (var i = root.Start; i <= root.EndIndex.End; i++)
            {
                suffix.Add(this._input[i]);
            }

            if (root.Index != -1)
            {
                Console.Write($"{root.Index}: ");
                Console.Write(suffix.ToArray());
                Console.WriteLine();
                return;
            }

            foreach (var child in root.Children)
            {
                if (child != null)
                {
                    this.DfsTraversal(child, suffix);
                }
            }
        }

        public bool Validate()
        {
            for (var i = 0; i < this._input.Length; i++)
            {
                if (!this.Validate(this._root, this._input , i, i))
                {
                    return false;
                }
            }
            return true;
        }

        private bool Validate(SuffixNode root, char[] input, int index, int curr)
        {
            if (root == null)
            {
                Console.WriteLine($"Failed at {curr} for index {index}");
                return false;
            }

            if (root.Index != -1)
            {
                if (root.Index != index)
                {
                    Console.WriteLine($"Index is not same. Failed at {curr} for index {index}");
                    return false;
                }
                return true;
            }

            if (curr > input.Length)
            {
                Console.WriteLine($"Index is not same. Failed at {curr} for index {index}");
                return false;
            }

            var node = root.Children[input[curr]];
            if (node == null)
            {
                Console.WriteLine($"Failed at {curr} for index {index}");
                return false;
            }

            var j = 0;
            for (var i = node.Start; i <= node.EndIndex.End; i++)
            {
                if (input[curr + j] != input[i])
                {
                    Console.WriteLine($"Mismatch found {input[curr + j]} vs. {input[i]}");
                    return false;
                }
                j++;
            }
            curr += node.Length;
            return this.Validate(node, input, index, curr);
        }

        private void StartPhase(int i)
        {
            // set lastCreatedSuffixNode as null before start of each phrase.
            SuffixNode lastCreatedSuffixNode = null;
            // increase globle end index for leaf. Finish rule 1 extension for leaf.
            this._globalEndIndex.Increase();
            // increase remainingSuffixCount before start of each phrase.
            this._remainingSuffixCount++;
            while (this._remainingSuffixCount > 0)
            {
                if (this._activePoint.ActiveLength == 0)
                {
                    // If active length is 0, look for current character from root.
                    // When active length is 0, the active node must be root.
                    var nodeByChar = this.FindSuffixNodeByCharIndex(i);
                    if (nodeByChar != null)
                    {
                        // if current character from root is not null, then increase active length by 1,
                        // and break out of while loop.
                        this._activePoint.ActiveEdge = nodeByChar.Start;
                        this._activePoint.ActiveLength++;
                        break;
                    }
                    else
                    {
                        // create a new leaf node with current character from leaf. Rule 2 extension.
                        this._activePoint.ActiveNode.Children[this._input[i]] = SuffixNode.CreateNode(i, this._globalEndIndex);
                        this._remainingSuffixCount--;
                    }
                }
                else
                {
                    // If active length is not 0, it means we are traversing somewhere in middle.
                    // So check if next character is same as current char.
                    this.MoveForwardIfNeeded(i);
                    if (this.IsNextCharMatched(i))
                    {
                        // If next character is same with current character, then apply rule 3 extension 
                        // and do trick 2 (show stopper), break out while loop.

                        // Answer tusroy's TODO question:
                        // If lastCreatedSuffixNode is not null, then it must be X -> Z format. Z is current char.
                        // We can directly assign current active edge node to suffix link. 
                        if (lastCreatedSuffixNode != null)
                        {
                            lastCreatedSuffixNode.SuffixLink = this.GetCurrentActiveEdgeNode();
                        }
                        var edgeNode = this.GetCurrentActiveEdgeNode();
                        if (this._activePoint.ActiveLength < edgeNode.Length)
                        {
                            this._activePoint.ActiveLength++;
                        }
                        else
                        {
                            this._activePoint.ActiveNode = edgeNode;
                            this._activePoint.ActiveEdge = edgeNode.Children[this._input[i]].Start;
                            this._activePoint.ActiveLength = this._activePoint.ActiveLength - edgeNode.Length + 1; // actually it should be 1.
                        }
                        break;
                    }
                    else
                    {
                        // If next character is not same with current character, do rule 2 extension.
                        var edgeNode = this.GetCurrentActiveEdgeNode();
                        var internalNode = edgeNode;
                        if (edgeNode.Length > this._activePoint.ActiveLength)
                        {
                            //  if split at middle of edge node, create a new interal node first.
                            internalNode = SuffixNode.CreateNode(
                                edgeNode.Start,
                                new EndIndex(edgeNode.Start + this._activePoint.ActiveLength - 1));
                            // for every newly created internal node, set root as default suffix link.
                            internalNode.SuffixLink = this._root;
                            internalNode.Index = -1;
                            edgeNode.Start = edgeNode.Start + this._activePoint.ActiveLength;
                            internalNode.Children[this._input[edgeNode.Start]] = edgeNode;
                            this._activePoint.ActiveNode.Children[this._input[internalNode.Start]] = internalNode;
                        }

                        // create a leaf node.
                        var leafNode = SuffixNode.CreateNode(i, this._globalEndIndex);
                        internalNode.Children[this._input[i]] = leafNode;

                        // Add suffix link.
                        if (lastCreatedSuffixNode != null)
                        {
                            lastCreatedSuffixNode.SuffixLink = internalNode;
                        }
                        lastCreatedSuffixNode = internalNode;

                        // Once rule 2 extension is done, decrease remainingSuffixCount.
                        this._remainingSuffixCount--;

                        // if active node is not root, follow suffix link
                        if (this._activePoint.ActiveNode != this._root)
                        {
                            this._activePoint.ActiveNode = this._activePoint.ActiveNode.SuffixLink;
                        }
                        else
                        {
                            this._activePoint.ActiveEdge++;
                            this._activePoint.ActiveLength--;
                        }
                    }
                }
            }
        }

        // Move forward active point if current edge node's length is less than active length.
        private void MoveForwardIfNeeded(int index)
        {
            var edgeNode = this.GetCurrentActiveEdgeNode();
            if (edgeNode.Length < this._activePoint.ActiveLength)
            {
                // like X -> Y|?
                this._activePoint.ActiveNode = edgeNode;
                this._activePoint.ActiveEdge = this._activePoint.ActiveEdge + edgeNode.Length;
                this._activePoint.ActiveLength = this._activePoint.ActiveLength - edgeNode.Length;

                this.MoveForwardIfNeeded(index);
            }
        }

        // Find next character to be compared to current phase character.
        private bool IsNextCharMatched(int i)
        {
            // Let's say i character is Z
            var edgeNode = this.GetCurrentActiveEdgeNode();
            // like X|?
            if (edgeNode.Length > this._activePoint.ActiveLength)
            {
                // ? == Z
                return this._input[edgeNode.Start + this._activePoint.ActiveLength] == this._input[i];
            }
            
            // like X| -> ? 
            if (edgeNode.Length == this._activePoint.ActiveLength)
            {
                // ? == Z
                return edgeNode.Children[this._input[i]] != null;
            }

            // Always move forward if needed, then it won't get here.
            return false;
        }

        private SuffixNode GetCurrentActiveEdgeNode()
        {
            return this._activePoint.ActiveNode.Children[this._input[this._activePoint.ActiveEdge]];
        }

        private SuffixNode FindSuffixNodeByCharIndex(int index)
        {
            return this._activePoint.ActiveNode.Children[this._input[index]];
        }

        private void SetIndexUsingDfs(SuffixNode root, int val, int size)
        {
            if (root == null)
            {
                return;
            }

            val += root.Length;
            if (root.Index != -1)
            {
                root.Index = size - val;
                return;
            }

            foreach (var child in root.Children)
            {
                this.SetIndexUsingDfs(child, val, size);
            }
        }
    
    }
}