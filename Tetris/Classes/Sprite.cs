using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Sprite
    {
        protected Vector2 position;
        protected float rotation;
        protected Vector2 scale;
        protected Texture2D spriteSheet;
        protected Color color;
        protected SpriteEffects effect;
        protected Rectangle sourceRectangle;
        protected Vector2 origin;

        public Sprite(Texture2D spriteSheet, Vector2 position, float rotation, Vector2 scale, Color color, SpriteEffects effect, Rectangle sourceRectangle, Vector2 origin)
        {
            this.spriteSheet = spriteSheet;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.color = color;
            this.effect = effect;
            this.sourceRectangle = sourceRectangle;
            this.origin = origin;
        }

        public Sprite(Vector2 position, Texture2D spriteSheet)
            :this(spriteSheet,position,0,Vector2.One,Color.White,SpriteEffects.None, new Rectangle(0,0,spriteSheet.Width,spriteSheet.Height),Vector2.Zero) { }

        public Sprite(Vector2 position, Texture2D spriteSheet, Vector2 scale)
            : this(spriteSheet, position, 0, scale, Color.White, SpriteEffects.None, new Rectangle(0, 0, spriteSheet.Width, spriteSheet.Height), Vector2.Zero) { }

        public Sprite(Vector2 position, Texture2D spriteSheet, Vector2 scale, Rectangle sourceRectangle)
            : this(spriteSheet, position, 0, scale, Color.White, SpriteEffects.None, sourceRectangle, Vector2.Zero) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, position, sourceRectangle, color, rotation, origin, scale, effect, 1);
        }
    }
}
