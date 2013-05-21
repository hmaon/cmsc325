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

        
        public static int nav_AI_period = 200; // how many milliseconds to wait between AI updates
        public static int fire_AI_period = 100; // these numbers are pretty arbitrary... you don't want to run this stuff every frame, though, and there's no need

        int update_ticks = 0;

        double last_nav_AI = 0;
        double last_fire_AI = 0;

        float jitter;
        float skillz;

        // m is as usual
        // time jitter is purely a random number, so that all the AIs don't run at the same time; this is to avoid framerate stutter
        // deadliness is in the range of float.Epsilon..1 and mostly affects how well the AI aims
        public NPCShip(Model m, float timejitter, float deadliness)
            : base(m)
        {
            jitter = timejitter; // the AI isn't jittery; the exact moment at which it runs its code is... sort of... relative to the other NPCs
            skillz = deadliness;

            ROF = 4; // inferior weapons!

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
            ++update_ticks;

            if (update_ticks % 30 == 0) jitter = (float)Game1.main_instance.dispatcher.rand.NextDouble() * nav_AI_period; 

            bool alternate_propulsion = jitter < (nav_AI_period / 2.0f);
            

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

                if (sec_fraction < 0.4F && (velocity.LengthSquared() > 388*388) &&
                    ((velocity+acceleration).LengthSquared() < velocity.LengthSquared())) 
                    velocity *= (1.0F - 0.83f*sec_fraction); // put the space-brakes on :P

                float separation = (Game1.main_instance.protagonist.world.Translation - world.Translation).LengthSquared() / this.velocity.LengthSquared(); // approximate time to close with target, still grows in inaccuracy at higher speeds
                separation = (float)Math.Sqrt(separation);
                //float separation = 1.0f;
                Vector3 prediction = Game1.main_instance.protagonist.world.Translation + Game1.main_instance.protagonist.velocity * (separation * jitter/nav_AI_period) * skillz;
                Vector3 wtf = prediction - world.Translation;
                
                
                
                wtf.Normalize(); // direction toward our hated enemy, in world terms...

                // now, compare our own direction vectors to that vector to see what do
                Vector3 what_do = Vector3.Up;
                AI_turning = new Vector3(1, 0, 0);
                float dist = Vector3.DistanceSquared(wtf, world.Up);
                what_do *= 1.33F; // they go a little faster when moving forward? *shrug*

                float d2 = Vector3.DistanceSquared(wtf, world.Down);
                if (d2 < dist) { dist = d2; what_do = Vector3.Down; AI_turning = new Vector3(-1, 0, 0); }

                d2 = Vector3.DistanceSquared(wtf, world.Left);
                if (cautious) d2 /= 2; // overvalue strafing left and right when cautious
                if (d2 < dist) { dist = d2; what_do = Vector3.Left; AI_turning = new Vector3(0, 1, 0); }

                d2 = Vector3.DistanceSquared(wtf, world.Right);
                if (cautious) d2 /= 2;
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

                if (alternate_propulsion) AI_thrust = wtf;


                AI_turning /= 3.0F; // they turn slow... because they're so bad at aiming ._.
                                

            }
           
            if (alternate_propulsion) acceleration = AI_thrust;
            else Thrust(AI_thrust);
            

            if (!Game1.TestLineSphereIntersection(world.Translation, world.Translation + world.Forward * 3000, Game1.main_instance.protagonist.world.Translation, 30f - 29.5f*skillz))
                angular_thrust = AI_turning;
            else angular_thrust = Vector3.Zero;
            
            

            // if we're cheating hard, don't call the base method, as it overwrites acceleration
            if (!alternate_propulsion) base.UpdateActions(gameTime);

            // copying the rotation stuff from base, though
            else rotate(angular_thrust.Y * max_angular_acceleration, angular_thrust.X * max_angular_acceleration, angular_thrust.Z * max_angular_acceleration);

        }

    }
}
