using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class MouseInfo
{
    public MouseInfo()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    public MouseState PreviousState { get; private set; }
    public MouseState CurrentState { get; private set; }

    public Point Position
    {
        get => CurrentState.Position;
        set => SetPosition(x: value.X, y: value.Y);
    }

    public int X
    {
        get => CurrentState.X;
        set => SetPosition(x: value, y: CurrentState.Y);
    }

    public int Y
    {
        get => CurrentState.Y;
        set => SetPosition(x: CurrentState.X, y: value);
    }

    public Point PositionDelta => CurrentState.Position - PreviousState.Position;
    public int XDelta => CurrentState.X - PreviousState.X;
    public int YDelta => CurrentState.Y - PreviousState.Y;
    public bool WasMoved => PositionDelta != Point.Zero;
    public int ScrolLWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    public bool IsButtonDown(MouseButtonEnum button)
    {
        switch (button)
        {
            case MouseButtonEnum.Left:
                return CurrentState.LeftButton == ButtonState.Pressed;
            case MouseButtonEnum.Middle:
                return CurrentState.MiddleButton == ButtonState.Pressed;
            case MouseButtonEnum.Right:
                return CurrentState.RightButton == ButtonState.Pressed;
            case MouseButtonEnum.XButton1:
                return CurrentState.XButton1 == ButtonState.Pressed;
            case MouseButtonEnum.XButton2:
                return CurrentState.XButton2 == ButtonState.Pressed;
            default:
                return false;
        }
    }

    public bool IsButtonUp(MouseButtonEnum button)
    {
        switch (button)
        {
            case MouseButtonEnum.Left:
                return CurrentState.LeftButton == ButtonState.Released;
            case MouseButtonEnum.Middle:
                return CurrentState.MiddleButton == ButtonState.Released;
            case MouseButtonEnum.Right:
                return CurrentState.RightButton == ButtonState.Released;
            case MouseButtonEnum.XButton1:
                return CurrentState.XButton1 == ButtonState.Released;
            case MouseButtonEnum.XButton2:
                return CurrentState.XButton2 == ButtonState.Released;
            default:
                return false;
        }
    }

    public bool WasButtonJustPressed(MouseButtonEnum button)
    {
        switch (button)
        {
            case MouseButtonEnum.Left:
                return CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released;
            case MouseButtonEnum.Middle:
                return CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released;
            case MouseButtonEnum.Right:
                return CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released;
            case MouseButtonEnum.XButton1:
                return CurrentState.XButton1 == ButtonState.Pressed && PreviousState.XButton1 == ButtonState.Released;
            case MouseButtonEnum.XButton2:
                return CurrentState.XButton2 == ButtonState.Pressed && PreviousState.XButton2 == ButtonState.Released;
            default:
                return false;
        }
    }

    public bool WasButtonJustReleased(MouseButtonEnum button)
    {
        switch (button)
        {
            case MouseButtonEnum.Left:
                return CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
            case MouseButtonEnum.Middle:
                return CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed;
            case MouseButtonEnum.Right:
                return CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed;
            case MouseButtonEnum.XButton1:
                return CurrentState.XButton1 == ButtonState.Released && PreviousState.XButton1 == ButtonState.Pressed;
            case MouseButtonEnum.XButton2:
                return CurrentState.XButton2 == ButtonState.Released && PreviousState.XButton2 == ButtonState.Pressed;
            default:
                return false;
        }
    }

    public void SetPosition(int x, int y)
    {
        Mouse.SetPosition(x: x, y: y);
        CurrentState = new MouseState(
            x: x,
            y: y,
            scrollWheel: CurrentState.ScrollWheelValue,
            leftButton: CurrentState.LeftButton,
            middleButton: CurrentState.MiddleButton,
            rightButton: CurrentState.RightButton,
            xButton1: CurrentState.XButton1,
            xButton2: CurrentState.XButton2
        );
    }
}