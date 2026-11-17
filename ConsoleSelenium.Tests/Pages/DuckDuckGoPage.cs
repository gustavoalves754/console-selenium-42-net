using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ConsoleSelenium.Tests.Pages;

internal sealed class DuckDuckGoPage
{
    private const string BaseUrl = "https://duckduckgo.com/";

    private static readonly By SearchInput = By.Name("q");

    private static readonly By Suggestions = By.CssSelector(
        "[role='listbox'] [role='option'], .search-suggestions-list li, .sbox-suggestions li");

    private static readonly By OrganicResultLinks = By.CssSelector(
        "article[data-testid='result'] h2 a, [data-testid='result-title-a'], .result__a");

    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public DuckDuckGoPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        _wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
    }

    public string CurrentUrl => _driver.Url;

    public string Title => _driver.Title;

    public void Open()
    {
        _driver.Navigate().GoToUrl(BaseUrl);
        _wait.Until(driver => driver.FindElement(SearchInput).Displayed);
    }

    public void Search(string term)
    {
        TypeSearchTerm(term);
        _driver.FindElement(SearchInput).SendKeys(Keys.Enter);
        WaitForResultsPage();
    }

    public void TypeSearchTerm(string term)
    {
        IWebElement input = _wait.Until(driver => driver.FindElement(SearchInput));
        input.Clear();
        input.SendKeys(term);
    }

    public string SelectSuggestion(int position)
    {
        IReadOnlyCollection<IWebElement> availableSuggestions = _wait.Until(driver =>
        {
            IReadOnlyCollection<IWebElement> elements = driver.FindElements(Suggestions)
                .Where(element => element.Displayed)
                .ToArray();

            return elements.Count >= position ? elements : null;
        })!;

        IWebElement suggestion = availableSuggestions.ElementAt(position - 1);
        string selectedText = suggestion.Text.Trim();
        suggestion.Click();
        WaitForResultsPage();
        return selectedText;
    }

    public string OpenFirstOrganicResult()
    {
        string resultsUrl = _driver.Url;
        IWebElement firstResult = _wait.Until(driver => driver.FindElements(OrganicResultLinks)
            .FirstOrDefault(link => link.Displayed && !string.IsNullOrWhiteSpace(link.GetAttribute("href"))))!;

        firstResult.Click();
        _wait.Until(driver =>
            !string.Equals(driver.Url, resultsUrl, StringComparison.OrdinalIgnoreCase) &&
            !driver.Url.Contains("duckduckgo.com/?q=", StringComparison.OrdinalIgnoreCase));

        return resultsUrl;
    }

    public void GoBackToResults(string expectedResultsUrl)
    {
        _driver.Navigate().Back();
        _wait.Until(driver =>
            driver.Url.Contains("duckduckgo.com", StringComparison.OrdinalIgnoreCase) &&
            (driver.Url.Contains("q=", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(driver.Url, expectedResultsUrl, StringComparison.OrdinalIgnoreCase)));
    }

    private void WaitForResultsPage()
    {
        _wait.Until(driver =>
            driver.Url.Contains("duckduckgo.com", StringComparison.OrdinalIgnoreCase) &&
            driver.Url.Contains("q=", StringComparison.OrdinalIgnoreCase));
    }
}
