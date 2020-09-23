using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace W3DO.ConverterAudioToText
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                String userInput = Utils.RequestUserInfo();           
                List<string> listFiles = new List<string>();
                List<string> files = Utils.SubDirSearch(userInput, listFiles);
                List<Translate> filesProcessed = await Utils.FileProcess(files);
                foreach (var item in filesProcessed)
                {
                    Utils.GenerateFile(item);
                }
            } catch (Exception ex) {
                Utils.ConsoleWriteError(ex.Message);
            }
        }
    }
}