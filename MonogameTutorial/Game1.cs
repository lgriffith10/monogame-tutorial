using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace MonogameTutorial;

public class Game1 : Core
{

    // Speed multiplier when moving.
    private const float MovementSpeed = 5.0f;

    // Defines the bat animated sprite.
    private AnimatedSprite _bat;

    // Tracks the position of the bat.
    private Vector2 _batPosition;

    // Tracks the velocity of the bat.
    private Vector2 _batVelocity;

    // Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _roomBounds;

    // Defines the slime animated sprite.
    private AnimatedSprite _slime;

    // Tracks the position of the slime.
    private Vector2 _slimePosition;

    // Defines the tilemap to draw.
    private Tilemap _tilemap;

    public Game1() : base(title: "Dungeon Slime", width: 1280, height: 720, isFullScreen: false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();

        var screenBounds = GraphicsDevice.PresentationParameters.Bounds;

        _roomBounds = new Rectangle(
            x: (int)_tilemap.TileWidth,
            y: (int)_tilemap.TileHeight,
            width: screenBounds.Width - (int)_tilemap.TileWidth * 2,
            height: screenBounds.Height - (int)_tilemap.TileHeight * 2
        );

        // Initial slime position will be the center tile of the tile map.
        var centerRow = _tilemap.Rows / 2;
        var centerColumn = _tilemap.Columns / 2;
        _slimePosition = new Vector2(x: centerColumn * _tilemap.TileWidth, y: centerRow * _tilemap.TileHeight);

        // Initial bat position will the in the top left corner of the room
        _batPosition = new Vector2(x: _roomBounds.Left, y: _roomBounds.Top);

        // Assign the initial random velocity to the bat.
        AssignRandomBatVelocity();
    }

    protected override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        var atlas = TextureAtlas.FromFile(content: Content, fileName: "images/atlas-definition.xml");

        // Create the slime animated sprite from the atlas.
        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(x: 4.0f, y: 4.0f);

        // Create the bat animated sprite from the atlas.
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(x: 4.0f, y: 4.0f);

        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(content: Content, filename: "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(x: 4.0f, y: 4.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update the slime animated sprite.
        _slime.Update(gameTime);

        // Update the bat animated sprite.
        _bat.Update(gameTime);

        // Check for keyboard input and handle it.
        CheckKeyboardInput();

        // Check for gamepad input and handle it.
        CheckGamePadInput();

        // Creating a bounding circle for the slime
        var slimeBounds = new Circle(
            x: (int)(_slimePosition.X + _slime.Width * 0.5f),
            y: (int)(_slimePosition.Y + _slime.Height * 0.5f),
            radius: (int)(_slime.Width * 0.5f)
        );

        // Use distance based checks to determine if the slime is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // move it back inside.
        if (slimeBounds.Left < _roomBounds.Left)
        {
            _slimePosition.X = _roomBounds.Left;
        }
        else if (slimeBounds.Right > _roomBounds.Right)
        {
            _slimePosition.X = _roomBounds.Right - _slime.Width;
        }

        if (slimeBounds.Top < _roomBounds.Top)
        {
            _slimePosition.Y = _roomBounds.Top;
        }
        else if (slimeBounds.Bottom > _roomBounds.Bottom)
        {
            _slimePosition.Y = _roomBounds.Bottom - _slime.Height;
        }

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
        if (batBounds.Left < _roomBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newBatPosition.X = _roomBounds.Left;
        }
        else if (batBounds.Right > _roomBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newBatPosition.X = _roomBounds.Right - _bat.Width;
        }

        if (batBounds.Top < _roomBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newBatPosition.Y = _roomBounds.Top;
        }
        else if (batBounds.Bottom > _roomBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newBatPosition.Y = _roomBounds.Bottom - _bat.Height;
        }

        // If the normal is anything but Vector2.Zero, this means the bat had
        // moved outside the screen edge so we should reflect it about the
        // normal.
        if (normal != Vector2.Zero)
        {
            _batVelocity = Vector2.Reflect(vector: _batVelocity, normal: normal);
        }

        _batPosition = newBatPosition;

        if (slimeBounds.Intersects(batBounds))
        {
            // Choose a random row and column based on the total number of each
            var column = Random.Shared.Next(minValue: 1, maxValue: _tilemap.Columns - 1);
            var row = Random.Shared.Next(minValue: 1, maxValue: _tilemap.Rows - 1);

            // Change the bat position by setting the x and y values equal to
            // the column and row multiplied by the width and height.
            _batPosition = new Vector2(x: column * _bat.Width, y: row * _bat.Height);

            // Assign a new random velocity to the bat
            AssignRandomBatVelocity();
        }

        base.Update(gameTime);
    }

    private void AssignRandomBatVelocity()
    {
        // Generate a random angle
        var angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        // Convert angle to a direction vector
        var x = (float)Math.Cos(angle);
        var y = (float)Math.Sin(angle);
        var direction = new Vector2(x: x, y: y);

        // Multiply the direction vector by the movement speed
        _batVelocity = direction * MovementSpeed;
    }

    private void CheckKeyboardInput()
    {
        // If the space key is held down, the movement speed increases by 1.5
        var speed = MovementSpeed;
        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        // If the W or Up keys are down, move the slime up on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.Z) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            _slimePosition.Y -= speed;
        }

        // if the S or Down keys are down, move the slime down on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            _slimePosition.Y += speed;
        }

        // If the A or Left keys are down, move the slime left on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.Q) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            _slimePosition.X -= speed;
        }

        // If the D or Right keys are down, move the slime right on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            _slimePosition.X += speed;
        }
    }

    private void CheckGamePadInput()
    {
        var gamePadOne = Input.GamePads[(int)PlayerIndex.One];

        // If the A button is held down, the movement speed increases by 1.5
        // and the gamepad vibrates as feedback to the player.
        var speed = MovementSpeed;
        if (gamePadOne.IsButtonDown(Buttons.A))
        {
            speed *= 1.5f;
            GamePad.SetVibration(playerIndex: PlayerIndex.One, leftMotor: 1.0f, rightMotor: 1.0f);
        }
        else
        {
            GamePad.SetVibration(playerIndex: PlayerIndex.One, leftMotor: 0.0f, rightMotor: 0.0f);
        }

        // Check thumbstick first since it has priority over which gamepad input
        // is movement.  It has priority since the thumbstick values provide a
        // more granular analog value that can be used for movement.
        if (gamePadOne.LeftThumbStick != Vector2.Zero)
        {
            _slimePosition.X += gamePadOne.LeftThumbStick.X * speed;
            _slimePosition.Y -= gamePadOne.LeftThumbStick.Y * speed;
        }
        else
        {
            // If DPadUp is down, move the slime up on the screen.
            if (gamePadOne.IsButtonDown(Buttons.DPadUp))
            {
                _slimePosition.Y -= speed;
            }

            // If DPadDown is down, move the slime down on the screen.
            if (gamePadOne.IsButtonDown(Buttons.DPadDown))
            {
                _slimePosition.Y += speed;
            }

            // If DPapLeft is down, move the slime left on the screen.
            if (gamePadOne.IsButtonDown(Buttons.DPadLeft))
            {
                _slimePosition.X -= speed;
            }

            // If DPadRight is down, move the slime right on the screen.
            if (gamePadOne.IsButtonDown(Buttons.DPadRight))
            {
                _slimePosition.X += speed;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap.
        _tilemap.Draw(SpriteBatch);

        // Draw the slime sprite.
        _slime.Draw(spriteBatch: SpriteBatch, position: _slimePosition);

        // Draw the bat sprite.
        _bat.Draw(spriteBatch: SpriteBatch, position: _batPosition);

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}