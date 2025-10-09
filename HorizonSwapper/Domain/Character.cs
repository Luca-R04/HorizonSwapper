using System.IO;

public class Character
{
    public string OriginalRootUUID { get; set; }
    public string Name { get; set; }
    public string VariantUUID { get; set; }
    public string ImagePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images", "Characters", $"{Name}.jpg");
}
