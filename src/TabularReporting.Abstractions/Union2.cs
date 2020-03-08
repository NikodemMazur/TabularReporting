using System;

namespace TabularReporting
{
    /// <summary>
    /// Implements a "discriminated union" of two types (what a shame C# has no built-in support for this).
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public abstract class Union2<A, B>
    {
        // external classes cannot inherit
        private Union2() { }
        public abstract T Extract<T>(Func<A, T> f, Func<B, T> g);
        public abstract void Act(Action<A> f, Action<B> g);

        public sealed class Case1 : Union2<A, B>
        {
            readonly A _item;
            public Case1(A item) : base()
            {
                if (item as object == null)
                    throw new ArgumentNullException(nameof(item));
                _item = item;
            }
            public override T Extract<T>(Func<A, T> f, Func<B, T> g)
            {
                return f(_item);
            }
            public override void Act(Action<A> f, Action<B> g)
            {
                f(_item);
            }
        }

        public sealed class Case2 : Union2<A, B>
        {
            readonly B _item;
            public Case2(B item) : base()
            {
                if (item as object == null)
                    throw new ArgumentNullException(nameof(item));
                _item = item;
            }
            public override T Extract<T>(Func<A, T> f, Func<B, T> g)
            {
                return g(_item);
            }
            public override void Act(Action<A> f, Action<B> g)
            {
                g(_item);
            }
        }
    }
}