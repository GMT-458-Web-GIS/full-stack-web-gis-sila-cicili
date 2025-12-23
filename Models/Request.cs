using System;
using System.Collections.Generic;

namespace LibrarySystem.Models
{
    public partial class Request
    {
        public int RequestId { get; set; }
        public int? UserId { get; set; }
        public int? BookId { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? Status { get; set; }

        public virtual Book? Book { get; set; }
        public virtual User? User { get; set; }
    }
}