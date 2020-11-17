using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corutine
{
    interface ICorutine
    {
        bool MoveNext();
    }

    abstract class Corutine : ICorutine
    {
        public virtual bool MoveNext()
        {
            return false;
        }

        public static ICorutine Call(IEnumerable<ICorutine> e)
        {
            return new CorutineStack(e);
        }

        public static ICorutine Call(params object[] arr)
        {
            var cr = new CorutineTask();
            foreach (var a in arr)
            {
                if (a is ICorutine)
                {
                    cr.Add(a as ICorutine);
                }
                else if (a is IEnumerable<ICorutine>)
                {
                    cr.Add(a as IEnumerable<ICorutine>);
                }

            }
            return cr;
        }
    }

    class CorutineSleep : Corutine
    {
        private DateTime _tick;
        private int _total;
        public CorutineSleep(int msTime)
        {
            _tick = DateTime.Now;
            _total = msTime;
        }
        public override bool MoveNext()
        {
            double elapsed = (DateTime.Now - _tick).TotalMilliseconds;
            return elapsed < _total;
        }
    }

    class CorutineStack : Corutine
    {
        private IEnumerator<ICorutine> _e;
        private ICorutine _cr;

        public CorutineStack(IEnumerable<ICorutine> e)
        {
            _e = e.GetEnumerator();
        }
        public override bool MoveNext()
        {
            if (_cr != null)
            {
                if (_cr.MoveNext())
                    return true;
                _cr = null;
            }

            if (_e.MoveNext())
            {
                if (_e.Current != null)
                    _cr = _e.Current;
                return true;
            }

            return false;
        }
    }

    class CorutineTask : Corutine
    {
        private List<ICorutine> _arr = new List<ICorutine>();
        private List<ICorutine> _tmp = new List<ICorutine>();

        public void Add(IEnumerable<ICorutine> e)
        {
            _arr.Add(new CorutineStack(e));
        }

        public void Add(ICorutine cr)
        {
            _arr.Add(cr);
        }

        public override bool MoveNext()
        {
            if (_arr.Count == 0)
                return false;

            _tmp.AddRange(_arr);
            _arr.Clear();
            foreach (var cr in _tmp)
            {
                if (cr.MoveNext())
                    _arr.Add(cr);
            }
            _tmp.Clear();

            return _arr.Count > 0;
        }

    }
}
