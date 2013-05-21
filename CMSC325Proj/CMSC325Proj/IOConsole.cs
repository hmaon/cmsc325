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




// This class does multiple things, really. 
// It serves as a debug console of sorts for the game to print various instrumentation code output
// It also implements some kind of rudimentary character buffer for keyborad input.
// It may also implement some rudimentary line editing for typing stuff on the console! 

namespace CMSC325Proj
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class IOConsole : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public static IOConsole instance; // I'm lazy.


        //
        // stuff to do with the on-screen output
        //
        const int maxlines = 100; // the intention is to implement a scrollback buffer but that might not be ready yet :)

        int displaylines = 20; // actual height of the console "terminal" in lines of text
        int scrollback = 0; // in partial screens ?

        private LinkedList<string> lines = new LinkedList<string>();
        public bool drawing = false;

        SpriteBatch spriteBatch;
        SpriteFont font;

        Keys consolekey = Keys.F12;

        // There is no Write() method! Who needs it! F- it!


        // format is the format and /ar+gs/ are the sounds of pirates boarding your ship
        // they are basically passed to String.Format and the result is enqueued into lines
        public void WriteLine(string format, object[] arrrrrrrgs)
        {
            string s;

            if (arrrrrrrgs == null)
            {
                s = String.Format("{0:s}", format);
                // my hack to make this stupid method more C-like, because that's what I like
            } else s = String.Format(format, arrrrrrrgs);

            if (lines.Count >= maxlines) lines.RemoveLast();

            lines.AddFirst(s);
            
        }


        public void WriteLine(string format)
        {
            WriteLine(format, (object[])null);
        }


        public void WriteLine(string format, object one_thing)
        {
            object[] arr;

            if (one_thing == null)
            {
                WriteLine(String.Format("{0:s}", format));
            }
            else WriteLine(String.Format(format, one_thing));            
        }


        //
        // stuff to do with the keystroke buffer thing
        //
        const int charbufsize = 16; // good enough for DOS!
        private Queue<char> keybuffer = new Queue<char>(16);
        Keys[] previouskeys = null;
        bool takinginput = false;

        void KeyDown(Keys k)
        {
            char c = (char)k;
            bool shift = Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift);

            switch (k)
            {
                case Keys.OemQuotes: c = shift ? '"' : '\''; break;
                case Keys.OemQuestion: c = shift ? '?' : '/'; break;
                case Keys.OemPipe: c = shift ? '|' : '\\'; break;
                case Keys.OemPeriod: c = shift ? '>' : '.'; break;
                case Keys.OemComma: c = shift ? '<' : ','; break;
                case Keys.OemSemicolon: c = shift ? ':' : ';'; break;
                case Keys.OemMinus: c = shift ? '_' : '-'; break;
                case Keys.OemPlus: c = shift ? '+' : '='; break;
                case (Keys)'0': c = shift ? ')' : '0'; break;
                case (Keys)'1': c = shift ? '!' : '1'; break;
                case (Keys)'2': c = shift ? '@' : '2'; break;
                case (Keys)'3': c = shift ? '#' : '3'; break;
                case (Keys)'4': c = shift ? '$' : '4'; break;
                case (Keys)'5': c = shift ? '%' : '5'; break;
                case (Keys)'6': c = shift ? '^' : '6'; break;
                case (Keys)'7': c = shift ? '&' : '7'; break;
                case (Keys)'8': c = shift ? '*' : '8'; break;
                case (Keys)'9': c = shift ? '(' : '9'; break;
                case Keys.OemTilde: c = shift ? '~' : '`'; break;
                case Keys.Left: return; // at some point, these should move a cursor but it is not this point
                case Keys.Right: return;
                case Keys.Up: return; // don't move the menu and stuff while typing into the console :P
                case Keys.Down: return;
                case Keys.LeftShift: return;
                case Keys.RightShift: return;
                default:

                    if (Char.IsLetter(c) && !shift) c = Char.ToLower(c); // the default is an uppercase letter :P

                    if (!(Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || c == (char)Keys.Back || " \t\n\r".Contains(c))) return; // don't enqueue random nonsense

                    break;
            }

            if (keybuffer.Count > charbufsize) return; // drop keys if our buffer is "full" - it's an artificial limit but whatever. Get your chars out of the queue or we start dropping keys!

            keybuffer.Enqueue(c); // yay!
        }

        void KeyUp(Keys k)
        {
            // this'll be relevant if I implement typematic stuffs
        }

        public bool havechar()
        {
            return keybuffer.Count > 0;
        }


        // getchar()! except it doesn't block, it returns 0 if the buffer is empty
        public char getchar()
        {
            if (keybuffer.Count == 0) return (char)0;

            return keybuffer.Dequeue();
        }



        //
        // console line entry stuff
        //
        const int inputbuffersize = 120;
        char[] inputbuffer = new char[inputbuffersize];
        int cursor = 0;
        int len = 0;
        String inputstring;

        bool allowconsoletoggle = true;



        public IOConsole(Game game)
            : base(game)
        {            
           
            // TODO: Construct any child components here
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            font = Game.Content.Load<SpriteFont>(@"fonts\ConsoleFont");
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


        // stupid helper function
        static bool KeyIn(Keys[] where, Keys what)
        {
            foreach (Keys k in where)
            {
                if (k == what) return true;
            }

            return false;
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // update keyb buffer queue
            Keys[] tempkeys = Keyboard.GetState().GetPressedKeys();
            char c;

            // not sure whether Keyboard is available during Initialize()...
            if (previouskeys != null) 
            {
                foreach (Keys k in tempkeys)
                {
                    if (!(KeyIn(previouskeys, k))) KeyDown(k);
                }

#if false
                // not relevant yet
                foreach (Keys k if previouskeys)
                {
                    if (!(KeyIn(tempkeys, k))) KeyUp(k);
                }
#endif
            }

            // toggle console?
            if (allowconsoletoggle &&
                Keyboard.GetState().IsKeyDown(consolekey) &&
                !KeyIn(previouskeys, consolekey))
            {
                drawing = !drawing; // toggle display of console
                if (drawing)
                {
                    takinginput = true; // turn on key handling for this stuff
                    // don't turn it off again, in case some other component needs it
                    // leaving it on will just waste a tiny bit of CPU

                    WriteLine("Console On.", null);
                }
            };


            if (takinginput) while((c = getchar()) != 0)
            {
                if (c == (char)consolekey) continue; // no tildes, or whatever :P

                if (c == (char)Keys.Enter)
                {
                    // echo the string, K?
                    WriteLine(inputstring, null);

                    ((Game1)Game).dispatcher.RunCommand(inputstring);

                    cursor = 0;
                    len = 0;
                    continue;
                }

                if (c == (char)Keys.Back)
                {
                    if (cursor > 0 && len > 0)
                    {
                        inputbuffer[cursor - 1] = (char)0;
                        if (cursor < len)
                        {
                            // deletes a char in the middle of the input buffer...
                            // not relevant until moving the cursor with arrow keys is implemented :P
                            Array.Copy(inputbuffer, cursor, inputbuffer, cursor - 1, len - cursor);
                        }
                        --cursor;
                        --len;
                        inputstring = new String(inputbuffer, 0, len);
                    }
                    
                    continue;
                }

                if (len < inputbuffersize)
                {
                    if (!font.Characters.Contains(c)) c = '?'; 
                    inputbuffer[cursor] = c;
                    ++cursor;
                    ++len;
                    inputstring = new String(inputbuffer, 0, len);
                }
            }


            // as the *last* thing we do, copy current keys to previous
            previouskeys = new Keys[tempkeys.Count()]; // would be cool to do away with this in optimization (TODO)
            Array.Copy(tempkeys, previouskeys, tempkeys.Length);

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            Vector2 coords = new Vector2(0F);
            int start = (int)(scrollback * displaylines * 0.5);

            if (start < 0) start = 0;

            if (drawing)
            {
                spriteBatch.Begin();

                //spriteBatch.DrawString(font, "TEST", Vector2.Zero, Color.Gray);

                int i = 0;
                coords.Y = (displaylines-1) * font.LineSpacing;

                
                // draw the console text!
                foreach (string line in lines.ToArray())
                {
                    if (i >= start)
                    {
                        spriteBatch.DrawString(font, line, coords, Color.Gray);
                        coords.Y -= font.LineSpacing;
                    }

                    if (++i > displaylines) break;
                }

                // draw the input line below
                if (len > 0)
                {
                    coords.Y = displaylines * font.LineSpacing;
                    spriteBatch.DrawString(font, inputstring, coords, Color.Gray);
                }

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
