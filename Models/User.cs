using System;
using System.Collections.Generic;

namespace LibrarySystem.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Role { get; set; }

    public string? AccountStatus { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
