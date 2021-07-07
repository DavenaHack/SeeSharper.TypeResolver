using System;
using System.Collections.Generic;

namespace Mimp.SeeSharper.TypeResolver
{
    /// <summary>
    /// A delegate to resolve the type name from <paramref name="fullTypeName"/>.
    /// </summary>
    /// <param name="fullTypeName"></param>
    /// <param name="assembly">Resolved assembly</param>
    /// <param name="namespace">Resolved namespace</param>
    /// <param name="type">Provided type name</param>
    /// <param name="genericTypes">Resolved generic types</param>
    /// <returns></returns>
    public delegate string? ResolveTypeName(string fullTypeName, string? assembly, string? @namespace, string? type, IEnumerable<Type>[] genericTypes);
}
