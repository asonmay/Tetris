using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Tetromino
    {
        private int speed;
        private Block[] blocks;
        private Point gridPos;
        public Tetromino()
        {
            
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int i = 0; i < blocks.Length; i++)
            {
                blocks[i].Draw(spriteBatch);
            }
        }
    }
}
