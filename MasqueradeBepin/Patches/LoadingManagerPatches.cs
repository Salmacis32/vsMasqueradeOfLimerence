using MasqueradeBepin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VampireSurvivors.Framework.DLC;

namespace MasqueradeBepin.Patches
{
    public class LoadingManagerPatches : IClassPatcher
    {
        public Type TargetClass => typeof(LoadingManager);

        public IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            var instructions = new List<PatchInstruction>();
            instructions.Add(new PatchInstruction(TargetClass, nameof(LoadingManager.LoadDlcs), typeof(LoadingManagerPatches).GetMethod(nameof(PreLoadDlcs))));
            return instructions;
        }

        public static void PreLoadDlcs(LoadingManager __instance)
        {
            DlcData data = ScriptableObject.CreateInstance<DlcData>();
            data._DlcType = Common.VSML_DLC_TYPE; data._Title = "Masquerade of Limerence";
            data._ContentGroupType = VampireSurvivors.Data.ContentGroupType.BASE; data._ExpectedVersion = Common.BMD_VERSION;
            data._HasBeenReleased = true; 
            DlcSystem.DlcCatalog._DlcData.TryAdd(Common.VSML_DLC_TYPE, data);
            DlcSystem.SelectedDlc.TryAdd(Common.VSML_DLC_TYPE, true);
        }
    }
}
