using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace Tetris
{
    public class Game1 : Game
    {
        private enum gameState
        {
            Home,
            Game,
            GameOver,
            LeaderBoard,
            Settings,
        }

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private gameState currentGameState;
        private Vector2 gameGridPos;
        private bool isTetrominoFalling;

        private Dictionary<TetrominoType, Point[]> TetromioOffsets;

        private Color[,] tetrominosPlaced;
        private Tetromino fallingTetromino;
        private TetrominoType nextTetromino;
        private TetrominoType savedTetromino;

        private Random random;

        private int[] stoppingPoints;

        private bool isBeingPressed;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 760;
            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            random = new Random();

            currentGameState = gameState.Game;
            gameGridPos = new Vector2(190, 50);

            TetromioOffsets = new Dictionary<TetrominoType, Point[]>()
            {
                [TetrominoType.T] = new[] {new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1)},
                [TetrominoType.L] = new[] { new Point(0, 0), new Point(0, -1), new Point(0, 1), new Point(1, 1)},
                [TetrominoType.Block] = new[] { new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1) },
                [TetrominoType.ReversedL] = new[] { new Point(0, 0), new Point(0, -1), new Point(0, 1), new Point(-1, 1) },
                [TetrominoType.Straight] = new[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(2, 0) },
                [TetrominoType.Z] = new[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(2, 1) },
                [TetrominoType.ReversedZ] = new[] { new Point(0, 0), new Point(-1, 0), new Point(-1, 1), new Point(-2, 1) },
            };

            tetrominosPlaced = new Color[10,20];
            nextTetromino = TetrominoType.T;
            fallingTetromino = new Tetromino(new Point(5, 0), TetromioOffsets[nextTetromino], Color.Red, gameGridPos, new Point(32, 32), nextTetromino);
            nextTetromino = (TetrominoType)random.Next(0, 7);
            isTetrominoFalling = true;
            

            stoppingPoints = new int[10];
            for(int i = 0; i < stoppingPoints.Length; i++)
            {
                stoppingPoints[i] = 19;
            }

            for(int x = 0; x < tetrominosPlaced.GetLength(0); x++)
            {
                for(int y = 0; y < tetrominosPlaced.GetLength(1); y++)
                {
                    tetrominosPlaced[x, y] = Color.Transparent;
                }
            }

            isBeingPressed = false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (currentGameState)
            {
                case gameState.Game:
                    RunGame(10, gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        private void RunGame(int gridWidth, GameTime gameTime)
        {
            if(isTetrominoFalling)
            {
                fallingTetromino.Update(gridWidth, gameTime, stoppingPoints, out isTetrominoFalling);
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {                   
                    if(!isBeingPressed)
                    {
                        isBeingPressed = true;
                        TetrominoType temp = savedTetromino;
                        savedTetromino = fallingTetromino.type;
                        fallingTetromino = new Tetromino(new Point(5, 0), TetromioOffsets[nextTetromino], new Color(random.Next(3, 20) * 10, random.Next(3, 20) * 10, random.Next(3, 20) * 10), gameGridPos, new Point(32, 32), savedTetromino);
                    }
                }
                else
                {
                    isBeingPressed = false;
                }
            }
            else
            {
                for(int i = 0; i < fallingTetromino.blocks.Length; i++)
                {
                    tetrominosPlaced[fallingTetromino.blocks[i].gridPos.X, fallingTetromino.blocks[i].gridPos.Y] = fallingTetromino.color;
                }

                fallingTetromino = new Tetromino(new Point(5, 0), TetromioOffsets[nextTetromino], new Color(random.Next(3,20) * 10,random.Next(3,20) * 10,random.Next(3,20) * 10), gameGridPos, new Point(32, 32), nextTetromino);
                nextTetromino = (TetrominoType)random.Next(0, 7);
                isTetrominoFalling = true;

                for (int y = 0; y < tetrominosPlaced.GetLength(1); y++)
                {
                    bool isFullRow = true;
                    for (int x = 0; x < tetrominosPlaced.GetLength(0); x++)
                    {
                        if (tetrominosPlaced[x, y] == Color.Transparent)
                        {
                            isFullRow = false;
                        }
                    }
                    if(isFullRow)
                    {
                        DeleteRow(y);
                    }
                }
            }
        }

        private void DeleteRow(int y)
        {
            for (int i = y; i >= 0; i--)
            {
                for (int g = 0; g < tetrominosPlaced.GetLength(0); g++)
                {
                    if (i - 1 < 0)
                    {
                        tetrominosPlaced[g, i] = Color.Transparent;
                    }
                    else
                    {
                        tetrominosPlaced[g, i] = tetrominosPlaced[g, i - 1];
                    }
                }
            }
            for(int i = 0; i < stoppingPoints.Length; i++)
            {
                stoppingPoints[i]++;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            switch (currentGameState)
            {
                case gameState.Game:
                    DrawGame();
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGame()
        {
            DrawGrid(new Point(10, 20), 32, 2, gameGridPos.ToPoint());
            DrawGrid(new Point(4, 4), 32, 2, new Point(30, 50));
            DrawGrid(new Point(4, 4), 32, 2, new Point(540, 50));

            fallingTetromino.Draw(spriteBatch);

            for (int x = 0; x < tetrominosPlaced.GetLength(0); x++)
            {
                for (int y = 0; y < tetrominosPlaced.GetLength(1); y++)
                {
                    if (tetrominosPlaced[x,y] != Color.Transparent)
                    {
                        spriteBatch.FillRectangle(new Rectangle((int)gameGridPos.X + x * 32, (int)gameGridPos.Y + y * 32, 32, 32), tetrominosPlaced[x, y]);
                    }
                }
            }
        }

        private void DrawGrid(Point size, int tileSize, int lineThickness, Point position)
        {
            for(int x = 0; x < size.X; x++)
            {
                for(int y = 0; y < size.Y; y++)
                {
                    if(y == stoppingPoints[x])
                    {
                        spriteBatch.DrawRectangle(new Rectangle(position.X + x * tileSize, position.Y + y * tileSize, tileSize, tileSize), Color.Red, lineThickness);
                    }
                    else
                    {
                        spriteBatch.DrawRectangle(new Rectangle(position.X + x * tileSize, position.Y + y * tileSize, tileSize, tileSize), Color.White, lineThickness);
                    }
                }
            }
        }
    }
}