using PawaPayGateway.Domain.Entities;
using PawaPayGateway.Domain.Interfaces;
using PawaPayGateway.Infrastructure.Data;

namespace PawaPayGateway.Infrastructure.Implementations
{
    public class GatewayRepository : RepositoryBase<Gateway>, IGatewayRepository
    {
        public GatewayRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
