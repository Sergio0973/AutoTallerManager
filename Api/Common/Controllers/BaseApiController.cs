using Microsoft.AspNetCore.Mvc;

namespace Api.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
}
