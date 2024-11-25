namespace WebApp.Data.enums;

public enum ArchivedStatus
{
    /// <summary>
    /// By default, the item is active
    /// </summary>
    Active,
    /// <summary>
    /// This should be used we want to keep the item but no longer want to see it in the active list,
    /// we should never delete production data as it would bring inconsistencies in the data
    /// </summary>
    Archived
}