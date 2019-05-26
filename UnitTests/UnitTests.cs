using DependencyInjectionEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests
{
    public interface IFoo
    { }

    public class Foo : IFoo
    {
        public Foo()
        {

        }
    }

    public class Doo
    {
        public Doo()
        {

        }
    }

    [TestClass]
    public class RegisterTests
    {
        [TestMethod]
        public void InvokeConstructor()
        {
            var di = new SimpleContainer();
        }

        [TestMethod]
        public void Register()
        {
            var di = new SimpleContainer();
            di.RegisterType<Foo>(false);
        }

        [TestMethod]
        public void register2()
        {
            var di = new SimpleContainer();
            di.RegisterType<Doo>(true);

        }

        [TestMethod]
        public void RegisterInterface()
        {
            var di = new SimpleContainer();
            di.RegisterType<IFoo,Foo>(true);
        }
    }

    [TestClass]
    public class ResolveTests
    {
        [TestMethod]
        public void Resolve1()
        {
            var di = new SimpleContainer();
            di.RegisterType<Foo>(false);

            var t1 = di.Resolve<Foo>();
            var t2 = di.Resolve<Foo>();

            Assert.AreNotSame(t1, t2);
        }

        [TestMethod]
        public void ResolveWithSingleton()
        {
            var di = new SimpleContainer();
            di.RegisterType<Foo>(true);

            var t1 = di.Resolve<Foo>();
            var t2 = di.Resolve<Foo>();

            Assert.AreSame(t1, t2);
        }

        [TestMethod]
        public void ResolveInterface()
        {
            var di = new SimpleContainer();
            di.RegisterType<IFoo, Foo>(false);

            var t2 = di.Resolve<IFoo>();

            Assert.IsInstanceOfType(t2, typeof(Foo));

        }

        [TestMethod]
        public void ResolveNotRegisteredInterface()
        {
            var di = new SimpleContainer();

            Assert.ThrowsException<KeyNotFoundException>(() => di.Resolve<IFoo>());
        }

        [TestMethod]
        public void ResolveNotRegisteredClass()
        {
            var di = new SimpleContainer();
            var t1 = di.Resolve<Foo>();
        }
    }
}
