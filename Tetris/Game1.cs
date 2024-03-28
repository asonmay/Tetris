using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

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

        private List<Tetromino> tetrominosPlaced;
        private Tetromino fallingTetromino;
        private TetrominoType nextTetromino;

        private Random random;

        private int[] stoppingPoints;

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
            };

            tetrominosPlaced = new List<Tetromino>();
            nextTetromino = TetrominoType.T;
            fallingTetromino = new Tetromino(new Point(5, 0), TetromioOffsets[nextTetromino], Color.Red, gameGridPos, new Point(32, 32), nextTetromino);
            //nextTetromino = (TetrominoType)random.Next(0, 7);
            isTetrominoFalling = true;
            

            stoppingPoints = new int[10];
            for(int i = 0; i < stoppingPoints.Length; i++)
            {
                stoppingPoints[i] = 19;
            }
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
            }
            else
            {
                tetrominosPlaced.Add(fallingTetromino);

                fallingTetromino = new Tetromino(new Point(5, 0), TetromioOffsets[nextTetromino], Color.Red, gameGridPos, new Point(32, 32), nextTetromino);
                //nextTetromino = (TetrominoType)random.Next(0, 7);
                isTetrominoFalling = true;
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

            for(int i = 0; i < tetrominosPlaced.Count; i++)
            {
                tetrominosPlaced[i].Draw(spriteBatch);
            }
        }

        private void DrawGrid(Point size, int tileSize, int lineThickness, Point position)
        {
            for(int x = 0; x < size.X; x++)
            {
                for(int y = 0; y < size.Y; y++)
                {
                    spriteBatch.DrawRectangle(new Rectangle(position.X + x*tileSize, position.Y + y*tileSize, tileSize, tileSize), Color.White, lineThickness);
                }
            }
        }
    }
}