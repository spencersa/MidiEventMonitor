using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiEventMonitor
{
    public class Program
    {
        static void Main(string[] args)
        {
            string command;
            bool quit = false;

            var monitor = new Monitor("notepad");

            while (!quit)
            {
                command = Console.ReadLine().ToLower();
                switch (command)
                {
                    case "-devices":
                    case "-a":
                        monitor.SendKey("hello world");
                        break;
                    case "-d":
                        var devices = monitor.GetConnectedDeviceInformation();
                        foreach (var device in devices)
                        {
                            Console.WriteLine(device);
                        }
                        break;

                    case "-quit":
                    case "-q":
                        break;
                    default:
                        Console.WriteLine("Unknown Command " + command);
                        break;
                }
            }
        }
    }
}
