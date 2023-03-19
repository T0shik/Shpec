using System;

namespace Shpec
{
    using Internal;

    public class Members
    {
        public Members(params object[] members)
        {
        }
    }
    
    public class Role
    {
        public Role(params object[] members)
        {
        }
    }

    public class Advice
    {
        public static AdviceChain Before(Delegate a) => null;
        public static AdviceChain After(Delegate a) => null;
        public static AdviceChain Get(Delegate a) => null;
        public static AdviceChain BeforeGet(Delegate a) => null;
        public static AdviceChain AfterGet(Delegate a) => null;
        public static AdviceChain Set(Delegate a) => null;
        public static AdviceChain BeforeSet(object a) => null;
        public static AdviceChain AfterSet(Delegate a) => null;
    }
}

namespace Shpec.Internal
{
    public class AdviceChain
    {
        public AdviceChain Before(object a) => null;
        public AdviceChain After(object a) => null;
        public AdviceChain Get(object a) => null;
        public AdviceChain BeforeGet(object a) => null;
        public AdviceChain AfterGet(object a) => null;
        public AdviceChain Set(object a) => null;
        public AdviceChain BeforeSet(object a) => null;
        public AdviceChain AfterSet(object a) => null;
    }
}