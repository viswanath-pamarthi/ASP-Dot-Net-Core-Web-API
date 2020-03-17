using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace RestApi.Infrastructure
{
    public class SearchOptionsProcessor<T, TEntity>
    {
        private readonly string[] _searchQuery;

        public SearchOptionsProcessor(string[] searchQuery)
        {
            _searchQuery = searchQuery;
        }

        public IEnumerable<SearchTerm> GetAllTerms()
        {
            if (_searchQuery == null) yield break;

            foreach( var expression in _searchQuery)
            {
                if (string.IsNullOrEmpty(expression)) continue;

                //each expression looks like:
                //"fieldName op value..." op(operator) can be lessthan or grater than
                var tokens = expression.Split(' ');

                //if no tokens left after aplitting to after splitting
                if(tokens.Length ==0)
                {
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name=expression
                    };
                    continue;
                }

                //if there are not enough tokens i.e. <3
                if(tokens.Length<3)
                {
                    yield return new SearchTerm
                    {
                        ValidSyntax=false,
                        Name=tokens[0]
                    };
                }

                //we have valid syntax
                yield return new SearchTerm
                {
                    ValidSyntax=true,
                    Name=tokens[0],
                    Operator=tokens[1],
                    Value=string.Join(" ",tokens.Skip(2))
                };

                //This method will return a list of search terms client specified in query string, .
                //However, not all search terms necessarly be valid searchable field in the model.
                //we need to cross verify with fields with searchable attributes in model(using reflection)
            }
        }

        internal IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();

            if (!terms.Any()) return query;//no terms return query as is

            var modifiedQuery = query;

            foreach(var term in terms)
            {
                var propertyInfo = ExpressionHelper
                    .GetPropertyInfo<TEntity>(term.Name);

                var obj = ExpressionHelper.Parameter<TEntity>();


                //Build up the LINQ expression backwards:
                //query = query.Where(x=> x.property == "Value");

                //x.property
                var left = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);

                //value
                var right = term.ExpressionProvider.GetValue(term.Value); //Expression.Constant(term.Value);

                //x/property== "value"
                var comparisionExpression = term.ExpressionProvider.GetComparision(left, term.Operator, right);    //Expression.Equal(left, right);

                //x => x.Property == "Value"
                var lambdaExpression = ExpressionHelper
                    .GetLambda<TEntity, bool>(obj, comparisionExpression);

                //query = query.where...
                modifiedQuery = ExpressionHelper.CallWhere(modifiedQuery, lambdaExpression);
            }

            return modifiedQuery;
        }

        private static IEnumerable<SearchTerm> GetTermsFromModel() =>
            typeof(T).GetTypeInfo()
            .DeclaredProperties
            .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any())
            .Select(p => new SearchTerm { Name=p.Name,
            ExpressionProvider=p.GetCustomAttribute<SearchableAttribute>().ExpressionProvider
            });

        public IEnumerable<SearchTerm> GetValidTerms()
        {
            var queryTerms = GetAllTerms()
                .Where(x => x.ValidSyntax)
                .ToArray();

            if (!queryTerms.Any()) yield break;//if there aren't any with valid syntax yield break return empty ienumerable

            var declaredTerms = GetTermsFromModel();

            foreach(var term in queryTerms)
            {
                var declareTerm = declaredTerms
                    .SingleOrDefault(x => x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));

                if (declareTerm == null) continue;//no  match

                yield return new SearchTerm
                {
                    Name = declareTerm.Name,
                    Operator = term.Operator,
                    ValidSyntax = term.ValidSyntax,
                    Value = term.Value,
                    ExpressionProvider = declareTerm.ExpressionProvider
                };
            }

        }
    }
}
