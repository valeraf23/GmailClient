using Gml.Filters;
using Gml.Interfaces;

namespace Gml
{
    public class By
    {
        private By(ISearchMailFilter filter)
        {
            Filter = filter;
        }

        public ISearchMailFilter Filter { get; }

        public static By Custom(string criteria)
        {
            return new By(new CustomFilter(criteria));
        }

        public static By From(string criteria)
        {
            return new By(new FromFilter(criteria));
        }

        public static By Subject(string criteria)
        {
            return new By(new SubjectFilter(criteria));
        }

        public static By To(string criteria)
        {
            return new By(new ToFilter(criteria));
        }
    }
}