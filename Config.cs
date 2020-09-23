using System;
using Microsoft.Extensions.Configuration;
using W3DO.ConverterAudioToText;

public static class W3Config
{
    public static string env = Environment.GetEnvironmentVariable("DOTNET_ENV");
    public static IConfiguration properties = Utils.StartConfig(env);

}