using Moq;
using ClipboardTranslator.Core.Translators;
using ClipboardTranslator.Core.Configuration;
using Moq.Protected;
using System.Net;
using System.Text;
using ClipboardTranslator.Core.Translators.Models;


namespace ClipboardTranslator.Tests;

public class AiTranslatorTests
{
    private const string SourceText = "Test";
    private const string ExpectedTranslation = "Переведённый текст";


    [Fact]
    public async Task TranslateAsync_WhenApiReturnsSuccessAndValidJson_ParsesAndReturnsTranslation()
    {
        // Arrange
        var mockMessageHandler = SetupMockHttpMessageHandler(HttpStatusCode.OK, CreateFakeSuccessJson(ExpectedTranslation));
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var actualTranslation = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Equal(ExpectedTranslation, actualTranslation);
        VerifySendAsyncCalled(mockMessageHandler);
    }


    [Theory]
    [InlineData(HttpStatusCode.BadRequest)] // 400
    [InlineData(HttpStatusCode.Unauthorized)] // 401
    [InlineData(HttpStatusCode.Forbidden)] // 403
    [InlineData(HttpStatusCode.NotFound)] // 404
    [InlineData(HttpStatusCode.InternalServerError)] // 500
    [InlineData(HttpStatusCode.ServiceUnavailable)] // 503
    public async Task TranslateAsync_WhenApiReturnsNonSuccessStatusCode_ReturnsNull(HttpStatusCode statusCode)
    {
        // Arrange
        var mockMessageHandler = SetupMockHttpMessageHandler(statusCode, """{ "error": "API error details" }"""); // Содержимое ошибки не так важно для этого теста
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler); // Убедимся, что запрос был отправлен
    }

    [Fact]
    public async Task TranslateAsync_WhenApiResponseIsNotValidJson_ReturnsNull()
    {
        // Arrange
        var mockMessageHandler = SetupMockHttpMessageHandler(HttpStatusCode.OK, "Это не JSON { invalid syntax");
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler);
    }

    [Fact]
    public async Task TranslateAsync_WhenJsonLacksCandidates_ReturnsNull()
    {
        // Arrange
        var jsonWithoutCandidates = """{ "some_other_field": "value" }""";
        var mockMessageHandler = SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonWithoutCandidates);
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler);
    }

    [Fact]
    public async Task TranslateAsync_WhenCandidatesIsEmpty_ReturnsNull()
    {
        // Arrange
        var jsonWithEmptyCandidates = """{ "candidates": [] }""";
        var mockMessageHandler = SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonWithEmptyCandidates);
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler);
    }

    [Fact]
    public async Task TranslateAsync_WhenCandidateLacksContent_ReturnsNull()
    {
        // Arrange
        var jsonWithoutContent = """{ "candidates": [ { "no_content_here": "..." } ] }""";
        var mockMessageHandler = SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonWithoutContent);
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler);
    }

    [Fact]
    public async Task TranslateAsync_WhenContentLacksParts_ReturnsNull()
    {
        // Arrange
        var jsonWithoutParts = """{ "candidates": [ { "content": { "no_parts_here": "..." } } ] }""";
        var mockMessageHandler = SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonWithoutParts);
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler);
    }

    [Fact]
    public async Task TranslateAsync_WhenPartsIsEmpty_ReturnsNull()
    {
        // Arrange
        var jsonWithEmptyParts = """{ "candidates": [ { "content": { "parts": [] } } ] }""";
        var mockMessageHandler = SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonWithEmptyParts);
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler);
    }

    [Fact]
    public async Task TranslateAsync_WhenPartLacksText_ReturnsNull()
    {
        // Arrange
        var jsonWithoutText = """{ "candidates": [ { "content": { "parts": [ { "no_text_here": "..." } ] } } ] }""";
        var mockMessageHandler = SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonWithoutText);
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler);
    }

    [Theory]
    [InlineData("")] 
    [InlineData("   ")] 
    [InlineData(null)]
    public async Task TranslateAsync_WhenTextInPartIsNullOrWhitespace_ReturnsNull(string? textValue)
    {
        // Arrange
        string jsonWithSpecificText;
        if (textValue == null)
        {
            jsonWithSpecificText = """{ "candidates": [ { "content": { "parts": [ { } ] } } ] }""";
        }
        else
        {
            jsonWithSpecificText = CreateFakeSuccessJson(textValue);
        }

        var mockMessageHandler = SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonWithSpecificText);
        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler);
    }

    [Fact]
    public async Task TranslateAsync_WhenHttpClientThrowsException_ReturnsNull()
    {
        // Arrange
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Simulated network error"));

        var httpClient = new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("http://fake-gemini-api/") };
        var config = CreateDummyConfig();
        var translator = new AiTranslator(config, httpClient);

        // Act
        var result = await translator.TranslateAsync(SourceText);

        // Assert
        Assert.Null(result);
        VerifySendAsyncCalled(mockMessageHandler);

    }

    private TranslatorConfig CreateDummyConfig()
    {
        return new TranslatorConfig("ai",
            new GeminiOptions(ApiKey: "dummy-key", ModelId: "dummy-model", Instructions: "dummy-instructions"),
            new LanguagePair(SourceLang: "src", TargetLang: "trg")
        );
    }

    private string CreateFakeSuccessJson(string translation)
    {
        string escapedTranslation = System.Text.Json.JsonSerializer.Serialize(translation);
        escapedTranslation = escapedTranslation.Trim('"');

        return $$"""
            {
                "candidates": [
                    {
                        "content": {
                            "parts": [
                                { "text": "{{escapedTranslation}}" }
                            ]
                        }
                    }
                ]
            }
            """;
    }

    private Mock<HttpMessageHandler> SetupMockHttpMessageHandler(HttpStatusCode statusCode, string responseContent, string contentType = "application/json")
    {
        var mockResponse = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(responseContent, Encoding.UTF8, contentType)
        };

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        return mockHandler;
    }

    private void VerifySendAsyncCalled(Mock<HttpMessageHandler> mockHandler, Times? times = null)
    {
        mockHandler.Protected().Verify(
          "SendAsync",
          times ?? Times.Once(),
          ItExpr.IsAny<HttpRequestMessage>(),
          ItExpr.IsAny<CancellationToken>());
    }
}

