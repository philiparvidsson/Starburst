namespace Fab5.Engine.Subsystems
{
    using System;

    using Fab5.Engine.Components;
    using Fab5.Engine.Core;
    using Microsoft.Xna.Framework.Input;
    using Engine;
    public class Inputhandler_System : Subsystem {
        private void fire(Entity entity, Ship_Info ship, Weapon weapon) {
            if (weapon.timeSinceLastShot >= weapon.fire_rate && ship.energy_value >= weapon.energy_cost) {
                var message = new { Origin = entity, Weapon = weapon, Ship = ship };
                Fab5_Game.inst().message("fireInput", message);
            }
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
            return previous.ThumbSticks.Left.Y >= threshold - 1 && current.ThumbSticks.Left.Y < threshold - 1;
        }
        private bool is_gamepad_thumbstick_left(float threshold, GamePadState current, GamePadState previous) {
            return previous.ThumbSticks.Left.X >= threshold - 1 && current.ThumbSticks.Left.X < threshold - 1;
        }

        private bool is_gamepad_thumbstick_right(float threshold, GamePadState current, GamePadState previous) {
            return previous.ThumbSticks.Left.X <= threshold && current.ThumbSticks.Left.X > threshold;
        }


        public override void draw(float t, float dt) {
            var entities = Fab5_Game.inst().get_entities_fast(typeof(Input));
            int num_components = entities.Count;

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var input = entity.get_component<Input>();
                var angle = entity.get_component<Angle>();

                // all player controlled objects get an angular drag force to prevent the players from going insane lol
                angle.ang_vel -= angle.ang_vel * 5.0f * dt;

                if (!input.enabled) {
                    continue;
                }

                var velocity = entity.get_component<Velocity>();

                var ship = entity.get_component<Ship_Info>();
                Primary_Weapon primaryWeapon = entity.get_component<Primary_Weapon>();
                Secondary_Weapon secondaryWeapon = entity.get_component<Secondary_Weapon>();

                var max_speed = ship.top_velocity;
                var acc       = ship.acceleration;

                float turn = 0.0f;

                KeyboardState currentKeyboardState = Keyboard.GetState();

                // Keyboard device
                if (input.device == Input.InputType.Keyboard) {
                    // @To-do: keys for managing powerups

                    if (currentKeyboardState.IsKeyDown(input.left))
                        turn -= 1.0f;
                    if (currentKeyboardState.IsKeyDown(input.right))
                        turn += 1.0f;


                    input.throttle = 0.0f;

                    if (currentKeyboardState.IsKeyDown(input.up)) {
                        input.throttle = 1.0f;
                    }
                    else if (currentKeyboardState.IsKeyDown(input.down)) {
                        input.throttle = -1.0f;
                    }


                    if (currentKeyboardState.IsKeyDown(input.primary_fire))
                        fire(entity, ship, primaryWeapon);
                    if (currentKeyboardState.IsKeyDown(input.secondary_fire))
                        fire(entity, ship, secondaryWeapon);
                }
                // Gamepad device
                else {
                    GamePadState state = GamePad.GetState(input.gp_index);
                    turn = state.ThumbSticks.Left.X;
                    //turn = Math.Sign((float)Math.Atan2(-state.ThumbSticks.Left.Y, state.ThumbSticks.Left.X) - angle.angle) * 10.0f;;
                    input.throttle = state.Triggers.Right - state.Triggers.Left;

                    if (state.Buttons.A == ButtonState.Pressed) {
                        fire(entity, ship, primaryWeapon);
                    }
                    if (state.Buttons.X == ButtonState.Pressed) {
                        fire(entity, ship, secondaryWeapon);
                    }

                    if (state.Buttons.LeftShoulder == ButtonState.Released &&
                        state.Buttons.RightShoulder == ButtonState.Released)
                    {
                        input.can_switch_powerups = true;
                    }
                    if (input.can_switch_powerups && state.Buttons.LeftShoulder == ButtonState.Pressed) {
                        input.can_switch_powerups = false;
                        ship.powerup_inv_index -= 1;
                        if (ship.powerup_inv_index < 0) ship.powerup_inv_index = ship.max_powerups_inv-1;
                    }
                    if (input.can_switch_powerups && state.Buttons.RightShoulder == ButtonState.Pressed) {
                        input.can_switch_powerups = false;
                        ship.powerup_inv_index += 1;
                        if (ship.powerup_inv_index >= ship.max_powerups_inv) ship.powerup_inv_index = 0;
                    }
                    if (state.Buttons.Y == ButtonState.Released) input.can_use_powerup = true;
                    if (input.can_use_powerup && state.Buttons.Y == ButtonState.Pressed && ship.powerup_inv_index < ship.powerup_inv.Length) {
                        input.can_use_powerup = false;
                        ship.use_powerup(ship.powerup_inv_index);
                    }

                    if (is_gamepad_clicked(Buttons.Start, state, input.gamepadState))
                        Fab5_Game.inst().message("start", null);
                    input.gamepadState = state;
                }

                if (Math.Abs(turn) > 0.01f) {
                    var ang_acc = 30.0f * dt;

                    angle.ang_vel += ang_acc * turn;

                    if (angle.ang_vel > 5.0f) {
                        angle.ang_vel = 5.0f;
                    }
                    else if (angle.ang_vel < -5.0f) {
                        angle.ang_vel = -5.0f;
                    }
                }
                else {
                    angle.ang_vel = 0.0f;
                }
                if (Math.Abs(input.throttle) > 0.01f) {
                    //Fab5_Game.inst().message("play_sound", new { name = "thrust", gp_index = input.gp_index, pos = pos });

                    velocity.x += dt * (float)(Math.Cos(angle.angle)) * acc * input.throttle;
                    velocity.y += dt * (float)(Math.Sin(angle.angle)) * acc * input.throttle;

                    var speed = (float)Math.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);

                    if (speed > max_speed) {
                        velocity.x = max_speed * (velocity.x / speed);
                        velocity.y = max_speed * (velocity.y / speed);
                    }
                }
                else {
                    //Fab5_Game.inst().message("stop_sound", new { name = "thrust", gp_index = input.gp_index, pos = pos });
                }


                // Misc keys
                if (input.keyboardState.IsKeyDown(Keys.N))
                {
                    Fab5_Game.inst().message("change_song", null);
                }

                if (input.keyboardState.IsKeyDown(Keys.M))
                {
                    Fab5_Game.inst().message("play_sound", new { name = "mute" });
                }

                if (currentKeyboardState.IsKeyDown(Keys.LeftAlt) && is_key_clicked(Keys.Enter, currentKeyboardState, input.keyboardState))
                    Fab5_Game.inst().message("fullscreen", null);
                else if (is_key_clicked(Keys.Escape, currentKeyboardState, input.keyboardState))
                    Fab5_Game.inst().message("start", null);

                input.keyboardState = Keyboard.GetState();
            }
        }
    }

}
