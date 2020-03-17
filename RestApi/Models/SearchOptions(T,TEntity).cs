using RestApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    public class SearchOptions<T, TEntity> : IValidatableObject
    {
        //Any search parameters in the query string of incoming request will be bound to this search array
        public string[] Search { get; set; }

        //validate the search terms gave by client are valid
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var processor = new SearchOptionsProcessor<T, TEntity>(Search);

            var validTerms = processor.GetValidTerms().Select(x => x.Name);
            var invalidTerms = processor.GetAllTerms().Select(x => x.Name)
                .Except(validTerms, StringComparer.OrdinalIgnoreCase);


            foreach (var term in invalidTerms)
            {
                yield return new ValidationResult(
                    $"Invalid search term '{term}'.",
                    new[] { nameof(Search) });
            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            //query = query
            var processor = new SearchOptionsProcessor<T, TEntity>(Search);
            return processor.Apply(query);
        }
    }
}
