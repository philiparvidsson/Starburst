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
            var entities = Fab5_Game.inst().get_entities_fast(typeof(Inputhandler));
            int num_components = entities.Count;

            for (int i = 0; i < num_components; i++)
            {
                var entity = entities[i];
                var input = entity.get_component<Inputhandler>();
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

                // Keyboard device
                if (input.device == Inputhandler.InputType.Keyboard) {
                    input.keyboardState = Keyboard.GetState();

                    if (input.keyboardState.IsKeyDown(input.left))
                        turn -= 1.0f;
                    if (input.keyboardState.IsKeyDown(input.right))
                        turn += 1.0f;


                    input.throttle = 0.0f;

                    if (input.keyboardState.IsKeyDown(input.up)) {
                        input.throttle = 1.0f;
                    }
                    else if (input.keyboardState.IsKeyDown(input.down)) {
                        input.throttle = -1.0f;
                    }


                    if (input.keyboardState.IsKeyDown(input.primary_fire))
                        fire(entity, ship, primaryWeapon);
                    if (input.keyboardState.IsKeyDown(input.secondary_fire))
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
                    //Fab5_Game.inst().message("throttle", new { gp_index = input.gp_index });

                    velocity.x += dt * (float)(Math.Cos(angle.angle)) * acc * input.throttle;
                    velocity.y += dt * (float)(Math.Sin(angle.angle)) * acc * input.throttle;

                    var speed = (float)Math.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);

                    if (speed > max_speed) {
                        velocity.x = max_speed * (velocity.x / speed);
                        velocity.y = max_speed * (velocity.y / speed);
                    }
                }
                else {
                    //Fab5_Game.inst().message("nothrottle", new { gp_index = input.gp_index });
                }


                // Misc keys
                if (input.keyboardState.IsKeyDown(Keys.N)) {
                    Fab5_Game.inst().message("songchanged", null);
                }

                if (input.keyboardState.IsKeyDown(Keys.M)) {
                    Fab5_Game.inst().message("mute", null);
                }
            }
        }
    }

}
