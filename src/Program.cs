using System;

namespace Authorizer
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
