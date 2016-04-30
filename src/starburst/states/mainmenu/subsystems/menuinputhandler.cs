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
            return previous.IsKeyDown(key) && current.IsKeyUp(key);
        }
        private bool is_gamepad_clicked(Buttons key, GamePadState current, GamePadState previous) {
            return previous.IsButtonDown(key) && current.IsButtonUp(key);
        }
        public override void update(float t, float dt)
            {

            var entities = Fab5_Game.inst().get_entities_fast(typeof(Inputhandler));
            int num_components = entities.Count;

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var inputHandler = entity.get_component<Inputhandler>();

                if (inputHandler.device == Inputhandler.InputType.Keyboard) {
                    KeyboardState currentKeyboardState = Keyboard.GetState();
                    if (is_key_clicked(inputHandler.up, currentKeyboardState, inputHandler.keyboardState)) {
                        Fab5_Game.inst().message("up", new { Player = entity });
                    }
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

                    inputHandler.keyboardState = currentKeyboardState;
                }
                else {
                    GamePadState currentState = GamePad.GetState(inputHandler.gp_index);

                    if (is_gamepad_clicked(Buttons.LeftThumbstickUp, currentState, inputHandler.gamepadState))
                        Fab5_Game.inst().message("up", new { Player = entity });
                    if (is_gamepad_clicked(Buttons.LeftThumbstickDown, currentState, inputHandler.gamepadState)) 
                        Fab5_Game.inst().message("down", new { Player = entity });
                    if (is_gamepad_clicked(Buttons.LeftThumbstickLeft, currentState, inputHandler.gamepadState))
                        Fab5_Game.inst().message("left", new { Player = entity });
                    if (is_gamepad_clicked(Buttons.LeftThumbstickRight, currentState, inputHandler.gamepadState))
                        Fab5_Game.inst().message("right", new { Player = entity });
                    if (is_gamepad_clicked(Buttons.A, currentState, inputHandler.gamepadState)) 
                        Fab5_Game.inst().message("select", new { Player = entity });
                    if (is_gamepad_clicked(Buttons.B, currentState, inputHandler.gamepadState))
                        Fab5_Game.inst().message("back", new { Player = entity });
                    inputHandler.gamepadState = currentState;
                }
                /*
                if (currentInput.keyboardState.IsKeyDown(Keys.M))
                    Fab5_Game.inst().message("mute", null);*/
            }
        }
    }

}
