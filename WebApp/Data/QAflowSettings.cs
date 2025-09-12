using WebApp.Data.enums;

namespace WebApp.Data;

public class QAflowSettings
{
    public int Id { get; set; }
    public QAflowOptionsSettings QAflowOptionsSettings { get; set; }
    public bool IsIntegrationEnabled { get; set; }
}