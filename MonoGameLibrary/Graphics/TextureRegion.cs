using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TextureRegion
{
    public TextureRegion()
    {
    }

    public TextureRegion(
        Texture2D texture,
        int x,
        int y,
        int width,
        int height
    )
    {
        Texture = texture;
        SourceRectangle = new Rectangle(x: x, y: y, width: width, height: height);
    }

    public Texture2D Texture { get; set; }
    public Rectangle SourceRectangle { get; set; }

    public int Width => SourceRectangle.Width;
    public int Height => SourceRectangle.Height;

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        Draw(spriteBatch: spriteBatch, position: position, color: color, rotation: 0.0f, origin: Vector2.Zero, scale: Vector2.One, effects: SpriteEffects.None, layerDepth: 0.0f);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        Draw(
            spriteBatch: spriteBatch,
            position: position,
            color: color,
            rotation: rotation,
            origin: origin,
            scale: new Vector2(x: scale, y: scale),
            effects: effects,
            layerDepth: layerDepth
        );
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(
            texture: Texture,
            position: position,
            sourceRectangle: SourceRectangle,
            color: color,
            rotation: rotation,
            origin: origin,
            scale: scale,
            effects: effects,
            layerDepth: layerDepth
        );
    }
}