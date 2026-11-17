# Console Selenium 42

Atividade de automação de testes com Selenium WebDriver, MSTest e .NET 10. Os testes utilizam o site do [DuckDuckGo](https://duckduckgo.com/) para validar busca, autocompletar e navegação pelo histórico do navegador.

## Tecnologias utilizadas

- .NET 10
- MSTest 4
- Selenium WebDriver
- Selenium Support
- Google Chrome

## Estrutura da solução

```text
ConsoleSelenium42.sln
├── ConsoleSelenium
│   └── SearchTestData.cs
└── ConsoleSelenium.Tests
    ├── Infrastructure
    │   └── ChromeDriverFactory.cs
    ├── Pages
    │   └── DuckDuckGoPage.cs
    └── GoogleBuscaDesafioTest.cs
```

## Cenários automatizados

1. Pesquisa por `Selenium WebDriver C#` e validação do termo no título ou na URL.
2. Pesquisa por `C# unit testing`, espera explícita das sugestões e seleção da terceira opção.
3. Pesquisa por `Automated Testing`, abertura do primeiro resultado externo e retorno com `Navigation.Back()`.

## Como executar

Na pasta raiz do projeto, execute:

```bash
dotnet restore
dotnet test ConsoleSelenium42.sln --configuration Release
```

O Selenium Manager localiza o driver compatível com o Chrome instalado. Os testes são executados em modo headless.

> O DuckDuckGo pode exibir uma verificação anti-bot após várias buscas automatizadas. Quando isso acontece, o teste é marcado como inconclusivo e não tenta resolver o CAPTCHA. Aguarde alguns minutos e execute novamente.
