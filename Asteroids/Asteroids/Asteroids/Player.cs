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
        public int respawns;

        public Player(Entity e)
        {
            entity = e;
            respawns = 3;
        }

        public void SetPosition(Vector3 vec)
        {
            entity.Position = MatrixHelpers.VectorToVector(vec);
        }
    }
}
