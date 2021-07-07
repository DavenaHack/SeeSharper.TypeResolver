namespace Mimp.SeeSharper.TypeResolver
{
    /// <summary>
    /// A delegate to resolve the assembly from <paramref name="fullTypeName"/>.
    /// </summary>
    /// <param name="fullTypeName"></param>
    /// <param name="assembly">Provided assembly</param>
    /// <returns></returns>
    public delegate string? ResolveAssembly(string fullTypeName, string? assembly);
}
