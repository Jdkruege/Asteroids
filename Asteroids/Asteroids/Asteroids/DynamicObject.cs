using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using BEPUphysics.Entities;

namespace Asteroids
{
    class DynamicObject
    {
        private Drawable _drawObj;
        public Entity entity;
        float xScale, yScale, zScale;

        public DynamicObject(Game game, Entity e,  Model model, Texture2D texture, float scaleX, float scaleY, float scaleZ)
        {
            _drawObj = new Drawable(game, model, texture);
            entity = e;

            xScale = scaleX;
            yScale = scaleY;
            zScale = scaleZ;
        }

        public void Draw(Matrix viewMat)
        {
            Matrix worldMat = MatrixHelpers.MatrixToMatrix(entity.WorldTransform);

            worldMat = Matrix.CreateScale(xScale, yScale, zScale) * worldMat; 

            _drawObj.Draw(worldMat, viewMat);
        }

    }
}
