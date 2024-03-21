using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;

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
            DrawGrid(new Point(10, 20), 32, 2, new Point(190, 50));
            DrawGrid(new Point(4, 4), 32, 2, new Point(30, 50));
            DrawGrid(new Point(4, 4), 32, 2, new Point(540, 50));
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