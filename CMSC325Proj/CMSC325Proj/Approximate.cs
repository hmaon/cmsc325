// CMSC325 Project
// Greg Velichansky
// UMUC id#: 0031695

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMSC325Proj
{
    class Approximate
    {
        public const float PI = (float)Math.PI;


        public static float Sin(float x)
        {
            // this parabolic approximation is cribbed from http://www.devmaster.net/forums/showthread.php?t=5784
            // it's not as good as the other method promises to be but it's still good enough for some cheap eye-candy effects


            const float B = 4/PI;
            const float C = -4/(PI*PI);

            if (x > 0)
            {
                x += PI;
                x = x % (2 * PI);
                x -= PI;
            }
            else
            {
                x -= PI;
                x = x % (2 * PI);
                x += PI;
            }

            float y = B * x + C * x * Math.Abs(x);

#if true
            //  const float Q = 0.775;
                const float P = 0.225F;

                y = P * (y * Math.Abs(y) - y) + y;   // Q * y + P * y * abs(y)
#endif

            return y;
        }



        public static float Sin5(float x)
        {
            // this parabolic approximation is cribbed from http://www.coranac.com/2009/07/sines/
            // It is equation 17 from that page, aka S sub 5
            // It's a fifth-order polynomial with 3 decimal points of precision, which is more than good enough for some cheap special effects in some menus and splash screens.
            // A graph of the actual error of this approximation: http://www.wolframalpha.com/input/?i=0.5*%28%282*x%29%2FPI%29%28PI-%28%282*x%29%2FPI%29^2%28%282*PI-5%29-%28%282*x%29%2FPI%29^2%28PI-3%29%29%29-sine%28x%29+for+x+%3D+0+to+PI%2F2
            // pretty sweet, right?

            // S5(x) = 0.5*z(PI-z^2((2*PI-5)-z^2(PI-3)))
            // where z = x/(0.5 * PI) = 2x/PI

            float z = 2 * x / PI;

            // this function only actually approximates a quarter circle so some junk is needed here to do the correct reflection
            // this junk probably kills this function's performance, especially in C# :(

            if (z > 0)
            {
                z += 2;
                z = z % 4;
                z -= 2;

            }
            else
            {
                z -= 2;
                z = z % 4;
                z += 2;
            }

            if (z > 1) z = 2 - z; // reflection
            else if (z < -1) z = (-2) - z;


            return 0.5F * z*(PI - (z*z) *((2 * PI - 5) - (z*z) *(PI - 3)));
        }




    }
}
