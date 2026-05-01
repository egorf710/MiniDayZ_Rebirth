using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GAME._scripts.Inventory.Signals
{
    public class Signal_UpdateDropItems
    {
        public bool observ;
        public Signal_UpdateDropItems(bool obs)
        {
            observ = obs;
        }
    }
}
