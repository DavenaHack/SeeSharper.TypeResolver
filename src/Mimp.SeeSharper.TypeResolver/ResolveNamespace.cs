namespace Mimp.SeeSharper.TypeResolver
{
    /// <summary>
    /// A delegate to resolve the namespace from <paramref name="fullTypeName"/>.
    /// </summary>
    /// <param name="fullTypeName"></param>
    /// <param name="assembly">Resolved assembly</param>
    /// <param name="namespace">Provided namespace</param>
    /// <returns></returns>
    public delegate string? ResolveNamespace(string fullTypeName, string? assembly, string? @namespace);
}
