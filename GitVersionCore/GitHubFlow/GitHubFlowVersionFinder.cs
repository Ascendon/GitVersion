namespace GitVersion
{
    public class GitHubFlowVersionFinder : IVersionFinder
    {
        public SemanticVersion FindVersion(GitVersionContext context)
        {
            var repositoryDirectory = context.Repository.GetRepositoryDirectory();
            var lastTaggedReleaseFinder = new LastTaggedReleaseFinder(context);
            var nextVersionTxtFileFinder = new NextVersionTxtFileFinder(repositoryDirectory, context.Configuration);
            var nextSemverCalculator = new NextSemverCalculator(nextVersionTxtFileFinder, lastTaggedReleaseFinder, context);
            return new BuildNumberCalculator(nextSemverCalculator, lastTaggedReleaseFinder, context.Repository).GetBuildNumber(context);
        }
    }
}