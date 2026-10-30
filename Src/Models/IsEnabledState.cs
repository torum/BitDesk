using System;
using System.Collections.Generic;
using System.Text;

namespace BitDesk.Models;

internal record IsEnabledState
{
    public bool IsEnabled { get; set; }
    public PairCodes PairCode { get; set; }
}
