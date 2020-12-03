using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Pocothon5.Models;

namespace Pocothon5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
           
            _logger = logger;
        }

        public IActionResult Index()
        {
            string[] waveFiles = Directory.GetFiles(@"C:\Pocathon6\AppData", "*.wav")
                                           .Select(Path.GetFileName)
                                           .ToArray();
            ViewBag.files = waveFiles;
            return View();
        }

        [HttpPost]
        public IActionResult Submit()
        {
           
            var file = @"C:\Pocathon6\AppData\"+Request.Form["audioFile"];
            var speechConfig = SpeechConfig.FromSubscription("d6d5967d6f9c402faddbccf68a2a3dc4", "Eastus");
            var resulttext = FromFile(speechConfig, file).GetAwaiter().GetResult();
            string[] waveFiles = Directory.GetFiles(@"C:\Pocathon6\AppData", "*.wav")
                                           .Select(Path.GetFileName)
                                           .ToArray();
            ViewBag.files = waveFiles;
            ViewBag.ResultText = resulttext;
            return View("Index");
        }

        async static Task<string> FromFile(SpeechConfig speechConfig, string file)
        {
            //var file = @"C:\PocathonDec20\cognitive-services-speech-sdk-master\cognitive-services-speech-sdk-master\sampledata\audiofiles\myVoiceIsMyPassportVerifyMe01.wav";
            //var file = @"C:\PocathonDec20\samples\CallCenter1.wav";
            using var audioConfig = AudioConfig.FromWavFileInput(file);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
            return result.Text;
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
