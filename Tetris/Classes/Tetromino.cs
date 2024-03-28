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
    public enum TetrominoType
    {
        T,
        L,
        Straight,
        Block,
        ReversedL,
        Z,
        ReversedZ,
    }

    public class Tetromino
    {
        private Block[] blocks;
        private Point gridPos;
        private Point[] blockOffsets;
        private Color color;
        private Point size;
        private Vector2 gamePos;
        private bool hasPressed;
        private int rotation;
        private TetrominoType type;
        private bool isPlaced;
        private TimeSpan time;

        public Tetromino(Point gridPos, Point[] blockOffsets, Color color, Vector2 gamePos, Point size, TetrominoType type)
        {
            blocks = new Block[blockOffsets.Length];
            for (int i = 0; i < blocks.Length; i++)
            {
                Vector2 blockGridPos = new Vector2(gridPos.X + blockOffsets[i].X, gridPos.Y + blockOffsets[i].Y);
                Vector2 pos = new Vector2(gamePos.X + (blockGridPos.X * size.X), gamePos.Y + (blockGridPos.Y * size.Y));
                blocks[i] = new Block(pos, color, size, blockGridPos.ToPoint());
            }

            this.gridPos = gridPos;
            this.blockOffsets = blockOffsets;
            this.color = color;
            this.gamePos = gamePos;
            this.size = size;
            this.type = type;

            isPlaced = false;
            hasPressed = false;
            time = TimeSpan.Zero;
        }

        public void Update(int gridWidth, GameTime gameTime, int[] stopingPoint, out bool isFalling)
        {
            isFalling = true;
            for(int i = 0; i < blocks.Length; i++)
            {  
                blocks[i].position = new Vector2(gamePos.X + (blocks[i].gridPos.X * size.X), gamePos.Y + (blocks[i].gridPos.Y * size.Y));
            }

            if(!isPlaced)
            {
                time += gameTime.ElapsedGameTime;
                MoveIntoGrid(gridWidth);

                if(time.TotalMilliseconds >= 750)
                {
                    MoveDown();
                    time = TimeSpan.Zero;
                }

                for(int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i].gridPos.Y >= stopingPoint[blocks[i].gridPos.X])
                    {
                        stopingPoint[blocks[i].gridPos.X] = blocks[i].gridPos.Y;
                        isFalling = false;
                    }
                }

                if(isFalling == false)
                {
                    return;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    if (!hasPressed)
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
                else if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    if (!hasPressed)
                    {
                        Rotate(gridWidth);
                        hasPressed = true;
                    }
                }
                else
                {
                    hasPressed = false;
                }
            }
            isFalling = true;
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

        private void Rotate(int gridWidth)
        {
            if(type != TetrominoType.Block)
            {
                rotation += 90;
                if(type == TetrominoType.Straight && rotation >= 180)
                { 
                    rotation = 0;
                }
                else if(rotation >= 360)
                {
                    rotation = 0;
                }
            }

            for(int i = 0; i < blocks.Length; i++)
            {
                if(rotation == 0)
                {
                    blocks[i].gridPos = new Point(gridPos.X + blockOffsets[i].X, gridPos.Y + blockOffsets[i].Y);
                }
                if (rotation == 90)
                {
                    blocks[i].gridPos = new Point(gridPos.X - blockOffsets[i].Y, gridPos.Y - blockOffsets[i].X);
                }  
                if(rotation == 180)
                {
                    blocks[i].gridPos = new Point(gridPos.X - blockOffsets[i].X, gridPos.Y - blockOffsets[i].Y);
                }
                if (rotation == 270)
                {
                    blocks[i].gridPos = new Point(gridPos.X + blockOffsets[i].Y, gridPos.Y + blockOffsets[i].X);
                }
            }

            MoveIntoGrid(gridWidth);
        }

        private void MoveIntoGrid(int gridWidth)
        {
            for(int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i].gridPos.X < 0)
                {
                    MoveRight();
                }
                if (blocks[i].gridPos.X >= gridWidth)
                {
                    MoveLeft();
                }
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
