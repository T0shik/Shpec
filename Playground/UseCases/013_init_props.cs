﻿using Shpec;

namespace Playground.UseCases;

public partial class init_props : IUseCase
{
    Members _p => new(
        Props.Name,
        Props.Score,
        Props.Active
    );

    public void Execute()
    {
        var r = new init_props("Foo", 5, true);
        Console.WriteLine(r);
    }
}

public class Props
{
    public static string Name => Declare._property(immutable: true);
    public static int Score => Declare._property(immutable: true);
    public static bool Active => Declare._property(immutable: true);
}