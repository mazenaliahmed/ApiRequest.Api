using ApiRequest.Core.Entities;
using ApiRequest.Core.Interfaces;
using ApiRequest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiRequest.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // الحقول الخاصة بالمستودعات العامة
        private IGenericRepository<User> _users;
        private IGenericRepository<Request> _requests;
        private IGenericRepository<RequestAssignment> _requestAssignments;
        private IGenericRepository<RequestDocument> _requestDocuments;
        private IGenericRepository<UserToken> _userTokens;

        // الخاصية الخاصة بمستودع الـ RequestDocument المخصص
        public IRequestDocumentRepository RequestDocumentRepository { get; private set; }

        // Constructor
        public UnitOfWork(
            AppDbContext context,
            IRequestDocumentRepository requestDocumentRepository
        )
        {
            _context = context;

            // هنا نحقن الـ RequestDocumentRepository المخصص
            RequestDocumentRepository = requestDocumentRepository;
        }

        // خصائص عامة تعيد Repositories من GenericRepository عند الطلب (Lazy initialization)
        public IGenericRepository<User> Users
            => _users ??= new GenericRepository<User>(_context);

        public IGenericRepository<Request> Requests
            => _requests ??= new GenericRepository<Request>(_context);

        public IGenericRepository<RequestAssignment> RequestAssignments
            => _requestAssignments ??= new GenericRepository<RequestAssignment>(_context);

        public IGenericRepository<RequestDocument> RequestDocuments
            => _requestDocuments ??= new GenericRepository<RequestDocument>(_context);

        public IGenericRepository<UserToken> UserTokens
            => _userTokens ??= new GenericRepository<UserToken>(_context);

        // حفظ التغييرات في الكيان DbContext
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // مثال لدالة تجلب جميع الطلبات مع المستندات (من جدول Requests) إذا أردت استعمالها مباشرة في UnitOfWork
        public async Task<IEnumerable<Request>> GetAllRequestsWithDocumentsAsync()
        {
            return await _context.Requests
                .Include(r => r.RequestDocuments)
                .ToListAsync();
        }

        // للتخلص من الذاكرة
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
