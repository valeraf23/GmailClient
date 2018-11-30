using Gml.Interfaces;

namespace Gml.Filters
{
    public class ToFilter : ISearchMailFilter
    {
        public ToFilter(string to)
        {
            Address = to;
        }

        public string Address { get; set; }

        public string GetCondition()
        {
            return string.Format("to:{0}", Address);
        }
    }
}