using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Xervizio {
    public static class ExpressionExtensions {
        public static MemberExpression MemberExpression<TModel, TValue>(this Expression<Func<TModel, TValue>> expression) {
            return expression.Body as MemberExpression;
        }

    }
}
