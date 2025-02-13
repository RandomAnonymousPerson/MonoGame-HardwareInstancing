﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace VFRZInstancing
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TileMap map;
        private SpriteFont _spriteFont;
        private FrameCounter _frameCounter;

        public Game1()
        {
            _frameCounter = new FrameCounter();
            _graphics = new GraphicsDeviceManager(this);

            this.map = new TileMap(this, 128, 128);
            this.Components.Add(this.map);
        }

        protected override void Initialize()
        {
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferMultiSampling = true;
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics.SynchronizeWithVerticalRetrace = false;

            _graphics.ApplyChanges();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(16);
            //Wen man Borderless angibt  wird grafikarte nich voll benutzt
            this.Window.IsBorderless = false;
            this.Window.Position = new Point(0, 0);
    


            this.Content.RootDirectory = "Content";

         
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("Arial");

            // TODO: use this.Content to load your game content here
     
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _frameCounter.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            RenderTarget2D renderTarget = map.Draw(gameTime);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, TileMap.SS_PointBorder, DepthStencilState.DepthRead, RasterizerState.CullNone, null, null);

            if (map.useRenderTarget)
            {
                _spriteBatch.Draw(renderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, map.scale, SpriteEffects.None, 0);
            }

            _frameCounter.DrawFps(_spriteBatch, _spriteFont, new Vector2(1, 1), Color.White);
            _spriteBatch.DrawString(_spriteFont, "F1 to Change Random Textures from Spritesheet", new Vector2(1,20), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_spriteFont, "F2 Change used Array Each Frame: "+map.ChangeArrayEachFrame, new Vector2(1, 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_spriteFont, "F3 Change PixelWidth, Currently: " + ((map.ImageWidth32Pixel? "32 Pixel":"30 Pixel")), new Vector2(1, 60), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_spriteFont, "F4 Change use of RenderTarget, Currently: " + ((map.useRenderTarget ? "true" : "false")), new Vector2(1, 80), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);


            _spriteBatch.End();
            base.Draw(gameTime);
        }


    }
}
