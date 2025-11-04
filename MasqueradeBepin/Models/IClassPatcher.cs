using System;
using System.Collections;
using System.Collections.Generic;

namespace MasqueradeBepin.Models
{
    public interface IClassPatcher
    {
        Type TargetClass { get; }
        IEnumerable<PatchInstruction> GeneratePatchInstructions();
    }
}
