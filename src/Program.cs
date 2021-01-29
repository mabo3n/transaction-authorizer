using System;

namespace Nu
{
    class Program
    {
        static void Main(string[] args)
        {
            var jointArgs = string.Join(',', args);
            Console.WriteLine($"Hello World {jointArgs}!");
        }
    }
}
