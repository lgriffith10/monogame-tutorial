namespace MonoGameLibrary.Graphics;

public class Tileset
{
    private readonly TextureRegion[] _tiles;

    public Tileset(TextureRegion textureRegion, int tileWidth, int tileHeight)
    {
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Columns = textureRegion.Width / tileWidth;
        Rows = textureRegion.Height / tileHeight;
        Count = Columns * Rows;

        _tiles = new TextureRegion[Count];

        for (var i = 0; i < Count; i++)
        {
            var x = i % Columns * tileWidth;
            var y = i / Columns * tileHeight;

            _tiles[i] = new TextureRegion(
                texture: textureRegion.Texture,
                x: textureRegion.SourceRectangle.X + x,
                y: textureRegion.SourceRectangle.Y + y,
                width: tileWidth,
                height: tileHeight
            );
        }
    }

    public int TileWidth { get; }
    public int TileHeight { get; }
    public int Columns { get; }
    public int Rows { get; }
    public int Count { get; }

    public TextureRegion GetTile(int index)
    {
        return _tiles[index];
    }

    public TextureRegion GetTile(int column, int row)
    {
        var index = row * Columns + column;
        return _tiles[index];
    }
}