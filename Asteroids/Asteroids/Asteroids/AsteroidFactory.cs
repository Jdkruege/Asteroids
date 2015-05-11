using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class AsteroidFactory
    {
        

        private Game game;
        
        private float radius;
        private float mass;
        private static Material material = new Material(1, 1, 1);

        private Model model1, model2;
        private Texture2D texture;
        public Asteroid.Size size;

        public AsteroidFactory(Game game, Asteroid.Size size, float radius, float mass, Model model1, Model model2, Texture2D texture)
        {
            this.game = game;
            this.size = size;
            this.radius = radius;
            this.mass = mass;

            this.model1 = model1;
            this.model2 = model2;
            this.texture = texture;
        }

        public Asteroid ConstructAsteroid(int id, bool isModel1, Vector3 pos, Vector3 axis, float angle, Vector3 linVel, Vector3 anglVel)
        {
            Sphere asteroid = new Sphere(MatrixHelpers.VectorToVector(pos), radius, mass)
            {
                Material = material,
                LinearDamping = 0f,
                AngularDamping = 0f,
                Orientation = MatrixHelpers.QuaternionToQuaternion(Quaternion.CreateFromAxisAngle(axis, angle)),
                LinearVelocity = MatrixHelpers.VectorToVector(linVel),
                AngularVelocity = MatrixHelpers.VectorToVector(anglVel)
            };

            InfoTag tag = new InfoTag() { name = "Asteroid", id = id };
            asteroid.Tag = tag;
            asteroid.CollisionInformation.Tag = tag;

            Model model = isModel1 ? model1 : model2;

            return new Asteroid(game, size, asteroid, model, texture);
        }
    }
}
