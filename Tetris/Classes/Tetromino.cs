using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        private Point[] blockOffsets;
        private Color color;
        private Point size;
        private Vector2 gamePos;
        private bool hasPressed;
        private Point[] currentBlockOffsets;
        private int rotation;

        public Tetromino(int speed, Point gridPos, Point[] blockOffsets, Color color, Vector2 gamePos, Point size)
        {
            blocks = new Block[blockOffsets.Length];
            for(int i = 0; i < blocks.Length; i++)
            {
                Vector2 blockGridPos = new Vector2(gridPos.X + blockOffsets[i].X, gridPos.Y + blockOffsets[i].Y);
                Vector2 pos = new Vector2(gamePos.X + (blockGridPos.X * size.X), gamePos.Y + (blockGridPos.Y * size.Y));
                blocks[i] = new Block(pos, color, size, blockGridPos.ToPoint());
            }

            this.speed = speed;
            this.gridPos = gridPos;
            this.blockOffsets = blockOffsets;
            this.color = color;
            this.gamePos = gamePos;
            this.size = size;

            hasPressed = false;
            currentBlockOffsets = blockOffsets;
        }

        public void Update()
        {
            for(int i = 0; i < blocks.Length; i++)
            {  
                blocks[i].position = new Vector2(gamePos.X + (blocks[i].gridPos.X * size.X), gamePos.Y + (blocks[i].gridPos.Y * size.Y));
            }

            if(Keyboard.GetState().IsKeyDown(Keys.S))
            { 
                if(!hasPressed)
                {
                    MoveDown();
                    hasPressed = true;
                }               
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (!hasPressed)
                {
                    MoveLeft();
                    hasPressed = true;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (!hasPressed)
                {
                    MoveRight();
                    hasPressed = true;
                }
            }
            else
            {
                hasPressed = false;
            }
        }

        private void MoveDown()
        {
            gridPos.Y++;
            for(int i = 0; i < blocks.Length; i++)
            {
                blocks[i].gridPos.Y++;
            }
        }

        private void MoveRight()
        {
            gridPos.X++;
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].gridPos.X++;
            }
        }

        private void MoveLeft()
        {
            gridPos.X--;
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].gridPos.X--;
            }
        }

        private void Rotate()
        {
            for(int i = 0; i < blocks.Length; i++)
            {
                if (rotation == 90)
                {
                    currentBlockOffsets[i] = new Point(-blockOffsets[i].Y, -blockOffsets[i].X);
                }  
                if(rotation == 180)
                {
                    currentBlockOffsets[i] = new Point(-blockOffsets[i].X, -blockOffsets[i].Y);
                }
                if (rotation == 270)
                {
                    currentBlockOffsets[i] = new Point(blockOffsets[i].Y, blockOffsets[i].X);
                }
                blocks[i].gridPos = new Point(gridPos.X + currentBlockOffsets[i].X, gridPos.Y + currentBlockOffsets[i].Y);
            }
            
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
