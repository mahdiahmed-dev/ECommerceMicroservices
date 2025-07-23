namespace OrderService.Services
{
    public interface IProductApiClient
    {
        Task<bool> AreProductIdsValidAsync(List<Guid> productIds);
    }
}
