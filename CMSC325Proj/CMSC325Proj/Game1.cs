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
    public enum gameScreen
    {
        gameMenu = 1, // main menu, e.g., New, Load, Save, Quit
        gameIntro,    // OMG, welcome to this awesome game!
        missionIntro, // mission briefing, I guess
        debriefing,   // stats and scores and such on mission completion
        storyScreen,  // a bunch of text telling you story stuff, most likely
        inSpace,      // you're flying and fighting! (and debugging! :/ )
        shipOptions,  // ship commands what don't fit on the controller; also the pause screen?
        death,        // oh snap, you died and the story didn't call for you to eject safely? sucker.
        credits,      // let's scroll my name a whole bunch, woo
        miniGame,     // terribly dumb minigame that can be played on the ship's computer?
        shipUpgrades, // you're a docked freelancer and you're upgrading your ship?
        trade,        // you're a docked freelancer and you're buying and selling cargo? 
        mysterymeat   // invalid value, just in case it's needed for some kind of debugging
    }



    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public PlayerShip protagonist = null; // initialized in Dispatcher when a game is started, for now


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;

        public Camera camera { get; protected set; }

        public ModelManager modelManager;
        public ModelManager bulletManager;

        MainMenu gameMenu;

        HUD hud;

        public SkyBox stars; // the high-res starfield doesn't look so bad; previous disparaging comment removed :3

        public IOConsole debugcons;

        public Stack<gameScreen> screen;

        public Dispatcher dispatcher;

        bool just_escaped = false;

        public Model spaceship;
        public Model invader;
        public Model bullet;

        public SoundEffect soundKlaxon;
        public SoundEffectInstance soundEngineInstance; // is this necessary anymore?
        public SoundEffect soundPewPew;

        public SoundEffect soundYeah, soundYay, soundShot;

        //public Song btl;
        public Song gameover;

        double last_npc_check = 0;

        public int score = 0;




        // blar, a global variable to store the instance of Game1 for all the models to poke through
        // design? what design?
        public static Game1 main_instance; 




        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            screen = new Stack<gameScreen>(42); // new state stack with more than enough space for all game screens           

            Content.RootDirectory = "Content";

            main_instance = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            stars = new SkyBox(this);
            Components.Add(stars);

            camera = new Camera(this, new Vector3(0, 0, 50), Vector3.Zero, Vector3.Up);           
            //Components.Add(camera); // actually -- why the heck is the camera its own game component? Whatever... it doesn't need to be for my purposes.

            // we want the bullets to move before the models run their Update() calls
            // that's to avoid having the bullets taking one move before collision detection is ever run, which spoils point-blank shots :V
            bulletManager = new ModelManager(this);
            Components.Add(bulletManager);

            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            gameMenu = new MainMenu(this);
            Components.Add(gameMenu);

            screen.Push(gameScreen.gameMenu); // start at the main menu?

            debugcons = new IOConsole(this);
            Components.Add(debugcons);
            IOConsole.instance = debugcons;


            dispatcher = new Dispatcher(this);
            Components.Add(dispatcher); // for the sake of completeness... not sure if it'll use any of the actual game component interface, actually

            hud = new HUD(this, font);
            Components.Add(hud);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            font = Content.Load<SpriteFont>(@"fonts\FancyFontA");

            invader = Content.Load<Model>(@"models\ClassicGreen");

            IOConsole.instance.WriteLine(invader.Meshes[0].BoundingSphere.Radius.ToString());

            spaceship = Content.Load<Model>(@"models\spaceship"); // stupid thing points in the wrong direction :(
            

            bullet = Content.Load<Model>(@"models\AtSign");



            soundPewPew = Content.Load<SoundEffect>(@"Chip102");
            soundKlaxon = Content.Load<SoundEffect>(@"klaxon1");
            soundYeah = Content.Load<SoundEffect>(@"YEAH");
            soundYay = Content.Load<SoundEffect>(@"timtube_yay");

            soundShot = Content.Load<SoundEffect>(@"mixedhit");

            

            //btl = Content.Load<Song>(@"Music\03 - Breaking The Law");
            gameover = Content.Load<Song>(@"awwboobooo");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        //Vector3 playerLinearMomentum = new Vector3(0,0,0);

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float secFraction = gameTime.ElapsedGameTime.Milliseconds / (float)1000.0;

            
            // Allows the game to exit... but actually, don't discard our whole game or anything, huh? 
            // push a new main menu enum onto the stack
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (!just_escaped) dispatcher.RunCommand("push gameMenu"); // dispatcher.RunCommand("pop");

                just_escaped = true;
            }
            else just_escaped = false;


            // erase bullets that ought to be erased now...
            for (int i = 0; i < bulletManager.models.Count; )
            {
                if (!bulletManager.models[i].active) bulletManager.models.RemoveAt(i);
                else ++i;
            }


            if (screen.Peek() == gameScreen.inSpace && protagonist != null)
            {
                // see what's happening with the player's ship and update the camera to that POV
                protagonist.Update(gameTime);

                camera.UseThisPOV(protagonist.GetWorld());

#if true
                // check for cowardly bourgeois deserter traitor ships who have apparently fled the field
                for (int i = 0; i < modelManager.models.Count; )
                {
                    if (Vector3.DistanceSquared(protagonist.world.Translation, modelManager.models[i].world.Translation) > 6500 * 6500)
                    {
                        // attention, space invader!
                        // report to victorious people's glorious uranium mines for job reassignment!
                        modelManager.models.RemoveAt(i);
                        // your sacrifice will not be in vain, comrade invader
                    }
                    else ++i;
                }
#endif


                // perhaps spawn more NPCs?
                if (gameTime.TotalGameTime.TotalSeconds > last_npc_check + 20)
                {
                    last_npc_check = gameTime.TotalGameTime.TotalSeconds;

                    dispatcher.RunCommand("spawn_moar");
                    soundKlaxon.Play();
                }
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            CleanUpAfterBatchSprite();

            GraphicsDevice.Clear(Color.Black);



            base.Draw(gameTime);

#if false
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "TEST", Vector2.Zero, Color.Gray);
            spriteBatch.DrawString(font, playerLinearMomentum.ToString(), new Vector2(0, 15), Color.Gray);

            spriteBatch.End();
#endif
        }

        public void CleanUpAfterBatchSprite()
        {            
            //SpriteBatch messes up the rendering settings on our device, fix it fix it fix it!
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            DepthStencilState depthBufferState;

            depthBufferState = new DepthStencilState(); // this seems suboptimal, optimize later D:
            depthBufferState.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = depthBufferState;
        }


        // p1 and p2 define a line
        // sc is the center of a sphere and r is its radius
        // returns true if the two intersect
        public static bool TestLineSphereIntersection(Vector3 p1, Vector3 p2, Vector3 sc, float r)
        {
            // some code adapted from http://paulbourke.net/geometry/sphereline/
            
            // this is just the part at the bottom, since we don't actually care about the precise
            // intersection with the bounding sphere
                        
            float x1 = p1.X, x2 = p2.X, x3 = sc.X;
            float y1 = p1.Y, y2 = p2.Y, y3 = sc.Y;
            float z1 = p1.Z, z2 = p2.Z, z3 = sc.Z;

            float u =
            ((x3 - x1) * (x2 - x1) + (y3 - y1) * (y2 - y1) + (z3 - z1) * (z2 - z1)) /
            ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));

            if (u < 0 || u > 1) return false;

            Vector3 p = (p2 - p1) * u + p1; // the point on the line closest to the center of the bounding sphere

            if (Vector3.DistanceSquared(p, sc) > r * r) return false; // check whether that point is actually within the damn sphere

            return true;


            // NOTE: This could also be done by making a BoundingBox for the projectile's path and using BoundingSphere.Intersect()
            // but this might be more efficient, plus it looks like I actually implemented some math instead of relying entirely on
            // XNA's awesome built-in stuff.
        }

    }
}
