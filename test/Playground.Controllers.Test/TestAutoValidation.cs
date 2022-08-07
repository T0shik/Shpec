using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Playground.Controllers.Controllers;
using Xunit;

[Collection(nameof(WebAppCollection))]
public class TestAutoValidation
{
    private readonly WebApp _app;

    public TestAutoValidation(WebApp app)
    {
        _app = app;
    }

    [Fact]
    public async Task GetFromQuery()
    {
        var client = _app.CreateClient();
        
        var response = await client.GetAsync("/get");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ValidationFailedResponse>();

        Assert.NotNull(content);
        Assert.Equal(400, content.Status);
        Assert.Contains("Page", content.Errors);
        Assert.Contains("Size", content.Errors);
    }

    [Fact]
    public async Task GetFromQueryPartial()
    {
        var client = _app.CreateClient();
        
        var response = await client.GetAsync("/get?page=5");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ValidationFailedResponse>();

        Assert.NotNull(content);
        Assert.Equal(400, content.Status);
        Assert.Contains("Size", content.Errors);
    }

    [Fact]
    public async Task PostFromBody()
    {
        var client = _app.CreateClient();
        
        var response = await client.PostAsJsonAsync("/post", new { });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ValidationFailedResponse>();

        Assert.NotNull(content);
        Assert.Equal(400, content.Status);
        Assert.Contains("Page", content.Errors);
        Assert.Contains("Size", content.Errors);
    }

    [Fact]
    public async Task PostFromBodyPartial()
    {
        var client = _app.CreateClient();

        var response = await client.PostAsJsonAsync("/post", new Query() { Page = 5 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ValidationFailedResponse>();

        Assert.NotNull(content);
        Assert.Equal(400, content.Status);
        Assert.Contains("Size", content.Errors);
    }
}