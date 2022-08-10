using System;

namespace Shpec
{
    using Internal;

    public class Members
    {
        public Members(params object[] members)
        {
        }

        public Concern<T> Concern<T>(T t) => null;
    }

    public static class Concern
    {
        public static Concern<T> For<T>(T v) => null;
    }
}

namespace Shpec.Internal
{
    public class Concern<T> : Members
    {
        public Concern<T> Before(params Action<T>[] interceptors) => null;
        public Concern<T> After(params Action<T>[] interceptors) => null;
        public Concern<T> Get(params Action<T>[] interceptors) => null;
        public Concern<T> BeforeGet(params Action<T>[] interceptors) => null;
        public Concern<T> AfterGet(params Action<T>[] interceptors) => null;
        public Concern<T> Set(params Action<T>[] interceptors) => null;
        public Concern<T> BeforeSet(params Action<T>[] interceptors) => null;
        public Concern<T> AfterSet(params Action<T>[] interceptors) => null;

        public Concern<T> Before(params Func<T, T>[] interceptors) => null;
        public Concern<T> After(params Func<T, T>[] interceptors) => null;
        public Concern<T> Get(params Func<T, T>[] interceptors) => null;
        public Concern<T> BeforeGet(params Func<T, T>[] interceptors) => null;
        public Concern<T> AfterGet(params Func<T, T>[] interceptors) => null;
        public Concern<T> Set(params Func<T, T>[] interceptors) => null;
        public Concern<T> BeforeSet(params Func<T, T>[] interceptors) => null;
        public Concern<T> AfterSet(params Func<T, T>[] interceptors) => null;
    }
}