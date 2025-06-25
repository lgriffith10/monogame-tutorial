using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class KeyboardInfo
{
    public KeyboardInfo()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    public KeyboardState PreviousState { get; private set; }
    public KeyboardState CurrentState { get; private set; }

    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    public bool IsKeyDown(Keys key)
    {
        return CurrentState.IsKeyDown(key: key);
    }

    public bool IsKeyUp(Keys key)
    {
        return CurrentState.IsKeyUp(key: key);
    }

    public bool WasKeyJustPressed(Keys key)
    {
        return CurrentState.IsKeyDown(key: key) && PreviousState.IsKeyUp(key: key);
    }

    public bool WasKeyJustReleased(Keys key)
    {
        return CurrentState.IsKeyUp(key: key) && PreviousState.IsKeyDown(key: key);
    }
}