using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Punchy
{
    sealed class ProcessorWrapper
    {
        ProcessorWrapper()
        {
        }

        public static Processor Instance
        {
            get
            {
                if (!Nested.instance.IsAlive)
                {
                    Nested.instance.Target = new Processor();
                }
                return (Processor)Nested.instance.Target;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly WeakReference instance = new WeakReference(new Processor());
        }
    }
}
