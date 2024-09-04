using AutoMapper;
using Core.Entities.OrderEntities;
using Core.ErrorHandling;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Service;
public class DeliveryMethodService(IUnitOfWork _unitOfWork, IMapper _mapper) : IDeliveryMethodService
{
    public async Task<Result<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethodsAsync()
    {
        var deliveryMethodsRepo = _unitOfWork.Repository<OrderDeliveryMethod>();

        var deliveryMethods = await deliveryMethodsRepo.GetAllAsync();

        return Result.Success(deliveryMethods);
    }

    public async Task<Result<OrderDeliveryMethod>> GetDeliveryMethodByIdAsync(int id)
    {
        var deliveryMethodsRepo = _unitOfWork.Repository<OrderDeliveryMethod>();

        var deliveryMethod = await deliveryMethodsRepo.GetEntityAsync(id);

        if (deliveryMethod == null)
            return Result.Failure<OrderDeliveryMethod>(new Error(404, $"Delivery method with id {id} not found"));

        return Result.Success(deliveryMethod);
    }

    public async Task<Result<OrderDeliveryMethod>> CreateDeliveryMethodAsync(OrderDeliveryMethod deliveryMethod)
    {
        var deliveryMethodsRepo = _unitOfWork.Repository<OrderDeliveryMethod>();

        await deliveryMethodsRepo.AddAsync(deliveryMethod);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<OrderDeliveryMethod>(new Error(500, "Failed to create delivery method"));

        return Result.Success(deliveryMethod);
    }

    public async Task<Result<OrderDeliveryMethod>> UpdateDeliveryMethodAsync(int id, OrderDeliveryMethod deliveryMethod)
    {
        var deliveryMethodsRepo = _unitOfWork.Repository<OrderDeliveryMethod>();

        var existingDeliveryMethod = await deliveryMethodsRepo.GetEntityAsync(id);

        if (existingDeliveryMethod == null)
            return Result.Failure<OrderDeliveryMethod>(new Error(404, $"Delivery method with id {id} not found"));

        _mapper.Map(deliveryMethod, existingDeliveryMethod);

        deliveryMethodsRepo.Update(existingDeliveryMethod);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<OrderDeliveryMethod>(new Error(500, "Failed to update delivery method"));

        return Result.Success(existingDeliveryMethod);
    }

    public async Task<Result<OrderDeliveryMethod>> DeleteDeliveryMethodAsync(int id)
    {
        var deliveryMethodsRepo = _unitOfWork.Repository<OrderDeliveryMethod>();

        var deliveryMethod = await deliveryMethodsRepo.GetEntityAsync(id);

        if (deliveryMethod == null)
            return Result.Failure<OrderDeliveryMethod>(new Error(404, $"Delivery method with id {id} not found"));

        deliveryMethodsRepo.Delete(deliveryMethod);

        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<OrderDeliveryMethod>(new Error(500, "Failed to delete delivery method"));

        return Result.Success(deliveryMethod);
    }

}