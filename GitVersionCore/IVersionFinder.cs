namespace GitVersion
{
    public interface IVersionFinder
    {
        SemanticVersion FindVersion(GitVersionContext context);
    }
}