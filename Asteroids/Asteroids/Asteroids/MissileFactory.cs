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
    class MissileFactory
    {
        private Game game;
        private float radius;
        private float mass;
        private static Material material = new Material(1, 1, 1);

        private Model model;
        private Texture2D texture;

        public MissileFactory(Game game, float radius, float mass, Model model, Texture2D texture)
        {
            this.game = game;
            this.radius = radius;
            this.mass = mass;

            this.model = model;
            this.texture = texture;
        }

        public Missile ConstructMissile(int id, Matrix world, Vector3 linVel)
        {
            Sphere missile = new Sphere(MatrixHelpers.VectorToVector(new Vector3(0, 0, 0)), radius, mass)
            {
                Material = material,
                LinearDamping = 0f,
                AngularDamping = 0f,
                WorldTransform = MatrixHelpers.MatrixToMatrix(world),
                LinearVelocity = MatrixHelpers.VectorToVector(linVel)
            };

            InfoTag tag = new InfoTag() { name = "Missile", id = id };

            missile.Tag = tag;
            missile.CollisionInformation.Tag = tag;

            return new Missile(game, missile, model, texture);
        }
    }
}
