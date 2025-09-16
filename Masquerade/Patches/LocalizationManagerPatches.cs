using HarmonyLib;
using Il2CppI2.Loc;
using System.Text;

namespace Masquerade.Patches
{
    /// <summary>
    /// Harmony patches for the LocalizationManager class
    /// </summary>
    [HarmonyPatch(typeof(LocalizationManager))]
    public static class LocalizationManagerPatches
    {
        public static LanguageSourceData Source;

        public static void Deinitialize()
        {
            Source = null;
        }
        
        /// <summary>
        /// Updates the localization source with modded weapon text.
        /// </summary>
        /// <remarks>
        /// I'm not sure if this is the best place to do this, but it works for now.
        /// LanguageSourceData needs more investigation so that this can possibly be better.
        /// </remarks>
        [HarmonyPatch(nameof(LocalizationManager.UpdateSources))]
        [HarmonyPostfix]
        public static void GetTransPost()
        {
            if (LocalizationManager.Sources.Count == 0) return;
            if (Source == null) Source = LocalizationManager.GetSourceContaining("weaponLang/{HELLFIRE}name");
            foreach (var weapon in vsMLCore.CustomWeapons)
            {
                StringBuilder sb = new StringBuilder("weaponLang/{");
                sb.Append(weapon.WeaponId); sb.Append("}name");
                if (!Source.ContainsTerm(sb.ToString()))
                {
                    var name = Source.AddTerm(sb.ToString(), eTermType.Text);
                    name.Languages[0] = weapon.WeaponName;
                }
                sb.Replace("name", "description");
                if (!Source.ContainsTerm(sb.ToString()))
                {
                    var desc = Source.AddTerm(sb.ToString(), eTermType.Text);
                    desc.Languages[0] = weapon.WeaponDescription;
                }
                sb.Replace("description", "tips");
                if (!Source.ContainsTerm(sb.ToString()))
                {
                    var desc = Source.AddTerm(sb.ToString(), eTermType.Text);
                    desc.Languages[0] = weapon.WeaponTips;
                }
            }
        }
    }
}
