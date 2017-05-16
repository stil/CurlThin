using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CurlThin.Samples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var samples = FindSamples<ISample>();

            Console.WriteLine("Available samples:");
            for (var i = 0; i < samples.Count; i++)
            {
                Console.WriteLine($"{i+1}. {samples[i].GetType().FullName}");
            }

            Console.Write($"Which one do you choose [1-{samples.Count}]: ");
            var selection = int.Parse(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine();

            samples[selection-1].Run();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Finished! Press ENTER to exit...");
            Console.ReadLine();
        }

        private static List<T> FindSamples<T>() where T : class
        {
            return typeof(Program).GetTypeInfo()
                .Assembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(T)) && t.GetConstructor(Type.EmptyTypes) != null)
                .Select(t => Activator.CreateInstance(t) as T)
                .OrderBy(arg => arg?.GetType().FullName)
                .ToList();
        }
    }
}