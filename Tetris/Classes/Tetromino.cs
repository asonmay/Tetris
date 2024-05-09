using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SharpDX.Direct2D1.Effects;
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
            this.color = color;
            this.gamePos = gamePos;
            this.size = size;
            this.type = type;

            isPlaced = false;
            hasPressed = true;
            time = TimeSpan.Zero;
        }

        public bool Update(int gridWidth, GameTime gameTime, int[] stopingPoint, out bool isFalling, Color[,] tetrominos)
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
                    if (blocks[i].gridPos.Y == stopingPoint[blocks[i].gridPos.X])
                    {
                        isFalling = false;
                        if (blocks[i].gridPos.Y <= 0)
                        {
                            gameOver = true;
                        }
                    }
                }

                time += gameTime.ElapsedGameTime;
                MoveIntoGrid(gridWidth, tetrominos);

                if (time.TotalMilliseconds >= 750)
                {
                    MoveDown(tetrominos, ref isFalling);
                    time = TimeSpan.Zero;
                }

                if (isFalling == false)
                {
                    for (int i = 0; i < blocks.Length; i++)
                    {
                        if (blocks[i].gridPos.Y <= stopingPoint[blocks[i].gridPos.X])
                        {
                            stopingPoint[blocks[i].gridPos.X] = blocks[i].gridPos.Y - 1;
                        }
                    }
                    return gameOver;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    if (!hasPressed)
                    {
                        isFalling = false;
                        bool isTouching = false;
                        while(!isTouching)
                        {
                            MoveDown(tetrominos, ref isFalling);
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
                        MoveLeft(gridWidth,tetrominos);
                        hasPressed = true;
                    }
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
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    if (!hasPressed)
                    {
                        MoveRight(gridWidth, tetrominos);
                        hasPressed = true;
                    }
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
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    if (!hasPressed)
                    {
                        Rotate(gridWidth, tetrominos);
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

        private void MoveDown(Color[,] tetromino, ref bool isfalling)
        {
            bool temp = false;
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i].gridPos.Y < 19 && tetromino[blocks[i].gridPos.X, blocks[i].gridPos.Y + 1] != Color.Transparent)
                {
                    temp = true;
                }
            }
            if (!temp)
            {
                gridPos.Y++;
                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i].gridPos.Y++;
                }
            }
            else
            {
                isfalling = true;
            }
        }

        private void MoveRight(int gridWidth, Color[,] tetromino)
        {
            bool temp = false;
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i].gridPos.X + 1 < tetromino.GetLength(0) && blocks[i].gridPos.Y > 0 && tetromino[blocks[i].gridPos.X + 1, blocks[i].gridPos.Y] != Color.Transparent)
                {
                    temp = true;
                }
            }
            if(!temp)
            {
                gridPos.X++;
                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i].gridPos.X++;
                }
                MoveIntoGrid(gridWidth, tetromino);
            }
        }

        private void MoveLeft(int gridWidth, Color[,] tetromino)
        {
            bool temp = false;
            for(int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i].gridPos.X - 1 > 0 && blocks[i].gridPos.Y > 0 && tetromino[blocks[i].gridPos.X - 1, blocks[i].gridPos.Y] != Color.Transparent)
                {
                    temp = true;
                }
            }
            if(!temp)
            {
                gridPos.X--;
                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i].gridPos.X--;
                }
                MoveIntoGrid(gridWidth, tetromino);
            }
        }

        private void Rotate(int gridWidth, Color[,] tetromino)
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
                Point translationalPositoin = new Point(blocks[i].gridPos.X - gridPos.X, blocks[i].gridPos.Y - gridPos.Y);
                blocks[i].gridPos.X = (int)Math.Round(translationalPositoin.X * Math.Cos(Math.PI / 2) - translationalPositoin.Y * Math.Sin(Math.PI / 2)) + gridPos.X;
                blocks[i].gridPos.Y = (int)Math.Round(translationalPositoin.X * Math.Sin(Math.PI / 2) + translationalPositoin.Y * Math.Cos(Math.PI / 2)) + gridPos.Y;
            }

            MoveIntoGrid(gridWidth, tetromino);
        }

        private void MoveIntoGrid(int gridWidth, Color[,] tetrominos)
        {
            for(int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i].gridPos.X < 0)
                {
                    MoveRight(gridWidth, tetrominos);
                }
                if (blocks[i].gridPos.X >= gridWidth)
                {
                    MoveLeft(gridWidth, tetrominos);
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
