using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrcBot.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var ircBot = new IrcBot();
            ircBot.Start();
        }
    }
}
