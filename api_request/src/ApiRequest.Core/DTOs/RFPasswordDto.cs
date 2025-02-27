public class ForgotPasswordDto
{
    public string UserName { get; set; }
}

public class ResetPasswordDto
{
    public string UserName { get; set; }
    public int Otp { get; set; }
    public string NewPassword { get; set; }
}
