using NewLife.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.RedisCommont
{
    public interface IRedisHelper
    {
        Redis GetRedisClient();
    }
}
