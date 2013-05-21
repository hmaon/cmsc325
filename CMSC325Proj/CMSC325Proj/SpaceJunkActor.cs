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
    class SpaceJunkActor : ActorModel
    {
        public SpaceJunkActor(Model m, Vector3 position, Vector3 lookat, Vector3 up) : base(m)
        {
            this.MoveTo(position);
            rotation_matrix = Matrix.CreateLookAt(position, lookat, up);
            //rotation = Quaternion.CreateFromRotationMatrix(rotation_matrix);
        }

        public override void UpdateActions(GameTime gameTime)
        {
            // space junk doesn't do anything, really
        }


        public override void TakeHit(Vector3 whence, float future_use)
        {
            // do nothing, again
        }
    }
}
