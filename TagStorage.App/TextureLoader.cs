using System.Collections.Generic;
using System.IO;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;

namespace TagStorage.App;

public static class TextureLoader
{
    private static Dictionary<string, Texture> textures = new();

    public static Texture LoadTexture(GameHost host, string filename)
    {
        if (!textures.TryGetValue(filename, out Texture texture))
        {
            using var fileStream = File.OpenRead(filename);
            texture = Texture.FromStream(host.Renderer, fileStream);
            textures.Add(filename, texture);
        }

        return texture;
    }
}
