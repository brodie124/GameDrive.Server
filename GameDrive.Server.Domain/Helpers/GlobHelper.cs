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
    
}