using System.ComponentModel.DataAnnotations;

namespace WebApp.Data.enums;

public enum TestTypes
{
    [Display(Name = "Acceptance Tests")] AcceptanceTest,
    [Display(Name = "Regression Tests")] RegressionTest,
    [Display(Name = "Smoke Tests")] SmokeTest
}