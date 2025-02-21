using ApiRequest.Core.DTOs;
using ApiRequest.Core.Entities;
using ApiRequest.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiRequest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;


        public RequestsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //[HttpPost]
        //public async Task<ActionResult<RequestDto>> CreateRequest(CreateRequestDto createRequestDto)
        //{
        //    // In a real application, get the userId from the authenticated user's claims
        //    var userId = 1; // Temporary for demo

        //    var request = new Request
        //    {
        //        RequestNumber = GenerateRequestNumber(),
        //        Amount = createRequestDto.Amount,
        //        RequestDate = DateTime.UtcNow,
        //        EntryTime = DateTime.UtcNow,
        //        Notes = createRequestDto.Notes,
        //        UserId = userId,
        //        RequestStatus = "Pending"
        //    };

        //    await _unitOfWork.Requests.AddAsync(request);
        //    await _unitOfWork.SaveChangesAsync();

        //    if (createRequestDto.Documents != null && createRequestDto.Documents.Any())
        //    {
        //        foreach (var doc in createRequestDto.Documents)
        //        {
        //            var requestDocument = new RequestDocument
        //            {
        //                DocumentName = doc.DocumentName,
        //                RequestId = request.Id,
        //                AccountType = doc.AccountType,
        //                UploadTime = DateTime.UtcNow,
        //                DocumentContent = doc.DocumentContent
        //            };
        //            await _unitOfWork.RequestDocuments.AddAsync(requestDocument);
        //        }
        //        await _unitOfWork.SaveChangesAsync();
        //    }

        //    return Ok(new RequestDto
        //    {
        //        Id = request.Id,
        //        RequestNumber = request.RequestNumber,
        //        Amount = request.Amount,
        //        RequestDate = request.RequestDate,
        //        Notes = request.Notes,
        //        RequestStatus = request.RequestStatus,
        //        UserId = request.UserId
        //    });
        //}


        //[HttpPost]
        //public async Task<ActionResult<RequestDto>> CreateRequest(CreateRequestDto createRequestDto)
        //{
        //    // في تطبيق حقيقي؛ اجلب userId من Claims
        //    var userId = 1; // مؤقتًا لأغراض الاختبار

        //    var request = new Request
        //    {
        //        RequestNumber = GenerateRequestNumber(),
        //        Amount = createRequestDto.Amount,
        //        RequestDate = DateTime.UtcNow,
        //        EntryTime = DateTime.UtcNow,
        //        Notes = createRequestDto.Notes,
        //        UserId = userId,
        //        RequestStatus = "Pending"
        //    };

        //    // 1) إضافة طلب جديد في الداتا
        //    await _unitOfWork.Requests.AddAsync(request);
        //    // 2) حفظ التغييرات للحصول على request.Id
        //    await _unitOfWork.SaveChangesAsync();

        //    // 3) معالجة الملفات المرفقة
        //    if (createRequestDto.Documents != null && createRequestDto.Documents.Any())
        //    {
        //        // سننشئ مجلد فرعي باسم userId داخل مجلد "Uploads" مثلاً
        //        // يمكنك تغييره إلى أي اسم يناسبك.
        //        var userFolder = Path.Combine(_env.ContentRootPath, "Uploads", userId.ToString());
        //        // ننشئ المجلد إذا لم يكن موجودًا
        //        Directory.CreateDirectory(userFolder);

        //        foreach (var doc in createRequestDto.Documents)
        //        {
        //            // افترض أننا نريد تسمية الملف باسم doc.DocumentName (أو توليد اسم عشوائي)
        //            // إذا DocumentName يحتوي على امتداد، اتركه كما هو
        //            var fileName = doc.DocumentName;
        //            var fullPath = Path.Combine(userFolder, fileName);

        //            // إذا كان DocumentContent عبارة عن مصفوفة بايت مباشرة
        //            // نقوم ببساطة بكتابتها:
        //            await System.IO.File.WriteAllBytesAsync(fullPath, doc.DocumentContent);

        //            // إذا كانت DocumentContent نص Base64:
        //            // var fileBytes = Convert.FromBase64String(doc.DocumentContentBase64);
        //            // await System.IO.File.WriteAllBytesAsync(fullPath, fileBytes);

        //            // إنشاء سجل في RequestDocument يشير لمسار الملف بدلاً من المحتوى
        //            var requestDocument = new RequestDocument
        //            {
        //                DocumentName = fileName,    // الاسم الأصلي
        //                RequestId = request.Id,
        //                AccountType = doc.AccountType,
        //                UploadTime = DateTime.UtcNow,
        //                DocumentContent = Path.Combine("Uploads", userId.ToString(), fileName) // مسار نسبي
        //            };

        //            await _unitOfWork.RequestDocuments.AddAsync(requestDocument);
        //        }
        //        await _unitOfWork.SaveChangesAsync();
        //    }

        //    return Ok(new RequestDto
        //    {
        //        Id = request.Id,
        //        RequestNumber = request.RequestNumber,
        //        Amount = request.Amount,
        //        RequestDate = request.RequestDate,
        //        Notes = request.Notes,
        //        RequestStatus = request.RequestStatus,
        //        UserId = request.UserId
        //    });
        //}





        //[Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateRequest([FromForm] CreateRequestDto createRequestDto)
        {
            // تأكد من تسجيل الدخول والحصول على userId
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                // في حال لم يتم الحصول على userId، يمكنك إعادة خطأ
                var errorResponse = new StandardApiResponse<object>
                {
                    Success = false,
                    Message = "User not authenticated.",
                    Data = null,
                    Errors = "No user Id found in token/claims."
                };
                return Unauthorized(errorResponse);
            }

            long userId = long.Parse(userIdString);

            // 1) أنشئ كائن الطلب واحفظه
            var request = new Request
            {
                RequestNumber = GenerateRequestNumber(), // الدالة الخاصة بك لتوليد رقم الطلب
                Amount = createRequestDto.Amount,
                RequestDate = DateTime.UtcNow,
                EntryTime = DateTime.UtcNow,
                Notes = createRequestDto.Notes,
                UserId = userId,
                RequestStatus = "Pending"
            };

            await _unitOfWork.Requests.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            // 2) حفظ الملفات المرفوعة إن وجدت
            if (createRequestDto.File != null && createRequestDto.File.Any())
            {
                foreach (var doc in createRequestDto.File)
                {
                    if (doc != null && doc.Length > 0)
                    {
                        // تجهيز اسم المجلد
                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(doc.FileName);
                        var folderName = $"{fileNameWithoutExt}_{request.RequestNumber}";

                        // مسار المجلد الكامل داخل wwwroot/Uploads
                        var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads/Request", request.RequestNumber, folderName);

                        if (!Directory.Exists(uploadsRoot))
                            Directory.CreateDirectory(uploadsRoot);

                        // تجهيز اسم الملف النهائي
                        var fileExtension = Path.GetExtension(doc.FileName);
                        var finalFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var finalPath = Path.Combine(uploadsRoot, finalFileName);

                        // نسخ الملف
                        using (var stream = new FileStream(finalPath, FileMode.Create))
                        {
                            await doc.CopyToAsync(stream);
                        }

                        // حفظ بيانات المستند
                        var requestDocument = new RequestDocument
                        {
                            DocumentName = createRequestDto.DocumentName,
                            AccountType = "test", // أو حقول أخرى...
                            UploadTime = DateTime.UtcNow,
                            RequestId = request.Id,
                            DocumentContent = "test",
                            DocumentPath = Path.Combine("Uploads/Request", request.RequestNumber, folderName, finalFileName)
                        };

                        await _unitOfWork.RequestDocuments.AddAsync(requestDocument);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }

            // 3) تكوين كائن استجابة يعرض بيانات الـ Request
            var responseData = new
            {
                requestId = request.Id,
                requestNumber = request.RequestNumber,
                amount = request.Amount,
                requestDate = request.RequestDate,
                entryTime = request.EntryTime,
                notes = request.Notes,
                userId = request.UserId,
                requestStatus = request.RequestStatus
            };

            // 4) بناء استجابة JSON قياسية
            var response = new StandardApiResponse<object>
            {
                Success = true,
                Message = "Request created successfully.",
                Data = responseData,
                Errors = null
            };

            return Ok(response);
        }


        //[Authorize]
        //[HttpPut("update/{id}")]
        //public async Task<ActionResult> UpdateRequest(long id, [FromForm] UpdateRequestDto updateRequestDto)
        //{
        //    // 1) تأكد من تسجيل الدخول والحصول على userId
        //    var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userIdString))
        //    {
        //        var errorResponse = new StandardApiResponse<object>
        //        {
        //            Success = false,
        //            Message = "User not authenticated.",
        //            Data = null,
        //            Errors = "No user Id found in token/claims."
        //        };
        //        return Unauthorized(errorResponse);
        //    }
        //    long userId = long.Parse(userIdString);

        //    // 2) ابحث عن الطلب المطلوب تحديثه
        //    var request = await _unitOfWork.Requests.GetByIdAsync(id);
        //    if (request == null)
        //    {
        //        var notFoundResponse = new StandardApiResponse<object>
        //        {
        //            Success = false,
        //            Message = $"الطلب ذي الرقم {id} غير موجود.",
        //            Data = null,
        //            Errors = null
        //        };
        //        return NotFound(notFoundResponse);
        //    }

        //    // تحقق أن صاحب الطلب هو نفسه المستخدم الحالي
        //    if (request.UserId != userId)
        //    {
        //        var unauthorizedResponse = new StandardApiResponse<object>
        //        {
        //            Success = false,
        //            Message = "لا تملك الصلاحية لتعديل هذا الطلب.",
        //            Data = null,
        //            Errors = null
        //        };
        //        return Forbid(); // أو يمكنك إرجاع Unauthorized
        //    }

        //    // 3) تحديث الحقول المطلوبة (إذا لم تكن null أو فارغة)
        //    if (updateRequestDto.Amount.HasValue)
        //        request.Amount = updateRequestDto.Amount.Value;

        //    if (!string.IsNullOrEmpty(updateRequestDto.Notes))
        //        request.Notes = updateRequestDto.Notes;

        //    // إذا أردت تحديث حالة الطلب أو حقول أخرى، يمكنك فعل ذلك هنا
        //    // request.RequestStatus = ...

        //    _unitOfWork.Requests.Update(request);

        //    // 4) التحقق من الملفات وإضافتها إن وجدت
        //    if (updateRequestDto.File != null && updateRequestDto.File.Any())
        //    {
        //        // (أ) التحقق من عدد الملفات (مثال حصرها بـ10)
        //        int maxFilesCount = 10;
        //        if (updateRequestDto.File.Count > maxFilesCount)
        //        {
        //            var filesCountError = new StandardApiResponse<object>
        //            {
        //                Success = false,
        //                Message = $"لا يمكنك رفع أكثر من {maxFilesCount} ملف في نفس العملية.",
        //                Data = null,
        //                Errors = null
        //            };
        //            return BadRequest(filesCountError);
        //        }

        //        // (ب) التحقق من الامتدادات المسموحة
        //        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        //        // (ج) حجم الملف الأقصى (5 MB)
        //        long maxFileSize = 5 * 1024 * 1024; // 5 MB

        //        foreach (var doc in updateRequestDto.File)
        //        {
        //            // التحقق من وجود محتوى فعلي
        //            if (doc is { Length: > 0 })
        //            {
        //                var fileExtension = Path.GetExtension(doc.FileName).ToLower();

        //                // تحقق من الامتداد
        //                if (!allowedExtensions.Contains(fileExtension))
        //                {
        //                    var extensionErrorResponse = new StandardApiResponse<object>
        //                    {
        //                        Success = false,
        //                        Message = $"امتداد الملف '{fileExtension}' غير مسموح. الامتدادات المسموحة: {string.Join(", ", allowedExtensions)}",
        //                        Data = null,
        //                        Errors = null
        //                    };
        //                    return BadRequest(extensionErrorResponse);
        //                }

        //                // تحقق من الحجم
        //                if (doc.Length > maxFileSize)
        //                {
        //                    var sizeErrorResponse = new StandardApiResponse<object>
        //                    {
        //                        Success = false,
        //                        Message = $"حجم الملف أكبر من الحد الأقصى {maxFileSize / (1024 * 1024)} MB.",
        //                        Data = null,
        //                        Errors = null
        //                    };
        //                    return BadRequest(sizeErrorResponse);
        //                }

        //                // تابع عملية حفظ الملف كما في create
        //                // تجهيز اسم المجلد
        //                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(doc.FileName);
        //                var folderName = $"{fileNameWithoutExt}_{userId}";
        //                var uploadsRoot = Path.Combine(
        //                    Directory.GetCurrentDirectory(),
        //                    "wwwroot", "Uploads", folderName
        //                );

        //                if (!Directory.Exists(uploadsRoot))
        //                    Directory.CreateDirectory(uploadsRoot);

        //                var finalFileName = $"{Guid.NewGuid()}{fileExtension}";
        //                var finalPath = Path.Combine(uploadsRoot, finalFileName);

        //                using (var stream = new FileStream(finalPath, FileMode.Create))
        //                {
        //                    await doc.CopyToAsync(stream);
        //                }

        //                // حفظ بيانات المستند في قاعدة البيانات
        //                var requestDocument = new RequestDocument
        //                {
        //                    DocumentName = !string.IsNullOrEmpty(updateRequestDto.DocumentName)
        //                        ? updateRequestDto.DocumentName
        //                        : doc.FileName,
        //                    AccountType = "test", // أو أي قيم أخرى
        //                    UploadTime = DateTime.UtcNow,
        //                    RequestId = request.Id,
        //                    DocumentContent = "test", // أو محتوى حقيقي
        //                    DocumentPath = Path.Combine("Uploads", folderName, finalFileName)
        //                };

        //                await _unitOfWork.RequestDocuments.AddAsync(requestDocument);
        //            }
        //        }
        //    }

        //    // 5) حفظ التعديلات
        //    await _unitOfWork.SaveChangesAsync();

        //    // 6) بناء استجابة (JSON)
        //    var responseData = new
        //    {
        //        requestId = request.Id,
        //        requestNumber = request.RequestNumber,
        //        amount = request.Amount,
        //        requestDate = request.RequestDate,
        //        entryTime = request.EntryTime,
        //        notes = request.Notes,
        //        userId = request.UserId,
        //        requestStatus = request.RequestStatus
        //    };

        //    var response = new StandardApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "تم تحديث الطلب بنجاح.",
        //        Data = responseData,
        //        Errors = null
        //    };

        //    return Ok(response);
        //}

        // DTO يضم ملفّات جديدة + قائمة بالملفات المطلوب حذفها (IDs)
        public class UpdateRequestDto
        {
            public decimal? Amount { get; set; }
            public string Notes { get; set; }
            public List<IFormFile> NewFiles { get; set; }
            public List<long> FilesToDelete { get; set; }  // IDs للملفات المطلوب حذفها
                                                           // ... حقول أخرى
        }

        [HttpPut("UpdateRequest/{id}")]
        public async Task<IActionResult> UpdateRequest(long id, [FromForm] UpdateRequestDto dto)
        {
            // 1) التحقق من وجود الطلب
            var request = await _unitOfWork.Requests.GetByIdAsync(id);
            if (request == null)
                return NotFound(new { message = $"Request with ID={id} not found." });

            // 2) تعديل الحقول الأساسية
            if (dto.Amount.HasValue)
                request.Amount = dto.Amount.Value;
            if (!string.IsNullOrEmpty(dto.Notes))
                request.Notes = dto.Notes;
            // ... تعديل حقول أخرى لو أردت
            _unitOfWork.Requests.Update(request);

            // 3) حذف الملفات المطلوبة
            if (dto.FilesToDelete != null && dto.FilesToDelete.Any())
            {
                var docsToDelete = await _unitOfWork.RequestDocuments
                    .FindAsync(d => dto.FilesToDelete.Contains(d.Id) && d.RequestId == id);

                // حذف الملفات من DB
                _unitOfWork.RequestDocuments.RemoveRange(docsToDelete);

                // وأيضًا حذفها من القرص (إن أردت)
                foreach (var doc in docsToDelete)
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.DocumentPath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }

            // 4) إضافة الملفات الجديدة (إن وجدت)
            if (dto.NewFiles != null && dto.NewFiles.Any())
            {
                foreach (var newFile in dto.NewFiles)
                {
                    if (newFile is { Length: > 0 })
                    {
                        // ... نفس منطق الحفظ كما في الدوال السابقة (Create)
                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(newFile.FileName);
                        var folderName = $"{fileNameWithoutExt}_{request.UserId}";
                        var uploadsRoot = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot", "Uploads", folderName);

                        if (!Directory.Exists(uploadsRoot))
                            Directory.CreateDirectory(uploadsRoot);

                        var fileExtension = Path.GetExtension(newFile.FileName);
                        var finalFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var finalPath = Path.Combine(uploadsRoot, finalFileName);

                        using (var stream = new FileStream(finalPath, FileMode.Create))
                        {
                            await newFile.CopyToAsync(stream);
                        }

                        var requestDocument = new RequestDocument
                        {
                            DocumentName = newFile.FileName,
                            AccountType = "test", // ...
                            UploadTime = DateTime.UtcNow,
                            RequestId = request.Id,
                            DocumentContent = "test",
                            DocumentPath = Path.Combine("Uploads", folderName, finalFileName)
                        };
                        await _unitOfWork.RequestDocuments.AddAsync(requestDocument);
                    }
                }
            }

            // 5) الحفظ
            await _unitOfWork.SaveChangesAsync();

            // 6) إعادة بيانات محدثة أو رسالة نجاح
            return Ok(new { message = "Request updated successfully." });
        }


        [HttpGet("GetAllRequests")]
        public async Task<ActionResult> GetAllRequests()
        {
            try
            {
                // استدعاء الدالة التي تضمن جلب المستندات أيضًا
                var requests = await _unitOfWork.RequestDocumentRepository.GetAllWithDocumentsAsync();

                // بناء النتيجة
                var resultData = requests.Select(request => new
                {
                    requestId = request.Id,
                    requestNumber = request.RequestNumber,
                    amount = request.Amount,
                    requestDate = request.RequestDate,
                    entryTime = request.EntryTime,
                    notes = request.Notes,
                    userId = request.UserId,
                    requestStatus = request.RequestStatus,
                    documents = request.Documents.Select(doc => new
                    {
                        documentId = doc.Id,
                        documentName = doc.DocumentName,
                        accountType = doc.AccountType,
                        uploadTime = doc.UploadTime,
                        documentContent = doc.DocumentContent,
                        documentPath = doc.DocumentPath
                    }).ToList()
                });

                var response = new StandardApiResponse<object>
                {
                    Success = true,
                    Message = "Requests retrieved successfully (with Eager Loading).",
                    Data = resultData,
                    Errors = null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new StandardApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving requests.",
                    Data = null,
                    Errors = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }


        [HttpGet("docment-by-requestId/{requestId}")]
        public async Task<IActionResult> GetByRequestId(long requestId)
        {
            try
            {
                // استدعاء الدالة في RequestDocumentRepository عبر وحدة العمل
                var documents = await _unitOfWork.RequestDocumentRepository.GetByRequestIdAsync(requestId);

                // التحقق إن كانت القائمة فارغة
                if (!documents.Any())
                {
                    var noDocsResponse = new StandardApiResponse<object>
                    {
                        Success = false,
                        Message = $"لا توجد ملفات خاصة بالطلب ذي الرقم {requestId}.",
                        Data = null,
                        Errors = null
                    };
                    // يمكنك اختيار: return Ok(noDocsResponse) أو return NotFound(noDocsResponse)
                    return Ok(noDocsResponse);
                }

                // إن وجدنا مستندات، نحولها (إن أحببت) أو نعيدها كما هي
                var result = documents.Select(doc => new
                {
                    DocumentId = doc.Id,
                    DocumentName = doc.DocumentName,
                    AccountType = doc.AccountType,
                    UploadTime = doc.UploadTime,
                    DocumentContent = doc.DocumentContent,
                    DocumentPath = doc.DocumentPath
                });

                var response = new StandardApiResponse<object>
                {
                    Success = true,
                    Message = $"تم جلب الملفات الخاصة بالطلب ذي الرقم {requestId} بنجاح.",
                    Data = result,
                    Errors = null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new StandardApiResponse<object>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء جلب الملفات الخاصة بالطلب.",
                    Data = null,
                    Errors = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }




        //public async Task<ActionResult> CreateRequest([FromForm] CreateRequestDto createRequestDto)
        //{
        //    // مثال: الحصول على userId (هنا ثابت لأغراض الشرح)
        //    //var userId = 2;
        //    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        //    // 1) أنشئ كائن الطلب (Request) واحفظه
        //    var request = new Request
        //    {
        //        // RequestNumber أي طريقة لتوليده
        //        RequestNumber = GenerateRequestNumber(),
        //        Amount = createRequestDto.Amount,
        //        RequestDate = DateTime.UtcNow,
        //        EntryTime = DateTime.UtcNow,
        //        Notes = createRequestDto.Notes,
        //        UserId = long.Parse(userId),
        //        RequestStatus = "Pending"
        //    };

        //    await _unitOfWork.Requests.AddAsync(request);
        //    await _unitOfWork.SaveChangesAsync();

        //    // 2) معالجة الملفات المرفوعة
        //    if (createRequestDto.File != null && createRequestDto.File.Any())
        //    {
        //        foreach (var doc in createRequestDto.File)
        //        {
        //            // (أ) تأكد أن ملف الـ IFormFile ليس فارغًا
        //            if (doc != null && doc.Length > 0)
        //            {
        //                // تجهيز اسم المجلد. كمثال:
        //                // نفصل اسم الملف عن الامتداد، ونعمل مجلد باسم "filename_without_ext_userId"
        //                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(doc.FileName);
        //                var folderName = $"{fileNameWithoutExt}_{userId}";

        //                // مسار المجلد الكامل داخل wwwroot/Uploads
        //                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", folderName);

        //                // إن لم يكن المجلد موجودًا، فقم بإنشائه
        //                if (!Directory.Exists(uploadsRoot))
        //                    Directory.CreateDirectory(uploadsRoot);

        //                // (ب) تجهيز اسم الملف النهائي
        //                var fileExtension = Path.GetExtension(doc.FileName); // .pdf, .jpg, etc.
        //                var finalFileName = $"{Guid.NewGuid()}{fileExtension}";  // اسم فريد لتجنب التكرار
        //                var finalPath = Path.Combine(uploadsRoot, finalFileName);

        //                // (ج) نسخ/حفظ الملف في المسار النهائي
        //                using (var stream = new FileStream(finalPath, FileMode.Create))
        //                {
        //                    await doc.CopyToAsync(stream);
        //                }

        //                // (د) حفظ بيانات المستند في قاعدة البيانات
        //                var requestDocument = new RequestDocument
        //                {
        //                    DocumentName = createRequestDto.DocumentName,
        //                    AccountType = "test",
        //                    UploadTime = DateTime.UtcNow,
        //                    RequestId = request.Id,
        //                    DocumentContent = "test",

        //                    // المسار النسبي
        //                    DocumentPath = Path.Combine("Uploads", folderName, finalFileName)
        //                };

        //                await _unitOfWork.RequestDocuments.AddAsync(requestDocument);
        //            }
        //        }

        //        await _unitOfWork.SaveChangesAsync();
        //    }

        //    // 3) إرجاع النتيجة أو كائن DTO معيّن
        //    return Ok(new { message = "Request created successfully." });
        //}

        [HttpPost("assign")]
        public async Task<ActionResult> AssignRequest(AssignRequestDto assignRequestDto)
        {
            var request = await _unitOfWork.Requests.GetByIdAsync(assignRequestDto.RequestId);
            if (request == null)
                return NotFound("Request not found");

            var assignment = new RequestAssignment
            {
                RequestId = assignRequestDto.RequestId,
                ServiceProviderId = assignRequestDto.ServiceProviderId,
                ExecutionStatus = "Assigned"
            };

            await _unitOfWork.RequestAssignments.AddAsync(assignment);
            request.RequestStatus = "Assigned";
            _unitOfWork.Requests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateRequestStatus(long id, UpdateRequestStatusDto updateStatusDto)
        {
            var assignment = (await _unitOfWork.RequestAssignments.FindAsync(ra => ra.RequestId == id)).FirstOrDefault();
            if (assignment == null)
                return NotFound("Request assignment not found");

            assignment.ExecutionStatus = updateStatusDto.ExecutionStatus;
            _unitOfWork.RequestAssignments.Update(assignment);

            var request = await _unitOfWork.Requests.GetByIdAsync(id);
            request.RequestStatus = updateStatusDto.ExecutionStatus;
            _unitOfWork.Requests.Update(request);

            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("trader")]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetTraderRequests()
        {
            // In a real application, get the traderId from the authenticated user's claims
            var traderId = 1; // Temporary for demo

            var requests = await _unitOfWork.Requests.FindAsync(r => r.UserId == traderId);
            return Ok(requests.Select(r => new RequestDto
            {
                Id = r.Id,
                RequestNumber = r.RequestNumber,
                Amount = r.Amount,
                RequestDate = r.RequestDate,
                Notes = r.Notes,
                RequestStatus = r.RequestStatus,
                UserId = r.UserId
            }));
        }

        [HttpGet("service-provider")]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetServiceProviderRequests()
        {
            // In a real application, get the serviceProviderId from the authenticated user's claims
            var serviceProviderId = 2; // Temporary for demo

            var assignments = await _unitOfWork.RequestAssignments.FindAsync(ra => ra.ServiceProviderId == serviceProviderId);
            var requestIds = assignments.Select(a => a.RequestId);
            var requests = await _unitOfWork.Requests.FindAsync(r => requestIds.Contains(r.Id));

            return Ok(requests.Select(r => new RequestDto
            {
                Id = r.Id,
                RequestNumber = r.RequestNumber,
                Amount = r.Amount,
                RequestDate = r.RequestDate,
                Notes = r.Notes,
                RequestStatus = r.RequestStatus,
                UserId = r.UserId
            }));
        }

        private string GenerateRequestNumber()
        {
            return $"REQ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }
    }
}
