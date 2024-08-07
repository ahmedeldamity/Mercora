namespace Shared.Dtos
{
    public class ChangePasswordDto
    {
        public string email { get; set; }
        public string NewPassword { get; set; }
        public string VerificationCode { get; set; }
    }
}
