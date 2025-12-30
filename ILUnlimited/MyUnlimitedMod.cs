using HarmonyLib;
using BepInEx;
using UnityEngine;
using System;

[BepInPlugin("com.anda.infinitelives.limitbreak", "Instant Limit Break 999", "1.0.0")]
public class MyLimitBreakMod : BaseUnityPlugin
{
    void Awake()
    {
        var harmony = new Harmony("com.anda.infinitelives.limitbreak");
        harmony.PatchAll();
    }
}

// =====================================================================
// PATCH 1: UI / UX SLIDER (Class 'o')
// Fungsinya: Membiarkan mouse menggeser slider melebihi angka 99/200.
// =====================================================================
[HarmonyPatch(typeof(o), "rm")]
class Patch_UnlockSlider
{
    // Kita "cegat" parameter 'e' (Limit Maksimum) sebelum fungsi jalan
    static void Prefix(ref float e)
    {
        // Jika game meminta batas 99 (Human) atau 200 (Super), kita ubah jadi 999
        if (e == 99f || e == 200f)
        {
            e = 999f;
        }
    }
}

// =====================================================================
// PATCH 2: LOGIKA LATIHAN (Class 'Character', method 'he')
// Fungsinya: Saat stat bertambah, langsung ledakkan ke 999 jika menyentuh limit.
// =====================================================================
[HarmonyPatch(typeof(Character), "he")]
class Patch_UpdateStat
{
    static bool Prefix(Character __instance, int a, float b)
    {
        if (__instance.id != Characters.star) return true; // Biarkan NPC normal

        if (l.eyx > 0)
        {
            b *= j.esi;
            if (__instance.activeCostume == 2) // Super
            {
                __instance.superStat[a] += b;
                if (__instance.superStat[a] >= 200f) __instance.superStat[a] = 999f; // Limit Break
                if (__instance.superStat[a] > 999f) __instance.superStat[a] = 999f;
            }
            else // Human
            {
                __instance.stat[a] += b;
                if (__instance.stat[a] >= 99f) __instance.stat[a] = 999f; // Limit Break
                if (__instance.stat[a] > 999f) __instance.stat[a] = 999f;
            }
            // Batas minimal
            if (__instance.stat[a] < 50f) __instance.stat[a] = 50f;
            if (__instance.superStat[a] < 50f) __instance.superStat[a] = 50f;
        }
        return false;
    }
}

// =====================================================================
// PATCH 3: PENJAGA STAT (Class 'Character', method 'hk')
// Fungsinya: Mencegah game mereset stat 999 kembali ke 99/200.
// =====================================================================
[HarmonyPatch(typeof(Character), "hk")]
class Patch_RoutineCheck
{
    static bool Prefix(Character __instance)
    {
        if (__instance.id == Characters.star) // Khusus Player
        {
            for (int i = 1; i <= 6; i++)
            {
                if (__instance.stat[i] >= 99f) __instance.stat[i] = 999f;
                if (__instance.superStat[i] >= 200f) __instance.superStat[i] = 999f;

                // Batas bawah
                if (__instance.stat[i] < 50f) __instance.stat[i] = 50f;
                if (__instance.superStat[i] < 50f) __instance.superStat[i] = 50f;
            }
            // Health & Spirit Unlimited
            if (__instance.health > 100f) __instance.health = 999f;
            if (__instance.spirit > 100f) __instance.spirit = 999f;
        }
        else // NPC Tetap Normal
        {
            for (int i = 1; i <= 6; i++)
            {
                if (__instance.stat[i] > 99f) __instance.stat[i] = 99f;
                if (__instance.superStat[i] > 200f) __instance.superStat[i] = 200f;
                if (__instance.stat[i] < 50f) __instance.stat[i] = 50f;
                if (__instance.superStat[i] < 50f) __instance.superStat[i] = 50f;
            }
        }
        __instance.stat[0] = __instance.hv(0);
        return false;
    }
}

// =====================================================================
// PATCH 4: VISUAL BAR (Class 'Characters', method 'jz')
// Fungsinya: Agar bar hijau tidak tembus layar.
// =====================================================================
[HarmonyPatch(typeof(Characters), "jz")]
class Patch_VisualBar
{
    static void Prefix(ref float b)
    {
        if (b > 100f) b = 100f;
    }
}

// =====================================================================
// PATCH 5: ANTI-CRASH (Class 'Character', method 'jg')
// Fungsinya: Mencegah IndexOutOfRange karena stat terlalu tinggi.
// =====================================================================
[HarmonyPatch(typeof(Character), "jg")]
class Patch_FixAI_Crash
{
    static Exception Finalizer(Exception __exception)
    {
        return null; // Telan errornya, jangan biarkan crash
    }
}