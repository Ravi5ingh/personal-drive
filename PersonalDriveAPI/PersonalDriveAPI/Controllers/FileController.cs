using Microsoft.AspNetCore.Mvc;

namespace PersonalDriveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        public FileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetFiles/{apiKey}")]
        public async Task<List<string>> GetFiles(string apiKey)
        {
            if (!IsApiKeyValid(apiKey)) throw new UnauthorizedAccessException();

            return new DirectoryInfo(
                _configuration.GetValue<string>("FileDirectory")).EnumerateFiles()
                .Select(fi => fi.Name).ToList();
        }

        [HttpGet("Download/{apiKey}/{fileName}")]
        public async Task<IActionResult> Download(string apiKey, string fileName)
        {
            if (!IsApiKeyValid(apiKey)) throw new UnauthorizedAccessException();

            return File(
                new FileStream($@"{_configuration.GetValue<string>("FileDirectory")}/{fileName}", FileMode.Open, FileAccess.Read),
                "application/octet-stream");
        }

        private bool IsApiKeyValid(string apiKey)
        {
            return apiKey == _configuration.GetValue<string>("ApiKeySecret");
        }
    }
}
