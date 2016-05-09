namespace Fab5.Engine {

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public static class Gamepad_Util {
    [System.Diagnostics.Conditional("GAMEPAD_VIBRATION")]
    public static void vibrate(int gp_index, float left_vib, float right_vib) {
#if GAMEPAD_VIBRATION
        left_vib  = Math.Min(Math.Max(0.0f, left_vib ), 1.0f);
        right_vib = Math.Min(Math.Max(0.0f, right_vib), 1.0f);

        var left_motor  = (ushort)(left_vib  * 65535.0f);
        var right_motor = (ushort)(right_vib * 65535.0f);

        var vib = new SharpDX.XInput.Vibration {
            LeftMotorSpeed  = left_motor,
            RightMotorSpeed = right_motor
        };

        var gamepad = new SharpDX.XInput.Controller((SharpDX.XInput.UserIndex)gp_index);

        if (GamePad.GetState((PlayerIndex)gp_index).IsConnected) {
            gamepad.SetVibration(vib);
        }
#endif
    }
}

}
