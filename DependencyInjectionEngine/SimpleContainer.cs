﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace DependencyInjectionEngine
{
    public class SimpleContainer
    {

        Dictionary<Type, TypeInfo> registredList = new Dictionary<Type, TypeInfo>();

        public void RegisterType<T>(bool singleton) where T : class
        {
            registredList[typeof(T)] = new TypeInfo(typeof(T), singleton);
        }

        public void RegisterType<From, To>(bool singleton) where To : From
        {
            registredList[typeof(From)] = new TypeInfo(typeof(To), singleton);
        }

        public void RegisterInstance<T>(T instance)
        {
            registredList[typeof(T)] = new TypeInfo(instance.GetType(), true, instance);
        }


        public T Resolve<T>()
        {
            TypeInfo typeInfo;
            try
            {
                typeInfo = registredList[typeof(T)];
            }
            catch (KeyNotFoundException e)
            {
                if (!typeof(T).IsClass)
                {
                    throw
                        new KeyNotFoundException(String.Format(
                            "Cannot find type {0} in container registered types.",
                            typeof(T)), e);
                }
                else
                {
                    return (T)GetInstance(new TypeInfo(typeof(T), false));
                }
            }
            return (T)GetInstance(typeInfo);
        }

        private object GetInstance(TypeInfo typeInfo)
        {
            ConstructorInfo constructor;
            var resolvedParametersObjects = new List<object>();


            if (typeInfo.Instance != null && typeInfo.Singleton)
            {
                return typeInfo.Instance;
            }


            constructor = typeInfo.ResolveType.GetConstructors()
                .OrderByDescending(x => x.GetParameters().GetLength(0))
                .First();

            foreach (var (item, index) in constructor.GetParameters().WithIndex())
            {
                try
                {
                    Type type = item.ParameterType;
                    MethodInfo method = typeof(SimpleContainer).GetMethod("Resolve");
                    MethodInfo generic = method.MakeGenericMethod(type);
                    var result = generic.Invoke(this, null);

                    resolvedParametersObjects.Add(result);
                }
                catch (ArgumentException e)
                {
                    throw
                        new InvalidOperationException(
                            "Cannot resolve one of the constructors",
                            e);
                }

            }

            typeInfo.Instance = constructor.Invoke(resolvedParametersObjects.ToArray());

            return typeInfo.Instance;
        }
    }

    class TypeInfo
    {

        public Type ResolveType { get; set; }
        public bool Singleton { get; set; }
        public object Instance { get; set; }

        public TypeInfo(Type resolveType, bool singleton)
        {
            ResolveType = resolveType;
            Singleton = singleton;
        }

        public TypeInfo(Type resolveType, bool singleton, object instance) 
            : this(resolveType, singleton)
        {
            Instance = instance;
        }
    }
}
