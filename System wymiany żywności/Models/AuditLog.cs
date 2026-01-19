namespace System_wymiany_żywności.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } 
        public string Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
    }
}
