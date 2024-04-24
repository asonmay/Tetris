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
        None,
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
        public Block[] blocks;
        public Point gridPos;
        private Point[] blockOffsets;
        public Color color;
        private Point size;
        private Vector2 gamePos;
        private bool hasPressed;
        private int rotation;
        public TetrominoType type;
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
            hasPressed = true;
            time = TimeSpan.Zero;
        }

        public bool Update(int gridWidth, GameTime gameTime, int[] stopingPoint, out bool isFalling)
        {
            isFalling = true;
            bool gameOver = false;
            for(int i = 0; i < blocks.Length; i++)
            {  
                blocks[i].position = new Vector2(gamePos.X + (blocks[i].gridPos.X * size.X), gamePos.Y + (blocks[i].gridPos.Y * size.Y));
            }

            if(!isPlaced)
            {
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i].gridPos.Y == stopingPoint[blocks[i].gridPos.X] + 1)
                    {
                        isFalling = false;
                        if (blocks[i].gridPos.Y <= 0)
                        {
                            gameOver = true;
                        }
                    }
                }

                time += gameTime.ElapsedGameTime;
                MoveIntoGrid(gridWidth);

                if(time.TotalMilliseconds >= 750)
                {
                    MoveDown();
                    time = TimeSpan.Zero;
                }          

                if(isFalling == false)
                {
                    for(int i = 0; i < blocks.Length; i++)
                    {
                        if (blocks[i].gridPos.Y <= stopingPoint[blocks[i].gridPos.X])
                        {
                            stopingPoint[blocks[i].gridPos.X] = blocks[i].gridPos.Y - 2;
                        }
                    }
                    return gameOver;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    if (!hasPressed)
                    {
                        bool isTouching = false;
                        while(!isTouching)
                        {
                            MoveDown();
                            for (int i = 0; i < blocks.Length; i++)
                            {
                                if (blocks[i].gridPos.Y == stopingPoint[blocks[i].gridPos.X])
                                {
                                    isTouching = true;
                                }
                            }
                        }

                        for (int i = 0; i < blocks.Length; i++)
                        {
                            if (blocks[i].gridPos.Y <= stopingPoint[blocks[i].gridPos.X])
                            {
                                stopingPoint[blocks[i].gridPos.X] = blocks[i].gridPos.Y - 1;
                            }
                        }
                        
                        return gameOver;
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    if (!hasPressed)
                    {
                        MoveLeft(gridWidth);
                        hasPressed = true;
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    if (!hasPressed)
                    {
                        MoveRight(gridWidth);
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
            return gameOver;
        }

        private void MoveDown()
        {
            gridPos.Y++;
            for(int i = 0; i < blocks.Length; i++)
            {
                blocks[i].gridPos.Y++;
            }
        }

        private void MoveRight(int gridWidth)
        {
            gridPos.X++;
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].gridPos.X++;
            }           
            MoveIntoGrid(gridWidth);
        }

        private void MoveLeft(int gridWidth)
        {
            gridPos.X--;
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].gridPos.X--;
            }
            MoveIntoGrid(gridWidth);
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
                    MoveRight(gridWidth);
                }
                if (blocks[i].gridPos.X >= gridWidth)
                {
                    MoveLeft(gridWidth);
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
