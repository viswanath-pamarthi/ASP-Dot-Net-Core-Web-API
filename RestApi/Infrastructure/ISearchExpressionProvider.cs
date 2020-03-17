using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestApi.Infrastructure
{
    public interface ISearchExpressionProvider
    {
        ConstantExpression GetValue(string input);
        Expression GetComparision(
            MemberExpression left,
            string op,
            ConstantExpression right);
    }
}
