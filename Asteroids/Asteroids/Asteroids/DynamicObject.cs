using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class DynamicObject
    {
        private Drawable _drawObj;
        private Matrix _worldMat;

        public DynamicObject(Game game, Model model, Texture2D texture, Matrix worldMat)
        {
            _drawObj = new Drawable(game, model, texture);
        }

        public void Draw(Matrix viewMat)
        {
            _drawObj.Draw(_worldMat, viewMat);
        }

    }
}
