using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;

namespace Tetris
{
    public class Block : Sprite
    {
        private Color blockColor;
        private Point size;
        public Point gridPos;

        public Block(Vector2 position, float rotation, Vector2 scale, SpriteEffects effect, Vector2 origin, Color blockColor, Point size, Point gridPos)
            :base(null, position, rotation, scale, Color.White, effect, Rectangle.Empty, origin)
        {
            this.blockColor = blockColor;
            this.size = size;
            this.gridPos = gridPos;
        }

        public Block(Vector2 position, Color blockColor, Point size, Point gridPos)
            : this(position, 0, Vector2.One, SpriteEffects.None, Vector2.Zero, blockColor, size, gridPos) { }

        public Block(Vector2 position, Color blockColor, Point size, Point gridPos, Vector2 scale)
            : this(position, 0, scale, SpriteEffects.None, Vector2.Zero, blockColor, size, gridPos) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(new Rectangle(position.ToPoint(), size), blockColor);
        }
    }
}
