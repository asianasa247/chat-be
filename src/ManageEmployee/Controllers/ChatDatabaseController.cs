using ManageEmployee.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ManageEmployee.DataTransferObject;
namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatDatabaseController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly GeminiService _gemini;

        public ChatDatabaseController(IConfiguration config)
        {
            _config = config;

            var geminiApiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(geminiApiKey))
            {
                throw new Exception("Gemini API Key is not configured. Please add 'Gemini:ApiKey' to appsettings.json.");
            }

            var httpClient = new HttpClient();
            _gemini = new GeminiService(geminiApiKey, httpClient);
        }

        [HttpGet("init")]
        public async Task<IActionResult> Init()
        {
            var connString = BuildConnectionString();
            if (string.IsNullOrEmpty(connString))
            {
                return StatusCode(500, "Database connection string 'ConnStr' is not configured.");
            }

            string schema;
            try
            {
                schema = GetDatabaseSchema(connString);
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Error getting database schema: {ex.Message}");
                return StatusCode(500, $"Error getting database schema: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An unexpected error occurred while fetching schema: {ex.Message}");
                return StatusCode(500, $"An unexpected error occurred while fetching schema: {ex.Message}");
            }

            var prompt = $"Dựa vào thông tin database dưới đây, gợi ý 5 câu hỏi thường gặp mà người dùng có thể hỏi hệ thống. Trả lời bằng tiếng Việt.\n\nSchema:\n{schema}";

            string suggestions;
            try
            {
                suggestions = await _gemini.GenerateTextFromPrompt(prompt);
                suggestions = suggestions.Replace("```", "").Trim();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error generating suggestions from Gemini: {ex.Message}");
                return StatusCode(500, $"Error generating suggestions from Gemini: {ex.Message}");
            }

            return Ok(new
            {
                schema = schema,
                suggestions = suggestions
            });
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] QuestionDtoModel dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Question))
            {
                return BadRequest("Question cannot be empty.");
            }

            var connString = BuildConnectionString();
            if (string.IsNullOrEmpty(connString))
            {
                return StatusCode(500, "Database connection string 'ConnStr' is not configured.");
            }

            string schema;
            try
            {
                schema = GetDatabaseSchema(connString);
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Error getting database schema: {ex.Message}");
                return StatusCode(500, $"Error getting database schema: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An unexpected error occurred while fetching schema: {ex.Message}");
                return StatusCode(500, $"An unexpected error occurred while fetching schema: {ex.Message}");
            }

            var prompt = $"Schema: {schema}\n\nCâu hỏi: {dto.Question}\nViết câu lệnh SQL tương ứng bằng SQL Server:";

            string rawGeminiResponse;
            string sqlToExecute;

            try
            {
                rawGeminiResponse = await _gemini.GenerateTextFromPrompt(prompt);
                sqlToExecute = ExtractSqlFromGeminiResponse(rawGeminiResponse);

                Console.WriteLine($"SQL after cleaning: '{sqlToExecute}'");

                if (string.IsNullOrEmpty(sqlToExecute))
                {
                    return BadRequest("Could not extract a valid SQL query from AI response. Please try rephrasing your question.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error generating or cleaning SQL from Gemini: {ex.Message}");
                return StatusCode(500, $"Error generating or cleaning SQL from AI: {ex.Message}");
            }

            List<Dictionary<string, object>> result;
            try
            {
                result = ExecuteQuery(connString, sqlToExecute);
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"SQL Execution Error: {ex.Message} - SQL: {sqlToExecute}");
                return Ok(new
                {
                    query = sqlToExecute,
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An unexpected error occurred while executing the query: {ex.Message} - SQL: {sqlToExecute}");
                return StatusCode(500, $"An unexpected error occurred while executing the query: {ex.Message}");
            }

            return Ok(new
            {
                query = sqlToExecute,
                result = result
            });
        }

        private string BuildConnectionString()
        {
            var connTemplate = _config["ConnectionStrings:ConnStr"];
            var dbName = _config["ConnectionStrings:DbName"];

            if (string.IsNullOrEmpty(connTemplate) || string.IsNullOrEmpty(dbName))
                return null;

            return connTemplate.Replace("{dbName}", dbName);
        }

        private string GetDatabaseSchema(string connString)
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            var cmd = new SqlCommand(@"
            SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE
            FROM INFORMATION_SCHEMA.COLUMNS
            ORDER BY TABLE_NAME", conn);

            var reader = cmd.ExecuteReader();
            var schema = new Dictionary<string, List<string>>();

            while (reader.Read())
            {
                var table = reader.GetString(0);
                var column = reader.GetString(1);
                var type = reader.GetString(2);

                if (!schema.ContainsKey(table))
                    schema[table] = new List<string>();

                schema[table].Add($"{column} ({type})");
            }

            var schemaText = string.Join("\n", schema.Select(kvp =>
                $"Table {kvp.Key}: {string.Join(", ", kvp.Value)}"));

            return schemaText;
        }

        private List<Dictionary<string, object>> ExecuteQuery(string connString, string sql)
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            using var cmd = new SqlCommand(sql, conn);

            try
            {
                var reader = cmd.ExecuteReader();

                var results = new List<Dictionary<string, object>>();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader[i];
                    }
                    results.Add(row);
                }

                return results;
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"SQL Execution Error: {ex.Message} - SQL: {sql}");
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"General Execution Error: {ex.Message} - SQL: {sql}");
                throw;
            }
        }

        private string ExtractSqlFromGeminiResponse(string response)
        {
            if (string.IsNullOrEmpty(response)) return string.Empty;

            const string sqlStartTag = "```sql";
            const string sqlEndTag = "```";

            int startIndex = response.IndexOf(sqlStartTag, StringComparison.OrdinalIgnoreCase);
            int endIndex = response.LastIndexOf(sqlEndTag, StringComparison.OrdinalIgnoreCase);

            string sqlCode;

            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                sqlCode = response.Substring(startIndex + sqlStartTag.Length, endIndex - (startIndex + sqlStartTag.Length));
                if (sqlCode.StartsWith("\n"))
                {
                    sqlCode = sqlCode.Substring(1);
                }
            }
            else
            {
                Console.WriteLine("Warning: SQL code block (```sql...```) not found in Gemini response. Attempting to clean plain text.");
                sqlCode = response;
            }

            return sqlCode.Replace("`", "").Trim();
        }
    }
}
