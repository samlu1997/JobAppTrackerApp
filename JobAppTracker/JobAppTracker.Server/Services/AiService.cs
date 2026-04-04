using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using System.Text.Json.Serialization;

namespace JobAppTracker.Server.Services
{
    public class AIService
    {
        private readonly AnthropicClient _client;

        public AIService(IConfiguration configuration)
        {
            var apiKey = configuration["Anthropic:ApiKey"];
            _client = new AnthropicClient(apiKey);
        }

        public async Task<AIAnalysisResult> AnalyseJobDescription(string jobDescription)
        {
            var prompt = $$"""
                Analyse this job description and provide:
                1. The top 8-10 most important unique keywords to include in a CV (single words or short phrases only, no duplicates)
                2. The top 5-6 key skills to highlight (distinct from the keywords, focus on broader competencies)
                3. A brief cover letter draft tailored to this role

                Job Description:
                {{jobDescription}}

                Respond in this exact JSON format:
                {
                    "keywords": ["keyword1", "keyword2"],
                    "skills": ["skill1", "skill2"],
                    "coverLetter": "cover letter text here"
                }
    
                Respond with JSON only, no other text.
                """;

            var message = await _client.Messages.GetClaudeMessageAsync(
                new MessageParameters
                {
                    Messages = [new Message(RoleType.User, prompt)],
                    Model = "claude-haiku-4-5",
                    MaxTokens = 1024
                });

            var responseText = message.Content[0].ToString();

            // Strip markdown code blocks if Claude wraps the response
            responseText = responseText
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var result = System.Text.Json.JsonSerializer.Deserialize<AIAnalysisResult>(responseText);
            return result;
        }
    }

    public class AIAnalysisResult
    {
        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; }

        [JsonPropertyName("skills")]
        public List<string> Skills { get; set; }

        [JsonPropertyName("coverLetter")]
        public string CoverLetter { get; set; }
    }
}