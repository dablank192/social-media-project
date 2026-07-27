using System;

namespace vsa_w_controller_csharp.Model;

public class UserFollow
{
    public Guid FolloweeId {get; set;} // nguoi dc theo doi
    public Guid FollowerId {get; set;} //nguoi di theo doi
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public UserProfile Follower {get; set;}
    public UserProfile Followee {get; set;}
}
