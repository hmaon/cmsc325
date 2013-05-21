// CMSC325 Project
// Greg Velichansky
// UMUC id#: 0031695

using System;

namespace CMSC325Proj
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

