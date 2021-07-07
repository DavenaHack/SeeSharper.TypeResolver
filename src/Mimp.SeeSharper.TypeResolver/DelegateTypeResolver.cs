using Mimp.SeeSharper.TypeResolver.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Mimp.SeeSharper.TypeResolver
{
    /// <summary>
    /// <see cref="DelegateTypeResolver"/> is a <see cref="ITypeResolver"/> which resolve
    /// the <see cref="Type"/> with <see cref="Type.GetType(string)"/>. The behavior can override with delegates.
    /// </summary>
    /// <seealso cref="ResolveAssembly"/>
    /// <seealso cref="ResolveNamespace"/>
    /// <seealso cref="ResolveTypeName"/>
    /// <seealso cref="ResolveTypes"/>
    public class DelegateTypeResolver : ITypeResolver
    {


        private static readonly Regex _TypeRegex =
            new Regex(@"^\s*(?:([_a-zA-Z][_a-zA-Z0-9]*(?:\.[_a-zA-Z][_a-zA-Z0-9]*)*)\.)?([_a-zA-Z][_a-zA-Z0-9]*(?:`\d+)?)\s*((?:<.*>|\[.*\])\s*)?(?:,\s*((?:[_a-zA-Z][_a-zA-Z0-9]*(?:\.[_a-zA-Z][_a-zA-Z0-9]*)*)\s*(?:,\s*Version\s*=\s*\d+(?:\.\d+){0,3}\s*)?(?:,\s*Culture\s*=\s*[-a-zA-Z]+\s*)?(?:,\s*PublicKeyToken\s*=\s*[a-fA-F0-9]{16}\s*)?))?$", RegexOptions.IgnoreCase);
        private static readonly Regex _GenericTypesRegex =
            new Regex(@"^\s*(?:(?:<(?:(?:(?:\s*(),)*\s*())|(?:(?:(?:\s*<(.+)>\s*,)*\s*<(.+)>\s*))|(?:(?:(?:\s*\[(.+)\]\s*,)*\s*\[(.+)\]\s*))|(?:(?:(?:\s*([^,]+)\s*,)*\s*([^,]+)\s*)))>)|(?:\[(?:(?:(?:\s*(),)*\s*())|(?:(?:(?:\s*<(.+)>\s*,)*\s*<(.+)>\s*))|(?:(?:(?:\s*\[(.+)\]\s*,)*\s*\[(.+)\]\s*))|(?:(?:(?:\s*([^,]+)\s*,)*\s*([^,]+)\s*)))\]))\s*$");


        protected ResolveAssembly? ResolveAssemblyDelegate { get; set; }

        protected ResolveNamespace? ResolveNamespaceDelegate { get; set; }

        protected ResolveTypeName? ResolveTypeNameDelegate { get; set; }

        protected ResolveTypes? ResolveTypesDelegate { get; set; }


        public DelegateTypeResolver()
        {
            AddResolveTypeName(ChangePrimitiveTypeNames);
            AddResolveTypeName(AddGenericGraveAccentKey);
            AddResolveTypes(ResolveQualifiedTypeName);
        }


        public IEnumerable<Type> Resolve(string type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            static string? groupVal(Group group) => group.Success ? group.Value : null;
            var match = _TypeRegex.Match(type);
            if (!match.Success)
                throw new TypeResolveException($"Invalid type name: {type}");
            var ns = groupVal(match.Groups[1]);
            var tn = groupVal(match.Groups[2]);
            var gn = groupVal(match.Groups[3]);
            var an = groupVal(match.Groups[4]);

            IEnumerable<Type>[] genTypes = Array.Empty<IEnumerable<Type>>();
            if (gn is not null)
            {
                var genMatch = _GenericTypesRegex.Match(gn);
                if (!genMatch.Success)
                    throw new TypeResolveException($"Invalid type name: {type}");
                var gt = new List<IEnumerable<Type>>();
                for (int i = 1; i < genMatch.Groups.Count; i++)
                    try
                    {
                        var group = genMatch.Groups[i];
                        if (group.Success)
                            gt.Add(string.IsNullOrWhiteSpace(group.Value) ? Type.EmptyTypes : Resolve(group.Value));
                    }
                    catch (Exception ex)
                    {
                        throw new TypeResolveException($"Invalid type name: {type}", ex);
                    }
                genTypes = gt.ToArray();
            }

            if (ResolveAssemblyDelegate is not null)
                an = ResolveAssemblyDelegate(type, an);
            if (ResolveNamespaceDelegate is not null)
                ns = ResolveNamespaceDelegate(type, an, ns);
            if (ResolveTypeNameDelegate is not null)
                tn = ResolveTypeNameDelegate(type, an, ns, tn, genTypes);
            IEnumerable<Type> ts = Type.EmptyTypes;
            if (ResolveTypesDelegate is not null)
                ts = ResolveTypesDelegate(type, an, ns, tn, genTypes, ts);

            return ts;
        }


        public void AddResolveAssembly(ResolveAssembly resolveAssembly)
        {
            if (resolveAssembly is null)
                throw new ArgumentNullException(nameof(resolveAssembly));

            var old = ResolveAssemblyDelegate;
            ResolveAssemblyDelegate = old is null
                ? resolveAssembly
                : (typeName, assembly) => resolveAssembly(typeName, old(typeName, assembly));
        }

        public void ChangeAssembly(string? from, string to)
        {
            if (to is null)
                throw new ArgumentNullException(nameof(to));

            if (string.IsNullOrWhiteSpace(from))
                AddResolveAssembly((fullTypeName, assembly) =>
                {
                    if (string.IsNullOrWhiteSpace(assembly))
                        return to;
                    return assembly;
                });
            else
                AddResolveAssembly((fullTypeName, assembly) =>
                {
                    if (assembly?.StartsWith(from, StringComparison.InvariantCultureIgnoreCase) ?? false)
                        return to + assembly.Substring(from!.Length);
                    return assembly;
                });
        }


        public void AddResolveNamespace(ResolveNamespace resolveNamespace)
        {
            if (resolveNamespace is null)
                throw new ArgumentNullException(nameof(resolveNamespace));

            var old = ResolveNamespaceDelegate;
            ResolveNamespaceDelegate = old is null
                ? resolveNamespace
                : (fullTypeName, assembly, @namespace) => resolveNamespace(fullTypeName, assembly, old(fullTypeName, assembly, @namespace));
        }

        public void ChangeNamespace(string? from, string to)
        {
            if (to is null)
                throw new ArgumentNullException(nameof(to));

            if (string.IsNullOrWhiteSpace(from))
                AddResolveNamespace((fullTypeName, assembly, @namespace) =>
                {
                    if (string.IsNullOrWhiteSpace(@namespace))
                        return to;
                    return assembly;
                });
            else
                AddResolveNamespace((fullTypeName, assembly, @namespace) =>
                {
                    if (@namespace?.StartsWith(from, StringComparison.InvariantCultureIgnoreCase) ?? false)
                        return to + @namespace.Substring(from!.Length);
                    return @namespace;
                });
        }


        public void AddResolveTypeName(ResolveTypeName resolveTypeName)
        {
            if (resolveTypeName is null)
                throw new ArgumentNullException(nameof(resolveTypeName));

            var old = ResolveTypeNameDelegate;
            ResolveTypeNameDelegate = old is null
                ? resolveTypeName
                : (fullTypeName, assembly, @namespace, type, genericTypes) => resolveTypeName(fullTypeName, assembly, @namespace, old(fullTypeName, assembly, @namespace, type, genericTypes), genericTypes);
        }


        public void AddResolveTypes(ResolveTypes resolveTypes)
        {
            if (resolveTypes is null)
                throw new ArgumentNullException(nameof(resolveTypes));

            var old = ResolveTypesDelegate;
            ResolveTypesDelegate = old is null
                ? resolveTypes
                : (fullTypeName, assembly, @namespace, type, genericTypes, resolvedTypes) => resolveTypes(fullTypeName, assembly, @namespace, type, genericTypes, old(fullTypeName, assembly, @namespace, type, genericTypes, resolvedTypes));
        }


        #region Default delegates


        protected static string? ChangePrimitiveTypeNames(string fullTypeName, string? assembly, string? @namespace, string? type, IEnumerable<Type>[] genericTypes)
        {
            if (!string.IsNullOrWhiteSpace(@namespace) && !string.Equals(@namespace, "System", StringComparison.CurrentCultureIgnoreCase))
                return type;
            if (string.Equals(type, "bool", StringComparison.InvariantCultureIgnoreCase))
                return typeof(bool).Name;
            if (string.Equals(type, "float", StringComparison.InvariantCultureIgnoreCase))
                return typeof(float).Name;
            if (string.Equals(type, "int", StringComparison.InvariantCultureIgnoreCase))
                return typeof(int).Name;
            if (string.Equals(type, "long", StringComparison.InvariantCultureIgnoreCase))
                return typeof(long).Name;
            if (string.Equals(type, "short", StringComparison.InvariantCultureIgnoreCase))
                return typeof(short).Name;
            if (string.Equals(type, "uint", StringComparison.InvariantCultureIgnoreCase))
                return typeof(uint).Name;
            if (string.Equals(type, "ulong", StringComparison.InvariantCultureIgnoreCase))
                return typeof(ulong).Name;
            if (string.Equals(type, "ushort", StringComparison.InvariantCultureIgnoreCase))
                return typeof(ushort).Name;
            return type;
        }


        protected static string? AddGenericGraveAccentKey(string fullTypeName, string? assembly, string? @namespace, string? type, IEnumerable<Type>[] genericTypes)
        {
            if (type is null)
                return type;

            if (genericTypes.Length > 0)
            {
                var genNum = $"`{genericTypes.Length}";
                if (!type.EndsWith(genNum))
                    if (type.Contains("`"))
                        throw new TypeResolveException($@"Invalid number of generic types for ""{fullTypeName}""");
                    else
                        type = $"{type}{genNum}";
            }
            return type;
        }


        protected static IEnumerable<Type> ResolveQualifiedTypeName(string fullTypeName, string? assembly, string? @namespace, string? type, IEnumerable<Type>[] genericTypes, IEnumerable<Type> resolvedTypes)
        {
            if (string.IsNullOrWhiteSpace(type))
                return Type.EmptyTypes;

            var t = type;
            if (!string.IsNullOrWhiteSpace(@namespace))
                t = $"{@namespace}.{t}";

            var genTypes = genericTypes.Select(ts => ts.ToArray()).ToArray();
            var generic = IsGenericTypeDefinition(genTypes);
            if (generic.HasValue && !generic.Value)
            {
                var types = new List<Type>();
                foreach (var gts in GetGenericCombinations(genTypes))
                {
                    var gt = $"{t}[{string.Join(",", gts.Select(t => $"[{t.AssemblyQualifiedName}]"))}]";
                    if (!string.IsNullOrWhiteSpace(assembly))
                        gt = $"{gt}, {assembly}";
                    var gr = Type.GetType(gt, false, true);
                    if (gr is not null)
                        types.Add(gr);
                }
                return types.ToArray();
            }

            if (!string.IsNullOrWhiteSpace(assembly))
                t = $"{t}, {assembly}";
            
            var r = Type.GetType(t, false, true);
            return r is null ? Array.Empty<Type>() : new[] { r };
        }


        #endregion


        #region Helpers


        protected static IEnumerable<Type> FindTypes(IEnumerable<Type> types, string? assembly, string? @namespace, string? type, IEnumerable<Type>[] genericTypes)
        {
            var assemblyName = string.IsNullOrWhiteSpace(assembly) ? null : new AssemblyName(assembly);
            var genTypes = genericTypes.Select(ts => ts.ToArray()).ToArray();
            var generic = IsGenericTypeDefinition(genTypes);

            foreach (var t in types)
            {
                if (!string.Equals(t.Name, type, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (!string.IsNullOrWhiteSpace(@namespace) && !string.Equals(t.Namespace, @namespace, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                if (assemblyName is not null)
                {
                    var n = t.Assembly.GetName();
                    if (!string.Equals(assemblyName.Name, n.Name, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    if (assemblyName.Version is not null && n.Version is not null && assemblyName.Version < n.Version)
                        continue;
                }

                if (generic.HasValue)
                {
                    var gt = t;
                    if (!gt.IsGenericTypeDefinition)
                        if (gt.IsGenericType)
                            gt = gt.GetGenericTypeDefinition();
                        else
                            continue;
                    if (genTypes.Length != gt.GetGenericArguments().Length)
                        continue;

                    if (!generic.Value)
                        foreach (var gts in GetGenericCombinations(genTypes))
                            yield return gt.MakeGenericType(gts);
                    else
                        yield return gt;
                }
                else
                    yield return t;
            }
        }


        /// <summary>
        /// null - no generic
        /// true - generic type definition
        /// false - generic type
        /// </summary>
        /// <param name="genericTypes"></param>
        /// <returns></returns>
        protected static bool? IsGenericTypeDefinition(Type[][] genericTypes)
        {
            bool? isGenericTypeDefinition = null;
            foreach (var t in genericTypes)
            {
                if (t.Length > 0)
                {
                    if (!isGenericTypeDefinition.HasValue)
                        isGenericTypeDefinition = false;
                    else if (isGenericTypeDefinition.Value)
                        throw new TypeResolveException($"Generic types have all a value for a generic type of all haven't a value for a generic type definition");
                }
                else
                {
                    if (!isGenericTypeDefinition.HasValue)
                        isGenericTypeDefinition = true;
                    else if (!isGenericTypeDefinition.Value)
                        throw new TypeResolveException($"Generic types have all a value for a generic type of all haven't a value for a generic type definition");
                }
            }
            return isGenericTypeDefinition;
        }

        protected static IEnumerable<Type[]> GetGenericCombinations(Type[][] genericTypes)
        {
            var l = genericTypes.Length;
            if (l < 1)
                yield break;
            var indexes = new int[l];
            var i = l - 1;
            while (true)
            {
                var r = new Type[l];
                for (var a = 0; a < l; a++)
                    r[a] = genericTypes[a][indexes[a]];
                yield return r;

                var c = indexes[i] + 1;
                if (c < genericTypes[i].Length)
                    indexes[i] = c;
                else
                {
                    do
                    {
                        if (--i < 0)
                            yield break;
                    }
                    while (++indexes[i] == genericTypes[i].Length);
                    while (i < l - 1)
                        indexes[++i] = 0;
                }
            }
        }


        #endregion


    }
}
