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

    public class BasicModel
    {
        public Model model { get; protected set; }
        public Matrix world = Matrix.Identity;

        public BasicModel(Model m)
        {
            model = m;
        }

        // virtual method "to customize the actions performed during an update" --
        public virtual void Update(GameTime gameTime) { }
        // derp.

        // this is from the Learning XNA 4.0 book, really:
        public virtual void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count]; // probably not relevant to spaceships? unless I add moving parts?
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }
        }

        public virtual Matrix GetWorld()
        {
            return world;
        }
    }

}
