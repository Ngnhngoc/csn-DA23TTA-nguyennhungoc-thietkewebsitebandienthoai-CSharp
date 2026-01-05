using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace PhoneWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin cao cấp mới được vào đây
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // 1. Danh sách quyền
        [HttpGet]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        // 2. Tạo quyền mới (Giao diện)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 2. Tạo quyền mới (Xử lý)
        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    TempData["Success"] = $"Đã tạo quyền {roleName} thành công";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        // 3. Xóa quyền
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Đã xóa quyền thành công";
                }
            }
            return RedirectToAction("Index");
        }
    }
}