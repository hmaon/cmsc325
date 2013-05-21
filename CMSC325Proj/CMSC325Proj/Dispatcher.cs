// CMSC325 Project
// Greg Velichansky
// UMUC 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// 
    /// It's responsible for interpreting text commands and setting up game state.
    /// It could possibly execute series of commands as scripts of sorts? At some point?
    /// </summary>
    public class Dispatcher : Microsoft.Xna.Framework.GameComponent
    {
        private char[] separator = { ' ' }; // consts and arrays are bugging me - grr
        private Game1 game;
        public Random rand;

        public int lastspawn = 0;

        float NPCskill = 0.6f;

        public void look(string[] cmd)
        {
            IOConsole.instance.WriteLine("You are in a maze of twisty little passages, all alike.", null);
        }

        /// <summary>
        ///  this is the underpinning of a rudimentary scripting class
        ///  it would be relevant since you can't get IronPython and the like on the Xbox
        /// </summary>
        /// <param name="what_do"></param>
        public void RunCommand(string what_do)
        {
            string[] cmd;            
            object[] temp = new object[1];

            if (what_do == null || what_do.Length == 0) return;

            cmd = what_do.Split(separator);

            temp[0] = cmd;

            Type Us = this.GetType();
            try
            {
                Us.InvokeMember(cmd[0], BindingFlags.InvokeMethod, null, this, temp);
            }
            catch (MissingMethodException e)
            {
                IOConsole.instance.WriteLine("Error, dood: {0}", e.Message);
            }
        }

        public void hello(string[] cmd)
        {
            IOConsole.instance.WriteLine("Hi there!", null);
        }

        public void nop(string[] dummy) { }
        public void noop(string[] dummy) { }

        public void fnop(string[] dummy)
        {
            // the cutest 80387 floating point instruction! ~_~
        }



        public void pop(string[] cmd)
        {
            game.screen.Pop();
            if (game.screen.Count == 0) game.screen.Push(gameScreen.gameMenu);
            //temp[0] = ;
            IOConsole.instance.WriteLine("Top screen now: {0:s}", game.screen.Peek().ToString());
        }

        public void exit(string[] cmd)
        {
            game.Exit();
        }


        public void push(string[] cmd)
        {
            if (cmd.Length < 2)
            {
                IOConsole.instance.WriteLine("push what?", null);
                return;
            }
                    
            // C# has really awesome reflection features!!! :D
            gameScreen scr;
            if (!Enum.TryParse(cmd[1], true, out scr)) IOConsole.instance.WriteLine("Wtf is this: " + cmd[1]);
            else
            {
                if (game.screen.Peek() == scr) return; // this can happen if you push Esc while in the main menu... unless I've bothered to fix that already. anyway, duplicates are stupid on this stack; don't allow them.
                game.screen.Push(scr);
            }
        }

        /// <summary>
        /// Spawn an NPC to attack the player! The location is randomized!
        /// </summary>
        /// <param name="cmd"></param>
        public void spawn(string[] cmd)
        {
            Model which = game.invader;

            // if (rand.NextDouble() > 0.5) which = game.invader;
            // else which = game.spaceship;

            game.modelManager.models.Insert(0, new NPCShip(which, (float)rand.NextDouble()*NPCShip.nav_AI_period, NPCskill));
            game.modelManager.models[0].MoveTo(game.protagonist.world.Translation + new Vector3((float)rand.NextDouble() * 1500 - 750, (float)rand.NextDouble() * 1500 - 750, (float)rand.NextDouble() * 1000 - 1200));
        }

        /// <summary>
        /// This spawns *multiple* enemies!
        /// </summary>
        /// <param name="cmd">cmd[0] shall be the number to spawn</param>
        public void spawns(string[] cmd)
        {
            int count = int.Parse(cmd[1]);

            if (count > 100) count = 100; // honestly, there are limits...

            lastspawn = count;

            IOConsole.instance.WriteLine("Spawning {0}", count);

            for (int i = game.modelManager.models.Count; i < count; ++i) spawn(cmd);
        }


        /// <summary>
        /// Spawn new NPCs. Each successive wave spawns with one more NPC than in the last wave.
        /// </summary>
        /// <param name="cmd"></param>
        public void spawn_moar(string[] cmd)
        {
            NPCskill += 0.0025f; // make it a tiny bit more interesting?
            RunCommand("spawns " + (lastspawn + 2).ToString()); // RAWR >_<
        }

        /// <summary>
        /// get rid of old NPCs!
        /// </summary>
        /// <param name="cmd"></param>
        public void despawn(string[] cmd)
        {
            game.modelManager.models.Clear();
        }

        public void random_cheer(string[] cmd)
        {
            if (rand.NextDouble() < 0.25) game.soundYay.Play();
            else game.soundYeah.Play();
        }


        /// <summary>
        /// reset parameters and put us into space
        /// </summary>
        /// <param name="cmd"></param>
        public void newGame(string[] cmd)
        {
            // hmm... do stuff?

            game.protagonist = new PlayerShip(null); // the player's ship isn't actually *drawn* right now so model is null
            game.protagonist.MoveTo(Vector3.Zero);
            // TODO: have different types of ships to fly, duh!!!

            //game.modelManager.models.Insert(0, new BasicSpaceshipActor(game.invader));
            //game.modelManager.models[0].MoveTo(new Vector3(0, 0, -200));

            if (game.screen.Peek() == gameScreen.death) RunCommand("pop"); // I don't know... the stack management perhaps needs to be rethought?

            NPCskill = 0.66f;

            RunCommand("despawn");

            RunCommand("push inSpace");
            RunCommand("spawns 3"); // start with 3 little invader dudes
        }


        public void gotone(string[] cmd)
        {
            // relocated this out of BasicSpaceship class, where it really didn't feel at home
            random_cheer(null);
            game.score++;
            game.protagonist.HP += 2;
        }


        public void death(string[] cmd)
        {
            MediaPlayer.Play(game.gameover);
            RunCommand("pop");
            RunCommand("push death");
        }


        public void tunenpcs(string[] cmd)
        {
            if (cmd.Count() > 1)
            {
                try {
                    NPCskill = float.Parse(cmd[1]);
                } catch(Exception e)
                {
                    IOConsole.instance.WriteLine("dood, wtf: {0}", e);
                }
            }

            IOConsole.instance.WriteLine("NPC deadliness is tuned to: {0}", NPCskill);
        }


        // for hilarity!
        public void lolcat(string[] cmd)
        {
            game.stars.altmap();
            //MediaPlayer.Play(game.btl); // bonus hilarity
        }



        public Dispatcher(Game game)
            : base(game)
        {
            this.game = (Game1)game; // I guess this might not be conventional style?
            // but game.whatever seems easier than typing ((Game1)Game).whatever all the damn time

            rand = new Random();

            // TODO: Construct any child components here

            // pfft, as if.
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            // I don't wanna!

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            // STOP TELLING ME TO DO THINGS!!! Add your own stupid update code, stupid thing

            base.Update(gameTime); // oh, I guess you did.. OK, fine then...
        }
    }
}
