using System;
using System.IO;

namespace DalRefactor
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var filePath = args[0];
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new Exception("args[0] (File Path) required");

                if (!Directory.Exists(filePath))
                    throw new Exception($"Directory was not found: {filePath}");

                Console.WriteLine($"File Path ({filePath}) starting scan for *.cs file");

                FileProcess.StartScan(filePath);

                Console.WriteLine($"Finish file scan");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            Console.ReadLine();

        }
    }
}
