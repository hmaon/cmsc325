// CMSC325 Project
// Greg Velichansky
// UMUC id#: 0031695

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CMSC325Proj
{
    class Bullet : ActorModel
    {
        BasicSpaceshipActor from;

        public static int default_velocity = 1000;

        public Bullet(Model m, BasicSpaceshipActor origin)
            : base(m)
        {
            from = origin;
        }

        public override void TakeHit(Vector3 whence, float future_use)
        {
            // pass.. bullets deliver hits, not take them
        }

        public override void UpdateActions(GameTime gameTime)
        {
            float secFraction = gameTime.ElapsedGameTime.Milliseconds / (float)1000.0;
            Game1 game = Game1.main_instance;

            // these are not smart bullets... they just go straight. :3


            // check whether we've left the player's local area entirely
            if (Vector3.DistanceSquared(world.Translation, game.protagonist.world.Translation) > 3000.0F * 3000.0F)
            {
                active = false;
                return;
            }



            // let's do collision detection against ships here, though
            // that way there should be no confusion about whether bullets move or do this check first...
            Vector3 future = world.Translation + (velocity * secFraction);
            bool removebullet = false;


            // is the player hit?
            if (from != game.protagonist && 
                Game1.TestLineSphereIntersection(world.Translation,
                    future,
                    game.protagonist.world.Translation,
                    game.spaceship.Meshes[0].BoundingSphere.Radius))
            {
                IOConsole.instance.WriteLine("player is hit.");
                game.protagonist.TakeHit(world.Translation, 0);
                removebullet = true;
            }



            // check all the other ships, if any >_<
            // foreach (ActorModel who in modelManager.models) // same problem as above with foreach()
            if (!removebullet) for (int j = 0; j < game.modelManager.models.Count; )
                {
                    BasicSpaceshipActor who = game.modelManager.models[j] as BasicSpaceshipActor;

                    if (from != who &&
                        Game1.TestLineSphereIntersection(world.Translation,
                            future,
                            who.world.Translation,
                            who.model.Meshes[0].BoundingSphere.Radius*1.1f))  // XXX are we supposed to add BoundingSphere.Center to the translation as an offset? feck.
                    {
                        who.TakeHit(world.Translation, 0);
                        if (!who.active) game.modelManager.models.RemoveAt(j); // I guess they asploded?
                        removebullet = true;
                    }

                    ++j;
                }

            // this is getting twisted now :(
            if (removebullet) this.active = false;

        }

        public void Initialize()
        {
            // we can't do this in the constructor because of stupidity
            // base(m) will always set up the rotation matrix after our constructor runs

            world = from.world;

            if (from.model != null) Move(world.Forward * (from.model.Meshes[0].BoundingSphere.Radius + 1)); // put us a bit in front of the ship what fired us
            else Move(world.Forward * (Game1.main_instance.spaceship.Meshes[0].BoundingSphere.Radius + 1)); // the player's ship may not have a model set

            if (from == Game1.main_instance.protagonist) Move(world.Down); // this just looks more satisfying

            velocity = from.velocity + world.Forward * default_velocity; // 1000 m/s muzzle velocity is a bit better than 20th century guns and rifles according to wikipedia *shrug*
        }
    }
}
