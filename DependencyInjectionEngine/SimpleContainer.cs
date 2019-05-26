using System;
using System.Collections.Generic;
using System.Reflection;

namespace DependencyInjectionEngine
{
    public class SimpleContainer
    {

        Dictionary<Type, RegisteredTypeInfo> registredList = new Dictionary<Type, RegisteredTypeInfo>();

        public void RegisterType<T>(bool singleton) where T : class
        {
            registredList[typeof(T)] = new RegisteredTypeInfo(typeof(T), singleton);
        }

        public void RegisterType<From, To>(bool singleton) where To : From
        {
            registredList[typeof(From)] = new RegisteredTypeInfo(typeof(To), singleton);
        }

        public T Resolve<T>()
        {
            RegisteredTypeInfo typeInfo;
            try
            {
                typeInfo = registredList[typeof(T)];
            }
            catch (KeyNotFoundException e)
            {
                if (!typeof(T).IsClass)
                {
                    throw new KeyNotFoundException(String.Format("Cannot find type {0} in container registered types.", typeof(T)), e);
                }
                else
                {
                    return (T)typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]);
                }
            }

            return (T)typeInfo.GetInstance();
        }
    }

    class RegisteredTypeInfo
    {
        public RegisteredTypeInfo(Type resolveType, bool singleton)
        {
            this.ResolveType = resolveType;
            this.Singleton = singleton;
        }

        Type ResolveType { get; set; }
        bool Singleton { get; set; }
        object _instance;

        public void SetInstance(object instance)
        {
            if (_instance == null && Singleton)
            {
                _instance = instance;
            }
        }

        public object GetInstance()
        {
            if (_instance != null && Singleton)
            {
                return _instance;
            }

            _instance = ResolveType.GetConstructor(new Type[0]).Invoke(new object[0]);
            return _instance;
        }
    }
}
