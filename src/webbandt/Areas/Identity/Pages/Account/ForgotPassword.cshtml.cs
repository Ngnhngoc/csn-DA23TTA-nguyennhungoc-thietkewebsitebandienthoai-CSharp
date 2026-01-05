// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace webbandt.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                // Chỉ kiểm tra nếu user không tồn tại thôi. Bỏ đoạn check EmailConfirmed đi.
                if (user == null)
                {
                    // Nếu nhập mail linh tinh không có trong database thì mới đá về Login
                    return RedirectToPage("./Đăng nhập");
                }

                // --- BẮT ĐẦU ĐOẠN "ĐI TẮT" ---

                // 1. Tạo mã token reset password
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                // 2. Mã hóa token để an toàn trên URL
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                // 3. Chuyển hướng THẲNG sang trang đặt lại mật khẩu (kèm theo mã code)
                // Lưu ý: "./ResetPassword" là tên file trang đặt mật khẩu của bạn
                return RedirectToPage("./ResetPassword", new { code = code });

                // --- KẾT THÚC ---
            }

            return Page();
        }
    }
}
