var useCases = typeof(IUseCase).Assembly.GetTypes()
    .Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Contains(typeof(IUseCase)))
    .Select(x =>
    {
        try
        {
            Console.WriteLine($"running: {x.Name}");
            (Activator.CreateInstance(x) as IUseCase).Execute();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($" --- {x.Name} --- ");
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        return false;
    })
    .ToList();

var total = useCases.Count;
var success = useCases.Count(x => x);
Console.WriteLine($"Passing Use Cases: {success}/{total}");
var failures = total - success;
if (failures > 0)
{
    throw new($"Tests failed with {failures} failures.");
}

public interface IUseCase
{
    void Execute();
}

// todo: make sure to delete this:
public class Parent
{
    private Child _ch1Ld;

    public Child Ch1ld
    {
        get => _ch1Ld;
        set
        {
            _ch1Ld = value;
            _ch1Ld.Parent = this;
        }
    }

    public class Child
    {
        public Parent Parent { get; set; }
    }
}