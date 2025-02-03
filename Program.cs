using HtmlAgilityPack;
using System.Text;

Console.WriteLine("Hello, World!");
await GetTranslations();

static async Task GetTranslations()
{
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    using HttpClient client = CreateHttpClient();

    Console.Write("Enter a sentence to translate: ");
    string userInput = Console.ReadLine() ?? throw new NullReferenceException();

    string responseBody = await RequestTranslations(client, userInput);

    string result = ExtractTranslatedWord(responseBody);

    Console.WriteLine("\nTranslation Response:\n");
    Console.WriteLine(result);
}

static HttpClient CreateHttpClient()
{
    HttpClient client = new HttpClient();

    client.DefaultRequestHeaders.Add("Accept", "text/html");
    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,ru;q=0.8");
    client.DefaultRequestHeaders.Add("Priority", "u=1, i");
    client.DefaultRequestHeaders.Add("Sec-CH-UA", "\"Not A(Brand\";v=\"8\", \"Chromium\";v=\"132\", \"Google Chrome\";v=\"132\"");
    client.DefaultRequestHeaders.Add("Sec-CH-UA-Mobile", "?0");
    client.DefaultRequestHeaders.Add("Sec-CH-UA-Platform", "\"Windows\"");
    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");

    return client;
}

static async Task<string> RequestTranslations(HttpClient client, string userInput)
{
    string encodedQuery = "query=" + Uri.EscapeDataString("###" + userInput);
    var content = new StringContent(encodedQuery, Encoding.UTF8, "application/x-www-form-urlencoded");

    var request = new HttpRequestMessage(HttpMethod.Post,
        "https://dict.deepl.com/english-german/search?ajax=1&source=english&onlyDictEntries=1&translator=dnsof7h3k2lgh3gda&kind=context&eventkind=click&forleftside=true&il=en")
    {
        Content = content
    };

    request.Headers.Referrer = new Uri("https://www.deepl.com/");

    HttpResponseMessage response = await client.SendAsync(request);
    var bytes = await response.Content.ReadAsByteArrayAsync();
    string responseBody = Encoding.GetEncoding("iso-8859-15").GetString(bytes);
    return responseBody;
}

static string ExtractTranslatedWord(string responseBody)
{
    var htmlDoc = new HtmlDocument();
    htmlDoc.LoadHtml(responseBody);

    var translatedWordNode = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(@class, 'dictLink featured')]");

    return translatedWordNode != null ? translatedWordNode.InnerText.Trim() : "No translation found.";
}