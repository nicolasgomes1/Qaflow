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

    public void ShowTooltip(ElementReference elementReference, string text,
        TooltipPosition position = TooltipPosition.Left)
    {
        var options = new TooltipOptions
        {
            Position = position, // Automatically adjusts based on space
            Style =
                "max-width: 200px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;" // Prevent overflow
        };

        _tooltipService.Open(elementReference, text, options);
    }

    public void HideTooltip()
    {
        _tooltipService.Close();
    }
}