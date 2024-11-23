using System.ComponentModel.DataAnnotations;

namespace WebApp.Data.enums;

public enum TestScope
{
    [Display(Name = "API Tests")] ApiTests,

    [Display(Name = "UI Tests")] UiTests,

    [Display(Name = "DB Tests")] DatabaseTests
}