using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RestApi.Infrastructure
{
    public class SortOptionsProcessor<T, TEntity>
    {
        private readonly string[] _orderBy;

        public SortOptionsProcessor(string[] orderBy)
        {
            _orderBy = orderBy;
        }


        public IEnumerable<SortTerm> GetAllTerms()
        {
            if (_orderBy == null) yield break;//if oderby is null the we yield break and return empty enumerable

            foreach (var term in _orderBy)
            {
                if (string.IsNullOrEmpty(term)) continue;

                //each term should be split in to pair of tokens based on space character
                var tokens = term.Split(' ');

                //if no space character
                if (tokens.Length == 0)
                {
                    yield return new SortTerm { Name = term };
                    continue;
                }

                //if there is once or more tokens separated by space , we need to check whether second token is characters desc for descending
                var descending = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

                yield return new SortTerm
                {
                    Name = tokens[0],
                    Descending = descending
                };

                //This code will return all sort terms client specified in the query string.
                //however, not all of these will be valid sortable fields, we need to cross reference with fields that 
                //have the sortable attributes in model, we can get those by reflection
            }
        }

        public IEnumerable<SortTerm> GetValidTerms()
        {
            var queryTerms = GetAllTerms().ToArray();

            if (!queryTerms.Any()) yield break;

            var declaredTerms = GetTermsFromModel();

            foreach(var term in queryTerms)
            {
                //cross verify the sort term on model
                var declaredTerm = declaredTerms
                    .SingleOrDefault(x => x.Name.Equals(term.Name,StringComparison.OrdinalIgnoreCase));

                if (declaredTerm == null) continue;//not a valid term on model continue

                yield return new SortTerm
                {
                    Name = declaredTerm.Name,
                    EntityName=declaredTerm.EntityName,
                    Descending = term.Descending,
                    Default=declaredTerm.Default
                };

            }

        }

        //use Iqueryable not ienumerable
        //as Iqueryable contains an expression tree that can be transformed in to a database statement like sql and run on server side(database)
        //by adding to that expression tree, we can dynamically the query at runtime
        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();


            if (!terms.Any())
            {
                terms = GetTermsFromModel().Where(t => t.Default).ToArray();
            }
                if (!terms.Any()) return query;

            var modifiedQuery = query;//we may modify query more than once
            var useThenBy = false;//keep track of how many times we called orderby, if we called more than once we have to call  thenby

            foreach(var term in terms)
            {
                var propertyInfo = ExpressionHelper
                    .GetPropertyInfo<TEntity>(term.EntityName??term.Name);//get propety info of the property this term refers to

                var obj = ExpressionHelper.Parameter<TEntity>();//get reference to the entity object

                //Build Linq expression backwards:
                //query= query.OrderBy(x => x.Property) this is our goal

                //we are doing this now x => x.Property
                var key = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);

                //get entire expresion
                var keySelector = ExpressionHelper.GetLambda(typeof(TEntity), propertyInfo.PropertyType, obj, key);

                //query.OrderBy/ThenBy[Descending](x=>x.property)
                modifiedQuery = ExpressionHelper.CallOrderByOrThenBy(modifiedQuery, useThenBy, term.Descending, propertyInfo.PropertyType, keySelector);

                useThenBy = true;
            }

            return modifiedQuery;
        }

        private static IEnumerable<SortTerm> GetTermsFromModel() => typeof(T)
            .GetTypeInfo()//reflection
            .DeclaredProperties
            .Where(p => p.GetCustomAttributes<SortableAttribute>().Any())
            .Select(p => new SortTerm
            {
                Name = p.Name,
                EntityName = p.GetCustomAttribute<SortableAttribute>().EntityProperty,
                Default = p.GetCustomAttribute<SortableAttribute>().Default
            });
        
    }
}
