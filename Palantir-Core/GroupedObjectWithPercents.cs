using System;
using Ix.Palantir.Querying.Common;

namespace Ix.Palantir.Querying.Common
{
    public class GroupedObjectWithPercents<T> : GroupedObject<T>
    {
        public double PercentsFromAllUsers { get; set; }
        public double PerecentsFromActiveUsers { get; set; }
    }
}