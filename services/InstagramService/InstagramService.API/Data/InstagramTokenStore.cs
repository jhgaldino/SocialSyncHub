using System.Collections.Generic;
using System.Linq;

namespace InstagramService.API.Data
{
    public class InstagramTokenStore
    {
        private static readonly List<InstagramToken> _tokens = new();

        public void SaveToken(InstagramToken token)
        {
            _tokens.Add(token);
        }

        public InstagramToken GetByUserId(string userId)
        {
            return _tokens.FirstOrDefault(t => t.UserId == userId);
        }
    }
}
