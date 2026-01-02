using HarmonyLib;
using BepInEx;
using BepInEx.Configuration; // Wajib untuk fitur Config
using UnityEngine;
using System;
using System.Reflection;

[BepInPlugin("Hans.ILSlider", "Unlimited Slider by Hans", "1.0.0")]
public class MyGodSuperFix : BaseUnityPlugin
{
    // 1. Definisi Config
    public static ConfigEntry<bool> EnableInfiniteStamina;

    void Awake()
    {
        // 2. Inisialisasi Config (Default: true / Nyala)
        EnableInfiniteStamina = Config.Bind("General",
                                            "InfiniteStamina",
                                            true,
                                            "Set to true for unlimited spirit. Set to false for normal drain.");

        var harmony = new Harmony("com.anda.infinitelives.godfix_super");
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
// =====================================================================
[HarmonyPatch(typeof(Player), "bai")]
class Patch_GameplayStatCalc
{
    static void Postfix(Player __instance)
    {
        // Cek apakah ini Player Utama (Star)
        if (__instance.fmp == Characters.star)
        {
            // --- BAGIAN 1: STAT FIX (Agar tetap 5050) ---
            for (int j = 1; j <= 6; j++)
            {
                float baseStat = 0f;
                if (__instance.fth.id == 2)
                {
                    baseStat = __instance.fmq.superStat[j];
                }
                else
                {
                    baseStat = __instance.fmq.stat[j];
                }

                if (baseStat > 200f)
                {
                    __instance.fok[j] = baseStat;
                }
            }

            // --- BAGIAN 2: HEALTH FIX ---
            if (__instance.foa > 5050f) __instance.foa = 5050f;

            // --- BAGIAN 3: SPIRIT FIX ---

            // A. Pastikan Max Spirit Besar (Selalu Aktif agar bar tidak glitch)
            float targetMaxSpirit = __instance.foa * 10f;
            if (__instance.foi < targetMaxSpirit)
            {
                __instance.foi = targetMaxSpirit;
            }

            // B. CEK CONFIG UNTUK INFINITE STAMINA
            // Jika Config True -> Isi Penuh Terus
            // Jika Config False -> Biarkan berkurang (mungkin cepat habis karena stat 5050)
            if (MyGodSuperFix.EnableInfiniteStamina.Value)
            {
                if (__instance.foh < __instance.foi)
                {
                    __instance.foh = __instance.foi;
                }
            }
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