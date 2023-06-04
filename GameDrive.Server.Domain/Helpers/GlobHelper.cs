namespace GameDrive.Server.Domain.Helpers;

public static class GlobHelper
{

    public static string GetBasePathFromGlob(string glob)
    {
        var startingPath = glob;

        var questionIndex = glob.IndexOf('?');
        if (questionIndex != -1)
            startingPath = glob[..questionIndex];

        var wildcardIndex = glob.IndexOf('*');
        if (wildcardIndex != -1 && (wildcardIndex <= questionIndex || questionIndex == -1))
            startingPath = glob[..wildcardIndex];

        return startingPath;
    }

    public static string RemoveFileGlobalSuffix(string input)
    {
        var periodIndex = input.LastIndexOf('.');
        var forwardSlashIndex = input.LastIndexOf('/');
        var backSlashIndex = input.LastIndexOf('\\');
        var slashPeak = Math.Max(forwardSlashIndex, backSlashIndex);
        if (slashPeak >= periodIndex)
            return input;

        return slashPeak < 1 // Only substring if the length is greater than zero
            ? input 
            : input[..slashPeak];
    }
    
}