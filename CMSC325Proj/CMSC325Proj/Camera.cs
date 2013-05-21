// CMSC325 Project
// Greg Velichansky
// UMUC 

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


namespace CMSC325Proj
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera //: Microsoft.Xna.Framework.GameComponent
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        public Vector3 position { get; protected set; } 
        Vector3 direction;
        Vector3 up; // Up isn't just a vector - it was a also a great movie!
        Game Game;


        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(position, position + direction, up);
        }


        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 upparam)
            //: base(game)
        {
            // Initialize view matrix
            //view = Matrix.CreateLookAt(pos, target, upparam);

            this.Game = game;

            position = pos;
            direction = target - pos;
            up = upparam;

            direction.Normalize();

            CreateLookAt();

            // Initialize projection matrix
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                3, (float)10000);
        }

#if false
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
#endif

        public void Move(Vector3 dir)
        {
            position += dir;

            CreateLookAt();
        }


        public void PointAt(Vector3 target)
        {
            direction = (target - position);
            direction.Normalize();

            CreateLookAt();
        }


        public void PointAt(Vector3 target, Vector3 new_up)
        {
            up = new_up;
            direction = target - position;
            direction.Normalize();

            CreateLookAt();
        }


        public void UseThisPOV(Matrix new_world)
        {
            position = new_world.Translation;

            PointAt(position + new_world.Forward, new_world.Up);

            CreateLookAt();
        }
    }
}
