using Microsoft.AspNetCore.Identity;

namespace webbandt
{
    // Class này dùng để dịch thông báo lỗi sang tiếng Việt
    public class VietnameseIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = $"Đã xảy ra lỗi không xác định." }; }
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Lỗi đồng bộ dữ liệu, vui lòng thử lại." }; }
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "Mật khẩu không chính xác." }; }
        public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "Mã xác thực không hợp lệ." }; }
        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Tài khoản này đã được liên kết." }; }
        public override IdentityError InvalidUserName(string? userName) => new() { Code = nameof(InvalidUserName), Description = $"Tên đăng nhập '{userName}' không hợp lệ, chỉ chấp nhận chữ cái và số." };
        public override IdentityError InvalidEmail(string? email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"Email '{email}' không hợp lệ." }; }
        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"Tên đăng nhập '{userName}' đã được sử dụng." }; }
        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"Email '{email}' đã được sử dụng." }; }
        public override IdentityError InvalidRoleName(string? role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"Tên quyền '{role}' không hợp lệ." }; }
        public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"Tên quyền '{role}' đã tồn tại." }; }
        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "Người dùng đã có mật khẩu." }; }
        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Tính năng khóa tài khoản chưa được kích hoạt." }; }
        public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"Người dùng đã có quyền '{role}'." }; }
        public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"Người dùng không có quyền '{role}'." }; }

        // --- CÁC LỖI MẬT KHẨU ---
        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"Mật khẩu phải có ít nhất {length} ký tự." }; }
        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Mật khẩu phải có ít nhất một ký tự đặc biệt (!@#$%^&*)." }; }
        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Mật khẩu phải có ít nhất một chữ số ('0'-'9')." }; }
        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Mật khẩu phải có ít nhất một chữ thường ('a'-'z')." }; }
        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Mật khẩu phải có ít nhất một chữ hoa ('A'-'Z')." }; }
    }
}