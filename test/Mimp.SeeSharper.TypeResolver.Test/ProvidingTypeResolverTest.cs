using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimp.SeeSharper.TypeProvider;
using Mimp.SeeSharper.TypeResolver.Abstraction;
using Mimp.SeeSharper.TypeResolver.TypeProvider;
using System.Collections.Generic;

namespace Mimp.SeeSharper.TypeResolver.Test
{
    [TestClass]
    public class ProvidingTypeResolverTest
    {


        [TestMethod]
        public void TestResolve()
        {

            var resolver = new ProvidingTypeResolver(new EntryAssemblyTypeProvider());

            Assert.AreEqual(resolver.ResolveSingle("string"), typeof(string));

            Assert.AreEqual(resolver.ResolveSingle("IEnumerable<string>"), typeof(IEnumerable<string>));
            Assert.AreEqual(resolver.ResolveSingle("IEnumerable[string]"), typeof(IEnumerable<string>));

            Assert.AreEqual(resolver.ResolveSingle("IDictionary<,>"), typeof(IDictionary<,>));
            Assert.AreEqual(resolver.ResolveSingle("IDictionary[,]"), typeof(IDictionary<,>));

        }


    }
}
