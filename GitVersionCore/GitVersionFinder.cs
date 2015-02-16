namespace GitVersion
{
    using System.Linq;
    using LibGit2Sharp;

    public class GitVersionFinder
    {
        public SemanticVersion FindVersion(GitVersionContext context)
        {
            Logger.WriteInfo("Running against branch: " + context.CurrentBranch.Name);
            EnsureMainTopologyConstraints(context);

            IVersionFinder versionFinder = BuildVersionFinder(context);
            return versionFinder.FindVersion(context);
        }

        IVersionFinder BuildVersionFinder(GitVersionContext context)
        {
            if(context.Configuration.Strategy != null)
            {
                switch(context.Configuration.Strategy)
                {
                    case "GitFlow":
                        Logger.WriteInfo("GitFlow version strategy will be used");
                        return new GitFlowVersionFinder();
                    case "GitHubFlow":
                        Logger.WriteInfo("GitHubFlow version strategy will be used");
                        return new GitHubFlowVersionFinder();
                    default:
                        throw new WarningException(string.Format("Invalid Strategy specified ({0}). Allowed values are: GitFlow, GitHubFlow or empty", context.Configuration.Strategy));
                }
            }
            else if (ShouldGitHubFlowVersioningSchemeApply(context.Repository))
            {
                Logger.WriteInfo("GitHubFlow version strategy will be used");
                return new GitHubFlowVersionFinder();
            }
            else
            {
                Logger.WriteInfo("GitFlow version strategy will be used");
                return new GitFlowVersionFinder();
            }
        }

        static bool ShouldGitHubFlowVersioningSchemeApply(IRepository repo)
        {
            return repo.FindBranch("develop") == null;
        }

        void EnsureMainTopologyConstraints(GitVersionContext context)
        {
            EnsureLocalBranchExists(context.Repository, "master");
            // TODO somehow enforce this? EnsureLocalBranchExists(context.Repository, "develop");
            EnsureHeadIsNotDetached(context);
        }

        void EnsureHeadIsNotDetached(GitVersionContext context)
        {
            if (!context.CurrentBranch.IsDetachedHead())
            {
                return;
            }

            var message = string.Format("It looks like the branch being examined is a detached Head pointing to commit '{0}'. Without a proper branch name GitVersion cannot determine the build version.", context.CurrentCommit.Id.ToString(7));
            throw new WarningException(message);
        }

        void EnsureLocalBranchExists(IRepository repository, string branchName)
        {
            if (repository.FindBranch(branchName) != null)
            {
                return;
            }

            var existingBranches = string.Format("'{0}'", string.Join("', '", repository.Branches.Select(x => x.CanonicalName)));
            throw new WarningException(string.Format("This repository doesn't contain a branch named '{0}'. Please create one. Existing branches: {1}", branchName, existingBranches));
        }
    }
}