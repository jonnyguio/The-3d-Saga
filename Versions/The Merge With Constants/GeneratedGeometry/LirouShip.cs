#region Library Imports

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

#endregion

namespace Ship
{
    class LirouShip
    {
        #region Fields

        Vector3 position, rotation;        
        Model model;
        float moveSpeed, rotatingSpeed, scale;
        #region Get / Set
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public Vector3 Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }        
        public Model Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
            set
            {
                moveSpeed = value;
            }
        }

        public float RotatingSpeed
        {
            get
            {
                return rotatingSpeed;
            }
            set
            {
                rotatingSpeed = value;
            }
        }
        #endregion

        #endregion

        #region Constructors

        public LirouShip(Vector3 position, Vector3 rotation, float moveSpeed, float rotatingSpeed, float scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.moveSpeed = moveSpeed;
            this.rotatingSpeed = rotatingSpeed;
            this.scale = scale;
        }

        #endregion

        #region Methods

        public void Update(KeyboardState kb, Camera camera)
        {
            #region Follow Camera
            if (kb.IsKeyDown(Keys.Space))
            {
                if (this.rotation.X * 2 > - (camera.Rotation.Y * Math.PI * 2))
                {
                    this.rotation.X -= rotatingSpeed;
                }

                if (this.rotation.X * 2 < - (camera.Rotation.Y * Math.PI * 2))
                {
                    this.rotation.X += rotatingSpeed;
                }

                //if (this.rotation.Y > camera.Rotation.X)
                //{
                //    this.rotation.Y -= rotatingSpeed;
                //}

                //if (this.rotation.Y < camera.Rotation.X)
                //{
                //    this.rotation.Y += rotatingSpeed;
                //}
            }
            #endregion

            #region InputHandling
            if (kb.IsKeyDown(Keys.A))
            {
                if (rotation.Z + rotatingSpeed < Constants.MAXFORWARD)
                {
                    rotation.Z += rotatingSpeed;
                }
                else
                {
                    rotation.Z = Constants.MINBACKWARD;
                }
                //Console.WriteLine("Zuera: " + rotation);
            }

            if (kb.IsKeyDown(Keys.D))
            {
                if (rotation.Z - rotatingSpeed > Constants.MINBACKWARD)
                {
                    rotation.Z -= rotatingSpeed;
                }
                else
                {
                    rotation.Z = Constants.MAXFORWARD;
                }
                //Console.WriteLine("Zuera: " + rotation);
            }

            if (kb.IsKeyDown(Keys.W))
            {
                if (rotation.X - rotatingSpeed > Constants.MINBACKWARD)
                {
                    rotation.X -= rotatingSpeed;
                }
                else
                {
                    rotation.X = Constants.MAXFORWARD;
                }
                Console.WriteLine("Zuera: " + rotation);
            }

            if (kb.IsKeyDown(Keys.S))
            {
                if (rotation.X + rotatingSpeed < Constants.MAXFORWARD)
                {
                    rotation.X += rotatingSpeed;
                }
                else
                {
                    rotation.X = Constants.MINBACKWARD;
                }
                Console.WriteLine("Zuera: " + rotation);
            }

            if (kb.IsKeyDown(Keys.LeftShift))
            {
                position.Z -= (float)(moveSpeed * Math.Cos(rotation.X) * Math.Cos(rotation.Y));
                position.Y += (float)(moveSpeed * Math.Sin(rotation.X) * Math.Cos(rotation.Z));
                position.X -= (float)(moveSpeed * Math.Sin(rotation.Z) * Math.Sin(rotation.X));
                //Console.WriteLine("HUE: " + position);
            }

            if (kb.IsKeyDown(Keys.K))
            {
                position.Z += (float)(moveSpeed * Math.Cos(rotation.X) * Math.Cos(rotation.Y));
                position.Y -= (float)(moveSpeed * Math.Sin(rotation.X) * Math.Cos(rotation.Z));
                position.X += (float)(moveSpeed * Math.Sin(rotation.Z) * Math.Sin(rotation.X));
                //Console.WriteLine("HUE: " + modelPosition);
            }
            #endregion            
        }

        public void LoadContent(ContentManager Content)
        {
            model = Content.Load<Model>("Models\\p1_wedge");            
        }

        public void Draw(float aspectRatio, Matrix view)
        {
            //Matrix[] transforms = new Matrix[model.Bones.Count];
            //model.CopyAbsoluteBoneTransformsTo(transforms);            

            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    Matrix[] transforms = new Matrix[model.Bones.Count];
                    model.CopyAbsoluteBoneTransformsTo(transforms);

                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] 
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateRotationY(rotation.Y)
                        * Matrix.CreateRotationX(rotation.X)
                        * Matrix.CreateRotationZ(rotation.Z)
                        * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 1000000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        #endregion
    }
}