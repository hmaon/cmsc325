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
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteFont font;
        SpriteBatch spriteBatch;
        Game1 game;
        Camera OurCamera;
        SoundEffect blip = null;

        Vector2 linedim; // dimensions of the longest line of text on the menu

        Vector3 boxpos = new Vector3(0, 0, 30);

        ActorModel boxy;

        Vector2 testcoords;
        Vector3 testcoords3;
        Vector3 testcstart;

        int selection = 0;

        int sinceLastKey = 0; // time in ms since last menu selection move

        // fade out previous selections for a bit of fun
        int prev = 0;
        
        // animate the menu a bit for a bit more fun
        float animPoint = 0.0F; // 0 to 1; never equal to 1 but 0 and 1 are equivalent anyway
        float animPeriod = 5.0F; // seconds


        // config values
        static int ySpacing = 0;
        static Color normalCol = Color.LimeGreen; // food-themed color scheme?
        static Color selected = Color.LemonChiffon;
        static int maxEntries = 6; // whatever?
        static int minDelay = 200; // time in milliseconds that must pass before the menu will change selections again if the player is holding a button down

        Color[] colors;
        Color[] targetColors;
        TimeSpan[] colorSetAt;
        Dictionary<String,String> entries;
        



        public MainMenu(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            this.game = (Game1)game; // frightening up-cast

        }





        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            normalCol.A = 128;

             OurCamera = new Camera(game, new Vector3(0, 0, 50), Vector3.Zero, Vector3.Up);           ;

            colors = new Color[maxEntries];
            targetColors = new Color[maxEntries];
            colorSetAt = new TimeSpan[maxEntries];
            for (int i = 0; i < maxEntries; ++i)
            {
                targetColors[i] = (colors[i] = normalCol);
                // colorSetAt[i] = new GameTime(); // not necessary; colorSetAt[i] isn't relevant until targetColors[i]!=colors[i] and will be set at the same time as targetColors[i]                 
            }

            entries = new Dictionary<string, string>(maxEntries);            
            entries.Add("New Game", "newGame");
            entries.Add("Quit Game", "exit");
            entries.Add("*Space Shooter*", "noop");
            entries.Add("by", "noop");
            entries.Add("Greg Velichansky", "noop");

            moveSelection(0, null); // draw something as selected to start with, eh?

            base.Initialize();
        }


        void moveSelection(int offset, GameTime time)
        {
            if (offset != 0)
            {

                if (sinceLastKey < minDelay) return;

                sinceLastKey = 0; // OK, enough time has passed. We can move again. Derp.
            }

            if (blip != null) blip.Play();

            prev = selection;
            targetColors[prev] = normalCol;

            selection = (selection + offset) % entries.Count;
            if (selection == -1) selection = entries.Count - 1;
            targetColors[selection] = selected;

            if (time!=null) colorSetAt[prev] = colorSetAt[selection] = time.TotalGameTime;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState keys = Keyboard.GetState();
            GamePadState pad1 = GamePad.GetState(PlayerIndex.One);

            if (game.screen.Peek() != gameScreen.gameMenu) return;

            sinceLastKey += gameTime.ElapsedGameTime.Milliseconds;

            //game.camera.PointAt(boxy.world.Translation);

            // add a resume option that would get rid of stupid menu
            if (game.screen.Count > 1 && !entries.ContainsKey("Resume")) entries.Add("Resume", "pop");
            if (game.screen.Count == 1 && entries.ContainsKey("Resume")) entries.Remove("Resume");
            
            // TODO: whatever key got you to this menu from the game should probably get you back again
            // but it's not a high priority


            for (int i = 0; i < entries.Count;  ++i)
            {
                if (colors[i] != targetColors[i]) colors[i] = Color.Lerp(targetColors[i]==normalCol?selected:normalCol, targetColors[i], (float)(gameTime.TotalGameTime - colorSetAt[i]).TotalMilliseconds/minDelay);
            }

            // TODO: STOP TELLING ME TO DO THINGS, COMMENTS! FUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU

            


            animPoint = (float)(gameTime.TotalGameTime.TotalSeconds % animPeriod) / animPeriod;

            // move our pointless background effects around
            boxpos.X = (float)Math.Sin(animPoint * 2 * Approximate.PI);
            boxy.MoveTo(boxpos);

            //boxy.MoveTo(new Vector3(0, 0, 46));
            if ((0 - (float)Math.Cos(animPoint * 2 * Approximate.PI)) > 0) // -cos is the derivative of sin, tells us which direction the thing is moving
            {
                boxy.rotate(0.01F, 0.00F, 0.00F);
                //IOConsole.instance.WriteLine(boxy.rotation_matrix.Right.Y.ToString());
            }
            else
            {
                boxy.rotate(0.00F, 0.00F, 0.1F);
            }

            testcoords3 = game.GraphicsDevice.Viewport.Project(testcstart, OurCamera.projection, OurCamera.view, boxy.GetWorld());
            //testcoords = new Vector2(boxy.model.Meshes[0].BoundingSphere.Radius, 0);
            

            // handle key input, unless the console is up
            if (IOConsole.instance.drawing == true) return;
            
            if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S) || pad1.DPad.Down == ButtonState.Pressed)
            {
                moveSelection(+1, gameTime);
            }
            else if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W) || pad1.DPad.Up == ButtonState.Pressed)
            {
                moveSelection(-1, gameTime);
            }
            else if (keys.IsKeyDown(Keys.Enter) || pad1.Buttons.A == ButtonState.Pressed)
            {
                try
                {
                    game.dispatcher.RunCommand(entries[entries.Keys.ElementAt(selection)]);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    IOConsole.instance.WriteLine("Bogosity: " + e.ToString());
                }
            }


            base.Update(gameTime);
        }


        protected override void LoadContent()
        {
            Model mmm = Game.Content.Load<Model>(@"models\TESTtext");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            testcstart = new Vector3(mmm.Meshes[0].BoundingSphere.Radius, 0, 0);
                        
            font = Game.Content.Load<SpriteFont>(@"fonts\MainMenuFont");
            font.LineSpacing += ySpacing; // some fonts are stupid and don't actually leave any vertical space between lines
            linedim = font.MeasureString("Quit Game :( XXX"); // align left a bit by adding some nonsense characters to the reference string

            boxy = new SpaceJunkActor(mmm,Vector3.Zero, -Vector3.UnitZ, Vector3.Up);
            IOConsole.instance.WriteLine(boxy.model.Meshes[0].BoundingSphere.Radius.ToString(), null);

            blip = Game.Content.Load<SoundEffect>(@"sf3_sfx_menu_select");
            //blip.Play();
        }


        public override void Draw(GameTime gameTime)
        {
            Rectangle area = game.GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 at = new Vector2(area.Center.X - linedim.X/1 , area.Center.Y - (linedim.Y * 5) / 2);
            int i;

            if (game.screen.Peek() != gameScreen.gameMenu) return;


            if (at.X < area.Left+32) at.X = area.Left+32 ; // derp

            


            at.Y += font.LineSpacing * (float)Approximate.Sin(animPoint * 2 * Approximate.PI);
            
            

            this.game.CleanUpAfterBatchSprite();


            // draw random background crap            
            boxy.Draw(OurCamera);

            // engage 2D mode
            spriteBatch.Begin();

            testcoords.X = testcoords3.X;
            testcoords.Y = testcoords3.Y;
            spriteBatch.DrawString(font, "yay", testcoords, Color.AliceBlue);


            // draw the menu!
            
            i = 0;
            foreach (string s in entries.Keys)
            {
                spriteBatch.DrawString(font, s, at, colors[i]);
                at.Y += font.LineSpacing + ((font.LineSpacing / 5) * (float)Approximate.Sin((animPoint + i / 7.0F) * 2F * Approximate.PI + Approximate.PI / 2F));
                ++i;
            }
            
#if false
            spriteBatch.DrawString(font, "TEST!", at, selection == 0? selected : normalCol);
            at.Y += linedim.Y + ySpacing;
            spriteBatch.DrawString(font, "HI!", at, selection == 1? selected : normalCol);
            at.Y += linedim.Y + ySpacing;
            spriteBatch.DrawString(font, "SUP!", at, selection == 2? selected : normalCol);
#endif

            spriteBatch.End();
        }
    }
}
