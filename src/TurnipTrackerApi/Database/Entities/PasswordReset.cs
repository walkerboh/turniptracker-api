using System;

namespace TurnipTallyApi.Database.Entities
{
    public class PasswordReset
    {
        public long RegisteredUserId { get; set; }

        public DateTime ExpiryDate { get; set; }

        public Guid Key { get; set; }
    }
}