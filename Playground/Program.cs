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

Console.WriteLine($"Passing Use Cases: {useCases.Count(x => x)}/{useCases.Count}");


public interface IUseCase
{
    void Execute();
}
