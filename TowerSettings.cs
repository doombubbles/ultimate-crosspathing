using System;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using UnityEngine;

namespace UltimateCrosspathing;

public class TowerSettings : ModSettings
{
    private static readonly ModSettingCategory IndividualTowers = new("Individual Towers")
    {
        modifyCategory = category =>
        {
            var enableAll = category.AddButton(
                new Info("Enable All", 1000, -100, 562, 200, anchor: new Vector2(0.5f, 1)),
                VanillaSprites.GreenBtnLong, new Action(() =>
                {
                    foreach (var loadInfo in ModContent.GetContent<LoadInfo>())
                    {
                        loadInfo.Enabled.SetValue(true);
                    }
                }));
            enableAll.LayoutElement.ignoreLayout = true;
            enableAll.AddText(new Info("Text", InfoPreset.FillParent), "Enable All", 80f);

            var disableAll = category.AddButton(
                new Info("Disable All", -1000, -100, 562, 200, anchor: new Vector2(0.5f, 1)),
                VanillaSprites.RedBtnLong, new Action(() =>
                {
                    foreach (var loadInfo in ModContent.GetContent<LoadInfo>())
                    {
                        loadInfo.Enabled.SetValue(false);
                    }
                }));
            disableAll.LayoutElement.ignoreLayout = true;
            disableAll.AddText(new Info("Text", InfoPreset.FillParent), "Disable All", 80f);
        }
    };

    public static readonly ModSettingBool DartMonkeyEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.DartMonkeyIcon
    };

    public static readonly ModSettingBool BoomerangMonkeyEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.BoomerangMonkeyIcon
    };

    public static readonly ModSettingBool BombShooterEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.BombShooterIcon
    };

    public static readonly ModSettingBool TackShooterEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.TackShooterIcon
    };

    public static readonly ModSettingBool IceMonkeyEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.IceMonkeyIcon
    };

    public static readonly ModSettingBool GlueGunnerEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.GlueGunnerIcon
    };

    public static readonly ModSettingBool SniperMonkeyEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.SniperMonkeyIcon
    };

    public static readonly ModSettingBool MonkeySubEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.MonkeySubIcon
    };

    public static readonly ModSettingBool MonkeyBuccaneerEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.MonkeyBuccaneerIcon
    };

    public static readonly ModSettingBool MonkeyAceEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.MonkeyAceIcon
    };

    public static readonly ModSettingBool HeliPilotEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.HeliPilotIcon
    };

    public static readonly ModSettingBool MortarMonkeyEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.MortarMonkeyIcon
    };

    public static readonly ModSettingBool DartlingGunnerEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.DartlingGunnerIcon
    };

    public static readonly ModSettingBool WizardMonkeyEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.WizardIcon
    };

    public static readonly ModSettingBool SuperMonkeyEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.SuperMonkeyIcon
    };

    public static readonly ModSettingBool NinjaMonkeyEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.NInjaMonkeyIcon
    };

    public static readonly ModSettingBool AlchemistEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.AlchemistIcon
    };

    public static readonly ModSettingBool DruidEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.DruidIcon
    };

    public static readonly ModSettingBool BananaFarmEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.BananaFarmIcon2
    };

    public static readonly ModSettingBool SpikeFactoryEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.SpikeFactoryIcon
    };

    public static readonly ModSettingBool MonkeyVillageEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.MonkeyVillageIcon
    };

    public static readonly ModSettingBool EngineerMonkeyEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.EngineerMonkeyicon
    };

    public static readonly ModSettingBool BeastHandlerEnabled = new(true)
    {
        button = true,
        category = IndividualTowers,
        icon = VanillaSprites.BeastHandlerIcon
    };
}