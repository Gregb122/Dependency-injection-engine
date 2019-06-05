using DependencyInjectionEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTests
{
    [TestClass]
    public class SimpleRegistrationOfTypes
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

        [TestMethod]
        public void ContainerConstructor()
        {
            // act
            var di = new SimpleContainer();

            // assert is handled by the ExpectedException
        }

        [TestMethod]
        public void RegisterClassWithoutSingleton()
        {
            // arrange
            var di = new SimpleContainer();

            // act
            di.RegisterType<Foo>(false);

            // assert is handled by the ExpectedException
        }

        [TestMethod]
        public void RegisterClassWithSingleton()
        {
            // arrange
            var di = new SimpleContainer();

            // act
            di.RegisterType<Doo>(true);

            // assert is handled by the ExpectedException

        }

        [TestMethod]
        public void RegisterInterface()
        {
            // arrange
            var di = new SimpleContainer();

            // act
            di.RegisterType<IFoo,Foo>(true);

            // assert is handled by the ExpectedException
        }
    }

    [TestClass]
    public class SimpleResolving
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

        [TestMethod]
        public void Resolve2SameClassesWithoutSingleton()
        {
            // arrange
            var di = new SimpleContainer();
            di.RegisterType<Foo>(false);

            // act
            var t1 = di.Resolve<Foo>();
            var t2 = di.Resolve<Foo>();

            // assert
            Assert.AreNotSame(t1, t2);
        }

        [TestMethod]
        public void Resolve2SameClassesWithSingleton()
        {
            // arrange
            var di = new SimpleContainer();

            // act
            di.RegisterType<Foo>(true);

            var t1 = di.Resolve<Foo>();
            var t2 = di.Resolve<Foo>();

            // assert
            Assert.AreSame(t1, t2);
        }

        [TestMethod]
        public void ResolveInterface()
        {
            // arrange
            var di = new SimpleContainer();

            // act
            di.RegisterType<IFoo, Foo>(false);

            var t2 = di.Resolve<IFoo>();

            // assert
            Assert.IsInstanceOfType(t2, typeof(Foo));

        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ResolveNotRegisteredInterfaceException()
        {
            // arrange
            var di = new SimpleContainer();

            // act
            di.Resolve<IFoo>();

            // assert is handled by the ExpectedException
        }

        [TestMethod]
        public void ResolveNotRegisteredClass()
        {
            // arrange
            var di = new SimpleContainer();

            // act
            var t1 = di.Resolve<Foo>();

            // assert is handled by the ExpectedException
        }

        [TestMethod]
        public void Resolve2SameClassesWithRegistredInstance()
        {
            // arrange
            var di = new SimpleContainer();

            // act
            IFoo foo1 = new Foo();
            di.RegisterInstance<IFoo>(foo1);
            IFoo foo2 = di.Resolve<IFoo>();

            // assert
            Assert.AreSame(foo1, foo2);
        }

        [TestMethod]
        public void ResolveRegistredInstanceOfString()
        {
            // arrange
            var di = new SimpleContainer();

            // act
            string t1 = "test";
            di.RegisterInstance<string>(t1);
            string t2 = di.Resolve<string>();

            // assert
            Assert.AreSame(t1, t2);
        }
    }

    [TestClass]
    public class RecursiveResolve
    {
        public class A
        {
            public B b;
            public A(B b)
            {
                this.b = b;
            }
        }
        public class B { }

        [TestMethod]
        public void SimpleRecursiveResolve()
        {
            // arrange
            SimpleContainer c = new SimpleContainer();

            // act
            A a = c.Resolve<A>();            // assert            Assert.IsTrue(a.b != null);
        }

        public class X
        {
            public X(Y d, string s) { }
        }

        public class Y { }

        [TestMethod]
        [ExpectedException(typeof(TargetInvocationException))]
        public void ResolveNotRegsitredInstanceException()
        {
            // arrange
            SimpleContainer c = new SimpleContainer();

            // act
            X x = c.Resolve<X>();

            // assert is handled by the ExpectedException
        }

        [TestMethod]
        public void ResolveRegistredInstance()
        {
            // arrange
            SimpleContainer c = new SimpleContainer();

            // act
            c.RegisterInstance("ala ma kota");
            X x = c.Resolve<X>();

            // assert is handled by the ExpectedException
        }

        class Aa
        {
            public Aa(Bb b)
            { }
        }

        class Bb
        {
            public Bb(Aa a)
            { }
        }

        [TestMethod]
        [ExpectedException(typeof(TargetInvocationException))]
        public void ResolveCycleInGraph()
        {
            // arrange
            SimpleContainer c = new SimpleContainer();

            // act
            c.Resolve<Aa>();
        }
    }
}
