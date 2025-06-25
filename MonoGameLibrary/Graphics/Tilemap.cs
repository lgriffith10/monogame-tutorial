using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Tilemap
{
    private readonly int[] _tiles;
    private readonly Tileset _tileset;

    public Tilemap(Tileset tileset, int rows, int columns)
    {
        _tileset = tileset;
        Rows = rows;
        Columns = columns;
        Count = Columns * Rows;
        Scale = Vector2.One;
        _tiles = new int[Count];
    }

    public int Rows { get; }
    public int Columns { get; }
    public int Count { get; }
    public Vector2 Scale { get; set; }

    public float TileWidth => _tileset.TileWidth * Scale.X;
    public float TileHeight => _tileset.TileHeight * Scale.Y;

    public void SetTile(int index, int tilesetId)
    {
        _tiles[index] = tilesetId;
    }

    public void SetTile(int column, int row, int tilesetId)
    {
        var index = row * Columns + column;
        SetTile(index: index, tilesetId: tilesetId);
    }

    public TextureRegion GetTile(int index)
    {
        return _tileset.GetTile(index);
    }

    public TextureRegion GetTile(int column, int row)
    {
        var index = row * Columns + column;
        return _tileset.GetTile(index);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Count; i++)
        {
            var tilesetIndex = _tiles[i];
            var tile = _tileset.GetTile(tilesetIndex);

            var x = i % Columns;
            var y = i / Columns;

            var position = new Vector2(x: x * TileWidth, y: y * TileHeight);
            tile.Draw(
                spriteBatch: spriteBatch,
                position: position,
                color: Color.White,
                rotation: 0.0f,
                origin: Vector2.Zero,
                scale: Scale,
                effects: SpriteEffects.None,
                layerDepth: 0.0f
            );
        }
    }

    /// <summary>
    ///     Creates a new tilemap based on a tilemap xml configuration file.
    /// </summary>
    /// <param name="content">The content manager used to load the texture for the tileset.</param>
    /// <param name="filename">The path to the xml file, relative to the content root directory.</param>
    /// <returns>The tilemap created by this method.</returns>
    public static Tilemap FromFile(ContentManager content, string filename)
    {
        var filePath = Path.Combine(path1: content.RootDirectory, path2: filename);

        using (var stream = TitleContainer.OpenStream(filePath))
        {
            using (var reader = XmlReader.Create(stream))
            {
                var doc = XDocument.Load(reader);
                var root = doc.Root;

                // The <Tileset> element contains the information about the tileset
                // used by the tilemap.
                //
                // Example
                // <Tileset region="0 0 100 100" tileWidth="10" tileHeight="10">contentPath</Tileset>
                //
                // The region attribute represents the x, y, width, and height
                // components of the boundary for the texture region within the
                // texture at the contentPath specified.
                //
                // the tileWidth and tileHeight attributes specify the width and
                // height of each tile in the tileset.
                //
                // the contentPath value is the contentPath to the texture to
                // load that contains the tileset
                var tilesetElement = root.Element("Tileset");

                var regionAttribute = tilesetElement.Attribute("region").Value;
                var split = regionAttribute.Split(separator: " ", options: StringSplitOptions.RemoveEmptyEntries);
                var x = int.Parse(split[0]);
                var y = int.Parse(split[1]);
                var width = int.Parse(split[2]);
                var height = int.Parse(split[3]);

                var tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                var tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                var contentPath = tilesetElement.Value;

                // Load the texture 2d at the content path
                var texture = content.Load<Texture2D>(contentPath);

                // Create the texture region from the texture
                var textureRegion = new TextureRegion(texture: texture, x: x, y: y, width: width, height: height);

                // Create the tileset using the texture region
                var tileset = new Tileset(textureRegion: textureRegion, tileWidth: tileWidth, tileHeight: tileHeight);

                // The <Tiles> element contains lines of strings where each line
                // represents a row in the tilemap.  Each line is a space
                // separated string where each element represents a column in that
                // row.  The value of the column is the id of the tile in the
                // tileset to draw for that location.
                //
                // Example:
                // <Tiles>
                //      00 01 01 02
                //      03 04 04 05
                //      03 04 04 05
                //      06 07 07 08
                // </Tiles>
                var tilesElement = root.Element("Tiles");

                // Split the value of the tiles data into rows by splitting on
                // the new line character
                var rows = tilesElement.Value.Trim().Split(separator: '\n', options: StringSplitOptions.RemoveEmptyEntries);

                // Split the value of the first row to determine the total number of columns
                var columnCount = rows[0].Split(separator: " ", options: StringSplitOptions.RemoveEmptyEntries).Length;

                // Create the tilemap
                var tilemap = new Tilemap(tileset: tileset, rows: rows.Length, columns: columnCount);

                // Process each row
                for (var row = 0; row < rows.Length; row++)
                {
                    // Split the row into individual columns
                    var columns = rows[row].Trim().Split(separator: " ", options: StringSplitOptions.RemoveEmptyEntries);

                    // Process each column of the current row
                    for (var column = 0; column < columnCount; column++)
                    {
                        // Get the tileset index for this location
                        var tilesetIndex = int.Parse(columns[column]);

                        // Get the texture region of that tile from the tileset
                        var region = tileset.GetTile(tilesetIndex);

                        // Add that region to the tilemap at the row and column location
                        tilemap.SetTile(column: column, row: row, tilesetId: tilesetIndex);
                    }
                }

                return tilemap;
            }
        }
    }
}