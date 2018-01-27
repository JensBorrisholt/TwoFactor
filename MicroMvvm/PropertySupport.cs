using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MicroMvvm
{
    public static class PropertySupport
    {
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)
        {
            if (propertyExpresssion == null)
                throw new ArgumentNullException(nameof(propertyExpresssion));

            if (!(propertyExpresssion.Body is MemberExpression memberExpression))
                throw new ArgumentException("The expression is not a member access expression.", nameof(propertyExpresssion));

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("The member access expression does not access a property.",  nameof(propertyExpresssion));

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
                throw new ArgumentException("The referenced property is a static property.", nameof(propertyExpresssion));
            
            return memberExpression.Member.Name;
        }
    }
}
