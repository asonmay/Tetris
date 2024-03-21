using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Piece : Sprite
    {
        protected int speed; 

        public Piece(Texture2D spriteSheet, Vector2 position, float rotation, Vector2 scale, Color color, SpriteEffects effect, Rectangle sourceRectangle, Vector2 origin)
            :base(spriteSheet, position, rotation, scale, color, effect, sourceRectangle, origin) 
        {

        }
    }
}
