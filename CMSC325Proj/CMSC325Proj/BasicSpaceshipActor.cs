// CMSC325 Project
// Greg Velichansky
// UMUC 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CMSC325Proj
{
    public class BasicSpaceshipActor : ActorModel
    {
        protected float max_acceleration = 8.9f * 4; // m/s^2 (this is dumb and arbitrary; different ships should override or someting?) 
        // 4G seems like a fun default, though

        protected float max_angular_acceleration = Approximate.PI / 100.0F;  // θ/s^2 in radians

        protected bool safety = true; // limit max speed for automatic collision avoidance

        public int HP = -1;
        protected float ROF = 5; // rate of fire in bullets / sec
        double last_shot = 0;



        protected Vector3 thrust; // as in current linear thrust, duh
        // this is a very basic model of thrust where the thrust for each axis is recorded wholesale
        // individual thrusters aren't modeled right now
        // that can be implemented later once we implement damage and burnout of individual thruster modules

        protected Vector3 angular_thrust; // only one may be set at a time because physics blows and isn't fun anyway
        // the values are for pitch, yaw, roll, as in rotations about x,y,z; derp


        public BasicSpaceshipActor(Model m) : base(m)
        {
            thrust = new Vector3(0);
            angular_thrust = new Vector3(0);
            if (HP == -1) HP = 2; // weak!
        }

        /// <summary>
        /// called by player control code or by AI to apply some thrust, duh
        /// </summary>
        /// <param name="thrusters"></param>
        public void Thrust(Vector3 thrusters)
        {
            if (thrusters.X != 0) thrust.X = thrusters.X;
            if (thrusters.Y != 0) thrust.Y = thrusters.Y;
            if (thrusters.Z != 0) thrust.Z = thrusters.Z;
        }

        /// <summary>
        /// like above but for angular movement
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        public void Turn(float yaw, float pitch, float roll) // pick one!
        {
            // this is how I expect a videogame to behave, apparently
            if (yaw != 0) angular_thrust.Y = yaw;
            if (pitch != 0) angular_thrust.X = pitch;
            if (roll != 0) angular_thrust.Z = roll; // you wanna do a barrel roll? well TOO BAD. YOU ACTUALLY CAN'T. IT'S NOT IMPLEMENTED IN THE CONTROLS AND YOU DON'T NEED IT. STFU, STAR FOX.
        }


        /// <summary>
        /// this is called by Bullet when a ship is hit once
        /// </summary>
        /// <param name="whence">in case you have different HP pools for different shields and armor plates or whatnot</param>
        /// <param name="future_use">uh, for different types of projectiles in future code?</param>
        public override void TakeHit(Vector3 whence, float future_use)
        {
            // oh noooooes we is deadz?
            IOConsole.instance.WriteLine("ouch!");
            --HP;
            if (HP <= 0)
            {
                active = false; // or something... where's the earth-shattering kaboom?

                if (this != Game1.main_instance.protagonist)
                {
                    // the logic behind putting this code here instead of in NPCShip is murky
                    // in theory, there should be more than one NPCShip class? maybe?
                    // and more than one PlayerShip?
                    // XXX XXX XXX probably needs TLC in the event of any design improvements

                    Game1.main_instance.dispatcher.RunCommand("gotone");
                }
                else
                {
                    Game1.main_instance.dispatcher.RunCommand("death");
                }
            }
        }

        /// <summary>
        /// actually, we don't even bother with collisions. they're kind of infrequent anyway
        /// </summary>
        /// <param name="what_with"></param>
        public override void RegisterCollision(ActorModel what_with)
        {
            // booo wth!
            // where'd you get your pilot's license, a Walgreen's?
        }

        /// <summary>
        /// called from some Update() method to run the AI or control code
        /// </summary>
        /// <param name="gameTime"></param>
        public override void UpdateActions(GameTime gameTime)
        {
            //thrust = Vector3.Forward;
            // set the acceleration based on thrust values and transform it into inertial space (rotate it along with the ship, really)
            acceleration = thrust * max_acceleration /* * (gameTime.ElapsedGameTime.Milliseconds / 1000.0F) */; // whoops, multiplied by secFraction twice >_<
            acceleration = Vector3.TransformNormal(acceleration, GetWorld()); // TransformNormal effectively discards the translation of the matrix in the second parameter -- handy, in this case!

            // bleh, stupid rotation is dumb, learn to simulate it later :(
            rotate(angular_thrust.Y*max_angular_acceleration, angular_thrust.X*max_angular_acceleration, angular_thrust.Z*max_angular_acceleration);

            // nothing else without some AI or player controls, bubs.
        }

        /// <summary>
        /// just reset thrust vectors here; they're filled in anew from UpdateActions() on every Update()!
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            thrust = Vector3.Zero;
            angular_thrust = Vector3.Zero;

            

            base.Update(gameTime);
        }

        /// <summary>
        /// shoot one bullet. at the moment, only one type of bullet is supported and it may look like an '@' sign
        /// </summary>
        /// <param name="gt"></param>
        protected void fire(GameTime gt)
        {
            if ((gt.TotalGameTime.TotalMilliseconds - last_shot) > (1000.0 / ROF))
            {
                last_shot = gt.TotalGameTime.TotalMilliseconds;

                // IOConsole.instance.WriteLine("firing");

                Bullet bb = new Bullet(Game1.main_instance.bullet, this);
                bb.Initialize(); // sad but required

                Game1.main_instance.bulletManager.models.Add(bb);

                if (this == Game1.main_instance.protagonist) Game1.main_instance.soundPewPew.Play(); // play a horrid atari-sounding noise

                // that last method invocation is a mess of dependencies on static class variables, more or less
                // perhaps there's something wrong with the design? OFW.
            }
        }
    }
}
