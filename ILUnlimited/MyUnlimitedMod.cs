using HarmonyLib;
using BepInEx;
using UnityEngine;
using System;
using System.Reflection;

[BepInPlugin("com.anda.infinitelives.godfix_bai", "God Stats 5050 - Gameplay Fix", "1.0.0")]
public class MyGodFixBai : BaseUnityPlugin
{
    void Awake()
    {
        var harmony = new Harmony("com.anda.infinitelives.godfix_bai");
        harmony.PatchAll();
    }
}

// =====================================================================
// 1. SLIDER UNLOCK (0 - 5050)
// =====================================================================
[HarmonyPatch(typeof(o), "rm")]
class Patch_UnlockSlider
{
    static void Prefix(ref float d, ref float e)
    {
        if (e == 99f || e == 200f) e = 5050f;
        if (d == 50f) d = 0f;
    }
}

// =====================================================================
// 2. DATABASE LOGIC (Latihan & Editor)
// =====================================================================
[HarmonyPatch(typeof(Character), "he")]
class Patch_UpdateStat
{
    static bool Prefix(Character __instance, int a, float b)
    {
        if (__instance.id != Characters.star) return true;

        if (l.eyx > 0)
        {
            b *= j.esi;
            if (__instance.activeCostume == 2)
            {
                __instance.superStat[a] += b;
                if (__instance.superStat[a] > 5050f) __instance.superStat[a] = 5050f;
                if (__instance.superStat[a] < 0f) __instance.superStat[a] = 0f;
            }
            else
            {
                __instance.stat[a] += b;
                if (__instance.stat[a] > 5050f) __instance.stat[a] = 5050f;
                if (__instance.stat[a] < 0f) __instance.stat[a] = 0f;
            }
        }
        return false;
    }
}

[HarmonyPatch(typeof(Character), "hk")]
class Patch_RoutineCheck
{
    static bool Prefix(Character __instance)
    {
        if (__instance.id == Characters.star)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (__instance.stat[i] > 5050f) __instance.stat[i] = 5050f;
                if (__instance.superStat[i] > 5050f) __instance.superStat[i] = 5050f;
            }
            if (__instance.health > 5050f) __instance.health = 5050f;
            if (__instance.spirit > 5050f) __instance.spirit = 5050f;

            __instance.stat[0] = __instance.hv(0);
            return false;
        }
        return true;
    }
}

// =====================================================================
// 3. GAMEPLAY FIX (Method 'bai' di Player.cs)
// INI SOLUSINYA!
// =====================================================================
[HarmonyPatch(typeof(Player), "bai")]
class Patch_GameplayStatCalc
{
    static void Postfix(Player __instance)
    {
        // Cek apakah ini Player Utama (Star)
        if (__instance.fmp == Characters.star)
        {
            // 'fok' adalah array float yang menyimpan stat aktif saat ini (Gameplay Stat)
            // Indeks: 1=Popularity?, 2=Strength, 3=Skill, 4=Agility, 5=Stamina

            // Kita loop stat tempur (biasanya index 2 sampai 5)
            for (int j = 2; j <= 5; j++)
            {
                // Ambil nilai asli dari Database (fmq)
                float baseStat = 0f;

                // Cek wujud apa? (fth.id == 2 biasanya Super)
                if (__instance.fth.id == 2)
                {
                    baseStat = __instance.fmq.superStat[j];
                }
                else
                {
                    baseStat = __instance.fmq.stat[j];
                }

                // Jika base stat kita tinggi (misal 5000), 
                // tapi game baru saja memotongnya jadi 200 di method asli...
                // KEMBALIKAN KE NILAI TINGGI!
                if (baseStat > 200f)
                {
                    // Kita bisa tambahkan sedikit logika penalty jika mau (misal saat cedera),
                    // tapi untuk God Mode murni, kita langsung timpa saja.
                    __instance.fok[j] = baseStat;
                }
            }

            // Fix Health (foa) jika perlu
            // foa biasanya adalah health persentase (0.0 - 1.0) atau nilai mutlak tergantung game.
            // Di sini kita pastikan tidak dicap.
            // (Opsional, tergantung implementasi foa di method lain)
        }
    }
}

// =====================================================================
// 4. UTILITIES
// =====================================================================
[HarmonyPatch(typeof(Character), "jg")]
class Patch_AntiCrash
{
    static Exception Finalizer(Exception __exception) { return null; }
}

[HarmonyPatch(typeof(Character), "hj")]
class Patch_NoDecay
{
    static bool Prefix(Character __instance)
    {
        if (__instance.id == Characters.star) return false;
        return true;
    }
}