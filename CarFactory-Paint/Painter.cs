using System;
using System.Collections.Generic;
using CarFactory_Domain;
using CarFactory_Factory;

namespace CarFactory_Paint
{
    public class Painter : IPainter
    {
        public Car PaintCar(Car car, PaintJob paint)
        {
            if (car.Chassis == null) throw new Exception("Cannot paint a car without chassis");

            /*
             * Mix the paint
             * 
             * Unfortunately the paint mixing instructions needs to be unlocked
             * And we don't know the password!
             * 
             * Please only touch the "FindPaintPassword" function
             * 
             */
            var (passwordLength, encodedPassword) = paint.CreateInstructions();
            var solution = FindPaintPassword(passwordLength, encodedPassword);

            if (!paint.TryUnlockInstructions(solution))
            {
                throw new Exception("Could not unlock paint instructions");
            }
            car.PaintJob = paint;
            return car;
        }

        Dictionary<long, string> knowSolutions;

        object lockObj = new();

        // Not static since secret will be the same for the Painter instance, thus the lock will be acquired only for each Paint type
        private string FindPaintPassword(int passwordLength, long encodedPassword)
        {
            lock (lockObj)
            {
                if (knowSolutions?.TryGetValue(encodedPassword, out var existingSolution) ?? false)
                    return existingSolution;

                var rd = new Random();
                string CreateRandomString()
                {
                    char[] chars = new char[passwordLength];

                    for (int i = 0; i < passwordLength; i++)
                    {
                        chars[i] = PaintJob.ALLOWED_CHARACTERS[rd.Next(0, PaintJob.ALLOWED_CHARACTERS.Length)];
                    }

                    return new string(chars);
                }
                string str = CreateRandomString();

                while (PaintJob.EncodeString(str) != encodedPassword)
                    str = CreateRandomString();

                if (knowSolutions == null)
                    knowSolutions = new();

                knowSolutions.Add(encodedPassword, str);
                return str;
            }
        }
    }
}