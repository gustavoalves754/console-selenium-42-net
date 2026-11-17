using ConsoleSelenium.Tests.Infrastructure;
using ConsoleSelenium.Tests.Pages;
using OpenQA.Selenium;

namespace ConsoleSelenium.Tests;

[TestClass]
public sealed class GoogleBuscaDesafioTest
{
    private IWebDriver _driver = null!;
    private DuckDuckGoPage _duckDuckGo = null!;

    [TestInitialize]
    public void SetUp()
    {
        _driver = ChromeDriverFactory.Create();
        _duckDuckGo = new DuckDuckGoPage(_driver);
    }

    [TestCleanup]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    [TestMethod]
    public void PesquisaBasica_DeveExibirTermoNoTituloOuNaUrl()
    {
        _duckDuckGo.Open();
        _duckDuckGo.Search(SearchTestData.BasicSearch);
        EnsureSearchWasNotBlocked();

        bool termWasFound = ContainsIgnoringCase(_duckDuckGo.Title, SearchTestData.BasicSearch) ||
                            ContainsIgnoringCase(DecodedUrl(_duckDuckGo.CurrentUrl), SearchTestData.BasicSearch);

        Assert.IsTrue(
            termWasFound,
            $"O termo pesquisado não foi encontrado no título nem na URL. Título: {_duckDuckGo.Title}; URL: {_duckDuckGo.CurrentUrl}");
    }

    [TestMethod]
    public void Autocompletar_DeveSelecionarTerceiraSugestaoENavegarParaResultados()
    {
        _duckDuckGo.Open();
        _duckDuckGo.TypeSearchTerm(SearchTestData.SuggestionsSearch);

        string selectedSuggestion = _duckDuckGo.SelectSuggestion(position: 3);
        EnsureSearchWasNotBlocked();

        Assert.IsFalse(string.IsNullOrWhiteSpace(selectedSuggestion), "A terceira sugestão estava vazia.");
        Assert.IsTrue(
            _duckDuckGo.CurrentUrl.Contains("duckduckgo.com", StringComparison.OrdinalIgnoreCase) &&
            _duckDuckGo.CurrentUrl.Contains("q=", StringComparison.OrdinalIgnoreCase),
            $"A seleção não redirecionou para uma página de resultados. URL: {_duckDuckGo.CurrentUrl}");
        Assert.IsTrue(
            ContainsIgnoringCase(_duckDuckGo.Title, selectedSuggestion) ||
            ContainsIgnoringCase(DecodedUrl(_duckDuckGo.CurrentUrl), selectedSuggestion),
            $"A página de resultados não corresponde à sugestão '{selectedSuggestion}'.");
    }

    [TestMethod]
    public void Historico_DeveAbrirResultadoExternoEVoltarParaPesquisa()
    {
        _duckDuckGo.Open();
        _duckDuckGo.Search(SearchTestData.HistorySearch);
        EnsureSearchWasNotBlocked();

        string resultsUrl = _duckDuckGo.OpenFirstOrganicResult();
        string externalUrl = _duckDuckGo.CurrentUrl;

        Assert.AreNotEqual(resultsUrl, externalUrl, "O primeiro resultado não abriu uma nova página.");
        Assert.IsFalse(
            externalUrl.Contains("duckduckgo.com", StringComparison.OrdinalIgnoreCase),
            $"Era esperada uma página externa, mas a URL permaneceu no DuckDuckGo: {externalUrl}");

        _duckDuckGo.GoBackToResults(resultsUrl);

        Assert.IsTrue(
            _duckDuckGo.CurrentUrl.Contains("duckduckgo.com", StringComparison.OrdinalIgnoreCase) &&
            _duckDuckGo.CurrentUrl.Contains("q=", StringComparison.OrdinalIgnoreCase),
            $"A página de resultados não foi restaurada. URL: {_duckDuckGo.CurrentUrl}");
    }

    private static bool ContainsIgnoringCase(string source, string expected)
    {
        return source.Contains(expected, StringComparison.OrdinalIgnoreCase);
    }

    private static string DecodedUrl(string url)
    {
        return Uri.UnescapeDataString(url).Replace('+', ' ');
    }

    private void EnsureSearchWasNotBlocked()
    {
        if (_duckDuckGo.IsBotChallengeDisplayed())
        {
            Assert.Inconclusive(
                "O DuckDuckGo exibiu a verificação anti-bot. O teste não tenta resolver CAPTCHAs automaticamente.");
        }
    }
}
