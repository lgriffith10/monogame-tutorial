using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace MonogameTutorial;

public class Game1 : Core
{
    private const float MOVEMENT_SPEED = 5.0f;

    private AnimatedSprite _bat;

    private Vector2 _batPosition;
    private Vector2 _batVelocity;
    private AnimatedSprite _slime;

    private Vector2 _slimePosition;

    public Game1() : base(title: "Monogame Tutorial", width: 800, height: 600, isFullScreen: false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();

        _batPosition = new Vector2(x: _slime.Width + 10, y: 0);
        AssignRandomVelocityToBat();
    }

    protected override void LoadContent()
    {
        var atlasTexture = Content.Load<Texture2D>(assetName: "images/atlas");
        var atlas = TextureAtlas.FromFile(content: Content, fileName: "images/atlas-definition.xml");

        _slime = atlas.CreateAnimatedSprite(animationName: "slime-animation");
        _slime.Scale = new Vector2(x: 4.0f, y: 4.0f);

        _bat = atlas.CreateAnimatedSprite(animationName: "bat-animation");
        _bat.Scale = new Vector2(x: 4.0f, y: 4.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(playerIndex: PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(key: Keys.Escape))
            Exit();

        _slime.Update(gameTime: gameTime);
        _bat.Update(gameTime: gameTime);

        CheckKeyboardInput();
        CheckGamePadInput();

        // Create a bounding rectangle for the screen
        var screenBounds = new Rectangle(
            x: 0,
            y: 0,
            width: GraphicsDevice.PresentationParameters.BackBufferWidth,
            height: GraphicsDevice.PresentationParameters.BackBufferHeight
        );

        // Creating a bounding circle for the slime
        var slimeBounds = new Circle(
            x: (int)(_slimePosition.X + _slime.Width * 0.5f),
            y: (int)(_slimePosition.Y + _slime.Height * 0.5f),
            radius: (int)(_slime.Width * 0.5f)
        );

        // Use distance based checks to determine if the slime is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // move it back inside.
        if (slimeBounds.Left < screenBounds.Left)
            _slimePosition.X = screenBounds.Left;
        else if (slimeBounds.Right > screenBounds.Right) _slimePosition.X = screenBounds.Right - _slime.Width;

        if (slimeBounds.Top < screenBounds.Top)
            _slimePosition.Y = screenBounds.Top;
        else if (slimeBounds.Bottom > screenBounds.Bottom) _slimePosition.Y = screenBounds.Bottom - _slime.Height;

        // Calculate the new position of the bat based on the velocity
        var newBatPosition = _batPosition + _batVelocity;

        // Create a bounding circle for the bat
        var batBounds = new Circle(
            x: (int)(newBatPosition.X + _bat.Width * 0.5f),
            y: (int)(newBatPosition.Y + _bat.Height * 0.5f),
            radius: (int)(_bat.Width * 0.5f)
        );

        var normal = Vector2.Zero;

        // Use distance based checks to determine if the bat is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // reflect it about the screen edge normal
        if (batBounds.Left < screenBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newBatPosition.X = screenBounds.Left;
        }
        else if (batBounds.Right > screenBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newBatPosition.X = screenBounds.Right - _bat.Width;
        }

        if (batBounds.Top < screenBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newBatPosition.Y = screenBounds.Top;
        }
        else if (batBounds.Bottom > screenBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newBatPosition.Y = screenBounds.Bottom - _bat.Height;
        }

        // If the normal is anything but Vector2.Zero, this means the bat had
        // moved outside the screen edge so we should reflect it about the
        // normal.
        if (normal != Vector2.Zero) _batVelocity = Vector2.Reflect(vector: _batVelocity, normal: normal);

        _batPosition = newBatPosition;

        if (slimeBounds.Intersects(other: batBounds))
        {
            // Divide the width  and height of the screen into equal columns and
            // rows based on the width and height of the bat.
            var totalColumns = GraphicsDevice.PresentationParameters.BackBufferWidth / (int)_bat.Width;
            var totalRows = GraphicsDevice.PresentationParameters.BackBufferHeight / (int)_bat.Height;

            // Choose a random row and column based on the total number of each
            var column = Random.Shared.Next(minValue: 0, maxValue: totalColumns);
            var row = Random.Shared.Next(minValue: 0, maxValue: totalRows);

            // Change the bat position by setting the x and y values equal to
            // the column and row multiplied by the width and height.
            _batPosition = new Vector2(x: column * _bat.Width, y: row * _bat.Height);

            // Assign a new random velocity to the bat
            AssignRandomVelocityToBat();
        }

        base.Update(gameTime: gameTime);
    }

    private void AssignRandomVelocityToBat()
    {
        // Generate a random angle
        var angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        var x = (float)Math.Cos(d: angle);
        var y = (float)Math.Sin(a: angle);
        var direction = new Vector2(x: x, y: y);

        // Multiply the direction vector by the movement speed
        _batVelocity = direction * MOVEMENT_SPEED;
    }


    private void CheckKeyboardInput()
    {
        var speed = MOVEMENT_SPEED;

        if (Input.Keyboard.IsKeyDown(key: Keys.Space)) speed *= 1.5f;

        if (Input.Keyboard.IsKeyDown(key: Keys.Z) || Input.Keyboard.IsKeyDown(key: Keys.Up)) _slimePosition.Y -= speed;

        if (Input.Keyboard.IsKeyDown(key: Keys.S) || Input.Keyboard.IsKeyDown(key: Keys.Down)) _slimePosition.Y += speed;

        if (Input.Keyboard.IsKeyDown(key: Keys.Q) || Input.Keyboard.IsKeyDown(key: Keys.Left)) _slimePosition.X -= speed;

        if (Input.Keyboard.IsKeyDown(key: Keys.D) || Input.Keyboard.IsKeyDown(key: Keys.Right)) _slimePosition.X += speed;
    }

    private void CheckGamePadInput()
    {
        var gamePadOne = Input.GamePads[(int)PlayerIndex.One];

        var speed = MOVEMENT_SPEED;
        if (gamePadOne.IsButtonDown(button: Buttons.A))
        {
            speed *= 1.5f;
            gamePadOne.SetVibration(strength: 1.0f, duration: TimeSpan.FromSeconds(value: 1));
        }
        else
        {
            gamePadOne.StopVibration();
        }

        if (gamePadOne.LeftThumbStick != Vector2.Zero)
        {
            _slimePosition.X += gamePadOne.LeftThumbStick.X * speed;
            _slimePosition.Y -= gamePadOne.LeftThumbStick.Y * speed;
        }
        else
        {
            if (gamePadOne.IsButtonDown(button: Buttons.DPadUp)) _slimePosition.Y -= speed;
            if (gamePadOne.IsButtonDown(button: Buttons.DPadDown)) _slimePosition.Y += speed;
            if (gamePadOne.IsButtonDown(button: Buttons.DPadLeft)) _slimePosition.X -= speed;
            if (gamePadOne.IsButtonDown(button: Buttons.DPadRight)) _slimePosition.X += speed;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(color: Color.CornflowerBlue);

        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

        _slime.Draw(spriteBatch: SpriteBatch, position: _slimePosition);
        _bat.Draw(
            spriteBatch: SpriteBatch,
            position: _batPosition
        );

        SpriteBatch.End();

        base.Draw(gameTime: gameTime);
    }
}