
using System;
using System.Collections.Generic;
using System.Text;
using Demystifier.Internal;

namespace Demystifier
{
    public class ValueTupleResolvedParameter : ResolvedParameter
    {
        public IList<string> TupleNames { get; }

        public ValueTupleResolvedParameter(Type resolvedType, IList<string> tupleNames) 
            : base(resolvedType) 
            => TupleNames = tupleNames;

        protected override void AppendTypeName(StringBuilder sb)
        {
            if (ResolvedType is not null)
            {
                if (ResolvedType.IsValueTuple())
                {
                    AppendValueTupleParameterName(sb, ResolvedType);
                }
                else
                {
                    // Need to unwrap the first generic argument first.
                    sb.Append(TypeNameHelper.GetTypeNameForGenericType(ResolvedType));
                    sb.Append("<");
                    AppendValueTupleParameterName(sb, ResolvedType.GetGenericArguments()[0]);
                    sb.Append(">");
                }
            }
        }

        private void AppendValueTupleParameterName(StringBuilder sb, Type parameterType)
        {
            sb.Append("(");
            var args = parameterType.GetGenericArguments();
            for (var i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.AppendTypeDisplayName(args[i], fullName: false, includeGenericParameterNames: true);

                if (i >= TupleNames.Count)
                {
                    continue;
                }

                var argName = TupleNames[i];
                if (argName == null)
                {
                    continue;
                }

                sb.Append(" ");
                sb.Append(argName);
            }

            sb.Append(")");
        }
    }
}
