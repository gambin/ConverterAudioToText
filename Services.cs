using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace W3DO.ConverterAudioToText
{
    class Services
    {
        

        public static async Task<Translate> SpeechContinuousRecognitionAsync(string filePath)
        {
            Utils.ConsoleWriteInfo($"Iniciando conversão do arquivo: {filePath}");
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").          
            string subscriptionKey = W3Config.properties["subscriptionKey"];
            string subscriptionRegion = W3Config.properties["subscriptionRegion"];
            SpeechConfig config = SpeechConfig.FromSubscription(subscriptionKey, subscriptionRegion);

            string initialSilenceTimeoutMs = W3Config.properties["initialSilenceTimeoutMs"];
            string endSilenceTimeoutMs = W3Config.properties["endSilenceTimeoutMs"];

            config.SetProperty(PropertyId.SpeechServiceConnection_InitialSilenceTimeoutMs, initialSilenceTimeoutMs);
            config.SetProperty(PropertyId.SpeechServiceConnection_EndSilenceTimeoutMs, endSilenceTimeoutMs);
            config.OutputFormat = OutputFormat.Detailed;

            string language = W3Config.properties["languageScope"];
            bool endOfFile = false;
            Translate translateContent = null;

            // Creates a speech recognizer from file
            using (AudioConfig audioInput = AudioConfig.FromWavFileInput(filePath))
            using (SpeechRecognizer recognizer = new SpeechRecognizer(config, language, audioInput))
            {
                // Subscribes to events.
                recognizer.Recognizing += (s, e) =>
                {
                    // Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                    // Utils.ConsoleWriteInfo($"{W3Config.properties["message:recognizing"]}: {e.Result.Text}");
                };

                recognizer.Recognized += (s, e) =>
                {
                    SpeechRecognitionResult result = e.Result;
                    if (result.Reason == ResultReason.RecognizedSpeech)
                    Utils.ConsoleWriteInfo($"{W3Config.properties["message:recognizing"]}: {e.Result.Text}");
                    {
                        translateContent = new Translate{
                            Filename = filePath,
                            Content = result.Text
                        };
                        endOfFile = true;
                    }
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($" | -- Recognition Canceled. Reason: {e.Reason.ToString()}, CanceledReason: {e.Reason}");
                };


                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine($" | -- {W3Config.properties["message:sessionStarted"]} \n");
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine($" | -- {W3Config.properties["message:sessionStopped"]}");
                };

                recognizer.SpeechEndDetected += (s, e) =>
                {
                    Console.WriteLine($" | -- {W3Config.properties["message:speechEndDetected"]}");
                    endOfFile = true;
                };

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                do
                {
                    Thread.Sleep(1000);
                } while (!endOfFile);

                // Stops recognition.
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                Console.WriteLine("<================================================================>");
                return translateContent;
            }
        }

    }
}