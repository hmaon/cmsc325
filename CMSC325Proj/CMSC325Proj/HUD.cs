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
    public class HUD : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game;
        SpriteFont fn;
        float hpwid = 0;
        float gameoverwid = 0;
        Vector2 reticuledim;


        public HUD(Game game, SpriteFont font)
            : base(game)
        {
            // TODO: Construct any child components here

            this.game = game as Game1;
            fn = font; 
        }

        protected override void LoadContent()
        {
            if (fn == null) fn = game.Content.Load<SpriteFont>(@"fonts\FancyFontA");

            hpwid = fn.MeasureString("HP: 99").X;

            gameoverwid = fn.MeasureString("GAME OVER").X;

            reticuledim = fn.MeasureString("[ ]");

            base.LoadContent();
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
            // TODO: Add your update code here

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (game.screen.Peek() != gameScreen.inSpace && game.screen.Peek() != gameScreen.death) return;

            Rectangle area = game.GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 at = new Vector2(area.Center.X - hpwid/2, area.Bottom-60);
            SpriteBatch spriteBatch = new SpriteBatch(game.GraphicsDevice);


            spriteBatch.Begin();

            if (game.screen.Peek() == gameScreen.death)
            {
                spriteBatch.DrawString(fn, "GAME OVER", new Vector2(area.Center.X - gameoverwid/2, area.Center.Y), Color.White);
            }
            else
            {
                Vector2 vcent = new Vector2(area.Center.X, area.Center.Y);
                spriteBatch.DrawString(fn, "[ ]", vcent - (reticuledim / 2.0f), Color.DarkRed);
            }

            spriteBatch.DrawString(fn, String.Format("{0} m/s", game.protagonist.velocity.Length()), at, Color.DarkRed);
            at.Y += fn.LineSpacing + 2;
            spriteBatch.DrawString(fn, String.Format("HP: {0}", game.protagonist.HP), at, Color.DarkRed);
            at.Y += fn.LineSpacing + 2;
            spriteBatch.DrawString(fn, String.Format("XP: {0}", game.score), at, Color.DarkRed);
            at.Y = 10;
            spriteBatch.DrawString(fn, String.Format("{0} UFO", game.modelManager.models.Count()), at, Color.DarkRed);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
