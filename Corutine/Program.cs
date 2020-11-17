using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corutine
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = new Program();
            prog.Run();
            Console.ReadKey();
        }

        private void Run()
        {
            var cr = new CorutineTask();
            cr.Add(crRun());

            while (cr.MoveNext())
            {

            }
        }

        private IEnumerable<ICorutine> crRun()
        {
            yield return Corutine.Call(
                crPrint(5)
            );

            yield return Corutine.Call(
                crPrint(5),
                crPrint(3),
                crPrint(4)
            );

            yield return Corutine.Call(
                crPrint(3)
            );
        }

        private IEnumerable<ICorutine> crPrint(int n)
        {
            for (int i = 0; i < n; ++i)
            {
                Console.WriteLine(i);
                yield return Corutine.Call(crSleep(1000));
            }
        }

        private IEnumerable<ICorutine> crSleep(int msTime)
        {
            DateTime t = DateTime.Now;
            while (true)
            {
                double elapsed = (DateTime.Now - t).TotalMilliseconds;
                if (elapsed > msTime)
                    yield break;
                yield return null;
            }
        }
    }
}
