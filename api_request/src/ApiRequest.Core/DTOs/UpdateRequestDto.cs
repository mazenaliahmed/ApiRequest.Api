using Microsoft.AspNetCore.Http;

public class UpdateRequestDto
{
    public decimal? Amount { get; set; }
    public string Notes { get; set; }

    // إذا تريد تحديث الملفات أيضاً
    public List<IFormFile> File { get; set; }

    // إذا وثيقة واحدة فقط تحمل نفس الاسم لكل المرفقات
    // يمكنك أيضاً جعل DocumentName ضمن حقل لكل ملف لو أردت
    public string DocumentName { get; set; }
}
