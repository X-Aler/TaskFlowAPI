using Microsoft.AspNetCore.Mvc;

namespace AspLearn.Models;

public static class ServiceHelper
{
    public static IActionResult HandleStatus(this ControllerBase controller, ServiceResult result)
    {
        return result switch
        {
            ServiceResult.Ok => controller.Ok(),
            ServiceResult.BadRequest => controller.BadRequest(),
            ServiceResult.NotFound => controller.NotFound(),
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null),
        };
    }
}