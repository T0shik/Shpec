using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class WebApp : WebApplicationFactory<Program>
{
}

[CollectionDefinition(nameof(WebAppCollection))]
public class WebAppCollection : ICollectionFixture<WebApp>
{
}

public class ValidationFailedResponse
{
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public string TraceId { get; set; }
    public IDictionary<string, string[]> Errors { get; set; }
}