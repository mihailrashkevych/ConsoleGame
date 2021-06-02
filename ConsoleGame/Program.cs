using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = GetHmacKey();

            if (args.Length % 2 == 1 && args.Distinct().Count() == args.Count() && args.Length >= 3)
            {
                Console.Clear();
                while (true)
                {
                    Console.WriteLine("Available moves:");
                    for (int i = 0; i < args.Length; i++)
                    {
                        Console.WriteLine("{0} - {1}", i + 1, args[i]);
                    }

                    int userMove;
                    var cpuMove = GetRandomNumber(args.Length);
                    var hmac = GetHmac(key, args[cpuMove]);
                    Console.WriteLine();
                    Console.WriteLine("HMAC: {0}", hmac);
                    Console.WriteLine();
                    Console.WriteLine("Enter your move or '0' for exit:");
                    bool c = Int32.TryParse(Console.ReadLine(), out userMove);
                    if (userMove == 0 && c) break;

                    if (userMove > 0 && userMove <= args.Length)
                    {
                        Console.WriteLine("Your move is: {0}", args[userMove - 1]);
                        Console.WriteLine("Computer move: {0}", args[cpuMove]);
                        var half = (args.Length - 1) / 2;

                        if (userMove - cpuMove - 1 == 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("***  Draw  ***");
                            Console.WriteLine();
                        }
                        else if (Math.Abs(userMove - cpuMove) <= half && userMove - cpuMove > 0 || Math.Abs(userMove - cpuMove) >= half && userMove - cpuMove < 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("+++  Victory!  +++");
                            Console.WriteLine();
                        }
                        else
                        { 
                            Console.WriteLine(); 
                            Console.WriteLine("---  Lost..  ---"); 
                            Console.WriteLine();
                        }
                            
                        Console.WriteLine("HMAC key: {0}", key);
                        Console.WriteLine();
                    }
                    else Console.WriteLine("Wrong move! Enter number in range from '1' to '{0}' or '0' to exit", args.Length);

                    Console.WriteLine("Press 'Escape' for exit or any else key for next game");
                    if (Console.ReadKey().Key == ConsoleKey.Escape) break;
                    Console.Clear();
                }
            }
            else Console.WriteLine("The number of arguments is even, not unique, or less than 3, for example: dotnet run rock paper scissors lizard Spock");
        }

        public static string GetHmacKey()
        {
            var generator = RandomNumberGenerator.Create();
            var key = new byte[16];
            generator.GetNonZeroBytes(key);
            return BitConverter.ToString(key).Replace("-", "");
        }

        public static int GetRandomNumber(int range)
        {
            byte[] random = new byte[1];

            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(random);
            }
            return ((random[0] % range));  
        }
        
        public static string GetHmac(string key, string body)
        {
            byte[] bkey = Encoding.Unicode.GetBytes(key);
            byte[] bbody = Encoding.Unicode.GetBytes(body);
            byte[] hashValue;

            using (HMACSHA256 hmac = new HMACSHA256(bkey))
            {
                hashValue = hmac.ComputeHash(bbody);
            }
            return BitConverter.ToString(hashValue).Replace("-", "");
        }
    }
}