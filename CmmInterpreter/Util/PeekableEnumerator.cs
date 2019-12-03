using System;
using System.Collections.Generic;

namespace CmmInterpreter.Util
{
    public static class PeekableEnumeratorExtension
    {
        public static PeekableEnumerator<T> ToPeekable<T>(this IEnumerator<T> enumerator)
        {
            return new PeekableEnumerator<T>(enumerator);
        }
    }
    /// <summary>
    /// 可Peek的迭代器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PeekableEnumerator<T> : IEnumerator<T>
    {
        protected enum Status { Uninitialized, Starting, Started, Ending, Ended }

        protected IEnumerator<T> Enumerator;

        protected Status status;

        protected T current;

        protected T peek;

        /// <summary>
        /// 构造可以在当前位置获取迭代对象下一个元素的值的迭代器
        /// </summary>
        /// <param name="enumerator"></param>
        public PeekableEnumerator(IEnumerator<T> enumerator)
        {
            this.Enumerator = enumerator;
            status = Status.Uninitialized;
            MoveNext();
        }

        public T Current
        {
            get
            {
                if (status == Status.Starting || status == Status.Ended)
                    return default;
                return current;
            }
        }

        object System.Collections.IEnumerator.Current => Current;

        public T Peek
        {
            get
            {
                if (Status.Ending == status || Status.Ended == status)
                    //throw new InvalidOperationException("Enumeration already finished.");
                    return default;
                return peek;
            }
        }

        public bool MoveNext()
        {
            current = peek;
            switch (status)
            {
                case Status.Uninitialized:
                case Status.Starting:
                    if (Enumerator.MoveNext())
                    {
                        status++;
                        peek = Enumerator.Current;
                    }
                    else
                        status = Status.Ending;
                    break;
                case Status.Started:
                    if (Enumerator.MoveNext())
                        peek = Enumerator.Current;
                    else
                        status++;
                    break;
                case Status.Ending:
                    status++;
                    break;
                case Status.Ended:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Status.Ended != status;
        }

        public void Reset()
        {
            Enumerator.Reset();
            status = Status.Uninitialized;
            MoveNext();
        }

        public void Dispose()
        {
            Enumerator.Dispose();
        }
    }

}
