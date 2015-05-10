using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class Player
    {
        public Entity entity;

        public Player(Entity e)
        {
            entity = e;
        }

        public void SetPosition(Vector3 vec)
        {
            entity.Position = MatrixHelpers.VectorToVector(vec);
        }
    }
}
