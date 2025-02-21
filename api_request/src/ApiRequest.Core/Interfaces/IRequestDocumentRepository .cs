using ApiRequest.Core.Entities;  // غيّر المسار حسب مكان وجود كلاس RequestDocument

namespace ApiRequest.Core.Interfaces
{
    public interface IRequestDocumentRepository : IGenericRepository<RequestDocument>
    {
        /// <summary>
        /// إحضار كل المستندات المرتبطة بطلب معين عن طريق requestId
        /// </summary>
        Task<IEnumerable<RequestDocument>> GetByRequestIdAsync(long requestId);

        /// <summary>
        /// إحضار كل المستندات التي تملك اسم معين (DocumentName)
        /// </summary>
        Task<IEnumerable<RequestDocument>> GetByDocumentNameAsync(string documentName);
        Task<IEnumerable<RequestWithDocumentsDto>> GetAllWithDocumentsAsync();


        /// <summary>
        /// حذف كل المستندات الخاصة بطلب معين
        /// </summary>
        Task DeleteByRequestIdAsync(long requestId);

        // يمكنك إضافة دوال أخرى حسب الحاجة
        // Example:
        // Task<RequestDocument> GetSingleDocumentAsync(long docId);
        // Task UpdateDocumentAsync(RequestDocument document);
    }
}
