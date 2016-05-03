namespace Fab5.Starburst.States.Main_Menu.Subsystems
{
    using System;

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Input;
    using Engine;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    public class Menu_Inputhandler_System : Subsystem
        {
        public override void init() {
            base.init();
        }
        private bool is_key_clicked(Keys key, KeyboardState current, KeyboardState previous) {
            return previous.IsKeyUp(key) && current.IsKeyDown(key);
        }
        private bool is_gamepad_clicked(Buttons key, GamePadState current, GamePadState previous) {
            return previous.IsButtonUp(key) && current.IsButtonDown(key);
        }
        private bool is_gamepad_thumbstick_up(float threshold, GamePadState current, GamePadState previous) {
            return previous.ThumbSticks.Left.Y <= threshold && current.ThumbSticks.Left.Y > threshold;
        }
        private bool is_gamepad_thumbstick_down(float threshold, GamePadState current, GamePadState previous) {
            return previous.ThumbSticks.Left.Y >= threshold-1 && current.ThumbSticks.Left.Y < threshold-1;
        }
        private bool is_gamepad_thumbstick_left(float threshold, GamePadState current, GamePadState previous) {
            return previous.ThumbSticks.Left.X >= threshold-1 && current.ThumbSticks.Left.X < threshold-1;
        }
        private bool is_gamepad_thumbstick_right(float threshold, GamePadState current, GamePadState previous) {
            return previous.ThumbSticks.Left.X <= threshold && current.ThumbSticks.Left.X > threshold;
        }
        public override void update(float t, float dt)
            {

            var entities = Fab5_Game.inst().get_entities_fast(typeof(Inputhandler));
            int num_components = entities.Count;
            KeyboardState currentKeyboardState = Keyboard.GetState();

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var inputHandler = entity.get_component<Inputhandler>();

                if (inputHandler.device == Inputhandler.InputType.Keyboard) {
                    if (is_key_clicked(inputHandler.up, currentKeyboardState, inputHandler.keyboardState))
                        Fab5_Game.inst().message("up", new { Player = entity });
                    if (is_key_clicked(inputHandler.left, currentKeyboardState, inputHandler.keyboardState))
                        Fab5_Game.inst().message("left", new { Player = entity });
                    if (is_key_clicked(inputHandler.down, currentKeyboardState, inputHandler.keyboardState))
                        Fab5_Game.inst().message("down", new { Player = entity });
                    if (is_key_clicked(inputHandler.right, currentKeyboardState, inputHandler.keyboardState))
                        Fab5_Game.inst().message("right", new { Player = entity });
                    if (is_key_clicked(inputHandler.primary_fire, currentKeyboardState, inputHandler.keyboardState))
                        Fab5_Game.inst().message("select", new { Player = entity });
                    if (is_key_clicked(inputHandler.secondary_fire, currentKeyboardState, inputHandler.keyboardState))
                        Fab5_Game.inst().message("back", new { Player = entity });
                }
                else {
                    GamePadState currentState = GamePad.GetState(inputHandler.gp_index);
                    float threshold = .5f; // tröskelvärde för styrspak
                    if (is_gamepad_thumbstick_up(threshold, currentState, inputHandler.gamepadState))
                        Fab5_Game.inst().message("up", new { Player = entity });
                    if (is_gamepad_thumbstick_down(threshold, currentState, inputHandler.gamepadState)) 
                        Fab5_Game.inst().message("down", new { Player = entity });
                    if (is_gamepad_thumbstick_left(threshold, currentState, inputHandler.gamepadState))
                        Fab5_Game.inst().message("left", new { Player = entity });
                    if (is_gamepad_thumbstick_right(threshold, currentState, inputHandler.gamepadState))
                        Fab5_Game.inst().message("right", new { Player = entity });
                    if (is_gamepad_clicked(Buttons.A, currentState, inputHandler.gamepadState)) 
                        Fab5_Game.inst().message("select", new { Player = entity });
                    if (is_gamepad_clicked(Buttons.B, currentState, inputHandler.gamepadState))
                        Fab5_Game.inst().message("back", new { Player = entity });
                    if (is_gamepad_clicked(Buttons.Start, currentState, inputHandler.gamepadState))
                        Fab5_Game.inst().message("start", null);
                    inputHandler.gamepadState = currentState;
                }
                if (currentKeyboardState.IsKeyDown(Keys.LeftAlt) && is_key_clicked(Keys.Enter, currentKeyboardState, inputHandler.keyboardState))
                    Fab5_Game.inst().message("fullscreen", null);
                else if (is_key_clicked(Keys.Enter, currentKeyboardState, inputHandler.keyboardState))
                    Fab5_Game.inst().message("start", null);
                else if (is_key_clicked(Keys.Escape, currentKeyboardState, inputHandler.keyboardState))
                    Fab5_Game.inst().message("escape", null);

                inputHandler.keyboardState = currentKeyboardState;
            }
        }
    }

}
