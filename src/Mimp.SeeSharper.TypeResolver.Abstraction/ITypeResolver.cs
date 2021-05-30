using System;
using System.Collections.Generic;

namespace Mimp.SeeSharper.TypeResolver.Abstraction
{
    public interface ITypeResolver
    {

        public IEnumerable<Type> Resolve(string typeName);

    }
}
