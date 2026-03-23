using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KA2
{
    public class InputController
    {
        public Vector2 Movement { get; private set; }
        public bool IsFiring { get; private set; }

        public void ProcessInput()
        {
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            if (padState.IsConnected)
            {
                // 1. Handle Movement (Left Thumbstick or D-Pad)
                float xInput = padState.ThumbSticks.Left.X;

                if (padState.DPad.Left == ButtonState.Pressed) xInput = -1.0f;
                if (padState.DPad.Right == ButtonState.Pressed) xInput = 1.0f;

                Movement = new Vector2(xInput, 0);

                // 2. Handle Firing (A button or Right Trigger)
                IsFiring = (padState.Buttons.A == ButtonState.Pressed ||
                            padState.Triggers.Right > 0.5f);
            }
            else
            {
                // Fallback to Keyboard if controller is unplugged
                KeyboardState kstate = Keyboard.GetState();
                float x = 0;
                if (kstate.IsKeyDown(Keys.Left)) x = -1.0f;
                if (kstate.IsKeyDown(Keys.Right)) x = 1.0f;

                Movement = new Vector2(x, 0);
                IsFiring = kstate.IsKeyDown(Keys.Space);
            }
        }
    }
}