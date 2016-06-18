using System.Windows.Input;

namespace Gemini.Framework.Util
{
    public static class SpeedMultiplier
    {
        public static double Get()
        {
            var multi = 1.0;

            var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            var shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            var alt = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

            if (ctrl)
            {
                multi = shift ? 0.01 : 0.1;
            }
            else if (alt)
            {
                multi = shift ? 100.0 : 10.0;
            }

            return multi;
        }
    }
}
