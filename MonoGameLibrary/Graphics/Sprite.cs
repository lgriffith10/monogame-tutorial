using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Sprite
{
    public Sprite()
    {
    }

    public Sprite(TextureRegion region)
    {
        Region = region;
    }

    public TextureRegion Region { get; set; }
    public Color Color { get; set; } = Color.White;
    public float Rotation { get; set; } = 0.0f;
    public Vector2 Scale { get; set; } = Vector2.One;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;
    public float LayerDepth { get; set; } = 0.0f;

    public float Width => Region.Width * Scale.X;
    public float Height => Region.Height * Scale.Y;

    public void CenterOrigin()
    {
        Origin = new Vector2(x: Width, y: Height) * 0.5f;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Region.Draw(
            spriteBatch: spriteBatch,
            position: position,
            color: Color,
            rotation: Rotation,
            origin: Origin,
            scale: Scale,
            effects: Effects,
            layerDepth: LayerDepth
        );
    }
}