// CMSC325 Project
// Greg Velichansky
// UMUC 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CMSC325Proj
{
    public class PlayerShip : BasicSpaceshipActor
    {
        public PlayerShip(Model m) : base(m)
        {
            // but the player ship doesn't really get drawn usually
            // hrmmmm, except when the camera switches to some kind of chase perspective
            // anyway, whatever?

            this.HP = 50; // strong! too strong?
            this.max_acceleration *= 2;
        }

        public override void TakeHit(Vector3 whence, float future_use)
        {
            Game1.main_instance.soundShot.Play(0.5f, 0.0f, 0.0f);

            base.TakeHit(whence, future_use);
        }

        // the player's ship is controlled through some terrible controls :V
        // sucks to be the player
        public override void UpdateActions(GameTime gameTime)
        {
            float secFraction = gameTime.ElapsedGameTime.Milliseconds / 1000F;
            float turnspeed = 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift)) turnspeed = 0.25f;


            // stupid movement keys for testing
            if (Keyboard.GetState().IsKeyDown(Keys.W)) Thrust(Vector3.Forward);
            if (Keyboard.GetState().IsKeyDown(Keys.S)) Thrust(Vector3.Backward);
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) Thrust(Vector3.Left);
            if (Keyboard.GetState().IsKeyDown(Keys.E)) Thrust(Vector3.Right);

            if (Keyboard.GetState().IsKeyDown(Keys.R)) Thrust(Vector3.Up);
            if (Keyboard.GetState().IsKeyDown(Keys.C)) Thrust(Vector3.Down);

            if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left)) Turn(turnspeed, 0, 0);
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right)) Turn(-turnspeed, 0, 0);
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) Turn(0, turnspeed, 0);
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) Turn(0, -turnspeed, 0);


            if (Keyboard.GetState().IsKeyDown(Keys.Space)) fire(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                // automated deceleration of some sort...
                // ULTRA space-brakes!
                // XXX these seem to be way more effective than intended but whatever, the controls are awful enough without making them more difficult

                if (Math.Abs(velocity.X) <= secFraction * max_acceleration) velocity.X = 0.0F;
                else velocity.X -= secFraction * max_acceleration * Math.Sign(velocity.X);

                if (Math.Abs(velocity.Y) <= secFraction * max_acceleration) velocity.Y = 0.0F;
                else velocity.Y -= secFraction * max_acceleration * Math.Sign(velocity.Y);

                if (Math.Abs(velocity.Z) <= secFraction * max_acceleration) velocity.Z = 0.0F;
                else velocity.Z -= secFraction * max_acceleration * Math.Sign(velocity.Z);

                // this is a hack, as it grossly confuses local space with world space but it doesn't actually matter much
                // it'll still slow the ship down and the player probably won't even notice the difference
            }

            // something else set the damn camera, what has access to the stupid-ass structure
            // camera.Move(playerLinearMomentum);

            base.UpdateActions(gameTime); // the base method does the math for translating local-space thrust into inertial-space acceleration or whatevs
        }
    }
}
