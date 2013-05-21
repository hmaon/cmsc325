// CMSC325 Project
// Greg Velichansky
// UMUC id#: 0031695

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
    public class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public ModelManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (((Game1)Game).screen.Peek() != gameScreen.inSpace) return;

            //if (models.Count == 0) models.Add(new SpaceJunkActor(spaceship, Vector3.Zero, Vector3.UnitZ, Vector3.Up));

            // TODO: Add your update code here
            foreach (ActorModel m in models)
            {
                m.Update(gameTime);
            }


            base.Update(gameTime);
        }


        protected override void LoadContent()
        {
            //models.Add(new ActorModel(Game.Content.Load<Model>(@"models\spaceship"))); // load spaceship.x
            //spaceship = Game.Content.Load<Model>(@"models\Invader");            

            //models.Add(new SpaceJunkActor(spaceship, Vector3.Zero, Vector3.Forward, Vector3.Up));
            //models.Add(new SpaceJunkActor(spaceship, new Vector3(10,8,-20), Vector3.Forward, Vector3.Up));

            //models[0].velocity.Z = -1;

            base.LoadContent();
        }


        public override void Draw(GameTime gameTime)
        {
            if (((Game1)Game).screen.Peek() != gameScreen.inSpace) return;

            foreach (ActorModel m in models)
            {
                m.Draw(((Game1)Game).camera);
            }

            base.Draw(gameTime);
        }


        //public Model spaceship;
        public List<ActorModel> models = new List<ActorModel>();
    }
}
