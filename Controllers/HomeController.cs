using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pocothon5.Models;

namespace Pocothon5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration iConfig)
        {
            _logger = logger;
            configuration = iConfig;
        }

        public IActionResult Index()
        {
            var url = configuration.GetSection("AudioFileUrl").Value;
            string[] waveFiles = Directory.GetFiles(url, "*.wav")
                                           .Select(Path.GetFileName)
                                           .ToArray();
            
            ViewBag.files = waveFiles;
            return View();
        }

        [HttpPost]
        public JsonResult Submit(string filename)
        {
            var url = configuration.GetSection("AudioFileUrl").Value;
            var SubscriptionKey = configuration.GetSection("SubscriptionKey").Value;
            var SubscriptionRegion = configuration.GetSection("SubscriptionRegion").Value;
            var file = url+ filename;
            var speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, SubscriptionRegion);
            var resulttext = FromFile(speechConfig, file).GetAwaiter().GetResult();
            string[] waveFiles = Directory.GetFiles(url, "*.wav")
                                           .Select(Path.GetFileName)
                                           .ToArray();
            //Match with template

            int score = 0;
            FizzyMatch fizzy = new FizzyMatch();
            var templateString = "My voice is my passport verify me";
            var matchedIndices = fizzy.FuzzyMatch(resulttext, templateString, out score);
            resulttext = resulttext + "=" + score;
            

            return Json(resulttext);
        }

        async static Task<string> FromFile(SpeechConfig speechConfig, string file)
        {
            using var audioConfig = AudioConfig.FromWavFileInput(file);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
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
