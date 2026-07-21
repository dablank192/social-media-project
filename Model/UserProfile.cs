using System;
using Amazon;

namespace vsa_w_controller_csharp.Model;

public static class ProfileStatus
{
    public const string Active = "A";
    public const string InActive = "I";
}

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
    public string? IsPublic {get; set;} = ProfileStatus.Active;
    public string? PortfolioWebsiteUrl {get; set;} //linkedln, github, anything,...

    public User User {get; set;}
}
