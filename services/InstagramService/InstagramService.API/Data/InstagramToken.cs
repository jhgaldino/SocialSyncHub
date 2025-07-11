using System;

namespace InstagramService.API.Data
{
    public class InstagramToken
    {
        public int Id { get; set; }
        public string UserId { get; set; } // ID do usuário da sua aplicação
        public string InstagramUserId { get; set; }
        public string AccessToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
