using System.Text;
using System.Text.Json;

bool enableTestMode = false;

string userInput = string.Empty;
if (!enableTestMode)
{
    Console.Write("Enter a sentence to translate: ");
    userInput = Console.ReadLine() ?? throw new NullReferenceException();
}

string translated = string.Join("\n", await GetTranslations(userInput, testMode: enableTestMode));
Console.WriteLine("Translated: \n" + translated);

static async Task<List<string>> GetTranslations(string text, string targetLang = "DE", bool testMode = false)
{
    using HttpClient client = InitHttpClient();
    object requestBody = CreateRequestBody(text, targetLang);

    string jsonString = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

    string responseBody = string.Empty;
    if (!testMode)
    {
        HttpResponseMessage response = await client.PostAsync("https://www2.deepl.com/jsonrpc?method=LMT_handle_jobs", content);
        responseBody = await response.Content.ReadAsStringAsync();
    }
    else
    {
        responseBody = GetFakeResponseContent();
    }

Start:
    try
    {
        return ExtractAllTranslations(responseBody);
    }
    catch
    {
        Console.WriteLine(responseBody);
        Console.WriteLine("Press enter to try again");
        Console.ReadLine();
        goto Start;
    }
}

static HttpClient InitHttpClient()
{
    var client = new HttpClient();

    client.DefaultRequestHeaders.Add("accept", "*/*");
    client.DefaultRequestHeaders.Add("accept-Language", "en-US,en;q=0.9,ru;q=0.8");
    client.DefaultRequestHeaders.Add("cookie", "dapUid=42280e39-a03f-4143-83bb-732ec45d1927; LMTBID=v2|65aa0e99-ece8-4586-8544-ff71c06fd64b|2b419750af74c2a65980aa3837fd05da; privacySettings=%7B%22v%22%3A2%2C%22t%22%3A1738609218052%2C%22m%22%3A%22LAX%22%2C%22consent%22%3A%5B%22NECESSARY%22%2C%22PERFORMANCE%22%2C%22COMFORT%22%2C%22MARKETING%22%5D%7D; _ga=GA1.1.1899005998.1738613387; FPID=FPID2.2.wJPjiazBf7ToCMzuFpiAmy6P%2FUOzvbmsIBC2R53YYO0%3D.1738613387; FPAU=1.2.292683051.1738613389; userCountry=AM; verifiedBot=false; dapVn=2; releaseGroups=220.DF-1925.1.9_2455.DPAY-2828.2.2_3961.B2B-663.2.3_5030.B2B-444.2.7_5562.DWFA-732.2.2_7616.DWFA-777.2.2_8287.TC-1035.2.5_8393.DPAY-3431.2.2_8776.DM-1442.2.2_9824.AP-523.2.3_10382.DF-3962.1.2_10550.DWFA-884.2.2_10551.DAL-1134.2.2_11549.DM-1149.2.2_12498.DM-1867.2.3_12500.DF-3968.2.2_12645.DAL-1151.2.1_12687.TACO-153.2.2_12891.TACO-234.2.3_13132.DM-1798.2.2_13134.DF-3988.2.2_13135.DF-4076.2.2_13564.DF-4046.2.3_13870.DF-4078.2.2_13872.EXP-133.2.2_13913.TACO-235.2.2_13915.WDW-713.2.2_14056.DF-4050.2.2_14097.DM-1916.2.2_14299.WDW-558.2.2_14526.RI-246.2.7_14958.DF-4137.2.3_14960.CEX-685.2.2_14961.CEX-501.2.2_15325.DM-1418.2.7_15509.CEX-697.2.2_16021.DM-1471.2.2_16358.WDW-677.2.2_16419.CEX-879.2.2_16420.CEX-736.2.2_16753.DF-4044.2.3_16754.DM-1769.1.2_17268.CEX-937.2.4_17271.DF-4240.2.2_17272.WTT-1298.2.5_17685.DF-4246.2.2_17696.MTD-862.1.3_18115.DF-4260.2.2_18116.DF-4250.2.2_18117.CLAB-170.2.3_18131.DM-1931.2.1_18487.DF-4161.2.4_18488.DF-4244.2.2_18500.TACO-402.2.1_18501.CEX-865.2.2_18905.WDW-516.2.2_19665.WDW-920.2.2_19666.DAL-1445.2.2_19673.WTT-1586.1.3_20028.CEX-915.1.3_20040.B2B-1712.2.2_20042.DF-4302.2.3_20742.DF-4301.1.1_20773.AAEXP-18789.1.1_20774.AAEXP-18790.1.1_20775.AAEXP-18791.1.1_20776.AAEXP-18792.1.1_20777.AAEXP-18793.1.1_20778.AAEXP-18794.1.1_20779.AAEXP-18795.2.1_20780.AAEXP-18796.2.1_20781.AAEXP-18797.1.1_20782.AAEXP-18798.2.1_20783.AAEXP-18799.2.1_20784.AAEXP-18800.2.1_20785.AAEXP-18801.2.1_20786.AAEXP-18802.2.1_20787.AAEXP-18803.2.1_20788.AAEXP-18804.1.1_20789.AAEXP-18805.1.1_20790.AAEXP-18806.2.1_20791.AAEXP-18807.2.1_20792.AAEXP-18808.2.1_20793.AAEXP-18809.1.1_20794.AAEXP-18810.1.1_20795.AAEXP-18811.1.1_20796.AAEXP-18812.1.1_20797.AAEXP-18813.2.1_20798.AAEXP-18814.1.1_20799.AAEXP-18815.1.1_20800.AAEXP-18816.2.1_20801.AAEXP-18817.1.1_20802.AAEXP-18818.1.1_20803.AAEXP-18819.1.1_20804.AAEXP-18820.1.1_20805.AAEXP-18821.1.1_20806.AAEXP-18822.1.1_20807.AAEXP-18823.1.1_20808.AAEXP-18824.1.1_20809.AAEXP-18825.1.1_20810.AAEXP-18826.1.1_20811.AAEXP-18827.1.1_20812.AAEXP-18828.1.1_20813.AAEXP-18829.1.1_20814.AAEXP-18830.1.1_20815.AAEXP-18831.1.1_20816.AAEXP-18832.1.1_20817.AAEXP-18833.1.1_20818.AAEXP-18834.1.1_20819.AAEXP-18835.1.1_20820.AAEXP-18836.1.1_20821.AAEXP-18837.1.1_20822.AAEXP-18838.1.1_20823.AAEXP-18839.1.1_20824.AAEXP-18840.1.1_20825.AAEXP-18841.1.1_20826.AAEXP-18842.1.1_20827.AAEXP-18843.1.1_20828.AAEXP-18844.1.1_20829.AAEXP-18845.1.1_20830.AAEXP-18846.1.1_20831.AAEXP-18847.1.1_20832.AAEXP-18848.1.1_20833.AAEXP-18849.1.1_20834.AAEXP-18850.1.1_20835.AAEXP-18851.1.1_20991.DM-2135.1.1; _ga_66CXJP77N5=GS1.1.1738691620.2.0.1738691620.0.0.1261895195; _uetsid=d114d9c0e26a11efa7b229823885eb21; _uetvid=d1150b10e26a11efb6adfb3f54b65120; FPLC=rqQS%2FkzsxvnnZsqnknHfrI%2FBAojoDRRtEx0V7qW%2BjK8SB63S848XB%2B3yCCjP2EihdB%2BDC3JUaL7dfbqziP6Ogm6IpRVO3Ocd1yEcWMo1Of%2BwXQSL6iNJQ4wkk%2F%2Bv%2Bw%3D%3D; dapSid=%7B%22sid%22%3A%222f56c845-9447-489a-941d-098f7c0b34ba%22%2C%22lastUpdate%22%3A1738691646%7D");
    client.DefaultRequestHeaders.Add("origin", "https://www.deepl.com");
    client.DefaultRequestHeaders.Add("Priority", "u=1, i");
    client.DefaultRequestHeaders.Add("referer", "https://www.deepl.com/");
    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not A(Brand\";v=\"8\", \"Chromium\";v=\"132\", \"Google Chrome\";v=\"132\"");
    client.DefaultRequestHeaders.Add("Sec-CH-UA-Mobile", "?0");
    client.DefaultRequestHeaders.Add("Sec-CH-UA-Platform", "\"Windows\"");
    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36");

    return client;
}

static List<string> ExtractAllTranslations(string responseBody)
{
    List<string> allTranslations = new List<string>();

    using (JsonDocument doc = JsonDocument.Parse(responseBody))
    {
        var root = doc.RootElement;

        // Navigate to the translations array in the response
        var translations = root.GetProperty("result").GetProperty("translations");

        if (translations.GetArrayLength() > 0)
        {
            // Iterate over all translations
            foreach (var translation in translations.EnumerateArray())
            {
                var beams = translation.GetProperty("beams");

                // Iterate over all beams for this translation
                foreach (var beam in beams.EnumerateArray())
                {
                    // Get all sentences from the beam
                    var sentences = beam.GetProperty("sentences");

                    // Iterate over all sentences in the beam
                    foreach (var sentence in sentences.EnumerateArray())
                    {
                        // Extract the translated text
                        var translatedText = sentence.GetProperty("text").GetString();
                        if (!string.IsNullOrEmpty(translatedText))
                        {
                            allTranslations.Add(translatedText);
                        }
                    }
                }
            }
        }
    }

    // Return all translated texts
    return allTranslations;
}

static string GetFakeResponseContent() =>
    """
    {
      "jsonrpc": "2.0",
      "id": 34610036,
      "result": {
        "translations": [
          {
            "beams": [
              {
                "sentences": [
                  {
                    "text": "schöne Mütze Mann",
                    "ids": [
                      1
                    ]
                  }
                ],
                "num_symbols": 6,
                "rephrase_variant": {
                  "name": "OfDqUK5kIjQY5s6YfyfUEaW0meXtZesz8IL8Cw=="
                }
              },
              {
                "sentences": [
                  {
                    "text": "Schöne Mütze, Mann",
                    "ids": [
                      1
                    ]
                  }
                ],
                "num_symbols": 7,
                "rephrase_variant": {
                  "name": "OD+yHLIgBzg+ULZsLqf7BLMORTOHHDxkdYbBZg=="
                }
              },
              {
                "sentences": [
                  {
                    "text": "Schöne Kappe, Mann",
                    "ids": [
                      1
                    ]
                  }
                ],
                "num_symbols": 7,
                "rephrase_variant": {
                  "name": "P1pAUcuKR0JzqJxaezDG9RIdjQ/i0QhHPv/XwA=="
                }
              },
              {
                "sentences": [
                  {
                    "text": "Nette Mütze Mann",
                    "ids": [
                      1
                    ]
                  }
                ],
                "num_symbols": 7,
                "rephrase_variant": {
                  "name": "xpDkI6rtv0DnWHLAesyhorXVY7GKNpdyNzdQAg=="
                }
              }
            ],
            "quality": "normal"
          }
        ],
        "target_lang": "DE",
        "source_lang": "EN",
        "source_lang_is_confident": false,
        "detectedLanguages": {}
      }
    }
    """;

static object CreateRequestBody(string text, string targetLang) =>
$$"""
{
  "jsonrpc": "2.0",
  "method": "LMT_handle_jobs",
  "params": {
    "jobs": [
      {
        "kind": "default",
        "sentences": [
          {
            "text": "{{text}}",
            "id": 1,
            "prefix": ""
          }
        ],
        "raw_en_context_before": [],
        "raw_en_context_after": [],
        "preferred_num_beams": 4
      }
    ],
    "lang": {
      "target_lang": "{{targetLang}}",
      "preference": {
        "weight": {},
        "default": "default"
      },
      "source_lang_computed": "EN"
    },
    "priority": -1,
    "commonJobParams": {
      "quality": "fast",
      "mode": "translate",
      "browserType": 1,
      "textType": "plaintext"
    },
    "timestamp": 1738691647356
  },
  "id": 12450057
}
""";