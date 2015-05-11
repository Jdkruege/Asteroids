using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class InfoTag
    {
        public String name;
        public int id;

        public bool isSame(InfoTag tag)
        {
            return (tag.name.CompareTo(name) == 0) && (tag.id == id);
        }
    }
}
