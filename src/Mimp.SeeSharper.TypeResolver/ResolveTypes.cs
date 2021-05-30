using System;
using System.Collections.Generic;

namespace Mimp.SeeSharper.TypeResolver
{
    public delegate IEnumerable<Type> ResolveTypes(string typeName, string? assembly, string? @namespace, string? type, IEnumerable<Type>[] genericTypes, IEnumerable<Type> resolvedTypes);
}
