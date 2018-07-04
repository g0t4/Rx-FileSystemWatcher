using RxFileSystemWatcher;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RxFswSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                cts.Cancel();
            };

            var rxWatcher = new FileDropWatcher(@".", "*.txt");
            rxWatcher.Dropped.Subscribe(droppedFiled =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{droppedFiled.DroppedType} \t {droppedFiled.Name}");
                Console.ForegroundColor = ConsoleColor.Gray;
            },
            ex =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error happened while watching :{ex.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;
            },
            () =>
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Done watching ...");
                Console.ForegroundColor = ConsoleColor.Gray;
            },
            cts.Token);
            rxWatcher.Start();
            rxWatcher.PollExisting();

            while (!cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(500, cts.Token);
                    if (!File.Exists("foobar.txt"))
                    {
                        File.Create("foobar.txt");
                    }
                    else
                    {
                        Console.WriteLine("Writing to file ...");
                        await File.AppendAllTextAsync("foobar.txt", $"{DateTime.Now.ToLongTimeString()}\r\n", cts.Token);
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Could not write to file ...");
                    Console.ForegroundColor = ConsoleColor.Gray;

                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Stopping watcher ...");
            Console.ForegroundColor = ConsoleColor.Gray;

            rxWatcher.Stop();
        }
    }
}
