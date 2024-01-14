using UnityEditor;

public static class SettingsMenu
{
    [MenuItem("NWFramework/Settings/NWFrameworkSettings", priority = 1)]
    public static void OpenSettings()
    {
        SettingsService.OpenProjectSettings("NWFramework/NWFrameworkSettings");
    }
}