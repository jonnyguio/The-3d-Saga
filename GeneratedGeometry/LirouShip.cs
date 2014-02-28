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
    class LirouShip : CObject
    {
        #region Fields    
        
        float moveSpeed, rotatingSpeed;
        Vector3 velocity;
        bool collidingWall;
        #region Get / Set
        
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
            this.id = "ship";
        }

        #endregion

        #region Methods

        public void Update(KeyboardState kb, Camera camera, List<CObject> cobjects)
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
                if (rotation.Y + rotatingSpeed < Constants.MAXFORWARD)
                {
                    rotation.Y += rotatingSpeed;
                }
                else
                {
                    rotation.Y = Constants.MINBACKWARD;
                }
                //Console.WriteLine("Zuera: " + rotation);
            }

            if (kb.IsKeyDown(Keys.D))
            {
                if (rotation.Y - rotatingSpeed > Constants.MINBACKWARD)
                {
                    rotation.Y -= rotatingSpeed;
                }
                else
                {
                    rotation.Y = Constants.MAXFORWARD;
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
                //Console.WriteLine("Zuera: " + rotation);
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
                //Console.WriteLine("Zuera: " + rotation);
            }

            if (kb.IsKeyDown(Keys.LeftShift))
            {
                position.Z -= (float)(moveSpeed * Math.Cos(rotation.X) * Math.Cos(rotation.Y));
                //position.Y += (float)(moveSpeed * Math.Sin(rotation.X) * Math.Cos(rotation.Z));
                position.X -= (float)(moveSpeed * Math.Sin(rotation.Y));
                //Console.WriteLine("HUE: " + position);
                CollisionWall(cobjects);
            }

            if (kb.IsKeyDown(Keys.K))
            {
                position.Z += (float)(moveSpeed * Math.Cos(rotation.X) * Math.Cos(rotation.Y));
                //position.Y -= (float)(moveSpeed * Math.Sin(rotation.X) * Math.Cos(rotation.Z));
                position.X += (float)(moveSpeed * Math.Sin(rotation.Y));
                //Console.WriteLine("HUE: " + modelPosition);
            }
            #endregion            

            #region Collisions
            foreach (CObject cobject in cobjects)
            {
                if (cobject.ID == "floor")
                {
                    if (isCollidingWalls(cobject))
                    {
                        velocity.Y = 0.0f;
                    }
                    else
                    {
                        velocity.Y += 0.01f;
                    }
                }
                if (isCollidingBB(cobject) && cobject != (CObject)this)
                {
                   // Console.WriteLine("READY TO GO " + cobject.ID);
                }
            }
            #endregion
            position.Y -= velocity.Y;
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

        private void CollisionWall(List<CObject> cobjects)
        {
            List<CObject> walls = new List<CObject>();
            foreach (CObject cobject in cobjects)
            {
                if (cobject.ID.Contains("wall"))
                    walls.Add(cobject);
            }
            for (i = 0; i < 4; i++)
            {
                if (isCollidingWalls(walls[i]))
                {
                    position.X += (float)(moveSpeed * Math.Sin(rotation.Y));
                    if (isCollidingWalls(walls[i]))
                    {
                        position.X -= (float)(moveSpeed * Math.Sin(rotation.Y));
                        position.Z += (float)(moveSpeed * Math.Cos(rotation.X) * Math.Cos(rotation.Y));
                    }
                }
            }
            /*foreach (CObject cobject in cobjects)
            {
                if (cobject.ID.Contains("wall"))
                {
                    if (isCollidingWalls(cobject))
                    {
                        position.Z += (float)(moveSpeed * Math.Cos(rotation.X) * Math.Cos(rotation.Y));
                        if (isCollidingWalls(cobject))
                        {
                            position.Z -= (float)(moveSpeed * Math.Cos(rotation.X) * Math.Cos(rotation.Y));
                            position.X += (float)(moveSpeed * Math.Sin(rotation.Y));
                        }
                    }
                    if (isCollidingWalls(cobject))
                    {
                    }
                }
            }*/
        }
        #endregion
    }
}