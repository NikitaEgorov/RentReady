using System.Net;
using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

using TimeEntryFunction.Application;
using TimeEntryFunction.Models;

namespace TimeEntryFunction;

public class TimeEntryFunction
{
    private readonly TimeEntriesMaker timeEntriesMaker;

    public TimeEntryFunction(TimeEntriesMaker timeEntriesMaker) => this.timeEntriesMaker = timeEntriesMaker;

    [FunctionName("TimeEntryFunction")]
    [OpenApiOperation("Run")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(EntityRequest), Required = true)]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(Guid[]))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        HttpRequestMessage req,
        CancellationToken cancellationToken)
    {
        var request = await req.Content.ReadAsAsync<EntityRequest>(cancellationToken);

        request.Validate();

        var result = await this.timeEntriesMaker.MakeAsync(request.StartOn, request.EndOn, cancellationToken);

        return new OkObjectResult(result);
    }
}
