using System;

namespace Mimp.SeeSharper.TypeResolver.Abstraction
{
    /// <summary>
    /// Extensions method for <see cref="ITypeResolver"/>.
    /// </summary>
    public static class TypeResolverExtensions
    {


        /// <summary>
        /// Resovle <paramref name="typeName"/> to a single <see cref="Type"/>.
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TypeResolveException">
        /// If no or more than one type resolved.
        /// </exception>
        public static Type ResolveSingle(this ITypeResolver resolver, string typeName)
        {
            if (resolver is null)
                throw new ArgumentNullException(nameof(resolver));
            if (typeName is null)
                throw new ArgumentNullException(nameof(typeName));

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
