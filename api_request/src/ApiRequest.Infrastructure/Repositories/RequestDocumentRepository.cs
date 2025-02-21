using ApiRequest.Core.Entities;           // غيّر المسار حسب موقع RequestDocument
using ApiRequest.Core.Interfaces;         // غيّر المسار حسب موقع الواجهة
using ApiRequest.Infrastructure.Data;     // غيّر المسار حسب موقع AppDbContext
using Microsoft.EntityFrameworkCore;

namespace ApiRequest.Infrastructure.Repositories
{
    public class RequestDocumentRepository
        : GenericRepository<RequestDocument>, IRequestDocumentRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<RequestDocument> _dbSet;

        public RequestDocumentRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<RequestDocument>();
        }

        /// <summary>
        /// إحضار كل المستندات المرتبطة بطلب معين عن طريق requestId
        /// </summary>
        public async Task<IEnumerable<RequestDocument>> GetByRequestIdAsync(long requestId)
        {
            return await _dbSet
                .Where(d => d.RequestId == requestId)
                .ToListAsync();
        }

        /// <summary>
        /// إحضار كل المستندات التي تملك اسم معين (DocumentName)
        /// </summary>
        public async Task<IEnumerable<RequestDocument>> GetByDocumentNameAsync(string documentName)
        {
            return await _dbSet
                .Where(d => d.DocumentName == documentName)
                .ToListAsync();
        }

        /// <summary>
        /// حذف كل المستندات الخاصة بطلب معين
        /// </summary>
        public async Task DeleteByRequestIdAsync(long requestId)
        {
            var documents = await _dbSet
                .Where(d => d.RequestId == requestId)
                .ToListAsync();

            _dbSet.RemoveRange(documents);

            // الحفظ يتم عادةً في الـ UnitOfWork أو يمكنك استدعاء:
            // await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<RequestWithDocumentsDto>> GetAllWithDocumentsAsync()
        {
            // 1) جلب كل الطلبات من قاعدة البيانات مع المستندات (Eager Loading)
            var requests = await _context.Requests
                .Include(r => r.RequestDocuments) // تحميل المستندات التابعة
                .ToListAsync();

            // 2) تحويل كل كائن Request إلى RequestWithDocumentsDto
            var result = requests.Select(r => new RequestWithDocumentsDto
            {
                Id = r.Id,
                RequestNumber = r.RequestNumber,
                Amount = r.Amount,
                RequestDate = r.RequestDate,
                EntryTime = r.EntryTime,
                Notes = r.Notes,
                UserId = r.UserId,
                RequestStatus = r.RequestStatus,

                Documents = r.RequestDocuments.Select(d => new RequestDocumentDto
                {
                    Id = d.Id,
                    DocumentName = d.DocumentName,
                    AccountType = d.AccountType,
                    UploadTime = d.UploadTime,
                    DocumentPath = d.DocumentPath,
                    DocumentContent = d.DocumentContent
                }).ToList()
            });

            // 3) إعادة النتيجة (IEnumerable<RequestWithDocumentsDto>)
            return result;
        }




        // أمثلة لدوال أخرى قد تريدها:
        /*
        public async Task<RequestDocument> GetSingleDocumentAsync(long docId)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.Id == docId);
        }

        public async Task UpdateDocumentAsync(RequestDocument document)
        {
            Update(document); // موروثة من GenericRepository
            await _context.SaveChangesAsync(); 
        }
        */
    }
}
