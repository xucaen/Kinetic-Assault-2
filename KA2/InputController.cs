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
            // Initialize defaults for this frame
            float xInput = 0;
            bool fireInput = false;

            // --- 1. Check GamePad ---
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (padState.IsConnected)
            {
                xInput = padState.ThumbSticks.Left.X;

                if (padState.DPad.Left == ButtonState.Pressed) xInput = -1.0f;
                if (padState.DPad.Right == ButtonState.Pressed) xInput = 1.0f;

                fireInput = (padState.Buttons.A == ButtonState.Pressed ||
                             padState.Triggers.Right > 0.5f);
            }

            // --- 2. Check Keyboard/Mouse (Additive) ---
            KeyboardState kstate = Keyboard.GetState();
            MouseState mstate = Mouse.GetState();

            // Use "if" without "else" so we add to the existing xInput
            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A)) xInput -= 1.0f;
            if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D)) xInput += 1.0f;

            // Combine firing: if either source is true, IsFiring is true
            fireInput = fireInput || kstate.IsKeyDown(Keys.Space) || (mstate.LeftButton == ButtonState.Pressed);

            // --- 3. Finalize and Clamp ---
            // Clamp movement between -1 and 1 so holding Keyboard + Controller isn't "double fast"
            float finalX = MathHelper.Clamp(xInput, -1.0f, 1.0f);

            Movement = new Vector2(finalX, 0);
            IsFiring = fireInput;
        }
    }
}