using System.IO;

public class Character
{
    public string Name { get; set; }
    public string RootUUID { get; set; }
    public string VariantUUID { get; set; }
    public string ImagePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images", "Characters", $"{Name}.jpg");
}
