using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyebot3
{
    public class CornerModule
    {
        private LaplaceCaller laplaceCaller { get; set; }
        public CornerModule(LaplaceCaller caller)
        {
            laplaceCaller = caller;
            for (int i = 6; i > 0; i--) {
                laplaceCaller.callLaplace(i, i + 1, 7, 3, 20, 10000);
            }
        }
    }
}
