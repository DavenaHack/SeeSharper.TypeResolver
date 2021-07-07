using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimp.SeeSharper.TypeResolver.Abstraction;
using System.Collections.Generic;

namespace Mimp.SeeSharper.TypeResolver.Test
{
    [TestClass]
    public class DelegateTypeResolverTest
    {


        [TestMethod]
        public void TestResolve()
        {

            var resolver = new DelegateTypeResolver();

            Assert.AreEqual(resolver.ResolveSingle(typeof(string).FullName), typeof(string));

            Assert.AreEqual(resolver.ResolveSingle(typeof(IEnumerable<string>).FullName), typeof(IEnumerable<string>));

            Assert.AreEqual(resolver.ResolveSingle(typeof(IDictionary<,>).FullName), typeof(IDictionary<,>));

        }


        [TestMethod]
        public void TestChangeAssembly()
        {

            var resolver = new DelegateTypeResolver();
            resolver.ChangeAssembly("foo", typeof(string).Assembly.FullName);

            Assert.AreEqual(resolver.ResolveSingle($"{typeof(string).Namespace}.{typeof(string).Name}, foo"), typeof(string));

        }


        [TestMethod]
        public void TestChangeNamespace()
        {

            var resolver = new DelegateTypeResolver();
            resolver.ChangeNamespace("foo", typeof(string).Namespace);

            Assert.AreEqual(resolver.ResolveSingle($"foo.{typeof(string).Name}, {typeof(string).Assembly.FullName}"), typeof(string));

        }


    }
}
