
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace Demystifier.Internal
{
    /// <summary>
    /// A helper class that contains utilities methods for dealing with reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        private static PropertyInfo? transformerNamesLazyPropertyInfo;

        /// <summary>
        /// Returns true if the <paramref name="type"/> is a value tuple type.
        /// </summary>
        public static bool IsValueTuple(this Type type)
        {
            return type.Namespace == "System" && type.Name.Contains("ValueTuple`");
        }

        /// <summary>
        /// Returns true if the given <paramref name="attribute"/> is of type <code>TupleElementNameAttribute</code>.
        /// </summary>
        /// <remarks>
        /// To avoid compile-time dependency hell with System.ValueTuple, this method uses reflection and not checks statically that 
        /// the given <paramref name="attribute"/> is <code>TupleElementNameAttribute</code>.
        /// </remarks>
        public static bool IsTupleElementNameAttribute(this Attribute attribute)
        {
            var attributeType = attribute.GetType();
            return attributeType.Namespace == "System.Runtime.CompilerServices" &&
                   attributeType.Name == "TupleElementNamesAttribute";
        }

        /// <summary>
        /// Returns 'TransformNames' property value from a given <paramref name="attribute"/>.
        /// </summary>
        /// <remarks>
        /// To avoid compile-time dependency hell with System.ValueTuple, this method uses reflection 
        /// instead of casting the attribute to a specific type.
        /// </remarks>
        public static IList<string>? GetTransformerNames(this Attribute attribute)
        {
            Debug.Assert(attribute.IsTupleElementNameAttribute());

            var propertyInfo = GetTransformNamesPropertyInfo(attribute.GetType());
            return propertyInfo?.GetValue(attribute) as IList<string>;
        }

        private static PropertyInfo? GetTransformNamesPropertyInfo(Type attributeType)
        {
#pragma warning disable 8634
            return LazyInitializer.EnsureInitialized(ref transformerNamesLazyPropertyInfo,
#pragma warning restore 8634
                () => attributeType.GetProperty("TransformNames", BindingFlags.Instance | BindingFlags.Public)!);
        }
    }
}
