using System;

namespace vsa_w_controller_csharp.Model;

public class UserProfile
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public string? FirstName {get; set;}
    public string? LastName {get; set;}
    public string? MiddleName {get; set;}
    public string? HeadLine {get; set;}
    public string? AvatarUrl {get; set;}
    public string? Bio {get; set;}
    public string? PhoneNumber {get; set;}
    public string? ContactEmail {get; set;}
    public string? PortfolioWebsiteUrl {get; set;} //linkedln, github, anything,...

    public User User {get; set;}
}
