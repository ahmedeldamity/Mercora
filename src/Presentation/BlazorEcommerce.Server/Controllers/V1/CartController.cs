namespace BlazorEcommerce.Server.Controllers.V1;
public class CartController(ICartService cartService) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CartResponse>> CreateOrUpdateCart(CartRequest cartDto)
    {
        var result = await cartService.CreateOrUpdateCartAsync(cartDto);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    public async Task<ActionResult<CartResponse>> GetCart(string id)
    {
        var result = await cartService.GetCartAsync(id);

        return Ok(result.Value);
    }

    [HttpDelete]
    public async Task DeleteCart(string id)
    {
        await cartService.DeleteCartAsync(id);
    }

}