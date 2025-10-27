using BepInEx;

using HarmonyLib;

namespace FleaBrewCooldownFix;

[BepInAutoPlugin(id: "dev.clazex.fleabrewcooldownfix")]
public sealed partial class FleaBrewCooldownFixPlugin : BaseUnityPlugin {
	public static FleaBrewCooldownFixPlugin Instance { get; private set; } = null!;

	private Harmony Harmony { get; } = new(Id);

	private void Awake() {
		Instance = this;
		Harmony.PatchAll(typeof(FleaBrewCooldownFixPlugin));
		Logger.LogInfo($"Plugin {Name} ({Id}) v{Version} has loaded!");
	}

	private void OnDestroy() {
#if !DEBUG
		Logger.LogWarning("Unloading in release build");
#endif
		Harmony.UnpatchSelf();
	}

	[HarmonyPatch(typeof(HeroController), nameof(HeroController.DidAttack))]
	[HarmonyPostfix]
	private static void FixCooldown(HeroController __instance) {
		__instance.attack_cooldown = __instance.IsUsingQuickening
			? __instance.Config.QuickAttackCooldownTime
			: __instance.Config.AttackCooldownTime;

		if (__instance.attack_cooldown < __instance.attackDuration) {
			__instance.attack_cooldown = __instance.attackDuration;
		}
	}
}
