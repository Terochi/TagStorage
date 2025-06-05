using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;
using osuTK;

namespace TagStorage.App.Selector;

public partial class FileSelectionItem(string item) : DirectorySelectionItem(item)
{
    protected override Drawable CreateIcon(GameHost host)
    {
        try
        {
            Texture texture = TextureLoader.LoadTexture(host, FullName);
            float scale = 20f / Math.Max(texture.Width, texture.Height);
            int w = (int)Math.Floor(scale * texture.Width);
            int h = (int)Math.Floor(scale * texture.Height);
            return new Sprite
            {
                Texture = texture,
                Size = new Vector2(w, h)
            };
        }
        catch (Exception _)
        {
            return new SpriteIcon
            {
                Icon = FontAwesome.Solid.File,
                Size = new Vector2(20)
            };
        }
    }
}
