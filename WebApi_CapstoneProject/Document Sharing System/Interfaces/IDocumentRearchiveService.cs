namespace DSS.Interfaces
{
    public interface IDocumentRearchiveService
    {
        Task RearchiveExpiredDocumentsAsync();
    }
} 