using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solamis.Telegram
{
    public interface ITGApp
    {
        Task<string> Login(string ContactNumber, WTelegram.Client client);
    }
}
