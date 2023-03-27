using System;
using System.IO;
using System.Linq;
using System.Numerics;
using MagmaCipher;

namespace MagmaCipherMain
{
    class Program
    {
        public static bool KeyFileExists = false;
        public static bool FileInputExists = false;
        public static bool FileOutputExists = false;

        private static byte[] _key = new byte[AlgMagmaCipher.KeyLength];
        private static byte[] _iv = new byte[AlgMagmaCipher.BlockSize];

        static void Main(string[] args)
        {
            var rnd = new Random();
            rnd.NextBytes(_iv);
            try
            {
                switch (args[0])
                {
                    case "encrypt":
                        /*
                         * encrypt
-i
C:\Users\denzi\RiderProjects\MagmaCipher\MagmaCipherMain\Input.txt
-k
C:\Users\denzi\RiderProjects\MagmaCipher\MagmaCipherMain\KeyFile.txt
-o
C:\Users\denzi\RiderProjects\MagmaCipher\MagmaCipherMain\Output.txt
                         */
                        Encrypt(args);
                        break;
                    case "decrypt":
                        /*
                         * decrypt
-i
C:\Users\denzi\RiderProjects\MagmaCipher\MagmaCipherMain\Output.txt
-k
C:\Users\denzi\RiderProjects\MagmaCipher\MagmaCipherMain\KeyFile.txt
-o
C:\Users\denzi\RiderProjects\MagmaCipher\MagmaCipherMain\Input.txt
                        */
                            Decrypt(args);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникла непредвиденная ошибка:" + ex.Message);
            }
        }

        private static void Encrypt(string[] args)
        {
            try
            {
                var parameters = ForEachByArgs(args);

                var inputOutputData = CheckEmptyParams(parameters.Item2, parameters.Item3);
                GetKey(parameters.Item1);
                var plainText = inputOutputData.Item1;
                var cipher = new AlgMagmaCipher();

                var resultOfEncrypt = AlgMagmaCipher.EncryptStringToBytes(plainText, _key, _iv, cipher);

                WriteResult(resultOfEncrypt, inputOutputData.Item2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private static void GetKey(string filename)
        {
            var fullkey = File.ReadAllBytes(filename);
            _key = fullkey.Take(_key.Length).ToArray();
        }

        private static void Decrypt(string[] args)
        {
            try
            {
                var parameters = ForEachByArgs(args);

                var inputOutputData = CheckEmptyParams(parameters.Item2, parameters.Item3);
                GetKey(parameters.Item1);
                var plainText = File.ReadAllBytes(parameters.Item2);
                var cipher = new AlgMagmaCipher();

                var resultOfDecrypt = AlgMagmaCipher.DecryptStringFromBytes(plainText, _key, _iv, cipher);

                WriteResult(resultOfDecrypt, inputOutputData.Item2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private static void WriteResult(string resultOfEncrypt, string item2)
        {
            if (FileOutputExists)
            {
                File.WriteAllText(item2, resultOfEncrypt);
            }
            else
            {
                Console.WriteLine(resultOfEncrypt);
            }
        }

        private static void WriteResult(byte[] resultOfEncrypt, string item2)
        {
            if (FileOutputExists)
            {
                File.WriteAllBytes(item2, resultOfEncrypt);
            }
            else
            {
                Console.WriteLine(resultOfEncrypt);
            }
        }


        private static (string, string) CheckEmptyParams(string input, string output)
        {
            string resultInput = null;
            if (FileInputExists)
            {
                resultInput = File.ReadAllText(input);
            }
            else
            {
                Console.WriteLine("Введите шифруемый текст");
                resultInput = Console.ReadLine();
            }

            return (resultInput, output);
        }

        private static (string, string, string) ForEachByArgs(string[] args)
        {
            string fileInput = null;
            string fileOutput = null;
            string fileWithKey = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-k":
                        KeyFileExists = true;
                        i++;
                        fileWithKey = args[i];
                        break;
                    case "-i":
                        FileInputExists = true;
                        i++;
                        fileInput = args[i];
                        break;
                    case "-o":
                        FileOutputExists = true;
                        i++;
                        fileOutput = args[i];
                        break;
                    default:
                        var message = $"Нарушены правила ввода, неизвестная команда = {args[i]}";
                        throw new Exception(message);
                }
            }

            return (fileWithKey, fileInput, fileOutput);
        }
    }
}