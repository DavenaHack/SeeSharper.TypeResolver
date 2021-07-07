using System;
using System.Runtime.Serialization;

namespace Mimp.SeeSharper.TypeResolver.Abstraction
{
    /// <summary>
    /// Throws if <see cref="ITypeResolver"/> failed to resolve a <see cref="Type"/>.
    /// </summary>
    [Serializable]
    public class TypeResolveException : Exception
    {
    

        public TypeResolveException() { }
        
        public TypeResolveException(string? message) 
            : base(message) { }
        
        public TypeResolveException(string? message, Exception? inner) 
            : base(message, inner) { }


        protected TypeResolveException(
            SerializationInfo info,
            StreamingContext context
        ) : base(info, context) { }
        

    }
}
