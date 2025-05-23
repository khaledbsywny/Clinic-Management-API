using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected ActionResult<T> HandleResult<T>(T result)
    {
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    protected ActionResult HandleResult(bool success)
    {
        if (!success)
            return NotFound();

        return Ok();
    }

    protected ActionResult<T> HandleResult<T>(IEnumerable<T> result)
    {
        if (result == null || !result.Any())
            return NotFound();

        return Ok(result);
    }
} 