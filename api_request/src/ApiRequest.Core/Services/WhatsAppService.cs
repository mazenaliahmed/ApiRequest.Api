using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class WhatsAppService : IWhatsAppService
{
    private readonly string _accountSid = "AC634295d93bb498ebc11f159efcd66ead";
    private readonly string _authToken = "3d5c91bf56a8f56acd6e46e07a627046"; // ضع هنا التوكن الخاص بك
    private readonly string _whatsAppFromNumber = "whatsapp:+14155238886"; // رقم واتساب الخاص بك (أو رقم ساندبوكس)

    public WhatsAppService()
    {
        TwilioClient.Init(_accountSid, _authToken);
    }

    public async Task SendOtpAsync(string phoneNumber, string otp)
    {



        var messageOptions = new CreateMessageOptions(
      new PhoneNumber("whatsapp:+967730181737"));
        messageOptions.From = new PhoneNumber("whatsapp:+14155238886");
        messageOptions.Body = $"Test Mazen OTP is: {otp}:";




        var message = MessageResource.Create(messageOptions);
        Console.WriteLine(message.Body);
        // تأكد من أن رقم الهاتف يتضمن بادئة "whatsapp:"، مثال: "whatsapp:+1234567890"
        //var toNumber = "whatsapp:+967730181737";

        //var message = await MessageResource.CreateAsync(
        //    body: $"Your OTP is: {otp}",
        //    from: new PhoneNumber(_whatsAppFromNumber),
        //    to: new PhoneNumber(toNumber)
        //);
    }
}
