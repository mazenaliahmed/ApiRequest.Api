using ApiRequest.Core.Entities;

namespace ApiRequest.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Request> Requests { get; }
        IGenericRepository<RequestAssignment> RequestAssignments { get; }
        IGenericRepository<RequestDocument> RequestDocuments { get; }
        IGenericRepository<UserToken> UserTokens { get; }
        IRequestDocumentRepository RequestDocumentRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
