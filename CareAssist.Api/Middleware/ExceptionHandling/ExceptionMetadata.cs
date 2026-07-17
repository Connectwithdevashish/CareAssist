using System.Net;

namespace CareAssist.Api.Middleware.ExceptionHandling;

public sealed record ExceptionMetadata(
    HttpStatusCode StatusCode,
    string Title,
    string ProblemType);
