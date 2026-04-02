using Anthropic.SDK;
using Anthropic.SDK.Messaging;

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
                1. A list of the most important keywords to include in a CV
                2. Key skills to highlight
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
                    Model = "claude-sonnet-4-6",
                    MaxTokens = 1024
                });

            var responseText = message.Content[0].ToString();
            var result = System.Text.Json.JsonSerializer.Deserialize<AIAnalysisResult>(responseText);
            return result;
        }
    }

    public class AIAnalysisResult
    {
        public List<string> Keywords { get; set; }
        public List<string> Skills { get; set; }
        public string CoverLetter { get; set; }
    }
}