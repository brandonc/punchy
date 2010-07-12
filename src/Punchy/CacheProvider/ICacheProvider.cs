using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Punchy
{
    public interface ICacheProvider
    {
        string Get(string key);
        void Set(string key, string value);
    }
}
