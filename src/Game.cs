namespace Game4 {

using Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


using System;
using System.Collections.Generic;

public class GameImpl : Game {
    private readonly Game_Engine engine;
    private readonly GraphicsDeviceManager graphics;

    public GameImpl() {
        graphics = new GraphicsDeviceManager(this);
        //graphics.IsFullScreen = true;
        //graphics.ApplyChanges();

        Content.RootDirectory = "content";

        engine = new Game_Engine(Content);
    }

    protected override void Initialize() {
        // TODO: Add your initialization logic here
        engine.init();
        engine.add_subsystems(
            new Engine.Subsystems.Position_Integrator(),
            new Engine.Subsystems.Sprite_Renderer(new SpriteBatch(GraphicsDevice))
        );

        var ball = engine.create_entity<global::Game.Entities.Ball>();
        engine.entities.Add(ball);

        base.Initialize();

    }

    protected override void LoadContent() {

    }

    protected override void UnloadContent() {
        // TODO: Unload any non ContentManager content here
        Content.Unload();
    }

    protected override void Update(GameTime gameTime) {
        // TODO: Add your update logic here
        float dt = 1.0f / 60.0f;
        engine.update(dt);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        float dt = 1.0f / 60.0f;
        engine.draw(dt);

        base.Draw(gameTime);
    }
}

//----------------------------------------------------------------------------

}
