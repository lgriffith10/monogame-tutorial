using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Input;

public class InputManager
{
    public InputManager()
    {
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();
        GamePads = new GamePadInfo[4]; // Assuming a maximum of 4 gamepads
        for (var i = 0; i < GamePads.Length; i++) GamePads[i] = new GamePadInfo(playerIndex: (PlayerIndex)i);
    }

    public KeyboardInfo Keyboard { get; }
    public MouseInfo Mouse { get; }
    public GamePadInfo[] GamePads { get; }

    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();

        foreach (var g in GamePads)
            g.Update(gameTime: gameTime);
    }
}