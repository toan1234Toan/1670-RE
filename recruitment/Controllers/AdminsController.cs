using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using recruitment.Data;
using recruitment.Models;

namespace recruitment.Controllers
{
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminsController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;
            return View(await users.ToListAsync());
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_roleManager.Roles.OrderBy(r => r.Name).ToList(), "Name", "Name");
            var roles = _roleManager.Roles.ToList();
            Console.WriteLine($"Number of roles: {roles.Count}");
            foreach (var role in roles)
            {
                Console.WriteLine($"Role name: {role.Name}");
            }

            return View();
        }

        // POST: Admins/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Email")] IdentityUser user, string Role, string Password = "DefaultPassword123!")
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, Password);

                if (result.Succeeded && !string.IsNullOrEmpty(Role))
                {
                    await _userManager.AddToRoleAsync(user, Role);
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["Roles"] = new SelectList(_roleManager.Roles, "Name", "Name", Role);
            return View(user);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var rolesSelectList = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            ViewBag.Roles = new SelectList(rolesSelectList, "Value", "Text");
            ViewBag.UserRoles = userRoles;

            return View(user);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email")] IdentityUser user, string Role)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userToUpdate = await _userManager.FindByIdAsync(id);

                // Đảm bảo rằng user được tìm thấy
                if (userToUpdate == null)
                {
                    return NotFound($"Unable to load user with ID '{id}'.");
                }

                userToUpdate.UserName = user.UserName;
                userToUpdate.Email = user.Email;

                var updateResult = await _userManager.UpdateAsync(userToUpdate);
                if (updateResult.Succeeded)
                {
                    // Lấy danh sách roles hiện tại của người dùng
                    var currentRoles = await _userManager.GetRolesAsync(userToUpdate);

                    // Gỡ bỏ tất cả roles hiện tại của người dùng
                    var removeResult = await _userManager.RemoveFromRolesAsync(userToUpdate, currentRoles);

                    if (!removeResult.Succeeded)
                    {
                        // Xử lý lỗi khi không gỡ bỏ được role
                        ModelState.AddModelError("", "Cannot remove user existing roles");
                        return View(user);
                    }

                    // Thêm người dùng vào role mới nếu role được chọn
                    if (!string.IsNullOrEmpty(Role))
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(userToUpdate, Role);
                        if (!addRoleResult.Succeeded)
                        {
                            // Xử lý lỗi khi không thêm được role
                            ModelState.AddModelError("", "Cannot add user to role");
                            return View(user);
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }

                // Xử lý lỗi khi cập nhật thông tin người dùng không thành công
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Chuẩn bị lại Roles dropdown nếu view được hiển thị lại do có lỗi
            ViewData["Roles"] = new SelectList(_roleManager.Roles, "Name", "Name", Role);
            return View(user);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // Lấy danh sách roles của người dùng
            var userRoles = await _userManager.GetRolesAsync(user);

            // Truyền người dùng và roles vào view
            ViewBag.User = user; // Hoặc bạn có thể sử dụng ViewData["User"] = user;
            ViewBag.UserRoles = userRoles; // Hoặc ViewData["UserRoles"] = userRoles;

            return View();
        }


        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Xóa người dùng
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Xử lý trường hợp xóa không thành công
                // Hiển thị lỗi hoặc thông báo phù hợp
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                // Trả lại view Delete với thông tin người dùng để người dùng có thể thấy lỗi và thử lại
                ViewBag.User = user; // Hoặc bạn có thể sử dụng ViewData["User"] = user;
                var userRoles = await _userManager.GetRolesAsync(user);
                ViewBag.UserRoles = userRoles; // Hoặc ViewData["UserRoles"] = userRoles;
                return View("Delete");
            }
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Id.ToString() == id);
        }
    }
}
