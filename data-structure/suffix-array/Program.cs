namespace suffix_array
{
    using System;

    class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please input a string.");
            }

            var input = args[0];
            var suffixArray = new SuffixArray(input.ToCharArray());
            suffixArray.Build();
            suffixArray.Print();
            var lcp = suffixArray.BuildLCP();
            Console.Write("The LCP is: ");
            foreach (var i in lcp)
            {
                Console.Write($"{i} ");
            }
            Console.WriteLine();
        }
    }
}
