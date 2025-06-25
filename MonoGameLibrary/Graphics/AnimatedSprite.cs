using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite
{
    private Animation _animation;
    private int _currentFrame;
    private TimeSpan _elapsedTime;

    public AnimatedSprite()
    {
    }

    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }

    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[index: 0];
        }
    }

    public void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime;

        if (_elapsedTime >= Animation.Delay)
        {
            _elapsedTime -= Animation.Delay;
            _currentFrame++;

            if (_currentFrame >= Animation.Frames.Count) _currentFrame = 0;

            Region = Animation.Frames[index: _currentFrame];
        }
    }
}