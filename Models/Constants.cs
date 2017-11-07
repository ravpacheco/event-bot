using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace event_bot.Models
{
    public class Constants
    {
        public static string YES_USERS_LIST_NAME = "yesUsers";
        public static string MAYBE_USERS_LIST_NAME = "maybeUsers";

        public static string C4D_PREFIX_KEY = "c4d";
        public static string CONTEXT_PREFIX_KEY = $"{C4D_PREFIX_KEY}:context";
    }
}
