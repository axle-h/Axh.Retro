namespace Axh.Retro.CPU.X80.Util
{
    using System;
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

        public static MethodCallExpression GetMethodExpression<TSource, TArg0, TResult>(this Expression instance, Expression<Func<TSource, TArg0, TResult>> methodLambda, Expression arg0)
        {
            var methodInfo = GetMethodInfo(methodLambda);

            return Expression.Call(instance, methodInfo, new[] { arg0 });
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
    }
}
