using System;
using System.Collections.Generic;

namespace Mimp.SeeSharper.TypeResolver
{
    public delegate string? ResolveTypeName(string typeName, string? assembly, string? @namespace, string? type, IEnumerable<Type>[] genericTypes);
}
