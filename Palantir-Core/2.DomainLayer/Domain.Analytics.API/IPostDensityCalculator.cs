namespace Ix.Palantir.Domain.Analytics.API
{
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;

    public interface IPostDensityCalculator
    {
        IList<PostDensity> GetMostCrowdedTime(IList<Post> posts);
    }
}