using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trainer
{
    class Arguments
    {
        public bool Help { get; set; }
        public bool Verbose { get; set; }
        public bool SaveWorkImagesToDisk { get; set; }
        public string Root { get; set; }
    }
}
