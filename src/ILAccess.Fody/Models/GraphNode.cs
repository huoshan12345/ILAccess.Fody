namespace ILAccess.Fody.Models;

public static class GraphNode
{
    public static GraphNode<T> Create<T>(T value) => new(value);
}

public class GraphNode<T>
{
    public GraphNode(T value)
    {
        Value = value;
    }
        
    public T Value { get; }
    public List<GraphNode<T>> Children { get; } = new();
}
