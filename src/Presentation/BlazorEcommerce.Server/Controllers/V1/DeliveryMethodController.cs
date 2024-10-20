namespace BlazorEcommerce.Server.Controllers.V1;
public class DeliveryMethodController(IDeliveryMethodService deliveryMethodService) : BaseController
{
    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult<IReadOnlyList<OrderDeliveryMethodModel>>> GetAllDeliveryMethods()
    {
        var result = await deliveryMethodService.GetAllDeliveryMethodsAsync();

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [Cached(600)]
    public async Task<ActionResult<OrderDeliveryMethodModel>> GetDeliveryMethodById(int id)
    {
        var result = await deliveryMethodService.GetDeliveryMethodByIdAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<ActionResult<OrderDeliveryMethodModel>> CreateDeliveryMethod(OrderDeliveryMethodModel deliveryMethod)
    {
        var result = await deliveryMethodService.CreateDeliveryMethodAsync(deliveryMethod);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderDeliveryMethodModel>> UpdateDeliveryMethod(int id, OrderDeliveryMethodModel deliveryMethod)
    {
        var result = await deliveryMethodService.UpdateDeliveryMethodAsync(id, deliveryMethod);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<OrderDeliveryMethodModel>> DeleteDeliveryMethod(int id)
    {
        var result = await deliveryMethodService.DeleteDeliveryMethodAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}