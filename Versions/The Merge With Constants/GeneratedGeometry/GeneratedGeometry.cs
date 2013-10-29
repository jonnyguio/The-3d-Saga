#region File Description
//-----------------------------------------------------------------------------
// GeneratedGeometry.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace GeneratedGeometry
{
    /// <summary>
    /// Sample showing how to use geometry that is programatically
    /// generated as part of the content pipeline build process.
    /// </summary>
    public class GeneratedGeometryGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        Ship.Camera camera;
        float aspectRatio;

        Model terrain;
        Sky sky;
        Ship.LirouShip ship;

        #endregion

        #region Initialization


        public GeneratedGeometryGame()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;
            //IsMouseVisible = true;
            aspectRatio = (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            Content.RootDirectory = "Content";
            ship = new Ship.LirouShip(new Vector3(0, 50, 0), new Vector3(0, 0, 0), 1.0f, (float)(Math.PI / 50), 0.002f);
            camera = new Ship.Camera(25, ship.Position);
            #if WINDOWS_PHONE
                        // Frame rate is 30 fps by default for Windows Phone.
                        TargetElapsedTime = TimeSpan.FromTicks(333333);

                        graphics.IsFullScreen = true;
            #endif
        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            terrain = Content.Load<Model>("Terrain/terrain");
            sky = Content.Load<Sky>("sky");
            ship.LoadContent(this.Content);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState(); 
            HandleInput();
            ship.Update(ks, camera);
            camera.Update(Mouse.GetState(), gameTime, ship.Position, ship.Rotation, graphics);
            
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.Black);
            
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1, 10000);
            Matrix view = Matrix.CreateLookAt(camera.Position, ship.Position, Vector3.Up);
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the terrain first, then the sky. This is faster than
            // drawing the sky first, because the depth buffer can skip
            // bothering to draw sky pixels that are covered up by the
            // terrain. This trick works because the code used to draw
            // the sky forces all the sky vertices to be as far away as
            // possible, and turns depth testing on but depth writes off.

            DrawTerrain(view, projection);

            ship.Draw(aspectRatio, view);  

            sky.Draw(view, projection);

            // If there was any alpha blended translucent geometry in
            // the scene, that would be drawn here, after the sky.

            base.Draw(gameTime);
        }


        /// <summary>
        /// Helper for drawing the terrain model.
        /// </summary>
        void DrawTerrain(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in terrain.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    // Set the specular lighting to match the sky color.
                    effect.SpecularColor = new Vector3(0.6f, 0.4f, 0.2f);
                    effect.SpecularPower = 8;

                    // Set the fog to match the distant mountains
                    // that are drawn into the sky texture.
                    effect.FogEnabled = true;
                    effect.FogColor = new Vector3(0.15f);
                    effect.FogStart = 100;
                    effect.FogEnd = 320;
                }

                mesh.Draw();
            }
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Handles input for quitting the game.
        /// </summary>
        private void HandleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for exit.
            if (currentKeyboardState.IsKeyDown(Keys.Escape) ||
                currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (GeneratedGeometryGame game = new GeneratedGeometryGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
