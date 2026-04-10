using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Characters;
using Il2CppVampireSurvivors.Data.Stage;
using Il2CppVampireSurvivors.Signals;
using Il2CppVampireSurvivors.UI;
using Masquerade.Models;
using Masquerade.Util;

namespace Masquerade.Patches
{
    public class MainMenuPagePatches : ClassPatcher<MainMenuPage>
    {
        private static readonly StageType stageType = StageType.GREENACRES;
        private static readonly CharacterType characterType = CharacterType.TP_CHARLOTTE;

        public override IEnumerable<PatchInstruction> GeneratePatchInstructions()
        {
            return new[] {
                new PatchInstruction(TargetClass, nameof(MainMenuPage.OnShowStart), this.GetType().GetMethod(nameof(QuickStartScript)), prefix: false)
            };
        }

        public static void QuickStartScript(MainMenuPage __instance)
        {
            __instance._QuickStartButton.onClick.RemoveAllListeners();
            __instance._QuickStartButton.onClick.AddListener(new Action(() => QuickStartGame(__instance)));
        }

        private static void QuickStartGame(MainMenuPage page)
        {
            page._playerOptions.Config.SelectedCharacter = characterType;
            page._playerOptions.Config.SelectedStage = stageType;

            List<CharacterData> list = page._dataManager.GetConvertedCharacterData()[page._playerOptions.Config.SelectedCharacter].ToSystemList();
            if (list != null && list.Count > 0 && !string.IsNullOrEmpty(list[0].bgm))
            {
                BgmType selectedBGM = Enum.Parse<BgmType>(list[0].bgm);
                page._playerOptions.Config.SelectedBGM = selectedBGM;
                page._playerOptions.Config.SelectedBGMMod = BgmModType.Normal;
            }
            else
            {
                List<StageData> list2 = page._dataManager.GetConvertedStages()[page._playerOptions.Config.SelectedStage].ToSystemList();
                if (list2 != null && list2.Count > 0 && list2[0] != null)
                {
                    page._playerOptions.Config.SelectedBGM = list2[0].BGM;
                    page._playerOptions.Config.SelectedBGMMod = BgmModType.Normal;
                }
            }
            page._signalBus.Fire<UISignals.QuickStartGameSignal>();
        }
    }
}
