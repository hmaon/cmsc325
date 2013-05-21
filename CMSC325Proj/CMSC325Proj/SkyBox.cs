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
    public class SkyBox : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D cube_map, alt_map;
        Game1 game;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        BasicEffect be;


        public SkyBox(Game game)
            : base(game)
        {
            this.game = (Game1)game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public void altmap()
        {
            be.Texture = alt_map;
        }

        // woot
        protected override void LoadContent()
        {
            cube_map = game.Content.Load<Texture2D>(@"skycubemap-RC3");
            alt_map = game.Content.Load<Texture2D>(@"skycubemap-plus-cat");
            int scale = 100;

            be = new BasicEffect(game.GraphicsDevice);
            be.Texture = cube_map;
            be.TextureEnabled = true;
            be.SpecularPower = 0.0f;
            be.LightingEnabled = false; // I don't know! blarf.

            // the definition of a cube, lifted from http://codingadventures.blogspot.com/2007/07/rendering-cube-in-xna-first-pass.html
            // which probably originally got it from an older version of http://msdn.microsoft.com/en-us/library/bb203926.aspx ... OK.... duh..... 

            // I had to beat into XNA 4.0 shape a bit, though..
            //VertexDeclaration cubeVertexDeclaration = new VertexDeclaration(
            //       game.GraphicsDevice, VertexPositionColorTexture.VertexElements);

            VertexPositionNormalTexture[] cubeVertices = new VertexPositionNormalTexture[24];
            

            Vector3 topLeftFront = new Vector3(-1.0f, 1.0f, 1.0f);     topLeftFront *= scale;
            Vector3 bottomLeftFront = new Vector3(-1.0f, -1.0f, 1.0f); bottomLeftFront *= scale;
            Vector3 topRightFront = new Vector3(1.0f, 1.0f, 1.0f);     topRightFront *= scale;
            Vector3 bottomRightFront = new Vector3(1.0f, -1.0f, 1.0f); bottomRightFront *= scale;
            Vector3 topLeftBack = new Vector3(-1.0f, 1.0f, -1.0f);     topLeftBack *= scale;
            Vector3 topRightBack = new Vector3(1.0f, 1.0f, -1.0f);     topRightBack *= scale;
            Vector3 bottomLeftBack = new Vector3(-1.0f, -1.0f, -1.0f); bottomLeftBack *= scale;
            Vector3 bottomRightBack = new Vector3(1.0f, -1.0f, -1.0f); bottomRightBack *= scale;

            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 topRight = new Vector2(1.0f/3, 0.0f);      // texture coordinates will be for a cubemap, not a regular texture
            Vector2 bottomLeft = new Vector2(0.0f, 1.0f/4);    // the cubemap is 3x4 squares in the stupid layout spat out by
            Vector2 bottomRight = new Vector2(1.0f/3, 1.0f/4); // ATI's CubeMapGen.exe

            Vector2 Ypos = new Vector2(1.0f / 3, 0); // these define the offsets of the different texture squares in the cubemap, see referencemap.png
            Vector2 Yneg = new Vector2(1.0f / 3, 0.5f); // this process seems a little demented, though
            Vector2 Xpos = new Vector2(2.0f / 3, 0.25f);
            Vector2 Xneg = new Vector2(0, 0.25f);
            Vector2 Zpos = new Vector2(1.0f / 3, 0.25f); // which one of these is front and which is back? F.U., MS and your BS right-handed coordinate system.
            Vector2 Zneg = new Vector2(1.0f / 3, 0.75f); // and the horse your rode in on
            // I guess with a stupid-looking field of stars/nebulae, it might not matter if the skybox is ass-backwards, at least

            // update: Skybox is verified to be correct with clown-colored reference texture.

            

            // Front face
            cubeVertices[0] = new VertexPositionNormalTexture(topLeftFront, Vector3.UnitX, topLeft+Zneg); // NOTE: the normals are bogus but it doesn't matter as we're not doing any kind of lighting with the skybox! duh!
            cubeVertices[1] = new VertexPositionNormalTexture(bottomLeftFront, Vector3.UnitX, bottomLeft+Zneg);
            cubeVertices[2] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitX, topRight+Zneg);
            cubeVertices[3] = new VertexPositionNormalTexture(bottomRightFront, Vector3.UnitX, bottomRight+Zneg);

            // Back face
            cubeVertices[4] = new VertexPositionNormalTexture(topLeftBack, Vector3.UnitX, topRight+Zpos);
            cubeVertices[5] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitX, topLeft+Zpos);
            cubeVertices[6] = new VertexPositionNormalTexture(bottomLeftBack, Vector3.UnitX, bottomRight+Zpos);
            cubeVertices[7] = new VertexPositionNormalTexture(bottomRightBack, Vector3.UnitX, bottomLeft+Zpos);

            // Top face
            cubeVertices[8] = new VertexPositionNormalTexture(topLeftFront, Vector3.UnitX, bottomLeft+Ypos);
            cubeVertices[9] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitX, topRight+Ypos);
            cubeVertices[10] = new VertexPositionNormalTexture(topLeftBack, Vector3.UnitX, topLeft+Ypos);
            cubeVertices[11] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitX, bottomRight+Ypos);

            // Bottom face
            cubeVertices[12] = new VertexPositionNormalTexture(bottomLeftFront, Vector3.UnitX, topLeft+Yneg);
            cubeVertices[13] = new VertexPositionNormalTexture(bottomLeftBack, Vector3.UnitX, bottomLeft+Yneg);
            cubeVertices[14] = new VertexPositionNormalTexture(bottomRightBack, Vector3.UnitX, bottomRight+Yneg);
            cubeVertices[15] = new VertexPositionNormalTexture(bottomRightFront, Vector3.UnitX, topRight+Yneg);

            // Left face
            cubeVertices[16] = new VertexPositionNormalTexture(topLeftFront, Vector3.UnitX, topRight+Xneg);
            cubeVertices[17] = new VertexPositionNormalTexture(bottomLeftBack, Vector3.UnitX, bottomLeft+Xneg);
            cubeVertices[18] = new VertexPositionNormalTexture(bottomLeftFront, Vector3.UnitX, bottomRight+Xneg);
            cubeVertices[19] = new VertexPositionNormalTexture(topLeftBack, Vector3.UnitX, topLeft+Xneg);

            // Right face
            cubeVertices[20] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitX, topLeft+Xpos);
            cubeVertices[21] = new VertexPositionNormalTexture(bottomRightFront, Vector3.UnitX, bottomLeft+Xpos);
            cubeVertices[22] = new VertexPositionNormalTexture(bottomRightBack, Vector3.UnitX, bottomRight+Xpos);
            cubeVertices[23] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitX, topRight+Xpos);


#if true
            // these are outward-facing triangles, naturally... we want inward-facing ones, I guess?
            // but does that mess up our texture-stitching?
            // is our texture correctly mapped to begin with??? feck.
            
            // nevermind! handled by resetting rasterizerstate :D
           

            short[] cubeIndices = new short[] {
                             0,  1,  2,  // front face
                             1,  3,  2,
                             4,  5,  6,  // back face
                             6,  5,  7,
                             8,  9, 10,  // top face
                             8, 11,  9,
                             12, 13, 14, // bottom face
                             12, 14, 15,
                             16, 17, 18, // left face
                             19, 17, 16,
                             20, 21, 22, // right face
                             23, 20, 22  };
#else
            short[] cubeIndices = new short[] {
                             0,  2,  1,  // front face
                             1,  2,  3,
                             4,  6,  5,  // back face
                             6,  7,  5,
                             8,  10, 9,  // top face
                             8,  9,  11,
                             12, 14, 13, // bottom face
                             12, 15, 14,
                             16, 18, 17, // left face
                             19, 16, 17,
                             20, 22, 21, // right face
                             23, 22, 20  };
#endif

            // wow, these sure changed with 4.0:
            vertexBuffer = new VertexBuffer(
                 game.GraphicsDevice,
                 cubeVertices[0].GetType(),
                 cubeVertices.Count(),
                 BufferUsage.WriteOnly);

            vertexBuffer.SetData(cubeVertices);

            indexBuffer = new IndexBuffer(game.GraphicsDevice,
                 cubeIndices[0].GetType(),
                 cubeIndices.Count(),
                 BufferUsage.WriteOnly);

            indexBuffer.SetData(cubeIndices);

            // (for the better :D)


            depthThingy.DepthBufferEnable = false; // always draw sky box... also, it's drawn first anyway
            depthThingy.StencilEnable = false;

            rasterThingy.CullMode = CullMode.None; // draw the stupid ugly triangles without culling! woo! no need to rearrange them!
            saneRasterizer.CullMode = CullMode.CullCounterClockwiseFace;

            base.LoadContent();
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

        DepthStencilState depthThingy = new DepthStencilState();
        RasterizerState rasterThingy = new RasterizerState();
        RasterizerState saneRasterizer = new RasterizerState();

        Matrix special_view = new Matrix();

        // woop
        public override void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.DepthStencilState = depthThingy;
            game.GraphicsDevice.RasterizerState = rasterThingy;

            special_view = game.camera.view;
            special_view.Translation = Vector3.Zero; // the camera moves negligibly in relation to very distant objects... until we implement FTL propulsion, that is :/

            be.View = special_view;
            
            be.Projection = game.camera.projection;
            be.World = Matrix.Identity;
            
            game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            game.GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in be.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
            }

            game.CleanUpAfterBatchSprite(); // this resets the depth buffer stuff to the regular mode, woo.
            game.GraphicsDevice.RasterizerState = saneRasterizer; // turn on backface culling again too

            base.Draw(gameTime);
        }
    }
}
