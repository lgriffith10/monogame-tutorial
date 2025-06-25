using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{
    private readonly Dictionary<string, Animation> _animations;
    private readonly Dictionary<string, TextureRegion> _regions;

    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    public TextureAtlas(Texture2D texture)
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
        Texture = texture;
    }

    public Texture2D Texture { get; set; }

    public void AddRegion(string name, int x, int y, int width, int height)
    {
        var region = new TextureRegion(
            texture: Texture,
            x: x,
            y: y,
            width: width,
            height: height
        );

        _regions.Add(key: name, value: region);
    }

    public TextureRegion GetRegion(string name)
    {
        return _regions[key: name];
    }

    public bool RemoveRegion(string name)
    {
        return _regions.Remove(key: name);
    }

    public void AddAnimation(string name, Animation animation)
    {
        _animations.Add(key: name, value: animation);
    }

    public Animation GetAnimation(string name)
    {
        return _animations[key: name];
    }

    public bool RemoveAnimation(string name)
    {
        return _animations.Remove(key: name);
    }

    public void Clear()
    {
        _regions.Clear();
    }

    public Sprite CreateSprite(string regionName)
    {
        var region = GetRegion(name: regionName);
        return new Sprite(region: region);
    }

    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        var animation = GetAnimation(name: animationName);
        return new AnimatedSprite(animation: animation);
    }

    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        var atlas = new TextureAtlas();

        var filePath = Path.Combine(path1: content.RootDirectory, path2: fileName);

        using (var stream = TitleContainer.OpenStream(name: filePath))
        {
            using (var reader = XmlReader.Create(input: stream))
            {
                var doc = XDocument.Load(reader: reader);
                var root = doc.Root;

                var texturePath = root.Element(name: "Texture").Value;
                atlas.Texture = content.Load<Texture2D>(assetName: texturePath);

                var regions = root.Element(name: "Regions")?.Elements(name: "Region");

                if (regions != null)
                    foreach (var region in regions)
                    {
                        var name = region.Attribute(name: "name")?.Value;
                        var x = int.Parse(s: region.Attribute(name: "x")?.Value ?? "0");
                        var y = int.Parse(s: region.Attribute(name: "y")?.Value ?? "0");
                        var width = int.Parse(s: region.Attribute(name: "width")?.Value ?? "0");
                        var height = int.Parse(s: region.Attribute(name: "height")?.Value ?? "0");

                        if (!string.IsNullOrEmpty(value: name))
                            atlas.AddRegion(name: name, x: x, y: y, width: width, height: height);
                    }

                var animations = root.Element(name: "Animations")?.Elements(name: "Animation");

                if (animations != null)
                    foreach (var animation in animations)
                    {
                        var name = animation.Attribute(name: "name")?.Value;
                        var delay = TimeSpan.FromMilliseconds(
                            value: double.Parse(s: animation.Attribute(name: "delay")?.Value ?? "100")
                        );

                        var frames = new List<TextureRegion>();

                        foreach (var frame in animation.Elements(name: "Frame"))
                        {
                            var regionName = frame.Attribute(name: "region")!.Value;
                            var region = atlas.GetRegion(name: regionName);
                            frames.Add(item: region);
                        }

                        if (string.IsNullOrEmpty(value: name)) continue;
                        var anim = new Animation(frames: frames, delay: delay);
                        atlas.AddAnimation(name: name, animation: anim);
                    }

                return atlas;
            }
        }
    }
}