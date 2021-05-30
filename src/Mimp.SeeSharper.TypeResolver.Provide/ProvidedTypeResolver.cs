using Mimp.SeeSharper.TypeProvider.Abstraction;
using Mimp.SeeSharper.TypeResolver.Abstraction;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mimp.SeeSharper.TypeResolver.Provide
{
    public class ProvidedTypeResolver : DelegateTypeResolver
    {


        public ITypeProvider Provider { get; }


        public ProvidedTypeResolver(ITypeProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            ResolveTypesDelegate = ResolveTypesByProvider;
        }


        protected IEnumerable<Type> ResolveTypesByProvider(string typeName, string? assembly, string? @namespace, string? type, IEnumerable<Type>[] genericTypes, IEnumerable<Type> resolvedTypes)
        {
            var assemblyName = string.IsNullOrWhiteSpace(assembly) ? null : new AssemblyName(assembly);
            if (assemblyName is not null && Provider is IAssemblyTypeProvider provider)
            {
                IEnumerable<Assembly> assemblies;
                try
                {
                    assemblies = provider.GetAssemblies();
                }
                catch (Exception ex)
                {
                    throw new TypeResolveException($@"Can't provide assemblies", ex);
                }
                foreach (var a in assemblies)
                {
                    var n = a.GetName();
                    if (!string.Equals(assemblyName.Name, n.Name, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    if (assemblyName.Version is not null && n.Version is not null && assemblyName.Version < n.Version)
                        continue;
                    IEnumerable<Type> types;
                    try
                    {
                        types = provider.GetTypes(a);
                    }
                    catch (Exception ex)
                    {
                        throw new TypeResolveException($@"Can't provide types", ex);
                    }
                    foreach (var t in FindTypes(types, assembly, @namespace, type, genericTypes))
                        yield return t;
                }
            }
            else
            {
                IEnumerable<Type> types;
                try
                {
                    types = Provider.GetTypes();
                }
                catch (Exception ex)
                {
                    throw new TypeResolveException($@"Can't provide types", ex);
                }
                foreach (var t in FindTypes(types, assembly, @namespace, type, genericTypes))
                    yield return t;
            }
        }


    }
}
