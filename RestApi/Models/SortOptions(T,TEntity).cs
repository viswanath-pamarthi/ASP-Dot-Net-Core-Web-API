using RestApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{    
    /// <summary>
    ///  Model for holding query string arguments that are passed to controller from client
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class SortOptions<T, TEntity> : IValidatableObject//interface that asp.net core understands
    {
        //When a request comes in from Api, asp.net core mvc will automatically binds any orderby 
        //parameters from query string to this orderby property
        public string[] OrderBy { get; set; }
        /// <summary>
        /// Asp.Net coer calls this to validate the incoming parameters
        /// 
        /// As IValidatableObject interface is implemented , mvc will automatically calls this validate method. we can 
        /// validate the sort terms here and let mvc know if parameters passed by client are valid
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var processor = new SortOptionsProcessor<T, TEntity>(OrderBy);

            var validTerms = processor.GetValidTerms().Select(x => x.Name);
            var invalidTerms = processor.GetAllTerms().Select(x => x.Name).Except(validTerms, StringComparer.OrdinalIgnoreCase);

            foreach(var term in invalidTerms)
            {
                yield return new ValidationResult($"Invalid sort term'{term}'", new[] { nameof(OrderBy) });
            }
        }

        /// <summary>
        /// Service code will call this to apply these sort options to a database query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            //adding sort to a query with linq is simple as
            // query.OrderBy().ThenByDescending() - this only works if you know the sort criteria at compile time, we need to apply
            //different sort criteria depending on the query string of the request which is trickier

            var processor = new SortOptionsProcessor<T, TEntity>(OrderBy);

            return processor.Apply(query);
            
        }
    }
}
