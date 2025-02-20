namespace DevSettings.Interfaces
{
    public interface IBuildInfo
    {
        string BuildPresetName { get; }

        string BranchName { get; }

        int BuildNumber { get; }
    }
}
