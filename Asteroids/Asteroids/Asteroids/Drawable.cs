using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class Drawable
    {
        private Model _model;
        private Game _game;

        public Drawable(Game game, Model model, Texture2D texture)
        {
            _game = game;
            _model = model;

            foreach(ModelMesh mesh in _model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.Texture = texture;
                    effect.TextureEnabled = true;
                }
            }
        }

        public void Draw(Matrix worldMat, Matrix viewMat)
        {
            foreach(ModelMesh mesh in _model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    
                    effect.World = worldMat;
                    effect.View = viewMat;
                    effect.Projection = (Matrix)_game.Services.GetService(typeof(Matrix));
                }

                mesh.Draw();
            }
        }
    }
}
