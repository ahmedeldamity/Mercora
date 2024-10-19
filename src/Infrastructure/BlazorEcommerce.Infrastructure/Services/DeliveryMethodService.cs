namespace BlazorEcommerce.Infrastructure.Services;
public class DeliveryMethodService(IUnitOfWork unitOfWork, IMapper mapper) : IDeliveryMethodService
{
    public async Task<Result<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethodsAsync()
    {
        var deliveryMethodsRepo = unitOfWork.Repository<OrderDeliveryMethod>();

        var deliveryMethods = await deliveryMethodsRepo.GetAllAsync();

        return Result.Success(deliveryMethods);
    }

    public async Task<Result<OrderDeliveryMethod>> GetDeliveryMethodByIdAsync(int id)
    {
        var deliveryMethodsRepo = unitOfWork.Repository<OrderDeliveryMethod>();

        var deliveryMethod = await deliveryMethodsRepo.GetEntityAsync(id);

        return deliveryMethod == null ? Result.Failure<OrderDeliveryMethod>(new Error(404, $"Delivery method with id {id} not found")) : Result.Success(deliveryMethod);
    }

    public async Task<Result<OrderDeliveryMethod>> CreateDeliveryMethodAsync(OrderDeliveryMethod deliveryMethod)
    {
        var deliveryMethodsRepo = unitOfWork.Repository<OrderDeliveryMethod>();

        await deliveryMethodsRepo.AddAsync(deliveryMethod);

        var result = await unitOfWork.CompleteAsync();

        return result <= 0 ? Result.Failure<OrderDeliveryMethod>(new Error(500, "Failed to create delivery method")) : Result.Success(deliveryMethod);
    }

    public async Task<Result<OrderDeliveryMethod>> UpdateDeliveryMethodAsync(int id, OrderDeliveryMethod deliveryMethod)
    {
        var deliveryMethodsRepo = unitOfWork.Repository<OrderDeliveryMethod>();

        var existingDeliveryMethod = await deliveryMethodsRepo.GetEntityAsync(id);

        if (existingDeliveryMethod == null)
            return Result.Failure<OrderDeliveryMethod>(new Error(404, $"Delivery method with id {id} not found"));

        mapper.Map(deliveryMethod, existingDeliveryMethod);

        deliveryMethodsRepo.Update(existingDeliveryMethod);

        var result = await unitOfWork.CompleteAsync();

        return result <= 0 ? Result.Failure<OrderDeliveryMethod>(new Error(500, "Failed to update delivery method")) : Result.Success(existingDeliveryMethod);
    }

    public async Task<Result<OrderDeliveryMethod>> DeleteDeliveryMethodAsync(int id)
    {
        var deliveryMethodsRepo = unitOfWork.Repository<OrderDeliveryMethod>();

        var deliveryMethod = await deliveryMethodsRepo.GetEntityAsync(id);

        if (deliveryMethod == null)
            return Result.Failure<OrderDeliveryMethod>(new Error(404, $"Delivery method with id {id} not found"));

        deliveryMethodsRepo.Delete(deliveryMethod);

        var result = await unitOfWork.CompleteAsync();

        return result <= 0 ? Result.Failure<OrderDeliveryMethod>(new Error(500, "Failed to delete delivery method")) : Result.Success(deliveryMethod);
    }

}