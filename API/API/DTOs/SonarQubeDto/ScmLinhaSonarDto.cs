namespace API.DTOs.SonarQubeDto
{
    public class ScmLinhaSonarDto
    {
        public string? Autor { get; set; }
        public DateTime? Data { get; set; }
        // Hash do commit
        public string? Revisao { get; set; }
    }
}
