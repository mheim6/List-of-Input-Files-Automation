using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListInputFilesScript
{
    class IOHelper
    {
        internal static string ReadPasswordLine()
        {
            String pass = "";
            ConsoleKeyInfo key = Console.ReadKey(true);
            while (key.Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (pass.Length > 0)
                    {
                        pass = pass.Substring(0, pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                key = Console.ReadKey(true);
            }
            Console.WriteLine();
            return pass;
        }

        internal static bool ReadBoolean()
        {
            String input = Console.ReadLine();
            if (input.Length > 0 && (input[0] == 'y' || input[0] == 'Y')) return true;
            return false; // default
        }

        internal static int? ReadInt()
        {
            int output;
            if (int.TryParse(Console.ReadLine(), out output))
            {
                return output;
            }
            return null;
        }
    }
}