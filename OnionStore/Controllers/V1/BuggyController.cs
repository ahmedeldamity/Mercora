using Core.Entities.ProductEntities;
using Core.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Repository.Store;

namespace API.Controllers.V1;
public class BuggyController(StoreContext storeContext) : BaseController
{
    [HttpGet("notfound")]
    public ActionResult<Product> GetNotFound() // not found product
    {
        return NotFound(new Error(404));
    }

    [HttpGet("badrequest")]
    public ActionResult GetBadRequest() // bad request -> Client\Front-end Send Some Things Wrong
    {
        return BadRequest(new Error(400));
    }

    [HttpGet("unauthorize")] // when we need to return Unauthorized
    public ActionResult GetUnauthorizeError(int id)
    {
        return Unauthorized(new Error(401));
    }

    [HttpGet("badrequest/{id:int}")] // bad request -> validation error, because id is int and i send string 
    public ActionResult GetBadRequest(int id)
    {
        return Ok(new Error(400));
    }

    [HttpGet("servererror")] // server error = exception [null reference exception]
    public ActionResult GetServerError()
    {
        var product = storeContext.Products.Find(100);
        var productToReturn = product!.ToString();
        return Ok(productToReturn);
    }

    // Error Not Fount End Point
    // we use it when (Client access end point not exist | this end point must access it using token)

}