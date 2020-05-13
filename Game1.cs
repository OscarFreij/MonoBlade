using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using static MonoBlade.Core;

namespace MonoBlade
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public int NextItemId = 0;
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public List<GameObject> GameObjects;

        public Texture2D mouse;

        public Core.PhysicsEngine.Core PhysicsCore;

        System.Random random;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            PhysicsCore = new Core.PhysicsEngine.Core(9.82f, this);
            // TODO: Add your initialization logic here
            GameObjects = new List<GameObject>();

            GameObjects.Add(new Core.GameObject(NextItemId, "Player", this, 200, 50, true, new Vector2(20, 30), new Vector2(0, 0), false, false, 1, 100, 10, 20, 0));
            NextItemId++;

            GameObjects.Add(new Core.GameObject(NextItemId, "Enemy-1", this, 600, 100, false, new Vector2(20, 30), new Vector2(0, 0), false, true, 2, 10, 5, 10, 5));
            NextItemId++;

            GameObjects.Add(new Core.GameObject(2, "Ground-RE1", this, Window.ClientBounds.Width / 2, Window.ClientBounds.Height - Window.ClientBounds.Height / 3, false, new Vector2(600, 30), new Vector2(0, 0), false, false, 0, 0, 0, 0, 0));
            NextItemId++;
            GameObjects.Add(new Core.GameObject(3, "Ground-RE2", this, (Window.ClientBounds.Width / 2) - 315, Window.ClientBounds.Height - Window.ClientBounds.Height / 3, false, new Vector2(30, 60), new Vector2(0, 0), false, false, 0, 0, 0, 0, 0));
            NextItemId++;
            GameObjects.Add(new Core.GameObject(4, "Ground-RE3", this, (Window.ClientBounds.Width / 2) + 315, Window.ClientBounds.Height - Window.ClientBounds.Height / 3, false, new Vector2(30, 60), new Vector2(0, 0), false, false, 0, 0, 0, 0, 0));
            NextItemId++;

            random = new System.Random();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mouse = Content.Load<Texture2D>("sprites/base/Mouse_Simple");
            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            int EnemyCound = 0;

            foreach (var GameObject in GameObjects)
            {
                if (GameObject.Name.Contains("Enemy"))
                {
                    EnemyCound++;
                }
            }


            if (random.Next(1,100) == 1 && EnemyCound < 6)
            {
                GameObjects.Add(new Core.GameObject(NextItemId, "Enemy-" + NextItemId, this, 600, 100, false, new Vector2(20, 30), new Vector2(0, 0), false, true, 2, 10, 5, 10, 5));
                NextItemId++;
            }


            int BombCount = 0;
            foreach (var GameObject in GameObjects)
            {
                if (GameObject.Name.Contains("Bomb"))
                {
                    BombCount++;
                }
            }
            try
            {
                GameObject PlayerObject = null;
                foreach (var GameObject in GameObjects)
                {
                    if (GameObject.Name.Contains("Player"))
                    {
                        PlayerObject = GameObject;
                        break;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space) && BombCount == 0 && !PlayerObject.ColliderComponent.IsOnGround)
                {
                    GameObjects.Add(new Core.GameObject(NextItemId, "Bomb-" + NextItemId, this, PlayerObject.PositionComponent.Position.X, PlayerObject.PositionComponent.Position.Y + PlayerObject.ColliderComponent.Dimensions.Y/2 + 15, false, new Vector2(10, 10), new Vector2(0, 0), false, false, 3, 10, 5, 1, 100));
                }
            }
            catch
            {

            }
            
            





            // TODO: Add your update logic here
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Tick(gameTime);
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);


            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Draw();
            }
            spriteBatch.Draw(mouse, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
