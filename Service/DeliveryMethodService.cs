using Core.Entities.OrderEntities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Shared.Helpers;

namespace Service;
public class DeliveryMethodService(IUnitOfWork _unitOfWork) : IDeliveryMethodService
{
    public async Task<Result<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethodsAsync()
    {
        var deliveryMethodsRepo = _unitOfWork.Repository<OrderDeliveryMethod>();

        var deliveryMethods = await deliveryMethodsRepo.GetAllAsync();

        return Result.Success(deliveryMethods);
    }
}