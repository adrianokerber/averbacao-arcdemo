using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AverbacaoService.shared;

public sealed class HttpResponseFactory(IHttpContextAccessor httpContextAccessor) : IService<HttpResponseFactory>
{
    public IResult CreateSuccess200(object data) =>
        Results.Ok(new
        {
            Status = StatusCodes.Status200OK,
            Title = "Ok",
            Data = data
        });

    public IResult CreateError400(string title, string details) =>
        Results.BadRequest(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = title,
            Type = "Failure",
            Detail = details,
            Instance = httpContextAccessor.HttpContext!.Request.Path
        });
    
    public IResult CreateError500(string details) =>
        Results.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal server error",
            Type = "Critical",
            Detail = details,
            Instance = httpContextAccessor.HttpContext!.Request.Path
        });
}