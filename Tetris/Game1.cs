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

        private Tetromino test;
        private Dictionary<string, Point[]> TetromioOffsets;

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

            currentGameState = gameState.Game;
            gameGridPos = new Vector2(190, 50);

            TetromioOffsets = new Dictionary<string, Point[]>()
            {
                ["T"] = new[] {new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1)},
                ["L"] = new[] { new Point(0, 0), new Point(0, -1), new Point(0, 1), new Point(1, 1)},
                ["Block"] = new[] { new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1) },
                ["Reversed L"] = new[] { new Point(0, 0), new Point(0, -1), new Point(0, 1), new Point(-1, 1) },
                ["Straight"] = new[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(2, 0) },
            };

            test = new Tetromino(4, new Point(5, 5), TetromioOffsets["Straight"],Color.Red, gameGridPos,new Point(32,32));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (currentGameState)
            {
                case gameState.Game:
                    RunGame();
                    break;
            }

            base.Update(gameTime);
        }

        private void RunGame()
        {
            test.Update();
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

            test.Draw(spriteBatch);
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