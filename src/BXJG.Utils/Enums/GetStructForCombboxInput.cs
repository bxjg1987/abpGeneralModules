using BXJG.Utils.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.Utils.Enums
{
    public class GetStructForCombboxInput : GetForSelectInput
    {
        public string LocationSourceName { get; set; }
    }
}
