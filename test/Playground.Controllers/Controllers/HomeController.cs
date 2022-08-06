using Microsoft.AspNetCore.Mvc;
using Shpec;
using Shpec.Declare;

namespace Playground.Controllers.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet("/get")]
    public string TestGetValidation([FromQuery] Query query) => "Hello World";
    
    [HttpPost("/post")]
    public string TestPostValidation([FromBody] Query query) => "Hello World";
}


public partial class Query
{
    private Members _m => new(Property.Size, Property.Page);
}

public class Property
{
    public static int Size = Member<int>.Property().must(x => x > 0);
    public static int Page = Member<int>.Property().must(x => x > 0);
}