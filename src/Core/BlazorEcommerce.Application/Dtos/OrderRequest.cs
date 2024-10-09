namespace BlazorEcommerce.Application.Dtos;
public record OrderRequest(
    string BasketId,
    OrderAddressRequest ShippingAddress
);