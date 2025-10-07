using System.IO;
using System.Windows;

namespace HorizonSwapper.Services
{
    public static class CharacterOverrideService
    {
        private const string IniFileName = "mod_HorizonSwapper.ini";

        public static void WriteCharacterOverride(string gameDirectory, string rootUUID, string variantUUID)
        {
            try
            {
                if (string.IsNullOrEmpty(gameDirectory))
                    throw new ArgumentException("Game directory cannot be null or empty.");

                if (string.IsNullOrEmpty(rootUUID) || string.IsNullOrEmpty(variantUUID))
                    throw new ArgumentException("RootUUID or VariantUUID cannot be null or empty.");

                string iniPath = Path.Combine(gameDirectory, IniFileName);

                // Create the content
                string content =
                $@"[[CharacterOverride]]
RootUUID = ""{rootUUID}""
VariantUUID = ""{variantUUID}""";

                // Ensure the directory exists (just in case)
                Directory.CreateDirectory(gameDirectory);

                // Write to file (overwrite if exists)
                File.WriteAllText(iniPath, content);
            }
            catch (Exception ex)
            {
                // Handle any write errors gracefully
                System.Windows.MessageBox.Show(
                    $"Failed to create character swap:\n{ex.Message}",
                    "File Write Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
        }

        public static void RemoveCharacterOverrideFile(string gameDirectory)
        {
            try
            {
                if (string.IsNullOrEmpty(gameDirectory))
                    return;

                string iniPath = Path.Combine(gameDirectory, IniFileName);

                if (File.Exists(iniPath))
                {
                    File.Delete(iniPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to remove character swap:\n{ex.Message}",
                    "File Delete Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
