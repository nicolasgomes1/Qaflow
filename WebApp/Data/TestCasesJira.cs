namespace WebApp.Data;

public class TestCasesJira
{
    public int Id { get; set; }

    public int JiraId { get; set; }
    public string Key { get; set; } = string.Empty;

    public int TestCasesId { get; set; }

    public TestCases? TestCases { get; set; }
}