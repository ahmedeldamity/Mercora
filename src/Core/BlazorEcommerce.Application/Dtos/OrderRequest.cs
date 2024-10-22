namespace BlazorEcommerce.Application.Dtos;
public record OrderRequest(
    string CartId,
    OrderAddressRequest ShippingAddress
);