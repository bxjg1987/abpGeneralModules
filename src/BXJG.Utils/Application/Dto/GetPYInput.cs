using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Utils.Application
{
    public class GetPYInput
    {
        [Required]
        public string Chinese { get; set; }

        public bool Full { get; set; }

        public bool ToUpper { get; set; } = true;
    }
}
