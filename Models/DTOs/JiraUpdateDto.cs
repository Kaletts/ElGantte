using System.Collections.Generic;

namespace ElGantte.Models.DTOs
{
    public class JiraUpdateDto
    {
        public int Id { get; set; }
        public Dictionary<string, string> Values { get; set; } = new();
    }
}
