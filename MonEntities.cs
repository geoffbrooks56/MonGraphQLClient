namespace MonGraphQLClient;

public class MonBoard
{
    public MonBoard() { }

    public string BoardId { get; set; }

    public string Name { get; set; }

    public List<MonGroup> Groups { get; set; }
}

public class MonGroup
{
    public MonGroup() { }

    public string BoardId { get; set; }
    public string GroupId { get; set; }

    public string Title { get; set; }

    public List<MonGroupItem> Items { get; set; }
}

public class MonGroupItem
{
    public MonGroupItem() { }

    public string ItemId { get; set; }

    public string BoardId { get; set; }

    public string GroupId { get; set; }

    public string GroupTitle { get; set; }

    public string Name { get; set; }

    public string AccountNumber { get; set; }

    public string MainContact { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string LastActivityDate { get; set; }

    public string FollowUpDate { get; set; }

}

public class MonGroupConfigItem
{
    public MonGroupConfigItem() { }

    public string GroupName { get; set; }

    public string GroupId { get; set; }
}