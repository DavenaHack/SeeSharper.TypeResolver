using System;
using System.Collections.Generic;

namespace Mimp.SeeSharper.TypeResolver
{
    /// <summary>
    /// A delegate to resolve types from <paramref name="fullTypeName"/>.
    /// </summary>
    /// <param name="fullTypeName"></param>
    /// <param name="assembly">Resovled assembly</param>
    /// <param name="namespace">Resovled namespace</param>
    /// <param name="type">Resovled type name</param>
    /// <param name="genericTypes">Resolved generics</param>
    /// <param name="resolvedTypes">Already resolved types for <paramref name="fullTypeName"/></param>
    /// <returns></returns>
    public delegate IEnumerable<Type> ResolveTypes(string fullTypeName, string? assembly, string? @namespace, string? type, IEnumerable<Type>[] genericTypes, IEnumerable<Type> resolvedTypes);
}
