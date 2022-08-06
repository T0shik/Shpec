using Microsoft.AspNetCore.Mvc;
using Shpec;
using Shpec.AspNetCore;
using Shpec.Declare;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddShpecValidator();
var app = builder.Build();

app.MapGet("/", ([FromQuery] FilterQuery query) => "Hello World!");

app.Run();


public partial class FilterQuery
{
    private Members _m => new(Property.Size, Property.Page);

    public static bool TryParse(string query, out FilterQuery filterQuery)
    {
        filterQuery = new FilterQuery();
        return true;
    }
}

public class Property
{
    public static int Size = Member<int>.Property().must(x => x > 0);
    public static int Page = Member<int>.Property().must(x => x > 0);
}