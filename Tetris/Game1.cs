using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Newtonsoft.Json;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        private Point savedTetrominoGridPos;
        private Point nextTetrominoGridPos;
        private Vector2 gameGridPos;

        private gameState currentGameState;
        private SpriteFont gameFont;
        private SpriteFont gameOverFont;

        private Dictionary<TetrominoType, Point[]> TetromioOffsets;
        private bool isTetrominoFalling;
        private Color[,] tetrominosPlaced;
        private Tetromino fallingTetromino;
        private TetrominoType nextTetromino;
        private TetrominoType savedTetromino;

        private Random random;

        private int[] stoppingPoints;

        private bool isBeingPressed;

        private int score;
        private int level;

        private string myTextBoxDisplayCharacters;
        private bool isFocued = false;
        public static GameWindow gw;
        private bool beingPressed;
        private Rectangle textbox;

        public struct LeaderBoardEntry
        {
            public int score;
            public string name;

            public LeaderBoardEntry(string name, int score)
            {
                this.score = score;
                this.name = name;
            }
        }
        private List<LeaderBoardEntry> leaderBoard;

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
            savedTetrominoGridPos = new Point(30, 50);
            nextTetrominoGridPos = new Point(540, 50);
            gameGridPos = new Vector2(190, 50);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            random = new Random();

            currentGameState = gameState.Game;

            TetromioOffsets = new Dictionary<TetrominoType, Point[]>()
            {
                [TetrominoType.None] = new Point[0],
                [TetrominoType.T] = new[] {new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1)},
                [TetrominoType.L] = new[] { new Point(0, 0), new Point(0, -1), new Point(0, 1), new Point(1, 1)},
                [TetrominoType.Block] = new[] { new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1) },
                [TetrominoType.ReversedL] = new[] { new Point(0, 0), new Point(0, -1), new Point(0, 1), new Point(-1, 1) },
                [TetrominoType.Straight] = new[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(2, 0) },
                [TetrominoType.Z] = new[] { new Point(0, 0), new Point(-1, 0), new Point(0, 1), new Point(1, 1) },
                [TetrominoType.ReversedZ] = new[] { new Point(0, 0), new Point(-1, 0), new Point(-1, 1), new Point(-2, 1) },
            };

            tetrominosPlaced = new Color[10,20];
            nextTetromino = TetrominoType.T;
            fallingTetromino = new Tetromino(new Point(5, 0), TetromioOffsets[nextTetromino], Color.Red, gameGridPos, new Point(32, 32), nextTetromino);
            nextTetromino = (TetrominoType)random.Next(0, 7);
            isTetrominoFalling = true;
            savedTetromino = TetrominoType.None;

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

            gameFont = Content.Load<SpriteFont>("GameFont");
            gameOverFont = Content.Load<SpriteFont>("GameOverFont");
            level = 0;

            isFocued = false;
            beingPressed = false;
            myTextBoxDisplayCharacters = "";
            gw = Window;
            textbox = new Rectangle(savedTetrominoGridPos.X, savedTetrominoGridPos.Y + 4 * 32 + 150, 100, (int)gameFont.MeasureString("L").Y);

            leaderBoard = new List<LeaderBoardEntry>((LeaderBoardEntry[])System.Text.Json.JsonSerializer.Deserialize(File.ReadAllText("../../../Leaders.json"), typeof(LeaderBoardEntry[])));
        }

        private void OnInput(object sender, TextInputEventArgs e)
        {
            var k = e.Key;
            var c = e.Character;
            if (k == Keys.Space)
            {
                myTextBoxDisplayCharacters += ' ';
            }
            else if (k == Keys.Back)
            {
                myTextBoxDisplayCharacters = myTextBoxDisplayCharacters.Remove(myTextBoxDisplayCharacters.Length - 1, 1);
            }
            else
            {
                myTextBoxDisplayCharacters += c;
            }

            Console.WriteLine(myTextBoxDisplayCharacters);
        }

        private void FocusOnTextbox(bool isMouseClicked, Rectangle box, Point mousePos)
        {
            if (isMouseClicked && box.Intersects(new Rectangle(mousePos, new Point(1, 1))))
            {
                if (!beingPressed)
                {
                    beingPressed = true;
                    isFocued = !isFocued;
                    if (isFocued)
                    {
                        gw.TextInput += OnInput;
                    }
                    else if (!isFocued && myTextBoxDisplayCharacters != "")
                    {
                        gw.TextInput -= OnInput;
                        leaderBoard.Add(new LeaderBoardEntry(myTextBoxDisplayCharacters, score));
                        int currentIndex = 0;
                        int numberOfChanges = 0;
                        while (true)
                        {
                            if (currentIndex >= leaderBoard.Count - 1)
                            {
                                currentIndex = 0;
                                if (numberOfChanges == 0)
                                {
                                    break;
                                }
                                numberOfChanges = 0;
                            }
                            if (leaderBoard[currentIndex].score > leaderBoard[currentIndex + 1].score)
                            {
                                LeaderBoardEntry temp = leaderBoard[currentIndex];
                                leaderBoard[currentIndex] = leaderBoard[currentIndex + 1];
                                leaderBoard[currentIndex + 1] = temp;
                                numberOfChanges++;
                            }

                            currentIndex++;
                        }

                        leaderBoard.Reverse();
                    }
                }
            }
            else
            {
                beingPressed = false;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                string fileName = "Leaders.json";
                string jsonString = JsonConvert.SerializeObject(leaderBoard);
                File.WriteAllText(fileName, jsonString);
                Exit();
            }
               

            switch (currentGameState)
            {
                case gameState.Game:
                    RunGame(10, gameTime);
                    break;
                case gameState.GameOver:
                    UpdateLeaderBoard();
                    break;

            }

            base.Update(gameTime);
        }

        private void UpdateLeaderBoard()
        {
            FocusOnTextbox(Mouse.GetState().LeftButton == ButtonState.Pressed, textbox, Mouse.GetState().Position);

            //for (int i = leaderBoard.Count - 1; i > 11; i--)
            //{
            //    leaderBoard.RemoveAt(i);
            //}
            

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                currentGameState = gameState.Game;
                string fileName = "Leaders.json";
                string jsonString = JsonConvert.SerializeObject(leaderBoard);
                File.WriteAllText(fileName, jsonString);
                LoadContent();
            }
        }

        private void RunGame(int gridWidth, GameTime gameTime)
        {
            if(isTetrominoFalling)
            {
                if(fallingTetromino.Update(gridWidth, gameTime, stoppingPoints, out isTetrominoFalling))
                {
                    currentGameState = gameState.GameOver;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {                   
                    if(!isBeingPressed)
                    {
                        isBeingPressed = true;
                        if(savedTetromino == TetrominoType.None)
                        {
                            savedTetromino = fallingTetromino.type;
                            fallingTetromino = new Tetromino(new Point(5, 0), TetromioOffsets[nextTetromino], new Color(random.Next(3, 20) * 10, random.Next(3, 20) * 10, random.Next(3, 20) * 10), gameGridPos, new Point(32, 32), nextTetromino);
                            nextTetromino = (TetrominoType)random.Next(1, 7);
                        }
                        else
                        {
                            TetrominoType temp = savedTetromino;
                            savedTetromino = fallingTetromino.type;
                            fallingTetromino = new Tetromino(new Point(5, 0), TetromioOffsets[temp], new Color(random.Next(3, 20) * 10, random.Next(3, 20) * 10, random.Next(3, 20) * 10), gameGridPos, new Point(32, 32), temp);
                        }
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

                fallingTetromino = new Tetromino(new Point(5, 0), TetromioOffsets[nextTetromino], new Color(random.Next(3,20) * 10, random.Next(3,20) * 10, random.Next(3,20) * 10), gameGridPos, new Point(32, 32), nextTetromino);
                for(int i = 0; i < fallingTetromino.blocks.Length; i++)
                {
                    if (tetrominosPlaced[fallingTetromino.blocks[i].gridPos.X, fallingTetromino.blocks[i].gridPos.Y + 1] != Color.Transparent)
                    {
                        fallingTetromino.gridPos.X--;
                        currentGameState = gameState.GameOver;
                    }
                }
                nextTetromino = (TetrominoType)random.Next(1, 7);
                isTetrominoFalling = true;

                int rowsDeleted = 0;
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
                        rowsDeleted++;
                    }
                }

                switch (rowsDeleted)
                {
                    case 1:
                        score += 40 * (level + 1);
                        break;
                    case 2:
                        score += 100 * (level + 1);
                        break;
                    case 3:
                        score += 300 * (level + 1);
                        break;
                    case 4:
                        score += 1200 * (level + 1);
                        break;
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

                case gameState.GameOver:
                    DrawGameOver();
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGame()
        {
            spriteBatch.DrawString(gameFont, "Next", new Vector2(nextTetrominoGridPos.X, nextTetrominoGridPos.Y - 32), Color.White);
            spriteBatch.DrawString(gameFont, "Saved", new Vector2(savedTetrominoGridPos.X, savedTetrominoGridPos.Y - 32), Color.White);
            spriteBatch.DrawString(gameFont, $"Score: {score}", new Vector2(gameGridPos.X, gameGridPos.Y - 32), Color.White);

            DrawGrid(new Point(10, 20), 32, 2, gameGridPos.ToPoint());
            DrawGrid(new Point(4, 4), 32, 2, savedTetrominoGridPos);
            DrawGrid(new Point(4, 4), 32, 2, nextTetrominoGridPos);

            fallingTetromino.Draw(spriteBatch);
            DrawTetromino(savedTetromino, savedTetrominoGridPos);
            DrawTetromino(nextTetromino, nextTetrominoGridPos);

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

        private void DrawGameOver()
        {
            DrawGame();

            spriteBatch.DrawString(gameOverFont, "GAME OVER", new Vector2(gameGridPos.X + 48, gameGridPos.Y + 20 * 32), Color.White);
            spriteBatch.DrawString(gameFont, "PRESS SPACE", new Vector2(savedTetrominoGridPos.X - 10, savedTetrominoGridPos.Y + 5 * 32), Color.White);
            spriteBatch.DrawString(gameFont, "TO GO HOME", new Vector2(savedTetrominoGridPos.X, savedTetrominoGridPos.Y + 6 * 32 - 15), Color.White);

            for (int i = 0; i < 10; i++)
            {
                if (i < leaderBoard.Count)
                {
                    spriteBatch.DrawString(gameFont, $"#{i + 1} {leaderBoard[i].name} {leaderBoard[i].score}", new Vector2(nextTetrominoGridPos.X, nextTetrominoGridPos.Y + 4 * 32 + i * gameFont.MeasureString("L").Y), Color.White);
                }
                else
                {
                    break;
                }
            }

            spriteBatch.FillRectangle(textbox, Color.White);
            spriteBatch.DrawString(gameFont, myTextBoxDisplayCharacters, textbox.Location.ToVector2(), Color.Black);
        }

        private void DrawTetromino(TetrominoType type, Point gridPos)
        {
            for(int i = 0; i < TetromioOffsets[type].Length; i++)
            {
                spriteBatch.FillRectangle(new RectangleF(gridPos.X + (TetromioOffsets[type][i].X + 1) * 32, gridPos.Y + (TetromioOffsets[type][i].Y + 1) * 32, 32, 32), Color.Red);
            }
        }

        private void DrawGrid(Point size, int tileSize, int lineThickness, Point position)
        {
            for(int x = 0; x < size.X; x++)
            {
                for(int y = 0; y < size.Y; y++)
                {
                    spriteBatch.DrawRectangle(new Rectangle(position.X + x * tileSize, position.Y + y * tileSize, tileSize, tileSize), Color.White, lineThickness);
                }
            }
        }
    }
}