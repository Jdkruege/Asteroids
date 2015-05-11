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
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

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

        private float timeSinceLaunch = 0;

        private int asteroidId = 0, missileId = 0;

        private Random rand;

        private SpriteFont font;

        private bool gameOver = false, GameWon = false;

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

            rand = new Random(System.DateTime.Now.Millisecond + System.DateTime.Now.Second + System.DateTime.Now.Minute + System.DateTime.Now.Hour + System.DateTime.Now.Day + System.DateTime.Now.Month + System.DateTime.Now.Year);
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

            #region Set up Boundaries
            InfoTag tag = new InfoTag() { name = "Barrier", id = 0};
            Material bounceMaterial = new Material(1, 1, 1);

            Box botBox = new Box(new BEPUutilities.Vector3(0, -50, 0), 50, 1, 50);
            botBox.Material = bounceMaterial;
            botBox.Tag = tag;
            botBox.CollisionInformation.Tag = tag;

            Box topBox = new Box(new BEPUutilities.Vector3(0, 50, 0), 50, 1, 50);
            topBox.Material = bounceMaterial;
            topBox.Tag = tag;
            topBox.CollisionInformation.Tag = tag;

            Box leftBox = new Box(new BEPUutilities.Vector3(-50, 0, 0), 1, 50, 50);
            leftBox.Material = bounceMaterial;
            leftBox.Tag = tag;
            leftBox.CollisionInformation.Tag = tag;

            Box rightBox = new Box(new BEPUutilities.Vector3(50, 0, 0), 1, 50, 50);
            rightBox.Material = bounceMaterial;
            rightBox.Tag = tag;
            rightBox.CollisionInformation.Tag = tag;

            Box frontBox = new Box(new BEPUutilities.Vector3(0, 0, -50), 50, 50, 1);
            frontBox.Material = bounceMaterial;
            frontBox.Tag = tag;
            frontBox.CollisionInformation.Tag = tag;

            Box backBox = new Box(new BEPUutilities.Vector3(0, 0, 50), 50, 50, 1);
            backBox.Material = bounceMaterial;
            backBox.Tag = tag;
            backBox.CollisionInformation.Tag = tag;

            space.Add(botBox);
            space.Add(topBox);
            space.Add(leftBox);
            space.Add(rightBox);
            space.Add(frontBox);
            space.Add(backBox);
            #endregion

            largeFactory = new AsteroidFactory(this, Asteroid.Size.Large, 0.8f, 100, Content.Load<Model>("AsteroidLarge1"), Content.Load<Model>("AsteroidLarge2"), asteroidTex);
            mediumFactory = new AsteroidFactory(this, Asteroid.Size.Medium, .5f, 10, Content.Load<Model>("AsteroidMedium1"), Content.Load<Model>("AsteroidMedium2"), asteroidTex);
            smallFactory = new AsteroidFactory(this, Asteroid.Size.Small, .25f, 1, Content.Load<Model>("AsteroidSmall1"), Content.Load<Model>("AsteroidSmall2"), asteroidTex);
            missileFactory = new MissileFactory(this, .25f, .1f, Content.Load<Model>("Missile"), Content.Load<Texture2D>("Metal"));

            font = Content.Load<SpriteFont>("Basic");


            #region Player Setup

            InfoTag playerTag = new InfoTag() { name = "Player", id = 0 };

            Sphere playerSphere = new Sphere(new BEPUutilities.Vector3(0, 0, 5), .6f, 1);
            player = new Player(playerSphere);

            player.entity.Tag = playerTag;
            player.entity.CollisionInformation.Tag = playerTag;

            space.Add(playerSphere);
            camera = new Camera(this, playerSphere.Position, 5);
            #endregion
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

            timeSinceLaunch += gameTime.ElapsedGameTime.Milliseconds;

            if(gameStart)
            {
                for(int i = 0; i < 100; i++)
                {
                    Microsoft.Xna.Framework.Vector3 pos = new Microsoft.Xna.Framework.Vector3(rand.Next(-45, 45), rand.Next(-45, 45), rand.Next(-45, 45));
                    Microsoft.Xna.Framework.Vector3 axis = new Microsoft.Xna.Framework.Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
                    float angle = (float)rand.NextDouble() * 30;
                    Microsoft.Xna.Framework.Vector3 linVel = new Microsoft.Xna.Framework.Vector3(rand.Next(-5, 5), rand.Next(-5, 5), rand.Next(-5, 5));
                    Microsoft.Xna.Framework.Vector3 angVel = new Microsoft.Xna.Framework.Vector3(rand.Next(-3, 3), rand.Next(-3, 3), rand.Next(-3, 3));

                    if (i < 80)
                    {
                        Asteroid ast = largeFactory.ConstructAsteroid(++asteroidId, FlipCoin(), pos, axis, angle, linVel, angVel);

                        ast.entity.CollisionInformation.Events.CreatingPair += Events_PairCreated_Asteroid;
                        
                        space.Add(ast.entity);
                        _Asteroids.Add(ast);
                    }
                    else if (i < 90)
                    {
                        Asteroid ast = mediumFactory.ConstructAsteroid(++asteroidId, FlipCoin(), pos, axis, angle, linVel, angVel);

                        ast.entity.CollisionInformation.Events.CreatingPair += Events_PairCreated_Asteroid;
                        
                        space.Add(ast.entity);
                        _Asteroids.Add(ast);
                    }
                    else
                    {
                        Asteroid ast = smallFactory.ConstructAsteroid(++asteroidId, FlipCoin(), pos, axis, angle, linVel, angVel);

                        ast.entity.CollisionInformation.Events.CreatingPair += Events_PairCreated_Asteroid;
                        
                        space.Add(ast.entity);
                        _Asteroids.Add(ast);
                    }
                }

                gameStart = false;
            }

            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            if(MouseState.LeftButton == ButtonState.Pressed && timeSinceLaunch > 500 && !gameOver && !GameWon)
            {
                BEPUutilities.Matrix mat = camera.WorldMatrix;
                mat = BEPUutilities.Matrix.CreateTranslation(new BEPUutilities.Vector3(0, 0, -2)) * mat;

                Missile miss = missileFactory.ConstructMissile( ++missileId, MatrixHelpers.MatrixToMatrix(mat), MatrixHelpers.VectorToVector(camera.WorldMatrix.Forward * 25));

                miss.entity.CollisionInformation.Events.CreatingPair += Events_PairCreated_Missile;

                space.Add(miss.entity);
                _Missiles.Add(miss);

                timeSinceLaunch = 0;
            }

            if (!gameOver && !GameWon)
            {
                camera.Update((float)gameTime.ElapsedGameTime.TotalSeconds);


                player.SetPosition(MatrixHelpers.VectorToVector(camera.Position));

                space.Update();
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

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Respawns Left: " + player.respawns, new Microsoft.Xna.Framework.Vector2(20, 14), Color.Crimson);
            spriteBatch.DrawString(font, "Asteroids Left: " + (_Asteroids.Count), new Microsoft.Xna.Framework.Vector2(590, 14), Color.Crimson);

            if(gameOver)
            {
                spriteBatch.DrawString(font, "You lost.", new Microsoft.Xna.Framework.Vector2(320, 240), Color.Crimson);
            }

            if(GameWon)
            {
                spriteBatch.DrawString(font, "You won!", new Microsoft.Xna.Framework.Vector2(320, 240), Color.Crimson);
            }

            spriteBatch.End();

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

        public bool FlipCoin()
        {
            return rand.Next(0, 1) == 1;
        }

        // Blows up asteroid
        private void ExplodeAsteroid(Asteroid asteroid)
        {
            if(asteroid.size == Asteroid.Size.Large)
            {
                Asteroid ast = mediumFactory.ConstructAsteroid(++asteroidId, FlipCoin(), MatrixHelpers.VectorToVector(asteroid.entity.Position), new Microsoft.Xna.Framework.Vector3(0, 0, 0), 0, MatrixHelpers.VectorToVector(asteroid.entity.LinearVelocity), new Microsoft.Xna.Framework.Vector3(0, 2, 0));

                ast.entity.CollisionInformation.Events.CreatingPair += Events_PairCreated_Asteroid;
                
                space.Add(ast.entity);
                _Asteroids.Add(ast);
            }
            else if(asteroid.size == Asteroid.Size.Medium)
            {
                Asteroid ast = smallFactory.ConstructAsteroid(++asteroidId, FlipCoin(), MatrixHelpers.VectorToVector(asteroid.entity.Position), new Microsoft.Xna.Framework.Vector3(0, 0, 0), 0, MatrixHelpers.VectorToVector(asteroid.entity.LinearVelocity), new Microsoft.Xna.Framework.Vector3(0, 2, 0));

                ast.entity.CollisionInformation.Events.CreatingPair += Events_PairCreated_Asteroid;
                
                space.Add(ast.entity);
                _Asteroids.Add(ast);
            }

            _Asteroids.Remove(asteroid);
        }

        // Smashes asteroids and bounces them
        private void SmashAsteroid(Asteroid asteroid)
        {
            BEPUutilities.Vector3 linVel = asteroid.entity.LinearVelocity;
            linVel.X *= -1;
            linVel.Y *= -1;
            linVel.Z *= -1;

            if (asteroid.size == Asteroid.Size.Large)
            {
                Asteroid ast = mediumFactory.ConstructAsteroid(++asteroidId, FlipCoin(), MatrixHelpers.VectorToVector(asteroid.entity.Position), new Microsoft.Xna.Framework.Vector3(0, 0, 0), 0, MatrixHelpers.VectorToVector(linVel), new Microsoft.Xna.Framework.Vector3(0, 2, 0));

                ast.entity.CollisionInformation.Events.CreatingPair += Events_PairCreated_Asteroid;

                space.Add(ast.entity);
                _Asteroids.Add(ast);
            }
            else if (asteroid.size == Asteroid.Size.Medium)
            {
                Asteroid ast = smallFactory.ConstructAsteroid(++asteroidId, FlipCoin(), MatrixHelpers.VectorToVector(asteroid.entity.Position), new Microsoft.Xna.Framework.Vector3(0, 0, 0), 0, MatrixHelpers.VectorToVector(linVel), new Microsoft.Xna.Framework.Vector3(0, 2, 0));

                ast.entity.CollisionInformation.Events.CreatingPair += Events_PairCreated_Asteroid;

                space.Add(ast.entity);
                _Asteroids.Add(ast);
            }

            _Asteroids.Remove(asteroid);
        }

        public void KillPlayer()
        {
            player.respawns--;

            camera.Position = new BEPUutilities.Vector3(0, 0, 0);
            camera.Pitch = 0;
            camera.Yaw = 0;
        }

        // Collision events below
        void Events_PairCreated_Missile(EntityCollidable sender, BroadPhaseEntry other, NarrowPhasePair pair)
        {
            InfoTag tag = (InfoTag)other.Tag;
           
            if(tag.name == "Barrier")
            {
                tag = (InfoTag)sender.Tag;
                foreach(Missile mis in _Missiles)
                {
                    if(tag.isSame((InfoTag)mis.entity.Tag))
                    {
                        _Missiles.Remove(mis);
                        break;
                    }
                }
            }

            if(tag.name == "Asteroid")
            {
                foreach (Asteroid ast in _Asteroids)
                {
                    if (tag.isSame((InfoTag)ast.entity.Tag))
                    {
                        ExplodeAsteroid(ast);
                        break;
                    }
                }

                tag = (InfoTag)sender.Tag;
                foreach (Missile mis in _Missiles)
                {
                    if (tag.isSame((InfoTag)mis.entity.Tag))
                    {
                        _Missiles.Remove(mis);
                        break;
                    }
                }
            }

            if (tag.name == "Player")
            {
                KillPlayer();
            }
        }

        void Events_PairCreated_Asteroid(EntityCollidable sender, BroadPhaseEntry other, NarrowPhasePair pair)
        {
            InfoTag tag = (InfoTag)other.Tag;

            if (tag.name == "Asteroid")
            {
                tag = (InfoTag)sender.Tag;
                foreach (Asteroid ast in _Asteroids)
                {
                    if (tag.isSame((InfoTag)ast.entity.Tag))
                    {
                        SmashAsteroid(ast);
                        break;
                    }
                }
            }

            if(tag.name == "Player")
            {
                KillPlayer();
            }
        }
    }
}
