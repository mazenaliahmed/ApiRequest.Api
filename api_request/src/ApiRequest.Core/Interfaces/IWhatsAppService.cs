public interface IWhatsAppService
{
    Task SendOtpAsync(string phoneNumber, string otp);
}
