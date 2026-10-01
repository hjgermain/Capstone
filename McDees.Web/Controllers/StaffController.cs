using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace McDees.Web.Controllers;

[Authorize(Roles = "Employee,Manager,Admin")]
public sealed class StaffController : Controller
{
    public IActionResult Index() => View();
}
