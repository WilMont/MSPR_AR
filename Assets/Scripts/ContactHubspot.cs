using System.Collections.Generic;

/// <summary>
/// Model for user data.
/// </summary>
public class ContactHubspot
{
    public Dictionary<string, object> properties { get; set; }

    public ContactHubspot()
    {
        properties = new Dictionary<string, object>();

    }
    public ContactHubspot(string email, string firstname)
        : this()
    {
        properties.Add("email", email);
        properties.Add("firstname", firstname);
    }
}

