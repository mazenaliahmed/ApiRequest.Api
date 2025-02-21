using ApiRequest.Core.DTOs;
using ApiRequest.Core.Entities;
using ApiRequest.Core.Interfaces;
using ApiRequest.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ApiRequest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenService _tokenService;
        private readonly ILogger<UsersController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly string id_image = "IdImage";
        private readonly string trade_license_image = "TradeLicenseImage";
        private readonly IWhatsAppService _whatsAppService;

        public UsersController(IUnitOfWork unitOfWork, TokenService tokenService, ILogger<UsersController> logger, IWebHostEnvironment environment, IWhatsAppService whatsAppService)

        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _logger = logger;
            _environment = environment;
            _whatsAppService = whatsAppService;
        }

        //[HttpPost("register")]
        //public async Task<ActionResult<UserDto>> Register(CreateUserDto createUserDto)
        //{
        //    _logger.LogInformation("Register method started for user: {UserName}", createUserDto.UserName);

        //    try
        //    {
        //        // التحقق من وجود المستخدم
        //        _logger.LogInformation("Checking if user already exists: {UserName}", createUserDto.UserName);
        //        var existingUser = (await _unitOfWork.Users.FindAsync(u => u.UserName == createUserDto.UserName)).FirstOrDefault();
        //        if (existingUser != null)
        //        {
        //            _logger.LogWarning("Username already exists: {UserName}", createUserDto.UserName);
        //            return BadRequest("Username already exists");
        //        }

        //        // إنشاء المستخدم الجديد
        //        _logger.LogInformation("Creating new user: {UserName}", createUserDto.UserName);
        //        using var hmac = new HMACSHA512();
        //        var user = new User
        //        {
        //            UserName = createUserDto.UserName,
        //            FullName = createUserDto.FullName,
        //            Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(createUserDto.Password)),
        //            PasswordSalt = hmac.Key,
        //            AccountType = createUserDto.AccountType,
        //            IdType = createUserDto.IdType,
        //            IdNumber = createUserDto.IdNumber,
        //            PhoneNumber = createUserDto.PhoneNumber,
        //            Address = createUserDto.Address,
        //            IdImage = createUserDto?.IdImage, // صورة اليطافة او جوارز السفر   المسار
        //            TradeLicenseImage = createUserDto.TradeLicenseImage,  // صورة السجل التجاري   المسار 
        //            EntryTime = DateTime.UtcNow
        //        };

        //        // إضافة المستخدم إلى قاعدة البيانات
        //        _logger.LogInformation("Adding user to the database: {UserName}", createUserDto.UserName);
        //        await _unitOfWork.Users.AddAsync(user);

        //        // حفظ التغييرات
        //        _logger.LogInformation("Saving changes to the database for user: {UserName}", createUserDto.UserName);
        //        await _unitOfWork.SaveChangesAsync();

        //        // إنشاء التوكن
        //        _logger.LogInformation("Creating token for user: {UserName}", createUserDto.UserName);
        //        var token = _tokenService.CreateToken(user);

        //        _logger.LogInformation("User registered successfully: {UserName}", createUserDto.UserName);
        //        return Ok(new
        //        {
        //            User = new UserDto
        //            {
        //                Id = user.Id,
        //                UserName = user.UserName,
        //                FullName = user.FullName,
        //                AccountType = user.AccountType,
        //                IdType = user.IdType,
        //                IdNumber = user.IdNumber,
        //                PhoneNumber = user.PhoneNumber,
        //                Address = user.Address
        //            },
        //            Token = token
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // تسجيل الخطأ
        //        _logger.LogError(ex, "An error occurred while registering user: {UserName}", createUserDto.UserName);

        //        // إرجاع رسالة خطأ مفصلة
        //        return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        //    }
        //}

        //end  register 

        //[HttpPost("register")]
        //public async Task<ActionResult<UserDto>> Register([FromForm] CreateUserDto createUserDto)
        //{
        //    _logger.LogInformation("Register method started for user: {UserName}", createUserDto.UserName);

        //    try
        //    {
        //        // التحقق من وجود المستخدم مسبقًا
        //        _logger.LogInformation("Checking if user already exists: {UserName}", createUserDto.UserName);
        //        var existingUser = (await _unitOfWork.Users.FindAsync(u => u.UserName == createUserDto.UserName)).FirstOrDefault();
        //        if (existingUser != null)
        //        {
        //            _logger.LogWarning("Username already exists: {UserName}", createUserDto.UserName);
        //            return BadRequest("Username already exists");
        //        }

        //        // إنشاء المستخدم الجديد بدون مسارات الملفات أولًا
        //        _logger.LogInformation("Creating new user: {UserName}", createUserDto.UserName);
        //        using var hmac = new HMACSHA512();
        //        var user = new User
        //        {
        //            UserName = createUserDto.UserName,
        //            FullName = createUserDto.FullName,
        //            Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(createUserDto.Password)),
        //            PasswordSalt = hmac.Key,
        //            AccountType = createUserDto.AccountType,
        //            IdType = createUserDto.IdType,
        //            IdNumber = createUserDto.IdNumber,
        //            PhoneNumber = createUserDto.PhoneNumber,
        //            Address = createUserDto.Address,
        //            EntryTime = DateTime.UtcNow
        //        };

        //        // إضافة المستخدم إلى قاعدة البيانات وحفظه للحصول على الـ Id
        //        _logger.LogInformation("Adding user to the database: {UserName}", createUserDto.UserName);
        //        await _unitOfWork.Users.AddAsync(user);
        //        await _unitOfWork.SaveChangesAsync();

        //        // إنشاء مجلد خاص بالمستخدم في مجلد "uploads"
        //        string userFolder = Path.Combine(_environment.WebRootPath, "uploads/users", user.Id.ToString());
        //        if (!Directory.Exists(userFolder))
        //        {
        //            Directory.CreateDirectory(userFolder);
        //        }

        //        // معالجة رفع صورة الهوية (يُسمح برفعها مرة واحدة)
        //        if (createUserDto.TradeLicenseImage != null)
        //        {
        //            var idFile = createUserDto.IdImage;
        //            var idFileName = Path.GetFileNameWithoutExtension(idFile.FileName) + "_" + user.Id + Path.GetExtension(idFile.FileName);
        //            var idFullPath = Path.Combine(userFolder, idFileName);
        //            using (var stream = new FileStream(idFullPath, FileMode.Create))
        //            {
        //                await idFile.CopyToAsync(stream);
        //            }
        //            // تخزين المسار النسبي في سجل المستخدم
        //            user.IdImage = Path.Combine("uploads/users", user.Id.ToString(), id_image + Path.GetExtension(idFile.FileName)).Replace("\\", "/");
        //        }

        //        // معالجة رفع صورة السجل التجاري (مرة واحدة)
        //        if (createUserDto.TradeLicenseImage != null)
        //        {
        //            var tradeFile = createUserDto.TradeLicenseImage;
        //            var tradeFileName = Path.GetFileNameWithoutExtension(tradeFile.FileName) + "_" + user.Id + Path.GetExtension(tradeFile.FileName);
        //            var tradeFullPath = Path.Combine(userFolder, tradeFileName);
        //            using (var stream = new FileStream(tradeFullPath, FileMode.Create))
        //            {
        //                await tradeFile.CopyToAsync(stream);
        //            }
        //            // تخزين المسار النسبي في سجل المستخدم
        //            user.TradeLicenseImage = Path.Combine("uploads/users", user.Id.ToString(), trade_license_image + Path.GetExtension(tradeFile.FileName)).Replace("\\", "/");
        //        }

        //        // حفظ التحديثات بعد رفع الملفات
        //        await _unitOfWork.SaveChangesAsync();

        //        // إنشاء التوكن
        //        _logger.LogInformation("Creating token for user: {UserName}", createUserDto.UserName);
        //        var token = _tokenService.CreateToken(user);

        //        _logger.LogInformation("User registered successfully: {UserName}", createUserDto.UserName);
        //        return Ok(new
        //        {
        //            User = new UserDto
        //            {
        //                Id = user.Id,
        //                UserName = user.UserName,
        //                FullName = user.FullName,
        //                //AccountType = user.AccountType,
        //                IdType = user.IdType,
        //                IdNumber = user.IdNumber,
        //                PhoneNumber = user.PhoneNumber,
        //                Address = user.Address,
        //                //IdImage = user.IdImage,
        //                //TradeLicenseImage = user.TradeLicenseImage
        //            },
        //            Token = token
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while registering user: {UserName}", createUserDto.UserName);
        //        return StatusCode(500, "An error occurred while processing your request. Please try again later.");
        //    }
        //}
        [HttpPost("register")]

        public async Task<ActionResult> Register([FromForm] CreateUserDto createUserDto)
        {
            _logger.LogInformation("Register method started for user: {UserName}", createUserDto.UserName);

            try
            {
                // التحقق من وجود المستخدم مسبقًا
                var existingUser = (await _unitOfWork.Users.FindAsync(u => u.UserName == createUserDto.UserName)).FirstOrDefault();
                if (existingUser != null)
                {
                    _logger.LogWarning("Username already exists: {UserName}", createUserDto.UserName);
                    return BadRequest("Username already exists");
                }

                // إنشاء المستخدم الجديد بدون اعتماد الحساب حتى يتم التحقق من OTP
                using var hmac = new HMACSHA512();
                var user = new User
                {
                    UserName = createUserDto.UserName,
                    FullName = createUserDto.FullName,
                    Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(createUserDto.Password)),
                    PasswordSalt = hmac.Key,
                    AccountType = createUserDto.AccountType,
                    IdType = createUserDto.IdType,
                    IdNumber = createUserDto.IdNumber,
                    PhoneNumber = createUserDto.PhoneNumber,
                    Address = createUserDto.Address,
                    EntryTime = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // توليد OTP عشوائي (مثلاً رمز من 6 أرقام)
                var otp = new Random().Next(100000, 999999);
                user.Otp = otp;
                user.OtpGeneratedAt = DateTime.UtcNow;

                // إرسال OTP عبر واتساب إلى رقم الهاتف المدخل
                await _whatsAppService.SendOtpAsync(user.PhoneNumber, otp.ToString());

                // حفظ التغييرات بعد إضافة OTP
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("OTP sent to WhatsApp for phone number: {PhoneNumber} for user: {UserName}", user.PhoneNumber, user.UserName);

                // إرجاع رسالة بأن OTP قد تم إرساله لتأكيد الحساب
                return Ok(new
                {
                    Message = "OTP has been sent to your WhatsApp number. Please verify the OTP to activate your account.",
                    UserId = user.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user: {UserName}", createUserDto.UserName);
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }


        [HttpPost("login")]

        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            _logger.LogInformation("Login method started for user: {UserName}", loginDto.UserName);

            try
            {
                var user = (await _unitOfWork.Users.FindAsync(u => u.UserName == loginDto.UserName)).FirstOrDefault();
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserName}", loginDto.UserName);
                    return Unauthorized("Invalid username");
                }

                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                if (!computedHash.SequenceEqual(user.Password))
                {
                    _logger.LogWarning("Invalid password for user: {UserName}", loginDto.UserName);
                    return Unauthorized("Invalid password");
                }

                var token = _tokenService.CreateToken(user);

                _logger.LogInformation("User logged in successfully: {UserName}", loginDto.UserName);
                return Ok(new
                {
                    User = new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        AccountType = user.AccountType,
                        IdType = user.IdType,
                        IdNumber = user.IdNumber,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address
                    },
                    Token = token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user: {UserName}", loginDto.UserName);
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long id))
            {
                return Unauthorized();
            }

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                AccountType = user.AccountType,
                IdType = user.IdType,
                IdNumber = user.IdNumber,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            });
        }

        //[Authorize(Roles = "Admin")] // Only admins can approve users
        [Authorize]
        [HttpPost("approve")]
        public async Task<ActionResult> ApproveUser(ApproveUserDto approveUserDto)
        {


            _logger.LogInformation("User approval process started for userId: {UserId}", approveUserDto.UserId);

            try
            {

                var user = await _unitOfWork.Users.GetByIdAsync(approveUserDto.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for approval. UserId: {UserId}", approveUserDto.UserId);
                    return NotFound("User not found");
                }

                if (user.Status != "Pending")
                {
                    _logger.LogWarning("Invalid user status for approval. UserId: {UserId}, Current Status: {Status}",
                        approveUserDto.UserId, user.Status);
                    return BadRequest($"User is already {user.Status.ToLower()}");
                }

                // Update user status
                user.Status = approveUserDto.Status;
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User status updated successfully. UserId: {UserId}, New Status: {Status}",
                    approveUserDto.UserId, approveUserDto.Status);

                return Ok(new
                {
                    Message = $"User has been {approveUserDto.Status.ToLower()} successfully",
                    User = new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        AccountType = user.AccountType,
                        IdType = user.IdType,
                        IdNumber = user.IdNumber,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address,
                        Status = user.Status
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while approving user. UserId: {UserId}", approveUserDto.UserId);
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }
    }
}
