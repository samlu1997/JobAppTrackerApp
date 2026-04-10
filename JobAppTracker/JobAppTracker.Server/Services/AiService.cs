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

        public async Task<AIAnalysisResult> AnalyseJobDescription(string jobDescription, string cvText = "No CV provided yet")
        {
            var prompt = $$"""
                You are a professional CV and cover letter writer helping a job applicant apply for a role. Your goal is to produce a tailored, natural, and confident cover letter that feels like a real person wrote it, not a robot.

                You will be given:
                - The applicant's CV
                - The job description

                Rules for the cover letter:
                - Maximum 350 words
                - 4 paragraphs: (1) genuine interest in the specific role and company, referencing something concrete from the job description, (2) most relevant experience with at least one specific achievement or example, (3) working style and skills that match what the role asks for, (4) short confident close
                - Mirror the language and tone of the job description naturally, do not copy it word for word
                - Write in a confident, warm, and direct tone. Not corporate, not stiff, not overly formal
                - Never use hyphens
                - Never use bullet points or lists
                - Never use the word leverage
                - Never use clichés like "I am passionate about", "I would be a great fit", "I am excited to bring my skills", or "I thrive in"
                - Do not mention skills or experience the applicant does not have
                - If there is a gap in experience the job asks for, acknowledge it briefly and frame it positively without dwelling on it
                - Avoid starting consecutive sentences with I
                - Address it to "Dear Hiring Manager" unless a name is provided
                - End with the applicant's name, phone number, and email taken from their CV

                Rules for keywords and skills:
                - Keywords should be single words or short phrases that would pass an ATS scan for this specific role
                - Skills should be broader competencies distinct from the keywords
                - Do not include anything that is not evidenced somewhere in the applicant's CV

                Applicant CV:
                {{cvText}}

                Job Description:
                {{jobDescription}}

                Respond in this exact JSON format with no other text:
                {
                    "keywords": ["keyword1", "keyword2"],
                    "skills": ["skill1", "skill2"],
                    "coverLetter": "cover letter text here"
                }
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
