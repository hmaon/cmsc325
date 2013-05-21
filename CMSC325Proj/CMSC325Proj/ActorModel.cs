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

#if false
    // this might be a stupid idea... why keep these three separate types of things in one collection at all?
    // just to save on lines when we iterate over them to render them? that seems dumb. We can have separate loops...
    public enum Realness
    {
        solid = 1, // stuff.
        negligible, // stuff that's real but doesn't participate in regular collision calculations; e.g., dust, bullets
        visual // aka "virtual" but that's a reserved word - basically, stuff that the player sees but isn't real, 
               // like crosshairs or labels or hallucinations
    }
#endif


    public abstract class ActorModel : BasicModel
    {
        // called from the ModelManager's Update() method
        // run AI or check player controls in this method
        abstract public void UpdateActions(GameTime gameTime);

        // uh, hi, we've run into something
        // by default, this should do the calculations for an elastic collision
        // however, none of our spaceships will actually use this default
        // so there's no point in implementing it yet :/
        virtual public void RegisterCollision(ActorModel what_with)
        {
            // when worlds collide! rawr?
            
            // TODO insert physics here
        }

        // we been shot! oh noes
        // whence is the last location of the projectile that hit us in world coordinates
        // (or the origin of a laser beam)
        // this could be used to calculate damage to specific armor plates or modules
        abstract public void TakeHit(Vector3 whence, float future_use);

        // set this to false for any object that's been removed from the world and isn't
        // coming back; that's just in case another object is keeping a reference to it
        // (e.g. a wingman referencing a wing leader or a spacewhale referencing its mate)
        // also useful when an object wants to remove itself from a collection over which
        // modelManager is currently iterating... basically, it can't; that has to be deferred.
        public bool active;

        //
        // ****** end of abstract stuff ******
        //

        // things should have mass. 
        // mass should be expressed in kg.        
        public float mass { get; protected set; }
        // mass probably shouldn't be mutable to external entities? 
        // it's mostly a code design aesthetic

        // things have a velocity!
        // velocity is in m/s
        // (note: one XNA coordinate unit is one meter for our purposes, I guess... fuck it)
        public Vector3 velocity; // { get; protected set; }
        
        // things have a velocity d/dt!
        // aka acceleration
        public Vector3 acceleration { get; protected set; }


        // things really OUGHT to have angular momentum
        // but things can go implement that crap itself if they really want it
        // I'm not doing it at this point
        // or possibly any point ever in my life
        // Perhaps I would rather deliver pizza for a living.


        protected float sec_fraction;


        public ActorModel(Model m) : base(m) 
        {
            //rotation = new Quaternion(Vector3.Forward, 1.0F);
            rotation_matrix = Matrix.Identity;
            active = true;
            mass = 1.0F; 
        }

        //public Quaternion rotation { get; protected set; }
        //concatenating thousands of quaternion multiplications results in insane compounded error, even with constant re-normalization
        //so not bothering with it...

        public Matrix rotation_matrix { get; protected set; } // with no Quaternion being stored, this is totally redundant with "world"... FIXME

        public void Move(Vector3 f)
        {
            this.world.Translation += f;            
        }

        public void MoveTo(Vector3 f)
        {
            this.world.Translation = f;
        }


        /// <summary>
        /// I sort of cooked this up myself... it does a rotation in three steps, with a separate matrix for each axis
        /// maybe it's stupid? :(
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        public void rotate(float yaw, float pitch, float roll)
        {
            Quaternion rot;
            Vector3 trans;

            // NOTE:
            // from Mathematics for 3D Programming and Computer Graphics 2nd ed.
            // "The product of two quaternions q1 and q2 also represents a rotation. Specifically,
            // the product q1q2 represents the rotation resulting from first rotating by q2
            // and then by q1." (p. 90)
            // NOTE: No longer relevant; no quaternion multiplication is currently in use :3

            //rot = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

            //rotation = Quaternion.Multiply(rotation,rot); // multiplication is non-commutative, make sure the order is right
            //rotation.Normalize();

            // of course, the above is all wrong... we need to rotate about an arbitrary axis, woooooooooooooooooooooooooooooooooooooooooooooooo

            if (yaw != 0.0F)
            {
                // rotate about the Y axis or Up vector
#if false
                // OH GOD, this method of continuously multiplying quaternions resulted in massive compounded error after a few minutes
                // it is completely unusable
                rot = Quaternion.CreateFromAxisAngle(rotation_matrix.Up, yaw);
                rotation = Quaternion.Multiply(rot, rotation);
                rotation.Normalize();
                rotation_matrix = Matrix.CreateFromQuaternion(rotation); 
#endif
                rot = Quaternion.CreateFromAxisAngle(rotation_matrix.Up, yaw);
                rotation_matrix = Matrix.Transform(rotation_matrix, rot);                
            }

            if (pitch != 0.0F)
            {
                // rotate about the X axis or... uh... Right vector? this doesn't depend on handedness, that's only Z, right? fuck math in the ear tbh.
                rot = Quaternion.CreateFromAxisAngle(rotation_matrix.Right, pitch);
#if false
                rotation = Quaternion.Multiply(rot, rotation);
                rotation.Normalize();
                rotation_matrix = Matrix.CreateFromQuaternion(rotation);
#endif
                rotation_matrix = Matrix.Transform(rotation_matrix, rot);
            }

            if (roll != 0.0F)
            {
                // rotate about the Z axis or... uh... Forward vector? This DOES depend on handedness, does it not? feck.
                // no, perhaps the handedness only matters for the projection; math in an object's local space doesn't change
                rot = Quaternion.CreateFromAxisAngle(rotation_matrix.Forward, roll);
#if false
                rotation = Quaternion.Multiply(rot, rotation);
                rotation.Normalize();
                rotation_matrix = Matrix.CreateFromQuaternion(rotation);
#endif
                rotation_matrix = Matrix.Transform(rotation_matrix, rot);
            }

            trans = this.world.Translation;
            //this.world = Matrix.CreateFromQuaternion(rotation);
            this.world = rotation_matrix;
            this.world.Translation = trans;
        }


        public override void Update(GameTime gameTime)
        {
            if (!active) return;

            // run AI, then basic physics stuff
            UpdateActions(gameTime);

            sec_fraction = gameTime.ElapsedGameTime.Milliseconds / 1000.0F;

            velocity += acceleration * sec_fraction;

            Move(velocity * sec_fraction);

 	        base.Update(gameTime);
        }
    }
}
