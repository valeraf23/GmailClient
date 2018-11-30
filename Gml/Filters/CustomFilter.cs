using Gml.Interfaces;

namespace Gml.Filters
{
    public class CustomFilter : ISearchMailFilter
    {
        public CustomFilter(string searchCriteria)
        {
            SearchCriteria = searchCriteria;
        }

        public string SearchCriteria { get; set; }

        public string GetCondition()
        {
            return SearchCriteria;
        }
    }
}