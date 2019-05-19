using System;

public class AppSettings
{
    public enum CameraPosition { Outer, Inner };
    public static AppSettings Instance => instanceHolder.Value;
    private static readonly Lazy<AppSettings> instanceHolder =
        new Lazy<AppSettings>(() => new AppSettings());

    public CameraPosition CameraPositionType { get; set; }
    public float InnerCameraFOV { get; set; }
    public float OuterCameraFOV { get; set; }
    public float MenuCameraFOV { get; set; }
    public float ShapeSpeed { get; set; }

    private AppSettings()
    {
        //Загрузка из json
        CameraPositionType = CameraPosition.Inner;
        InnerCameraFOV = 100f;
        OuterCameraFOV = 60f;
        MenuCameraFOV = 40f;
        ShapeSpeed = 1f;
    }


}