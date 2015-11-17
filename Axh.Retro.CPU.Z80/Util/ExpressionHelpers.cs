namespace Axh.Retro.CPU.Z80.Util
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class ExpressionHelpers
    {
        public static MemberExpression GetPropertyExpression<TSource, TProperty>(this Expression instance, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType) && !propInfo.ReflectedType.IsAssignableFrom(type))
            {
                throw new ArgumentException($"Expresion '{propertyLambda}' refers to a property that is not from type {type}.");
            }
            
            return Expression.Property(instance, propInfo);
        }

        public static MethodInfo GetMethodInfo<TSource, TArg, TResult>(Expression<Func<TSource, TArg, TResult>> methodLambda)
        {
            var outermostExpression = methodLambda.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }


        public static MethodInfo GetMethodInfo<TSource, TArg1, TArg2, TResult>(Expression<Func<TSource, TArg1, TArg2, TResult>> methodLambda)
        {
            var outermostExpression = methodLambda.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }

        public static MethodInfo GetMethodInfo<TSource, TArg1, TArg2, TArg3>(Expression<Action<TSource, TArg1, TArg2, TArg3>> methodLambda)
        {
            var outermostExpression = methodLambda.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }

        public static MethodInfo GetMethodInfo<TSource, TArg1, TArg2>(Expression<Action<TSource, TArg1, TArg2>> methodLambda)
        {
            var outermostExpression = methodLambda.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }
        
        public static MethodInfo GetMethodInfo<TSource, TArg1>(Expression<Action<TSource, TArg1>> methodLambda)
        {
            var outermostExpression = methodLambda.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }

        public static MethodInfo GetMethodInfo<TSource>(Expression<Action<TSource>> methodLambda)
        {
            var outermostExpression = methodLambda.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }

        public static string DebugView(this Expression expression)
        {
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                DebugViewWriter.WriteTo(expression, writer);
                return writer.ToString();
            }
        }
    }
}
