namespace GitVersion
{
    using System.Linq;
    using LibGit2Sharp;

    class ReleaseVersionFinder 
    {
        public SemanticVersion FindVersion(GitVersionContext context)
        {
            var versionString = GetSuffix(context.CurrentBranch);
            var shortVersion = SemanticVersion.Parse(versionString, context.Configuration.TagPrefix);

            EnsureVersionIsValid(shortVersion, context.CurrentBranch);

            var applicableTagsInDescendingOrder = context.Repository.SemVerTagsRelatedToVersion(context.Configuration, shortVersion).OrderByDescending(tag => SemanticVersion.Parse(tag.Name, context.Configuration.TagPrefix)).ToList();
            var numberOfCommitsSinceLastTagOrBranchPoint = BranchCommitDifferenceFinder.NumberOfCommitsSinceLastTagOrBranchPoint(context, applicableTagsInDescendingOrder, BranchType.Release, "develop");
            var semanticVersionPreReleaseTag = RecentTagVersionExtractor.RetrieveMostRecentOptionalTagVersion(context, applicableTagsInDescendingOrder) ?? context.Configuration.ReleaseBranchTag + ".1";

            return new SemanticVersion
            {
                Major = shortVersion.Major,
                Minor = shortVersion.Minor,
                Patch = shortVersion.Patch,
                PreReleaseTag = semanticVersionPreReleaseTag,
                BuildMetaData = new SemanticVersionBuildMetaData(numberOfCommitsSinceLastTagOrBranchPoint, context.CurrentBranch.Name, context.CurrentCommit.Sha, context.CurrentCommit.When())
            };
        }

        static void EnsureVersionIsValid(SemanticVersion version, Branch branch)
        {
        }

        static string GetSuffix(Branch branch)
        {
            return branch.Name.TrimStart("release-").TrimStart("release/");
        }
    }
}
