using Microsoft.AspNetCore.Components;
using Radzen;

namespace WebApp.Services;

public class AppTooltipService
{
    private readonly TooltipService _tooltipService;

    public AppTooltipService(TooltipService tooltipService)
    {
        _tooltipService = tooltipService;
    }

    public void ShowTooltip(ElementReference elementReference, string text)
    {
        var options = new TooltipOptions
        {
            Style =
                "overflow: hidden; text-overflow: ellipsis; position: relative" // Prevent overflow
        };

        _tooltipService.Open(elementReference, text, options);
    }

    public void HideTooltip()
    {
        _tooltipService.Close();
    }
}