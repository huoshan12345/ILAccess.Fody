namespace ILAccess.Example;

public static class ListAccessors
{
    [ILAccessor(ILAccessorKind.Field, Name = "_items")]
    public static extern ref T[] Items<T>(this List<T> obj);

    [ILAccessor(ILAccessorKind.Method, Name = "Grow")]
    public static extern void Grow<T>(this List<T> obj, int capacity);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern List<T> Ctor<T>();

    public static void Add<T>(this List<T> obj, T item)
    {
        obj.Add(item);
    }

    public static void Test()
    {
        var list = Ctor<string>();
        list.Add("xxxxxxxxxxx");
        Console.WriteLine($"List[0]: {list[0]}");

        ref var items = ref list.Items();
        items[0] = "yyyyyy";
        Console.WriteLine($"List[0] after set items: {list[0]}");

        Console.WriteLine($"Capacity: {list.Capacity}");
        list.Grow(100);
        Console.WriteLine($"Capacity after Grow: {list.Capacity}");
    }

    public static void TestGeneric()
    {
        var list = ListAccessors<string>.Ctor();
        list.Add("xxxxxxxxxxx");
        Console.WriteLine($"List[0]: {list[0]}");

        ref var items = ref ListAccessors<string>.Items(list);
        items[0] = "yyyyyy";
        Console.WriteLine($"List[0] after set items: {list[0]}");

        Console.WriteLine($"Capacity: {list.Capacity}");
        ListAccessors<string>.Grow(list, 100);
        Console.WriteLine($"Capacity after Grow: {list.Capacity}");
    }
}

public static class ListAccessors<T>
{
    [ILAccessor(ILAccessorKind.Field, Name = "_items")]
    public static extern ref T[] Items(List<T> obj);

    [ILAccessor(ILAccessorKind.Method, Name = "Grow")]
    public static extern void Grow(List<T> obj, int capacity);

    [ILAccessor(ILAccessorKind.Constructor)]
    public static extern List<T> Ctor();
}