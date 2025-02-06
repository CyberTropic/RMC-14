using Robust.Shared.Map.Components;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.DayNight;

public sealed class RMCDayNightSystem : EntitySystem
{

    [Dependency] private readonly INetManager _net = default!;

    public List<string> coldSunset =
    [
        "#a8c3cf",
        "#7a9abb",
        "#6679a8",
        "#516a8b",
        "#38486e",
        "#2c2f4d",
        "#211b36",
        "#1f1b33",
        "#0c0a1b",
        "#000000",
    ];
    public List<string> warmSunset =
    [
        "#e3a979",
        "#e29658",
        "#da8b4a",
        "#a9633c",
        "#90422d",
        "#68333a",
        "#4d2b35",
        "#231935",
        "#050c27",
        "#000000",
    ];
    public List<string> warmSunrise =
    [
        "#000000",
        "#040712",
        "#111322",
        "#291642",
        "#3f2239",
        "#632c3d",
        "#b97034",
    ];

    public void LightTransition(RMCDayNightCycleComponent dayNight, List<string> transition, float duration)
    {
        dayNight.TransitionValues = [];
        for (var i = 0; i < transition.Count; i++)
        {
            dayNight.TransitionValues.Add(new() {ColorHex = transition[i], Time = (float)i / transition.Count * duration});
        }
        dayNight.TotalTransitionTime = TimeSpan.FromSeconds(duration);
        dayNight.RemainingTransitionTime = dayNight.TotalTransitionTime;

    }

    public override void Update(float frameTime)
    {
        if (_net.IsClient)
            return;

        var query = EntityQueryEnumerator<RMCDayNightCycleComponent, MapLightComponent>();
        while (query.MoveNext(out var uid, out var dayNight, out var mapLight))
        {
            dayNight.RemainingTransitionTime -= TimeSpan.FromSeconds(frameTime);

            if (dayNight.RemainingTransitionTime <= TimeSpan.Zero)
            {
                dayNight.RemainingTransitionTime = TimeSpan.Zero;
                continue;
            }

            var progress = dayNight.RemainingTransitionTime / dayNight.TotalTransitionTime;
            // Previous color = value before t, next color = value after t
            var currentColor = InterpolateHexColors(dayNight.PreviousColor, dayNight.NextColor, (float)progress);

            mapLight.AmbientLightColor = currentColor;
            Dirty(uid, mapLight);
            Dirty(uid, dayNight);

        }
    }

    private Color InterpolateHexColors(string hexColor1, string hexColor2, float t)
    {
        Color color1 = Color.FromHex(hexColor1);
        Color color2 = Color.FromHex(hexColor2);

        float r = color1.R + (color2.R - color1.R) * t;
        float g = color1.G + (color2.G - color1.G) * t;
        float b = color1.B + (color2.B - color1.B) * t;

        return new Color(r, g, b);
    }

}
