namespace Core.Dtos;
public record OrderRequest(
    string BasketId,
    OrderAddressRequest ShippingAddress
);