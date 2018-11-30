using Gml.Interfaces;

namespace Gml.Filters
{
    public class SubjectFilter : ISearchMailFilter
    {
        public SubjectFilter(string subject)
        {
            SearchCriteria = subject;
        }

        public string SearchCriteria { get; set; }

        public string GetCondition()
        {
            return $"subject:{SearchCriteria}";
        }
    }
}