using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.UpdateableSystems.ForceFields;
using BEPUutilities;
using BEPUphysics.Materials;

namespace Asteroids
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Microsoft.Xna.Framework.Matrix world = Microsoft.Xna.Framework.Matrix.CreateTranslation(new Microsoft.Xna.Framework.Vector3(0, 0, 0));
        private Microsoft.Xna.Framework.Matrix projection = Microsoft.Xna.Framework.Matrix.CreatePerspectiveFieldOfView(Microsoft.Xna.Framework.MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);

        private Space space;

        private Player player;

        private List<Asteroid> _Asteroids;
        private List<Missile> _Missiles;

        private Camera camera;

        public KeyboardState KeyboardState;
        public MouseState MouseState;

        private bool gameStart = true;

        private AsteroidFactory largeFactory, mediumFactory, smallFactory;

        private MissileFactory missileFactory;

        public Game()
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
            // TODO: Add your initialization logic here

            base.Initialize();

            this.Services.AddService(typeof(Microsoft.Xna.Framework.Matrix), projection);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            space = new Space();
            _Asteroids = new List<Asteroid>();
            _Missiles = new List<Missile>();

            Texture2D asteroidTex = Content.Load<Texture2D>("Asteroid");

            Material bounceMaterial = new Material(1, 1, 1);


            #region Set up Boundary
            Box botBox = new Box(new BEPUutilities.Vector3(0, -50, 0), 50, 1, 50);
            botBox.Material = bounceMaterial;
            Box topBox = new Box(new BEPUutilities.Vector3(0, 50, 0), 50, 1, 50);
            topBox.Material = bounceMaterial;
            Box leftBox = new Box(new BEPUutilities.Vector3(-50, 0, 0), 1, 50, 50);
            leftBox.Material = bounceMaterial;
            Box rightBox = new Box(new BEPUutilities.Vector3(50, 0, 0), 1, 50, 50);
            rightBox.Material = bounceMaterial;
            Box frontBox = new Box(new BEPUutilities.Vector3(0, 0, -50), 50, 50, 1);
            frontBox.Material = bounceMaterial;
            Box backBox = new Box(new BEPUutilities.Vector3(0, 0, 50), 50, 50, 1);
            backBox.Material = bounceMaterial;

            space.Add(botBox);
            space.Add(topBox);
            space.Add(leftBox);
            space.Add(rightBox);
            space.Add(frontBox);
            space.Add(backBox);
            #endregion

            largeFactory = new AsteroidFactory(this, AsteroidFactory.Size.Large, 0.8f, 100, Content.Load<Model>("AsteroidLarge1"), Content.Load<Model>("AsteroidLarge2"), asteroidTex);
            mediumFactory = new AsteroidFactory(this, AsteroidFactory.Size.Medium, .5f, 10, Content.Load<Model>("AsteroidMedium1"), Content.Load<Model>("AsteroidMedium2"), asteroidTex);
            smallFactory = new AsteroidFactory(this, AsteroidFactory.Size.Small, .25f, 1, Content.Load<Model>("AsteroidSmall1"), Content.Load<Model>("AsteroidSmall2"), asteroidTex);
            missileFactory = new MissileFactory(this, .25f, .1f, Content.Load<Model>("Missile"), Content.Load<Texture2D>("Metal"));

            Sphere playerSphere = new Sphere(new BEPUutilities.Vector3(0, 0, 5), .6f, 1);

            player = new Player(playerSphere);

            space.Add(playerSphere);

            camera = new Camera(this, playerSphere.Position, 5);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || KeyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            if(gameStart)
            {
                //Asteroid ast = smallFactory.ConstructAsteroid(true, new Microsoft.Xna.Framework.Vector3(-5, 0, 0), new Microsoft.Xna.Framework.Vector3(0, 1, 0), 30, new Microsoft.Xna.Framework.Vector3(5, 0, 0), new Microsoft.Xna.Framework.Vector3(0, 2, 0));
                //space.Add(ast.entity);
                //_Asteroids.Add(ast);

                //ast = smallFactory.ConstructAsteroid(true, new Microsoft.Xna.Framework.Vector3(5, 0, 0), new Microsoft.Xna.Framework.Vector3(0, 1, 0), -30, new Microsoft.Xna.Framework.Vector3(-5, 0, 0), new Microsoft.Xna.Framework.Vector3(0, -2, 0));
                //space.Add(ast.entity);
                //_Asteroids.Add(ast);

                Missile mis = missileFactory.ConstructMissile(new Microsoft.Xna.Framework.Vector3(0, 0, 0), new Microsoft.Xna.Framework.Vector3(0, 0, 0), 0, new Microsoft.Xna.Framework.Vector3(0, 0, 0), new Microsoft.Xna.Framework.Vector3(0, 0, 0));
                space.Add(mis.entity);
                _Missiles.Add(mis);

                gameStart = false;
            }

            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            camera.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            player.SetPosition(MatrixHelpers.VectorToVector(camera.Position));

            space.Update();
   
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach(Asteroid obj in _Asteroids)
            {
                obj.Draw(MatrixHelpers.MatrixToMatrix(camera.ViewMatrix));
            }

            foreach(Missile obj in _Missiles)
            {
                obj.Draw(MatrixHelpers.MatrixToMatrix(camera.ViewMatrix));
            }

            base.Draw(gameTime);
        }
    }
}
