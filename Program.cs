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
    using HttpClient client = new HttpClient();

    client.DefaultRequestHeaders.Add("Accept", "*/*");
    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,ru;q=0.8");
    client.DefaultRequestHeaders.Add("Priority", "u=1, i");
    client.DefaultRequestHeaders.Add("Sec-CH-UA", "\"Not A(Brand\";v=\"8\", \"Chromium\";v=\"132\", \"Google Chrome\";v=\"132\"");
    client.DefaultRequestHeaders.Add("Sec-CH-UA-Mobile", "?0");
    client.DefaultRequestHeaders.Add("Sec-CH-UA-Platform", "\"Windows\"");
    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");

    client.DefaultRequestHeaders.Referrer = new Uri("https://www.deepl.com/");

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
new
{
    jsonrpc = "2.0",
    method = "LMT_handle_jobs",
    @params = new
    {
        jobs = new[]
        {
            new
            {
                kind = "default",
                sentences = new[]
                {
                    new { text = text, id = 1, prefix = "" }
                },
                raw_en_context_before = new string[] { },
                raw_en_context_after = new string[] { },
                preferred_num_beams = 4
            }
        },
        lang = new
        {
            target_lang = targetLang,
            preference = new { weight = new { }, @default = "default" },
            source_lang_computed = "EN"
        },
        priority = 1,
        commonJobParams = new
        {
            quality = "normal",
            mode = "translate",
            browserType = 1,
            textType = "plaintext"
        },
        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
    },
    id = 39610999
};