using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Monogame1
{
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const float SKYRATIO = 2f / 3f;
        int screenWidth, screenHeight;
        Texture2D grass, startGameSplash, gameOverTexture;
        SpriteFont scoreFont, stateFont;
        Sprite dino, broccoli;
        bool spaceDown, gameStarted, gameOver;
        float broccoliSpeedMultiplier, gravitySpeed, dinoSpeedX, dinoJumpY, score;

        Random random;
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 900;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            screenHeight = GraphicsDevice.Viewport.Bounds.Height;
            screenWidth = GraphicsDevice.Viewport.Bounds.Width;
            this.IsMouseVisible = false;

            broccoliSpeedMultiplier = 0.5f;
            spaceDown = false;
            gameStarted = false;
            score = 0;
            random = new Random();
            dinoSpeedX = 1000f;
            dinoJumpY = -1200f;
            gravitySpeed = 30f;
            gameOver = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            grass = Content.Load<Texture2D>("grass");
            dino = new Sprite(GraphicsDevice, Content.Load<Texture2D>("ninja-cat-dino"), 1f);
            broccoli = new Sprite(GraphicsDevice, Content.Load<Texture2D>("broccoli"), 0.2f);
            startGameSplash = Content.Load<Texture2D>("start-splash");
            gameOverTexture = Content.Load<Texture2D>("game-over");
            scoreFont = Content.Load<SpriteFont>("Score");
            stateFont = Content.Load<SpriteFont>("GameState");


        }

        protected override void UnloadContent()
        {
            spriteBatch.Dispose();
        }

        public void SpawnBroccoli()
        {
            int direction = random.Next(1, 5);
            switch (direction)
            {
                case 1:
                    broccoli.x = -100;
                    broccoli.y = random.Next(0, screenHeight);
                    break;
                case 2:
                    broccoli.y = -100;
                    broccoli.x = random.Next(0, screenWidth);
                    break;
                case 3:
                    broccoli.x = screenWidth + 100;
                    broccoli.y = random.Next(0, screenHeight);
                    break;
                case 4:
                    broccoli.y = screenHeight + 100;
                    broccoli.x = random.Next(0, screenWidth);
                    break;
            }
            if (score % 5 == 0) broccoliSpeedMultiplier += 0.2f;

            broccoli.dX = (dino.x - broccoli.x) * broccoliSpeedMultiplier;
            broccoli.dY = (dino.y - broccoli.y) * broccoliSpeedMultiplier;
            broccoli.dA = 7f;
        }

        public void StartGame()
        {
            dino.x = screenWidth / 2;
            dino.y = screenHeight * SKYRATIO;
            broccoliSpeedMultiplier = 0.5f;
            SpawnBroccoli();
            score = 0;
        }


        void KeyboardHandler()
        {
            KeyboardState state = Keyboard.GetState();

            //quit the game if escape is pressed
            if (state.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (!gameStarted)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    StartGame();
                    gameStarted = true;
                    spaceDown = true;
                    gameOver = false;
                }
                return;
            }

            // jump if space is pressed
            if (state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.Up))
            {
                if (!spaceDown && dino.y >= screenHeight * SKYRATIO - 1) dino.dY = dinoJumpY;

                spaceDown = true;
            }
            else
                spaceDown = false;

            // Handle Left and Right
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                dino.dX = dinoSpeedX * -1;
            else if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                dino.dX = dinoSpeedX;
            else
                dino.dX = 0;

            if (gameOver && state.IsKeyDown(Keys.Enter))
            {
                StartGame();
                gameOver = false;
            }
        }
        protected override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardHandler();

            dino.Update(elapsedTime);
            broccoli.Update(elapsedTime);

            dino.dY += gravitySpeed;

            if (dino.y > screenHeight * SKYRATIO)
            {
                dino.dY = 0;
                dino.y = screenHeight * SKYRATIO;
            }

            if (dino.x > screenWidth - dino.texture.Width / 2)
            {
                dino.x = screenWidth - dino.texture.Width / 2;
                dino.dX = 0;
            }
            if (dino.x < 0 + dino.texture.Width / 2)
            {
                dino.x = 0 + dino.texture.Width / 2;
                dino.dX = 0;
            }

            if (broccoli.y > screenHeight + 100 || broccoli.y < -100 || broccoli.x > screenWidth + 100 || broccoli.x < -100)
            {
                SpawnBroccoli();
                score++;
            }

            if (gameOver)
            {
                dino.dX = 0;
                dino.dY = 0;
                broccoli.dX = 0;
                broccoli.dY = 0;
                broccoli.dA = 0;
            }
            if (dino.RectangleCollision(broccoli)) gameOver = true;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(grass, new Rectangle(0, (int)(screenHeight * SKYRATIO), screenWidth, screenHeight), Color.White);
            if (gameOver)
            {
                spriteBatch.Draw(gameOverTexture, new Vector2(screenWidth / 2 - gameOverTexture.Width / 2, screenHeight / 4 - gameOverTexture.Width / 2), Color.White);
                String pressEnter = "Press Enter to restart!";

                Vector2 pressEnterSize = stateFont.MeasureString(pressEnter);

                spriteBatch.DrawString(stateFont, pressEnter, new Vector2(screenWidth / 2 - pressEnterSize.X / 2, screenHeight - 200), Color.White);
            }
            broccoli.Draw(spriteBatch);
            dino.Draw(spriteBatch);
            spriteBatch.DrawString(scoreFont, score.ToString(), new Vector2(screenWidth - 100, 50), Color.Black);

            if (!gameStarted)
            {
                spriteBatch.Draw(startGameSplash, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                var title = "VEGGIE JUMP";
                var pressSpace = "Press Space to start";

                Vector2 titleSize = stateFont.MeasureString(title);
                Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);

                spriteBatch.DrawString(stateFont, title, new Vector2(screenWidth / 2 - titleSize.X / 2, screenHeight / 3), Color.ForestGreen);
                spriteBatch.DrawString(stateFont, pressSpace, new Vector2(screenWidth / 2 - pressSpaceSize.X / 2, screenHeight / 2), Color.White);
            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
