using UnityEngine;

public enum ColorId
{
    Unknown = 0,
    Red = 1,
    Green = 2,
    Blue = 3,
    Colored = 4
}

public static class ColorMaterialUtils
{
    public static int FindMutableMaterialIndex(Renderer renderer, params string[] slotHints)
    {
        if (renderer == null) return -1;

        Material[] shared = renderer.sharedMaterials;
        for (int i = 0; i < shared.Length; i++)
        {
            if (ContainsAny(shared[i]?.name, slotHints))
                return i;
        }

        Material[] runtime = renderer.materials;
        for (int i = 0; i < runtime.Length; i++)
        {
            if (ContainsAny(runtime[i]?.name, slotHints))
                return i;
        }

        return runtime.Length > 0 ? 0 : -1;
    }

    public static ColorId GetColorId(Material material)
    {
        if (material == null) return ColorId.Unknown;

        string name = material.name.ToLower();
        if (name.Contains("red")) return ColorId.Red;
        if (name.Contains("green")) return ColorId.Green;
        if (name.Contains("blue")) return ColorId.Blue;
        if (name.Contains("colored")) return ColorId.Colored;
        return ColorId.Unknown;
    }

    private static bool ContainsAny(string source, string[] hints)
    {
        if (string.IsNullOrEmpty(source) || hints == null || hints.Length == 0) return false;

        string normalized = source.ToLower();
        for (int i = 0; i < hints.Length; i++)
        {
            if (!string.IsNullOrEmpty(hints[i]) && normalized.Contains(hints[i].ToLower()))
                return true;
        }

        return false;
    }
}
