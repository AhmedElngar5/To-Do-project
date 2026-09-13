using AhmedOS.Domain.Entities;
using AhmedOS.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AhmedOS.Tests;

public class FlashcardAndIntegrationsTests
{
    [Fact]
    public void Flashcard_DefaultValues_ShouldStartInBox1()
    {
        var card = new Flashcard
        {
            Front = "What is CQRS?",
            Back = "Command Query Responsibility Segregation"
        };

        Assert.Equal(1, card.Box);
        Assert.Equal(0, card.ReviewCount);
        Assert.NotNull(card.NextReviewDate);
        Assert.True(card.NextReviewDate <= DateTime.UtcNow.AddMinutes(1));
    }

    [Fact]
    public void UserSettings_ShouldSupportGitHubAndGeminiConfiguration()
    {
        var settings = new UserSettings
        {
            GithubUsername = "AhmedElngar5",
            GeminiApiKey = "AIzaSyFakeKeyForTest12345"
        };

        Assert.Equal("AhmedElngar5", settings.GithubUsername);
        Assert.Equal("AIzaSyFakeKeyForTest12345", settings.GeminiApiKey);
    }

    [Fact]
    public async Task GeminiAiService_WithoutApiKey_FallsBackToLocalHeuristic()
    {
        var config = new ConfigurationBuilder().Build();
        var httpClient = new HttpClient();
        var gemini = new GeminiAiService(null!, config, httpClient);

        // Without any API key, calling DecomposeTaskAsync should use fallback heuristic without throwing
        var subtasks = await gemini.DecomposeTaskAsync("Implement Microservices");

        Assert.NotNull(subtasks);
        Assert.NotEmpty(subtasks);
        Assert.Contains(subtasks, s => s.Contains("Implement Microservices"));
    }
}
