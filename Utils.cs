using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace W3DO.ConverterAudioToText
{
    class Utils
    {
        public static List<string> SubDirSearch(string dirPath, List<string> listFiles)
        {
            try
            {
                string[] fileEntries = Directory.GetFiles(@dirPath, $"*.{W3Config.properties["audioFileType"]}");
                foreach (string file in fileEntries)
                {
                    listFiles.Add(file);
                }

                foreach (string currentDir in Directory.GetDirectories(dirPath))
                {
                    SubDirSearch(currentDir, listFiles);
                }

                return listFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static IConfiguration StartConfig(string env)
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env}.json", true, true)
            .AddEnvironmentVariables()
            .Build();
            return config;
        }

        public static String RequestUserInfo()
        {
            ConsoleWriteInfo($"{W3Config.properties["message:requestUserDirectory"]}");
            return Console.ReadLine();
        }

        public static void ConsoleWriteSuccess(string message)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SUCESS]: {message}");
            Console.ResetColor();
        }

        public static void ConsoleWriteWarning(string message)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN]: {message}");
            Console.ResetColor();
        }

        public static void ConsoleWriteInfo(string message)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[INFO]: {message}");
            Console.ResetColor();
        }
        public static void ConsoleWriteError(string message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"[ERROR]: {message}");
            Console.ResetColor();
        }

        public static void GenerateFile(string filePath, string result)
        {
            string fileName = $"{filePath}_{W3Config.properties["autoTranslateSufix"]}.txt";
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, result);
            }
            else if (File.Exists(fileName))
            {
                using (var tw = new StreamWriter(fileName, true))
                {
                    tw.WriteLine(result);
                }
            }
        }

        public static void GenerateFile(Translate translate)
        {
            string fileName = $"{translate.Filename}_{W3Config.properties["autoTranslateSufix"]}.txt";
            string result = translate.Content;

            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, result);
            }
            else if (File.Exists(fileName))
            {
                File.WriteAllText(fileName, string.Empty);
                using (var tw = new StreamWriter(fileName, true))
                {
                    tw.WriteLine(result);
                }
            }
            Utils.ConsoleWriteSuccess($"{W3Config.properties["message:writeSuccess"]}: {fileName}");
        }

        public static async Task<List<Translate>> FileProcess(List<string> files)
        {
            List<Translate> list = new List<Translate>();
            try
            {
                ConsoleWriteWarning($"{W3Config.properties[$"message:filesToProcessCounter"]}: {files.Count}");
                Console.WriteLine("<================================================================>");
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        Utils.ConsoleWriteWarning($"{W3Config.properties["message:extractingContent"]}: {file}");
                        list.Add(await Services.SpeechContinuousRecognitionAsync(file));
                    }
                }
                else
                {
                    Utils.ConsoleWriteError($"{W3Config.properties["message:filesNotFound"]}");
                }
            }
            catch (Exception ex)
            {
                Utils.ConsoleWriteError(ex.Message);
            }
            return list;
        }
    }
}