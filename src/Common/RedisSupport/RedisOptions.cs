using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RedisSupport
{
    public record RedisOptions
    {
        public string Host { get; init; } = "redis";
        public int Port { get; init; } = 6379;
    }
}
