using System;
using System.Collections.Generic;

namespace Mimp.SeeSharper.TypeResolver.Abstraction
{
    /// <summary>
    /// <see cref="ITypeResolver"/> resolve a <see cref="Type"/> from a <see cref="string"/>.
    /// </summary>
    public interface ITypeResolver
    {


        /// <summary>
        /// Resolve <see cref="Type"/> from <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TypeResolveException"></exception>
        public IEnumerable<Type> Resolve(string type);


    }
}
