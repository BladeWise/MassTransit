namespace MassTransit
{
    using System.Reflection.Emit;
    using System.Reflection;
    using System;
    using System.Collections.Concurrent;
    using MassTransit.Transports;

    public static class DelegatingBusBuilder
    {
        static readonly AssemblyBuilder _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new("MassTransit.Dynamic"), AssemblyBuilderAccess.RunAndCollect);

        static readonly ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule("MassTransit.Dynamic");

        static readonly ConcurrentDictionary<Type, Lazy<Type>> _delegatingBusTypes = [];

        static Type CreateDelegatingBusType<TBus>() where TBus : IBus
        {
            var busInterfaceType = typeof(TBus);
            var baseType = typeof(DelegatingBus<TBus>);
            var builder = _moduleBuilder;
            var classTypeName = $"DelegatingBusOf{busInterfaceType.Name}";
            var isNestedType = baseType is { IsNested: true, DeclaringType: not null };
            var ns = isNestedType ? baseType.DeclaringType!.Namespace : baseType.Namespace;
            if (ns is not (null or ""))
                ns += ".";

            var typeName = $"MassTransit.Dynamic.{(isNestedType ? $"{ns}{baseType.DeclaringType!.Name}+{classTypeName}" : $"{ns}{classTypeName}")}";

            // Create the type (no parent, we need to build the generic parameters first)
            var typeBuilder = builder.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed, baseType, [busInterfaceType]);

            Type[] baseCtorParameterTypes = [typeof(IBusInstanceResolver<TBus>)];
            // First get the generic constructor
            var baseCtor = baseType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, baseCtorParameterTypes, null) ?? throw new InvalidOperationException($"Cannot find suitable {baseType} constructor.");

            Type[] ctorParameterTypes = [typeof(IBusInstanceResolver<TBus>)];
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, ctorParameterTypes);

            var il = ctorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, baseCtor);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);

            return typeBuilder.CreateTypeInfo()
                              .AsType();
        }

        public static IBus CreateDelegatingBus(IBusInstanceResolver busInstanceResolver)
        {
            return new DelegatingBus(busInstanceResolver);
        }

        public static TBus CreateDelegatingBus<TBus>(IBusInstanceResolver<TBus> busInstanceResolver) where TBus : IBus
        {
            if (typeof(TBus) == typeof(IBus))
                return (TBus)(object)new DelegatingBus(busInstanceResolver);
            if (!typeof(TBus).IsInterface)
                throw new ArgumentException($"Cannot create delegating bus for type {typeof(TBus)} because it is not an interface.");
            return (TBus)Activator.CreateInstance(_delegatingBusTypes.GetOrAdd(typeof(TBus), _ => new(CreateDelegatingBusType<TBus>))
                                                                     .Value,
                busInstanceResolver)!;
        }
    }
}
