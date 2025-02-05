using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

class Program
{
    static void Main(string[] args)
    {
        // Path to the ChromeDriver (adjust based on your setup)
        string chromeDriverPath = @"D:\Garik\WebDrivers\chromedriver-win64";

        // Initialize ChromeDriver
        var options = new ChromeOptions();
        options.AddArgument("--headless"); // Run in headless mode (no browser window)
        options.AddArgument("--disable-gpu");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--log-level=3"); // Suppress unnecessary logs
        options.AddArgument("--disable-logging"); // Suppress unnecessary ChromeDriver logs

        using var driver = new ChromeDriver(chromeDriverPath, options);

        try
        {
            // Navigate to DeepL
            driver.Navigate().GoToUrl("https://www.deepl.com/en/translator");

            // Initialize WebDriverWait (15 seconds timeout)
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            Console.WriteLine("1");
            // Wait for the source text area to be visible
            var sourceTextArea = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[role='textbox']")));

            Console.WriteLine("2");
            // Input text to translate
            sourceTextArea.Clear();
            sourceTextArea.SendKeys("Hello world!");

            Console.WriteLine("3");
            // Wait for the target language button and click it
            //var targetLangButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@dl-test='translator-target-lang-btn']")));
            //targetLangButton.Click();

            Console.WriteLine("4");
            // Select German as the target language
            //var germanOption = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@dl-value='de']")));
            //germanOption.Click();

            Console.WriteLine("5");
            // Wait for the translation result to appear
            var translationResult = WaitForTranslation(wait, "//div[@dl-test='translator-target-result']");
            Console.WriteLine("Translation (German): " + translationResult);

            // Translate to French
            //ChangeLanguageAndPrintTranslation(wait, targetLangButton, "//button[@dl-value='fr']", "French");

            //// Translate to Italian
            //ChangeLanguageAndPrintTranslation(wait, targetLangButton, "//button[@dl-value='it']", "Italian");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        finally
        {
            // Close the browser
            driver.Quit();
        }
    }

    static void ChangeLanguageAndPrintTranslation(WebDriverWait wait, IWebElement targetLangButton, string languageXPath, string languageName)
    {
        try
        {
            // Open the language selection dropdown
            targetLangButton.Click();

            // Select the desired language
            var languageOption = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(languageXPath)));
            languageOption.Click();

            // Wait for the translation result to update
            var translationResult = WaitForTranslation(wait, "//div[@dl-test='translator-target-result']");
            Console.WriteLine($"Translation ({languageName}): " + translationResult);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during {languageName} translation: " + ex.Message);
        }
    }

    static string WaitForTranslation(WebDriverWait wait, string resultXPath)
    {
        try
        {
            // Wait until the result text is non-empty
            var translationResult = wait.Until(driver =>
            {
                var resultElement = driver.FindElement(By.XPath(resultXPath));
                return !string.IsNullOrEmpty(resultElement.Text) ? resultElement.Text : null;
            });

            return translationResult;
        }
        catch (WebDriverTimeoutException)
        {
            throw new Exception("Translation timed out. The translation result did not load in time.");
        }
    }
}
