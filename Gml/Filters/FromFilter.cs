using Gml.Interfaces;

namespace Gml.Filters
{
    public class FromFilter : ISearchMailFilter
    {
        public FromFilter(string from)
        {
            SearchCriteria = from;
        }

        public string SearchCriteria { get; set; }

        public string GetCondition()
        {
            return $"from:{SearchCriteria}";
        }
    }
}