using System;

namespace Mimp.SeeSharper.TypeResolver.Abstraction
{
    public static class TypeResolverExtensions
    {


        public static Type ResolveRequired(this ITypeResolver resolver, string typeName)
        {
            Type? r = null;
            foreach (var t in resolver.Resolve(typeName))
            {
                if (r is not null)
                    throw new TypeResolveException($@"Type name ""{typeName}"" is ambiguous.");
                r = t;
            }
            if (r is null)
                throw new TypeResolveException($@"Found no type with name ""{typeName}""");
            return r;
        }


    }
}
