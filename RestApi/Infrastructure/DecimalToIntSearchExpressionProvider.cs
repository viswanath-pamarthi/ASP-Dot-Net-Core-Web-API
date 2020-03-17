using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestApi.Infrastructure
{
    public class DecimalToIntSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if(!decimal.TryParse(input, out var dec))
            {
                throw new ArgumentException("Invalid search value");
            }

            //Binary math to remove decimal point from decimal without changing the value of the number
            var places = BitConverter.GetBytes(decimal.GetBits(dec)[3])[2];//this will get number of places indecimal

            if (places < 2) places = 2;

            var justDigits = (int)(dec * (decimal)Math.Pow(10, places));

            return Expression.Constant(justDigits);
        }

        public override Expression GetComparision(MemberExpression left, string op, ConstantExpression right)
        {
            switch (op.ToLower())
            {
                case "gt": return Expression.GreaterThan(left, right);
                case "gte": return Expression.GreaterThanOrEqual(left, right);
                case "lt": return Expression.LessThan(left, right);
                case "lte": return Expression.LessThanOrEqual(left, right);

                    //if nothing(operator) matches, fall back to base implementation
                default:
                    return base.GetComparision(left, op, right);
            }
        }

    }
}
