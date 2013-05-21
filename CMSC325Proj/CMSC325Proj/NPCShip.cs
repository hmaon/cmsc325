// CMSC325 Project
// Greg Velichansky
// UMUC 

// bad AI implemented here :D

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;




namespace CMSC325Proj
{
    class NPCShip : BasicSpaceshipActor
    {
        // hahaha, NPCs
        // they're so dumb.

        // I really don't have time to implement any really good AI at this point :(

        bool cautious = false;

        Vector3 AI_thrust = Vector3.Zero;
        Vector3 AI_turning = Vector3.Zero;
        Vector3 last_distance = new Vector3(9999F);

        
        public static int nav_AI_period = 77; // how many milliseconds to wait between AI updates
        public static int fire_AI_period = 100; // these numbers are pretty arbitrary... you don't want to run this stuff every frame, though, and there's no need

        int update_ticks = 0;

        double last_nav_AI = 0;
        double last_fire_AI = 0;

        float jitter;
        float skillz;

        float separation = 0; // estimated time to target

        // m is as usual
        // time jitter is purely a random number, so that all the AIs don't run at the same time; this is to avoid framerate stutter
        // deadliness is in the range of float.Epsilon..1 and mostly affects how well the AI aims
        public NPCShip(Model m, float timejitter, float deadliness)
            : base(m)
        {
            jitter = timejitter; // the AI isn't jittery; the exact moment at which it runs its code is... sort of... relative to the other NPCs
            skillz = deadliness;

            ROF = 2; // inferior weapons!

            if (skillz == 0) skillz = 0.5f;
            if (skillz > 1) skillz = 1; // you're gonna die, though.
        }

        /// <summary>
        /// uh oh...
        /// glorious invader's ship is made of wet noodle, only has 2 HPs :/
        /// </summary>
        /// <param name="whence"></param>
        /// <param name="future_use"></param>
        public override void TakeHit(Vector3 whence, float future_use)
        {
            cautious = true;

            base.TakeHit(whence, future_use);
        }

        /// <summary>
        /// glorious people's democratic proletariat AI of the space invaders' revolution?
        /// pilots badly but bravely for motherland and mothership
        /// </summary>
        /// <param name="gameTime"></param>
        public override void UpdateActions(GameTime gameTime)
        {
            Vector3 prediction;
            ++update_ticks;

            // if (update_ticks % 30 == 0) jitter = (float)Game1.main_instance.dispatcher.rand.NextDouble() * nav_AI_period; 

            // bool alternate_propulsion = jitter < (nav_AI_period / 2.0f);

            if (last_nav_AI == 0)
            {
                // first update, just set up our jittery execution
                last_fire_AI = gameTime.TotalGameTime.TotalMilliseconds + jitter;
                last_nav_AI = last_fire_AI;
            }

            if (gameTime.TotalGameTime.TotalMilliseconds > last_fire_AI + fire_AI_period)
            {
                last_fire_AI = gameTime.TotalGameTime.TotalMilliseconds;
                // try shooting!
                fire(gameTime);
            }

            if (gameTime.TotalGameTime.TotalMilliseconds > last_nav_AI + nav_AI_period)
            {
                last_nav_AI = gameTime.TotalGameTime.TotalMilliseconds;


                separation = (Game1.main_instance.protagonist.world.Translation - world.Translation).Length() / Bullet.default_velocity; // approximate time for bullet to intercept target, still grows in inaccuracy at higher speeds maybe                
                Vector3 wtf = Game1.main_instance.protagonist.world.Translation - world.Translation;

                if (sec_fraction < 0.4F && velocity.LengthSquared() > 330 * 330 &&
                    (velocity + wtf).LengthSquared() > wtf.LengthSquared()) // headed the wrong way, are we? :(
                    velocity *= (1.0F - 0.850f * sec_fraction); // put the space-brakes on :P
                // the NPC ships are completely hopeless at maneuvering without this cheat
                

                wtf.Normalize(); // direction toward our hated enemy, in world terms...






                // now, compare our own direction vectors to that vector to see what do
                Vector3 what_do = Vector3.Up;
                AI_turning = new Vector3(1, 0, 0);
                float dist = Vector3.DistanceSquared(wtf, world.Up);
                what_do *= 1.33F; // they go a little faster when moving forward? *shrug*

                float d2 = Vector3.DistanceSquared(wtf, world.Down);
                if (d2 < dist) { dist = d2; what_do = Vector3.Down; AI_turning = new Vector3(-1, 0, 0); }

                d2 = Vector3.DistanceSquared(wtf, world.Left);
                if (d2 < dist) { dist = d2; what_do = Vector3.Left; AI_turning = new Vector3(0, 1, 0); }

                d2 = Vector3.DistanceSquared(wtf, world.Right);
                if (d2 < dist) { dist = d2; what_do = Vector3.Right; AI_turning = new Vector3(0, -1, 0); }

                d2 = Vector3.DistanceSquared(wtf, world.Backward);
                if (d2 < dist) { dist = d2; what_do = Vector3.Backward; AI_turning = new Vector3(0, -1, 0); }

                d2 = Vector3.DistanceSquared(wtf, world.Forward);
                if (d2 < dist) { dist = d2; what_do = Vector3.Forward; /* this means nothing to our stupid turning strategy */ }



                AI_thrust = what_do;


                //
                // I know that was like the dumbest thing ever but I'm really tired, OK?
                //

                // Also, it doesn't even work right... we can cheat harder, though

                //if (alternate_propulsion) AI_thrust = wtf;

                if (cautious) wtf *= 1.5f; // light a fire under them, eh?

                AI_thrust = wtf;



                AI_turning /= 3.0F; // they turn slow... because they're so bad at aiming ._.


            }
           
            //if (alternate_propulsion) acceleration = AI_thrust;
            //else Thrust(AI_thrust);

            acceleration = AI_thrust;

            prediction = Game1.main_instance.protagonist.world.Translation + Game1.main_instance.protagonist.velocity * separation * skillz; // without skillz being in here, the AI's aim is actually uncanny and superhuman

            if (!Game1.TestLineSphereIntersection(world.Translation, world.Translation + world.Forward * 6000, prediction, 30f - 27f*skillz))
                angular_thrust = AI_turning;
            else angular_thrust = Vector3.Zero;
            
            

            // if we're cheating hard, don't call the base method, as it overwrites acceleration
            //if (!alternate_propulsion) base.UpdateActions(gameTime);

            // copying the rotation stuff from base, though
            /*else*/ rotate(angular_thrust.Y * max_angular_acceleration, angular_thrust.X * max_angular_acceleration, angular_thrust.Z * max_angular_acceleration);

        }

    }
}
