using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using WordSolver.Core;

namespace WordSolver.CLI
{
    class Program
    {
        const string usage = "Usage: dotnet run {letters} [shortest_word_length=3]";

        static async Task Main(string[] args)
        {

            // Get input from command line
            string letters = "";
            int lowerBound = 3;

            switch(args.Length)
            {
                case 0:
                    System.Console.WriteLine(usage);
                    return;
                case 1:
                    letters = args[0];
                    System.Console.WriteLine("Using default shortest word length of 3.");
                    lowerBound = 3;
                    break;
                case 2:
                    letters = args[0];
                    if (!int.TryParse(args[1], out lowerBound))
                    {
                        System.Console.WriteLine("Unable to parse second argument as an integer.");
                        System.Console.WriteLine("Using default shortest word length of 3.");
                        lowerBound = 3;
                    }
                    else if (lowerBound < 1)
                    {
                        System.Console.WriteLine($"Invalid shortest word length given: {lowerBound} (must be >= 1).");
                        System.Console.WriteLine("Using default shortest word length of 3.");
                        lowerBound = 3;
                    }
                    else
                    {
                        System.Console.WriteLine($"Using shortest word length of {lowerBound}.");
                    }
                    break;
                default:
                    System.Console.WriteLine(usage);
                    return;
            }

            IWordFinder wf = new WordFinder("../wordlist.txt"); // TODO un-hardcode

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var words = await Task.Run(() => wf.FindWords(letters, lowerBound));

            var last = words.LastOrDefault(); // (To force evaluation so that the stopwatch gets a valid result)
            sw.Stop();

            // Print the result
            foreach (var word in words)
                System.Console.WriteLine(word);

            System.Console.WriteLine($"CPU-bound (non-startup, non-IO) time: {sw.ElapsedMilliseconds}ms");
        }
    }
}
