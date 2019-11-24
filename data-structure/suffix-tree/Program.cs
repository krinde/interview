namespace suffix_tree
{
    using System;
    
    class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                var suffixTree = new SuffixTree(args[0].ToCharArray());
                suffixTree.Build();
                suffixTree.DfsTraversal();
                if (suffixTree.Validate())
                {
                    Console.WriteLine("Great! Validate passed.");
                }
                else
                {
                    Console.WriteLine("Bad! We got some error.");
                }
            }
            else
            {
                // randomly generate input strings.
                var iterations = 1000;
                for (var i = 0; i < iterations; i++)
                {
                    var str = GetRandomString(random.Next(1,100));
                    var suffixTree = new SuffixTree(str.ToCharArray());
                    suffixTree.Build();
                    if (suffixTree.Validate())
                    {
                        Console.WriteLine($"{i}. {str} built great!");
                    }
                    else
                    {
                        Console.WriteLine($"{i}. {str} built error.");
                        return;
                    }
                }
            }


        }

        static string GetRandomString(int length)
        {
            var str = "";
            for (var i = 0; i < length; i++)
            {
                str += (char)random.Next((int)'a', (int)'z' + 1);
            }
            return str;
        }
    }
}
