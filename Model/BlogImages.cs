using System;

namespace vsa_w_controller_csharp.Model;

public static class ImageStatus
{
    public const string Pending = "P";
    public const string Active = "A";
}

public class BlogImages
{
    public Guid Id {get; set;}
    public Guid? BlogId {get; set;}
    public string? StorageKey {get; set;}
    public int? DisplayOrder {get; set;}

    public Blog Blog {get; set;}
}
