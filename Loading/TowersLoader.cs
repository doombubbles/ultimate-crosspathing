using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using BTD_Mod_Helper.Extensions;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Runtime.Serialization;
using Il2CppSystem.Reflection;
using Il2CppSystem;
using Assets.Scripts.Simulation.SMath;
using System.IO;

public class TowersLoader
{
    BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    BinaryReader br = null;

    // NOTE: was a collection per type but it prevented inheriance e.g list of Products would required class type id
    object[] m;
    int mIndex = 1; // first element is null

    #region Read array

    private void LinkArray<T>() where T : Il2CppObjectBase
    {
        var setCount = br.ReadInt32();
        for (var i = 0; i < setCount; i++)
        {
            var arrIndex = br.ReadInt32();
            var arr = (Il2CppReferenceArray<T>)m[arrIndex];
            for (var j = 0; j < arr.Length; j++)
            {
                arr[j] = (T)m[br.ReadInt32()];
            }
        }
    }

    private void LinkList<T>() where T : Il2CppObjectBase
    {
        var setCount = br.ReadInt32();
        for (var i = 0; i < setCount; i++)
        {
            var arrIndex = br.ReadInt32();
            var arr = (List<T>)m[arrIndex];
            for (var j = 0; j < arr.Capacity; j++)
            {
                arr.Add((T)m[br.ReadInt32()]);
            }
        }
    }

    private void LinkDictionary<T>() where T : Il2CppObjectBase
    {
        var setCount = br.ReadInt32();
        for (var i = 0; i < setCount; i++)
        {
            var arrIndex = br.ReadInt32();
            var arr = (Dictionary<string, T>)m[arrIndex];
            var arrCount = br.ReadInt32();
            for (var j = 0; j < arrCount; j++)
            {
                var key = br.ReadString();
                var valueIndex = br.ReadInt32();
                arr[key] = (T)m[valueIndex];
            }
        }
    }

    private void LinkModelDictionary<T>() where T : Assets.Scripts.Models.Model
    {
        var setCount = br.ReadInt32();
        for (var i = 0; i < setCount; i++)
        {
            var arrIndex = br.ReadInt32();
            var arr = (Dictionary<string, T>)m[arrIndex];
            var arrCount = br.ReadInt32();
            for (var j = 0; j < arrCount; j++)
            {
                var valueIndex = br.ReadInt32();
                var obj = (T)m[valueIndex];
                arr[obj.name] = obj;
            }
        }
    }

    private void Read_a_Int32_Array()
    {
        var arrSetCount = br.ReadInt32();
        var count = arrSetCount;
        for (var i = 0; i < count; i++)
        {
            var arrCount = br.ReadInt32();
            var arr = new Il2CppStructArray<int>(arrCount);
            for (var j = 0; j < arr.Length; j++)
            {
                arr[j] = br.ReadInt32();
            }

            m[mIndex++] = arr;
        }
    }

    private void Read_a_Single_Array()
    {
        var arrSetCount = br.ReadInt32();
        var count = arrSetCount;
        for (var i = 0; i < count; i++)
        {
            var arrCount = br.ReadInt32();
            var arr = new Il2CppStructArray<float>(arrCount);
            for (var j = 0; j < arr.Length; j++)
            {
                arr[j] = br.ReadSingle();
            }

            m[mIndex++] = arr;
        }
    }

    private void Read_a_String_Array()
    {
        var arrSetCount = br.ReadInt32();
        var count = arrSetCount;
        for (var i = 0; i < count; i++)
        {
            var arrCount = br.ReadInt32();
            var arr = new Il2CppStringArray(arrCount);
            for (var j = 0; j < arr.Length; j++)
            {
                arr[j] = br.ReadBoolean() ? null : br.ReadString();
            }

            m[mIndex++] = arr;
        }
    }

    private void Read_a_Vector3_Array()
    {
        var arrSetCount = br.ReadInt32();
        var count = arrSetCount;
        for (var i = 0; i < count; i++)
        {
            var arrCount = br.ReadInt32();
            var arr = new Il2CppStructArray<Assets.Scripts.Simulation.SMath.Vector3>(arrCount);
            for (var j = 0; j < arr.Length; j++)
            {
                arr[j] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            }

            m[mIndex++] = arr;
        }
    }

    private void Read_a_TargetType_Array()
    {
        var arrSetCount = br.ReadInt32();
        var count = arrSetCount;
        for (var i = 0; i < count; i++)
        {
            var arrCount = br.ReadInt32();
            var arr = new Il2CppReferenceArray<Assets.Scripts.Models.Towers.TargetType>(arrCount);
            for (var j = 0; j < arr.Length; j++)
            {
                arr[j] = new Assets.Scripts.Models.Towers.TargetType
                    { id = br.ReadString(), isActionable = br.ReadBoolean() };
            }

            m[mIndex++] = arr;
        }
    }

    private void Read_a_AreaType_Array()
    {
        var arrSetCount = br.ReadInt32();
        var count = arrSetCount;
        for (var i = 0; i < count; i++)
        {
            var arrCount = br.ReadInt32();
            var arr = new Il2CppStructArray<Assets.Scripts.Models.Map.AreaType>(arrCount);
            for (var j = 0; j < arr.Length; j++)
            {
                arr[j] = (Assets.Scripts.Models.Map.AreaType)br.ReadInt32();
            }

            m[mIndex++] = arr;
        }
    }

    private void Read_l_String_List()
    {
        var arrSetCount = br.ReadInt32();
        var count = arrSetCount;
        for (var i = 0; i < count; i++)
        {
            var arrCount = br.ReadInt32();
            var arr = new List<string>(arrCount);
            for (var j = 0; j < arrCount; j++)
            {
                arr.Add(br.ReadBoolean() ? null : br.ReadString());
            }

            m[mIndex++] = arr;
        }
    }

    private void Read_String_v_Single_Dictionary()
    {
        var arrSetCount = br.ReadInt32();
        var count = arrSetCount;
        for (var i = 0; i < count; i++)
        {
            var arrCount = br.ReadInt32();
            var arr = new Dictionary<string, float>(arrCount);
            for (var j = 0; j < arrCount; j++)
            {
                var key = br.ReadBoolean() ? null : br.ReadString();
                var value = br.ReadSingle();
                arr[key] = value;
            }

            m[mIndex++] = arr;
        }
    }

    #endregion

    #region Read object records

    private void CreateArraySet<T>() where T : Il2CppObjectBase
    {
        var arrCount = br.ReadInt32();
        for (var i = 0; i < arrCount; i++)
        {
            m[mIndex++] = new Il2CppReferenceArray<T>(br.ReadInt32());
            ;
        }
    }

    private void CreateListSet<T>() where T : Il2CppObjectBase
    {
        var arrCount = br.ReadInt32();
        for (var i = 0; i < arrCount; i++)
        {
            m[mIndex++] = new List<T>(br.ReadInt32()); // set capactity
        }
    }

    private void CreateDictionarySet<K, T>()
    {
        var arrCount = br.ReadInt32();
        for (var i = 0; i < arrCount; i++)
        {
            m[mIndex++] = new Dictionary<K, T>(br.ReadInt32()); // set capactity
        }
    }

    private void Create_Records<T>() where T : Il2CppObjectBase
    {
        var count = br.ReadInt32();
        var t = Il2CppType.Of<T>();
        for (var i = 0; i < count; i++)
        {
            m[mIndex++] = FormatterServices.GetUninitializedObject(t).Cast<T>();
        }
    }

    #endregion

    #region Link object records

    private void Set_v_Model_Fields(int start, int count)
    {
        var t = Il2CppType.Of<Assets.Scripts.Models.Model>();
        var _nameField = t.GetField("_name", bindFlags);
        var childDependantsField = t.GetField("childDependants", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Model)m[i + start];
            _nameField.SetValue(v, br.ReadBoolean() ? null : String.Intern(br.ReadString()));
            childDependantsField.SetValue(v, (List<Assets.Scripts.Models.Model>)m[br.ReadInt32()]);
        }
    }

    private void Set_v_TowerModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.TowerModel>();
        var towerSizeField = t.GetField("towerSize", bindFlags);
        var cachedThrowMarkerHeightField = t.GetField("cachedThrowMarkerHeight", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.TowerModel)m[i + start];
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.baseId = br.ReadBoolean() ? null : br.ReadString();
            v.cost = br.ReadSingle();
            v.radius = br.ReadSingle();
            v.radiusSquared = br.ReadSingle();
            v.range = br.ReadSingle();
            v.ignoreBlockers = br.ReadBoolean();
            v.isGlobalRange = br.ReadBoolean();
            v.tier = br.ReadInt32();
            v.tiers = (Il2CppStructArray<int>)m[br.ReadInt32()];
            v.towerSet = br.ReadBoolean() ? null : br.ReadString();
            v.areaTypes = (Il2CppStructArray<Assets.Scripts.Models.Map.AreaType>)m[br.ReadInt32()];
            v.icon = (Assets.Scripts.Utils.SpriteReference)m[br.ReadInt32()];
            v.portrait = (Assets.Scripts.Utils.SpriteReference)m[br.ReadInt32()];
            v.instaIcon = (Assets.Scripts.Utils.SpriteReference)m[br.ReadInt32()];
            v.mods = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Mods.ApplyModModel>)m[br.ReadInt32()];
            v.ignoreTowerForSelection = br.ReadBoolean();
            v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Model>)m[br.ReadInt32()];
            v.footprint = (Assets.Scripts.Models.Towers.Behaviors.FootprintModel)m[br.ReadInt32()];
            v.dontDisplayUpgrades = br.ReadBoolean();
            v.emoteSpriteSmall = (Assets.Scripts.Utils.SpriteReference)m[br.ReadInt32()];
            v.emoteSpriteLarge = (Assets.Scripts.Utils.SpriteReference)m[br.ReadInt32()];
            v.doesntRotate = br.ReadBoolean();
            v.upgrades =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>)m[br.ReadInt32()];
            v.appliedUpgrades = (Il2CppStringArray)m[br.ReadInt32()];
            v.targetTypes = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TargetType>)m[br.ReadInt32()];
            v.paragonUpgrade = (Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel)m[br.ReadInt32()];
            v.isSubTower = br.ReadBoolean();
            v.isBakable = br.ReadBoolean();
            v.powerName = br.ReadBoolean() ? null : br.ReadString();
            v.showPowerTowerBuffs = br.ReadBoolean();
            v.animationSpeed = br.ReadSingle();
            v.towerSelectionMenuThemeId = br.ReadBoolean() ? null : br.ReadString();
            v.ignoreCoopAreas = br.ReadBoolean();
            v.canAlwaysBeSold = br.ReadBoolean();
            v.isParagon = br.ReadBoolean();
            v.sellbackModifierAdd = br.ReadSingle();
            towerSizeField.SetValue(v, br.ReadInt32().ToIl2Cpp());
            cachedThrowMarkerHeightField.SetValue(v, br.ReadSingle().ToIl2Cpp());
        }
    }

    private void Set_ar_Sprite_Fields(int start, int count)
    {
        Set_v_AssetReference_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Utils.AssetReference<UnityEngine.Sprite>)m[i + start];
        }
    }

    private void Set_v_AssetReference_Fields(int start, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Utils.AssetReference)m[i + start];
        }
    }

    private void Set_v_SpriteReference_Fields(int start, int count)
    {
        Set_ar_Sprite_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Utils.SpriteReference>();
        var guidRefField = t.GetField("guidRef", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Utils.SpriteReference)m[i + start];
            guidRefField.SetValue(v, br.ReadBoolean() ? null : br.ReadString());
        }
    }

    private void Set_v_ApplyModModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mods.ApplyModModel)m[i + start];
            v.mod = br.ReadBoolean() ? null : br.ReadString();
            v.target = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_TowerBehaviorModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.TowerBehaviorModel)m[i + start];
        }
    }

    private void Set_v_CreateEffectOnPlaceModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnPlaceModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_EffectModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Effects.EffectModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.scale = br.ReadSingle();
            v.lifespan = br.ReadSingle();
            v.fullscreen = br.ReadBoolean();
            v.useCenterPosition = br.ReadBoolean();
            v.useTransformPosition = br.ReadBoolean();
            v.useTransfromRotation = br.ReadBoolean();
            v.destroyOnTransformDestroy = br.ReadBoolean();
            v.alwaysUseAge = br.ReadBoolean();
            v.useRoundTime = br.ReadBoolean();
        }
    }

    private void Set_v_CreateEffectOnSellModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnSellModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_TowerBehaviorBuffModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TowerBehaviorBuffModel)m[i + start];
            v.buffLocsName = br.ReadBoolean() ? null : br.ReadString();
            v.buffIconName = br.ReadBoolean() ? null : br.ReadString();
            v.maxStackSize = br.ReadInt32();
            v.isGlobalRange = br.ReadBoolean();
        }
    }

    private void Set_v_SubmergeModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SubmergeModel)m[i + start];
            v.submergeAttackModel = (Assets.Scripts.Models.Towers.TowerBehaviorModel)m[br.ReadInt32()];
            v.abilityCooldownSpeedScale = br.ReadSingle();
            v.abilityCooldownSpeedScaleGlobal = br.ReadSingle();
            v.submergeDepth = br.ReadSingle();
            v.submergeSpeed = br.ReadSingle();
            v.heroXpScale = br.ReadSingle();
            v.attackDisplayPath = br.ReadBoolean() ? null : br.ReadString();
            v.submergeSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.emergeSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.supportMutatorPriority = br.ReadInt32();
        }
    }

    private void Set_v_AttackModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel)m[i + start];
            v.weapons = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Weapons.WeaponModel>)m[br.ReadInt32()];
            v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Model>)m[br.ReadInt32()];
            v.range = br.ReadSingle();
            v.targetProvider =
                (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel)m[br.ReadInt32()];
            v.offsetX = br.ReadSingle();
            v.offsetY = br.ReadSingle();
            v.offsetZ = br.ReadSingle();
            v.attackThroughWalls = br.ReadBoolean();
            v.fireWithoutTarget = br.ReadBoolean();
            v.framesBeforeRetarget = br.ReadInt32();
            v.addsToSharedGrid = br.ReadBoolean();
            v.sharedGridRange = br.ReadSingle();
        }
    }

    private void Set_v_WeaponModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
        var rateField = t.GetField("rate", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.WeaponModel)m[i + start];
            v.animation = br.ReadInt32();
            v.animationOffset = br.ReadSingle();
            v.animationOffsetFrames = br.ReadInt32();
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.ejectX = br.ReadSingle();
            v.ejectY = br.ReadSingle();
            v.ejectZ = br.ReadSingle();
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.rateFrames = br.ReadInt32();
            v.fireWithoutTarget = br.ReadBoolean();
            v.fireBetweenRounds = br.ReadBoolean();
            v.behaviors =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>)m[br.ReadInt32()];
            rateField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.useAttackPosition = br.ReadBoolean();
            v.startInCooldown = br.ReadBoolean();
            v.customStartCooldown = br.ReadSingle();
            v.customStartCooldownFrames = br.ReadInt32();
            v.animateOnMainAttack = br.ReadBoolean();
        }
    }

    private void Set_v_EmissionModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[i + start];
            v.behaviors =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel>)m[
                    br.ReadInt32()];
        }
    }

    private void Set_v_SingleEmissionModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel)m[i + start];
        }
    }

    private void Set_v_ProjectileModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[i + start];
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.id = br.ReadBoolean() ? null : br.ReadString();
            v.maxPierce = br.ReadSingle();
            v.pierce = br.ReadSingle();
            v.scale = br.ReadSingle();
            v.ignoreBlockers = br.ReadBoolean();
            v.usePointCollisionWithBloons = br.ReadBoolean();
            v.canCollisionBeBlockedByMapLos = br.ReadBoolean();
            v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>)m[br.ReadInt32()];
            v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Model>)m[br.ReadInt32()];
            v.collisionPasses = (Il2CppStructArray<int>)m[br.ReadInt32()];
            v.canCollideWithBloons = br.ReadBoolean();
            v.radius = br.ReadSingle();
            v.vsBlockerRadius = br.ReadSingle();
            v.hasDamageModifiers = br.ReadBoolean();
            v.dontUseCollisionChecker = br.ReadBoolean();
            v.checkCollisionFrames = br.ReadInt32();
            v.ignoreNonTargetable = br.ReadBoolean();
            v.ignorePierceExhaustion = br.ReadBoolean();
            v.saveId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_ProjectileBehaviorModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.ProjectileBehaviorModel)m[i + start];
            v.collisionPass = br.ReadInt32();
        }
    }

    private void Set_v_AgeModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel)m[i + start];
            v.rounds = br.ReadInt32();
            v.lifespanFrames = br.ReadInt32();
            v.useRoundTime = br.ReadBoolean();
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.endOfRoundClearBypassModel =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.EndOfRoundClearBypassModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_RemoveBloonModifiersModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.RemoveBloonModifiersModel)m[i + start];
            v.cleanseRegen = br.ReadBoolean();
            v.cleanseCamo = br.ReadBoolean();
            v.cleanseLead = br.ReadBoolean();
            v.cleanseFortified = br.ReadBoolean();
            v.cleanseOnlyIfDamaged = br.ReadBoolean();
            v.bloonTagExcludeList = (List<System.String>)m[br.ReadInt32()];
        }
    }

    private void Set_v_DisplayModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[i + start];
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.layer = br.ReadInt32();
            v.positionOffset = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            v.scale = br.ReadSingle();
            v.ignoreRotation = br.ReadBoolean();
            v.animationChanges = (List<Assets.Scripts.Models.GenericBehaviors.AnimationChange>)m[br.ReadInt32()];
            v.delayedReveal = br.ReadSingle();
        }
    }

    private void Set_v_WeaponBehaviorModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel)m[i + start];
        }
    }

    private void Set_v_EjectEffectModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectEffectModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.lifespan = br.ReadSingle();
            v.fullscreen = br.ReadBoolean();
            v.rotateToWeapon = br.ReadBoolean();
            v.useEjectPoint = br.ReadBoolean();
            v.useEmittedFrom = br.ReadBoolean();
            v.useMainAttackRotation = br.ReadBoolean();
        }
    }

    private void Set_v_IgnoreThrowMarkerModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.IgnoreThrowMarkerModel)m[i + start];
        }
    }

    private void Set_v_WeaponRateMinModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.WeaponRateMinModel)m[i + start];
            v.min = br.ReadSingle();
        }
    }

    private void Set_v_FilterModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterModel)m[i + start];
        }
    }

    private void Set_v_FilterOutTagModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterOutTagModel)m[i + start];
            v.tag = br.ReadBoolean() ? null : br.ReadString();
            v.disableWhenSupportMutatorIDs = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_DamageModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel)m[i + start];
            v.damage = br.ReadSingle();
            v.maxDamage = br.ReadSingle();
            v.distributeToChildren = br.ReadBoolean();
            v.overrideDistributeBlocker = br.ReadBoolean();
            v.createPopEffect = br.ReadBoolean();
            v.immuneBloonProperties = (BloonProperties)(br.ReadInt32());
        }
    }

    private void Set_v_ProjectileFilterModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileFilterModel)m[i + start];
            v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>)m[br.ReadInt32()];
        }
    }

    private void Set_v_AddTagToBloonModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddTagToBloonModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddTagToBloonModel)m[i + start];
            v.bloonTag = br.ReadBoolean() ? null : br.ReadString();
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFrames = br.ReadInt32();
            v.layers = br.ReadInt32();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.isUnique = br.ReadBoolean();
            v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>)m[br.ReadInt32()];
            v.overlayLayer = br.ReadInt32();
        }
    }

    private void Set_v_AssetPathModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Effects.AssetPathModel)m[i + start];
            v.assetPath = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_AttackBehaviorModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.AttackBehaviorModel)m[i + start];
        }
    }

    private void Set_v_AttackFilterModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.AttackFilterModel)m[i + start];
            v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>)m[br.ReadInt32()];
        }
    }

    private void Set_v_FilterInvisibleModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterInvisibleModel)m[i + start];
            v.isActive = br.ReadBoolean();
            v.ignoreBroadPhase = br.ReadBoolean();
        }
    }

    private void Set_v_SoundModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Audio.SoundModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_BuffIndicatorModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.GenericBehaviors.BuffIndicatorModel>();
        var _fullNameField = t.GetField("_fullName", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.GenericBehaviors.BuffIndicatorModel)m[i + start];
            v.buffName = br.ReadBoolean() ? null : br.ReadString();
            v.iconName = br.ReadBoolean() ? null : br.ReadString();
            v.stackable = br.ReadBoolean();
            v.maxStackSize = br.ReadInt32();
            v.globalRange = br.ReadBoolean();
            v.onlyShowBuffIfMutated = br.ReadBoolean();
            _fullNameField.SetValue(v, br.ReadBoolean() ? null : br.ReadString());
        }
    }

    private void Set_v_CreateSoundOnSellModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnSellModel)m[i + start];
            v.sound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_CreateSoundOnUpgradeModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnUpgradeModel)m[i + start];
            v.sound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound1 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound2 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound3 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound4 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound5 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound6 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound7 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound8 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_SubmergeEffectModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SubmergeEffectModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.baseTowerRange = br.ReadSingle();
            v.projectileModel = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.displayRadius = br.ReadSingle();
        }
    }

    private void Set_v_LinkProjectileRadiusToTowerRangeModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.LinkProjectileRadiusToTowerRangeModel)m[i + start];
            v.projectileModel = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.baseTowerRange = br.ReadSingle();
            v.projectileRadiusOffset = br.ReadSingle();
            v.displayRadius = br.ReadSingle();
        }
    }

    private void Set_v_CreateSoundOnTowerPlaceModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnTowerPlaceModel)m[i + start];
            v.sound1 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound2 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.heroSound1 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.heroSound2 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_BlankSoundModel_Fields(int start, int count)
    {
        Set_v_SoundModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Audio.BlankSoundModel)m[i + start];
        }
    }

    private void Set_v_CreateEffectOnUpgradeModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnUpgradeModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_EmissionWithOffsetsModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionWithOffsetsModel)m[i + start];
            v.throwMarkerOffsetModels =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel>)m[
                    br.ReadInt32()];
            v.projectileCount = br.ReadInt32();
            v.rotateProjectileWithTower = br.ReadBoolean();
            v.randomRotationCone = br.ReadSingle();
        }
    }

    private void Set_v_EmissionBehaviorModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel)m[i + start];
        }
    }

    private void Set_v_EmissionCamoIfTargetIsCamoModel_Fields(int start, int count)
    {
        Set_v_EmissionBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionCamoIfTargetIsCamoModel)m[
                    i + start];
        }
    }

    private void Set_v_TravelStraitModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        var speedField = t.GetField("speed", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel)m[i + start];
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFrames = br.ReadInt32();
            speedField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.speedFrames = br.ReadSingle();
        }
    }

    private void Set_v_TrackTargetModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetModel>();
        var turnRateField = t.GetField("turnRate", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetModel)m[i + start];
            v.distance = br.ReadSingle();
            v.trackNewTargets = br.ReadBoolean();
            v.constantlyAquireNewTarget = br.ReadBoolean();
            v.maxSeekAngle = br.ReadSingle();
            v.ignoreSeekAngle = br.ReadBoolean();
            v.overrideRotation = br.ReadBoolean();
            v.useLifetimeAsDistance = br.ReadBoolean();
            v.turnRatePerFrame = br.ReadSingle();
            turnRateField.SetValue(v, br.ReadSingle().ToIl2Cpp());
        }
    }

    private void Set_v_AlternateAnimationModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.AlternateAnimationModel)m[i + start];
            v.states = br.ReadInt32();
            v.originState = br.ReadInt32();
        }
    }

    private void Set_v_RotateToTargetModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetModel)m[i + start];
            v.onlyRotateDuringThrow = br.ReadBoolean();
            v.useThrowMarkerHeight = br.ReadBoolean();
            v.rotateOnlyOnThrow = br.ReadBoolean();
            v.additionalRotation = br.ReadInt32();
            v.rotateTower = br.ReadBoolean();
            v.useMainAttackRotation = br.ReadBoolean();
        }
    }

    private void Set_v_FilterInvisibleSubIntelModel_Fields(int start, int count)
    {
        Set_v_FilterInvisibleModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterInvisibleSubIntelModel)m[i + start];
        }
    }

    private void Set_v_TargetSupplierModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel)m[i + start];
            v.isOnSubTower = br.ReadBoolean();
        }
    }

    private void Set_v_TargetFirstSharedRangeModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstSharedRangeModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.isSharedRangeEnabled = br.ReadBoolean();
            v.isGlobalRange = br.ReadBoolean();
        }
    }

    private void Set_v_TargetLastSharedRangeModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastSharedRangeModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.isSharedRangeEnabled = br.ReadBoolean();
            v.isGlobalRange = br.ReadBoolean();
        }
    }

    private void Set_v_TargetCloseSharedRangeModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseSharedRangeModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.isSharedRangeEnabled = br.ReadBoolean();
            v.isGlobalRange = br.ReadBoolean();
        }
    }

    private void Set_v_TargetStrongSharedRangeModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongSharedRangeModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.isSharedRangeEnabled = br.ReadBoolean();
            v.isGlobalRange = br.ReadBoolean();
        }
    }

    private void Set_v_SubmergedTargetModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.SubmergedTargetModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_PreEmptiveStrikeLauncherModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.PreEmptiveStrikeLauncherModel)m[i + start];
            v.projectileModel = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emissionModel = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.throwOffsetX = br.ReadSingle();
            v.throwOffsetY = br.ReadSingle();
            v.throwOffsetZ = br.ReadSingle();
            v.ejectEffectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.animationState = br.ReadInt32();
        }
    }

    private void Set_v_FilterAllModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterAllModel)m[i + start];
        }
    }

    private void Set_v_InstantModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.InstantModel)m[i + start];
            v.destroyIfInvalid = br.ReadBoolean();
        }
    }

    private void Set_v_CreateEffectProjectileAfterTimeModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectProjectileAfterTimeModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.time = br.ReadSingle();
            v.timeFrames = br.ReadInt32();
        }
    }

    private void Set_v_Assets_Scripts_Models_Towers_Projectiles_Behaviors_CreateEffectOnExpireModel_Fields(int start,
        int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.fullscreen = br.ReadBoolean();
            v.randomRotation = br.ReadBoolean();
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_CreateSoundOnProjectileExpireModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileExpireModel)m[i + start];
            v.sound1 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound2 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound3 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound4 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound5 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_CreateProjectileOnExpireModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExpireModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.useRotation = br.ReadBoolean();
        }
    }

    private void Set_v_FilterAllExceptTargetModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterAllExceptTargetModel)m[i + start];
        }
    }

    private void Set_v_FilterWithTagModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterWithTagModel)m[i + start];
            v.moabTag = br.ReadBoolean();
            v.camoTag = br.ReadBoolean();
            v.growTag = br.ReadBoolean();
            v.fortifiedTag = br.ReadBoolean();
            v.tag = br.ReadBoolean() ? null : br.ReadString();
            v.inclusive = br.ReadBoolean();
        }
    }

    private void Set_v_InstantDamageEmissionModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.InstantDamageEmissionModel)m[i + start];
        }
    }

    private void Set_v_DamageModifierModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.DamageModifierModel)m[i + start];
        }
    }

    private void Set_v_DamageModifierForTagModel_Fields(int start, int count)
    {
        Set_v_DamageModifierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForTagModel)m[i + start];
            v.tag = br.ReadBoolean() ? null : br.ReadString();
            v.tags = (Il2CppStringArray)m[br.ReadInt32()];
            v.damageMultiplier = br.ReadSingle();
            v.damageAddative = br.ReadSingle();
            v.mustIncludeAllTags = br.ReadBoolean();
            v.applyOverMaxDamage = br.ReadBoolean();
        }
    }

    private void Set_v_DamageModifierForBloonTypeModel_Fields(int start, int count)
    {
        Set_v_DamageModifierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForBloonTypeModel)m[i + start];
            v.bloonId = br.ReadBoolean() ? null : br.ReadString();
            v.damageMultiplier = br.ReadSingle();
            v.damageAdditive = br.ReadSingle();
            v.includeChildren = br.ReadBoolean();
        }
    }

    private void Set_v_AbilityModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel>();
        var cooldownSpeedScaleField = t.GetField("cooldownSpeedScale", bindFlags);
        var animationOffsetField = t.GetField("animationOffset", bindFlags);
        var cooldownField = t.GetField("cooldown", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel)m[i + start];
            v.displayName = br.ReadBoolean() ? null : br.ReadString();
            v.description = br.ReadBoolean() ? null : br.ReadString();
            v.icon = (Assets.Scripts.Utils.SpriteReference)m[br.ReadInt32()];
            v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Model>)m[br.ReadInt32()];
            v.activateOnPreLeak = br.ReadBoolean();
            v.activateOnLeak = br.ReadBoolean();
            v.addedViaUpgrade = br.ReadBoolean() ? null : br.ReadString();
            v.cooldownFrames = br.ReadInt32();
            v.livesCost = br.ReadInt32();
            v.maxActivationsPerRound = br.ReadInt32();
            v.animation = br.ReadInt32();
            v.animationOffsetFrames = br.ReadInt32();
            v.enabled = br.ReadBoolean();
            v.canActivateBetweenRounds = br.ReadBoolean();
            v.resetCooldownOnTierUpgrade = br.ReadBoolean();
            v.disabledByAnotherTower = br.ReadBoolean();
            v.activateOnLivesLost = br.ReadBoolean();
            v.sharedCooldown = br.ReadBoolean();
            v.dontShowStacked = br.ReadBoolean();
            v.animateOnMainAttackDisplay = br.ReadBoolean();
            v.restrictAbilityAfterMaxRoundTimer = br.ReadBoolean();
            cooldownSpeedScaleField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            animationOffsetField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            cooldownField.SetValue(v, br.ReadSingle().ToIl2Cpp());
        }
    }

    private void Set_v_AbilityBehaviorModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityBehaviorModel)m[i + start];
        }
    }

    private void Set_v_ActivateAttackModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ActivateAttackModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ActivateAttackModel)m[i + start];
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFrames = br.ReadInt32();
            v.attacks =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>)m[br.ReadInt32()];
            v.processOnActivate = br.ReadBoolean();
            v.cancelIfNoTargets = br.ReadBoolean();
            v.turnOffExisting = br.ReadBoolean();
            v.endOnRoundEnd = br.ReadBoolean();
            v.endOnDefeatScreen = br.ReadBoolean();
            v.isOneShot = br.ReadBoolean();
        }
    }

    private void Set_v_TargetStrongModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_CreateSoundOnAbilityModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel)m[i + start];
            v.sound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.heroSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.heroSound2 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_FootprintModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.FootprintModel)m[i + start];
            v.doesntBlockTowerPlacement = br.ReadBoolean();
            v.ignoresPlacementCheck = br.ReadBoolean();
            v.ignoresTowerOverlap = br.ReadBoolean();
        }
    }

    private void Set_v_CircleFootprintModel_Fields(int start, int count)
    {
        Set_v_FootprintModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CircleFootprintModel)m[i + start];
            v.radius = br.ReadSingle();
        }
    }

    private void Set_v_UpgradePathModel_Fields(int start, int count)
    {
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
        var towerField = t.GetField("tower", bindFlags);
        var upgradeField = t.GetField("upgrade", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel)m[i + start];
            towerField.SetValue(v, br.ReadBoolean() ? null : br.ReadString());
            upgradeField.SetValue(v, br.ReadBoolean() ? null : br.ReadString());
        }
    }

    private void Set_v_ThrowMarkerOffsetModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel)m[i + start];
            v.ejectX = br.ReadSingle();
            v.ejectY = br.ReadSingle();
            v.ejectZ = br.ReadSingle();
            v.rotation = br.ReadSingle();
        }
    }

    private void Set_v_EmissionRotationOffProjectileDirectionModel_Fields(int start, int count)
    {
        Set_v_EmissionBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionRotationOffProjectileDirectionModel)
                m[i + start];
            v.startingOffset = br.ReadSingle();
            v.angleInBetween = br.ReadSingle();
            v.alwaysCentre = br.ReadBoolean();
        }
    }

    private void Set_v_CreateProjectileOnExhaustFractionModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExhaustFractionModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.fraction = br.ReadSingle();
            v.durationfraction = br.ReadSingle();
            v.canCreateInBetweenRounds = br.ReadBoolean();
        }
    }

    private void Set_v_ArcEmissionModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Emissions.ArcEmissionModel>();
        var CountField = t.GetField("Count", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.ArcEmissionModel)m[i + start];
            v.angle = br.ReadSingle();
            v.offsetStart = br.ReadSingle();
            v.offset = br.ReadSingle();
            v.sliceSize = br.ReadSingle();
            v.ignoreTowerRotation = br.ReadBoolean();
            v.useProjectileRotation = br.ReadBoolean();
            CountField.SetValue(v, br.ReadInt32().ToIl2Cpp());
        }
    }

    private void Set_v_SupportModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SupportModel)m[i + start];
            v.filters =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TowerFilters.TowerFilterModel>)m[br.ReadInt32()];
            v.isGlobal = br.ReadBoolean();
            v.isCustomRadius = br.ReadBoolean();
            v.customRadius = br.ReadSingle();
            v.appliesToOwningTower = br.ReadBoolean();
            v.showBuffIcon = br.ReadBoolean();
            v.buffLocsName = br.ReadBoolean() ? null : br.ReadString();
            v.buffIconName = br.ReadBoolean() ? null : br.ReadString();
            v.maxStackSize = br.ReadInt32();
            v.onlyShowBuffIfMutated = br.ReadBoolean();
        }
    }

    private void Set_v_SubCommanderSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SubCommanderSupportModel)m[i + start];
            v.isUnique = br.ReadBoolean();
            v.pierceIncrease = br.ReadInt32();
            v.damageIncrease = br.ReadInt32();
            v.damageScale = br.ReadSingle();
        }
    }

    private void Set_v_TowerFilterModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.TowerFilters.TowerFilterModel)m[i + start];
        }
    }

    private void Set_v_FilterInBaseTowerIdModel_Fields(int start, int count)
    {
        Set_v_TowerFilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.TowerFilters.FilterInBaseTowerIdModel)m[i + start];
            v.baseIds = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_FilterGlueLevelModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterGlueLevelModel)m[i + start];
            v.glueLevel = br.ReadInt32();
        }
    }

    private void Set_v_SlowModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowModel>();
        var multiplierField = t.GetField("multiplier", bindFlags);
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowModel)m[i + start];
            v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>)m[br.ReadInt32()];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            multiplierField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFrames = br.ReadInt32();
            v.layers = br.ReadInt32();
            v.overlayLayer = br.ReadInt32();
            v.glueLevel = br.ReadInt32();
            v.isUnique = br.ReadBoolean();
            v.dontRefreshDuration = br.ReadBoolean();
            v.cascadeMutators = br.ReadBoolean();
            v.removeMutatorIfNotMatching = br.ReadBoolean();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.mutationFilter = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_AddBehaviorToBloonModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddBehaviorToBloonModel)m[i + start];
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.layers = br.ReadInt32();
            v.lifespanFrames = br.ReadInt32();
            v.filter = (Assets.Scripts.Models.Towers.Filters.FilterModel)m[br.ReadInt32()];
            v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>)m[br.ReadInt32()];
            v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Bloons.BloonBehaviorModel>)m[br.ReadInt32()];
            v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>)m[br.ReadInt32()];
            v.overlayLayer = br.ReadInt32();
            v.isUnique = br.ReadBoolean();
            v.lastAppliesFirst = br.ReadBoolean();
            v.collideThisFrame = br.ReadBoolean();
            v.cascadeMutators = br.ReadBoolean();
            v.glueLevel = br.ReadInt32();
            v.applyOnlyIfDamaged = br.ReadBoolean();
            v.stackCount = br.ReadInt32();
        }
    }

    private void Set_v_DamageOverTimeCustomModel_Fields(int start, int count)
    {
        Set_v_DamageOverTimeModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.Behaviors.DamageOverTimeCustomModel)m[i + start];
            v.bloonTagsList = (Il2CppStringArray)m[br.ReadInt32()];
            v.multiplier = br.ReadSingle();
            v.additive = br.ReadSingle();
        }
    }

    private void Set_v_BloonBehaviorModelWithTowerTracking_Fields(int start, int count)
    {
        Set_v_BloonBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.BloonBehaviorModelWithTowerTracking)m[i + start];
        }
    }

    private void Set_v_BloonBehaviorModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.BloonBehaviorModel)m[i + start];
        }
    }

    private void Set_v_DamageOverTimeModel_Fields(int start, int count)
    {
        Set_v_BloonBehaviorModelWithTowerTracking_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel>();
        var intervalField = t.GetField("interval", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel)m[i + start];
            v.damage = br.ReadSingle();
            v.payloadCount = br.ReadInt32();
            v.immuneBloonProperties = (BloonProperties)(br.ReadInt32());
            v.intervalFrames = br.ReadInt32();
            intervalField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.displayPath = br.ReadBoolean() ? null : br.ReadString();
            v.displayLifetime = br.ReadSingle();
            v.triggerImmediate = br.ReadBoolean();
            v.rotateEffectWithBloon = br.ReadBoolean();
            v.initialDelay = br.ReadSingle();
            v.initialDelayFrames = br.ReadInt32();
            v.damageOnDestroy = br.ReadBoolean();
            v.overrideDistributionBlocker = br.ReadBoolean();
            v.damageModifierModels =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Projectiles.DamageModifierModel>)m[br.ReadInt32()];
        }
    }

    private void Set_v_CreateProjectileOnContactModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnContactModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.passOnCollidedWith = br.ReadBoolean();
            v.dontCreateAtBloon = br.ReadBoolean();
            v.passOnDirectionToContact = br.ReadBoolean();
        }
    }

    private void Set_v_SlowModifierForTagModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowModifierForTagModel)m[i + start];
            v.tag = br.ReadBoolean() ? null : br.ReadString();
            v.slowId = br.ReadBoolean() ? null : br.ReadString();
            v.slowMultiplier = br.ReadSingle();
            v.resetToUnmodified = br.ReadBoolean();
            v.preventMutation = br.ReadBoolean();
            v.lifespanOverride = br.ReadSingle();
        }
    }

    private void Set_v_RemoveMutatorsFromBloonModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.RemoveMutatorsFromBloonModel)m[i + start];
            v.key = br.ReadBoolean() ? null : br.ReadString();
            v.keys = (Il2CppStringArray)m[br.ReadInt32()];
            v.mutatorIds = br.ReadBoolean() ? null : br.ReadString();
            v.mutatorIdList = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_CreateSoundOnProjectileCollisionModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileCollisionModel)m[i + start];
            v.sound1 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound2 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound3 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound4 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound5 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_CreateEffectOnContactModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnContactModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_TargetFirstModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TargetLastModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TargetCloseModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_AddBonusDamagePerHitToBloonModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddBonusDamagePerHitToBloonModel)m[i + start];
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
            v.perHitDamageAddition = br.ReadSingle();
            v.layers = br.ReadInt32();
            v.isUnique = br.ReadBoolean();
            v.lastAppliesFirst = br.ReadBoolean();
            v.cascadeMutators = br.ReadBoolean();
        }
    }

    private void Set_v_RotateToParentModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToParentModel)m[i + start];
        }
    }

    private void Set_v_CreateEffectOnAbilityModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateEffectOnAbilityModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.randomRotation = br.ReadBoolean();
            v.centerEffect = br.ReadBoolean();
            v.destroyOnEnd = br.ReadBoolean();
            v.useAttackTransform = br.ReadBoolean();
            v.canSave = br.ReadBoolean();
        }
    }

    private void Set_v_EmitOnDestroyModel_Fields(int start, int count)
    {
        Set_v_BloonBehaviorModelWithTowerTracking_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.Behaviors.EmitOnDestroyModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_SlowForBloonModel_Fields(int start, int count)
    {
        Set_v_SlowModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowForBloonModel)m[i + start];
            v.bloonId = br.ReadBoolean() ? null : br.ReadString();
            v.bloonIds = (Il2CppStringArray)m[br.ReadInt32()];
            v.bloonTag = br.ReadBoolean() ? null : br.ReadString();
            v.bloonTags = (Il2CppStringArray)m[br.ReadInt32()];
            v.excluding = br.ReadBoolean();
        }
    }

    private void Set_v_EmitOnDamageModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.EmitOnDamageModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_SlowMaimMoabModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMaimMoabModel)m[i + start];
            v.moabDuration = br.ReadSingle();
            v.bfbDuration = br.ReadSingle();
            v.zomgDuration = br.ReadSingle();
            v.ddtDuration = br.ReadSingle();
            v.badDuration = br.ReadSingle();
            v.moabDurationFrames = br.ReadInt32();
            v.bfbDurationFrames = br.ReadInt32();
            v.zomgDurationFrames = br.ReadInt32();
            v.ddtDurationFrames = br.ReadInt32();
            v.badDurationFrames = br.ReadInt32();
            v.multiplier = br.ReadSingle();
            v.moabMutator =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMaimMoabModel.Mutator)m[br.ReadInt32()];
            v.bfbMutator =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMaimMoabModel.Mutator)m[br.ReadInt32()];
            v.zomgMutator =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMaimMoabModel.Mutator)m[br.ReadInt32()];
            v.ddtMutator =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMaimMoabModel.Mutator)m[br.ReadInt32()];
            v.badMutator =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMaimMoabModel.Mutator)m[br.ReadInt32()];
            v.bloonPerHitDamageAddition = br.ReadSingle();
            v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>)m[br.ReadInt32()];
            v.overlayLayer = br.ReadInt32();
        }
    }

    private void Set_v_BehaviorMutator_Fields(int start, int count)
    {
        var t = Il2CppType.Of<Assets.Scripts.Simulation.Objects.BehaviorMutator>();
        var usesSplitIdField = t.GetField("usesSplitId", bindFlags);
        var idMajorField = t.GetField("idMajor", bindFlags);
        var idMajorMinorField = t.GetField("idMajorMinor", bindFlags);
        var resultCacheField = t.GetField("resultCache", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Simulation.Objects.BehaviorMutator)m[i + start];
            v.id = br.ReadBoolean() ? null : br.ReadString();
            usesSplitIdField.SetValue(v, br.ReadBoolean().ToIl2Cpp());
            idMajorField.SetValue(v, br.ReadBoolean() ? null : br.ReadString());
            idMajorMinorField.SetValue(v, br.ReadBoolean() ? null : br.ReadString());
            v.isExclusiveInMutationList = br.ReadBoolean();
            v.priority = br.ReadInt32();
            v.glueLevel = br.ReadInt32();
            v.isFreeze = br.ReadBoolean();
            v.dontCopy = br.ReadBoolean();
            v.buffIndicator = (Assets.Scripts.Models.GenericBehaviors.BuffIndicatorModel)m[br.ReadInt32()];
            v.includesSubTowers = br.ReadBoolean();
            v.saveId = br.ReadBoolean() ? null : br.ReadString();
            resultCacheField.SetValue(v,
                (Dictionary<Assets.Scripts.Models.Model, Assets.Scripts.Models.Model>)m[br.ReadInt32()]);
        }
    }

    private void Set_v_Assets_Scripts_Models_Towers_Projectiles_Behaviors_SlowMaimMoabModel_Mutator_Fields(int start,
        int count)
    {
        Set_v_BehaviorMutator_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMaimMoabModel.Mutator)m[i + start];
            v.multiplier = br.ReadSingle();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.overlayLayer = br.ReadInt32();
            v.bloonPerHitDamageAddition = br.ReadSingle();
            v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>)m[br.ReadInt32()];
        }
    }

    private void Set_v_RetargetOnContactModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RetargetOnContactModel>();
        var delayField = t.GetField("delay", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.RetargetOnContactModel)m[i + start];
            v.distance = br.ReadSingle();
            v.maxBounces = br.ReadInt32();
            delayField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.delayFrames = br.ReadInt32();
            v.targetType.id = br.ReadString();
            v.targetType.actionOnCreate = br.ReadBoolean();
            v.expireIfNoTargetFound = br.ReadBoolean();
        }
    }

    private void Set_v_CreateEffectFromCollisionToCollisionModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectFromCollisionToCollisionModel)m[
                    i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.effectLength = br.ReadSingle();
        }
    }

    private void Set_v_CheckTargetsWithoutOffsetsModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.CheckTargetsWithoutOffsetsModel)m[i + start];
        }
    }

    private void Set_v_TargetCamoModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCamoModel)m[i + start];
        }
    }

    private void Set_v_TargetFirstPrioCamoModel_Fields(int start, int count)
    {
        Set_v_TargetCamoModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstPrioCamoModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TargetLastPrioCamoModel_Fields(int start, int count)
    {
        Set_v_TargetCamoModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastPrioCamoModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TargetClosePrioCamoModel_Fields(int start, int count)
    {
        Set_v_TargetCamoModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetClosePrioCamoModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TargetStrongPrioCamoModel_Fields(int start, int count)
    {
        Set_v_TargetCamoModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongPrioCamoModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TargetEliteTargettingModel_Fields(int start, int count)
    {
        Set_v_TargetCamoModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetEliteTargettingModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.percentageThroughMap = br.ReadSingle();
        }
    }

    private void Set_v_SwitchTargetSupplierOnUpgradeModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SwitchTargetSupplierOnUpgradeModel)m[i + start];
            v.targetSupplierName = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_PickupModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.PickupModel)m[i + start];
            v.collectRadius = br.ReadSingle();
            v.delay = br.ReadSingle();
            v.delayFrames = br.ReadSingle();
        }
    }

    private void Set_v_CreateSoundOnPickupModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnPickupModel)m[i + start];
            v.sound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_CashModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CashModel)m[i + start];
            v.minimum = br.ReadSingle();
            v.maximum = br.ReadSingle();
            v.bonusMultiplier = br.ReadSingle();
            v.salvage = br.ReadSingle();
            v.noTransformCash = br.ReadBoolean();
            v.distributeSalvage = br.ReadBoolean();
        }
    }

    private void Set_v_CreateTextEffectModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTextEffectModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.useTowerPosition = br.ReadBoolean();
        }
    }

    private void Set_v_OffsetModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.OffsetModel)m[i + start];
            v.range = br.ReadSingle();
            v.angleOffset = br.ReadSingle();
        }
    }

    private void Set_v_RandomPositionBasicModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RandomPositionBasicModel)m[i + start];
            v.minRadius = br.ReadSingle();
            v.maxRadius = br.ReadSingle();
            v.mapBorder = br.ReadSingle();
            v.useTerrainHeight = br.ReadBoolean();
        }
    }

    private void Set_v_TargetSupplierSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.TargetSupplierSupportModel>();
        var mutatorField = t.GetField("mutator", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TargetSupplierSupportModel)m[i + start];
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.targetSupplier =
                (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel)m[br.ReadInt32()];
            mutatorField.SetValue(v,
                (Assets.Scripts.Models.Towers.Behaviors.TargetSupplierSupportModel.MutatorTower)m[br.ReadInt32()]);
        }
    }

    private void Set_v_MutatorTower_Fields(int start, int count)
    {
        Set_v_BehaviorMutator_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.TargetSupplierSupportModel.MutatorTower>();
        var supportModelField = t.GetField("supportModel", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TargetSupplierSupportModel.MutatorTower)m[i + start];
            supportModelField.SetValue(v,
                (Assets.Scripts.Models.Towers.Behaviors.TargetSupplierSupportModel)m[br.ReadInt32()]);
        }
    }

    private void Set_v_RateSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.RateSupportModel)m[i + start];
            v.multiplier = br.ReadSingle();
            v.isUnique = br.ReadBoolean();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.priority = br.ReadInt32();
        }
    }

    private void Set_v_LeakDangerAttackSpeedModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.LeakDangerAttackSpeedModel)m[i + start];
            v.maxRateIncreasePercent = br.ReadSingle();
        }
    }

    private void Set_v_TurboModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.TurboModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.TurboModel)m[i + start];
            v.lifespanFrames = br.ReadInt32();
            v.multiplier = br.ReadSingle();
            v.projectileDisplay = (Assets.Scripts.Models.Effects.AssetPathModel)m[br.ReadInt32()];
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.extraDamage = br.ReadInt32();
            v.projectileRadiusScaleBonus = br.ReadSingle();
            v.dontRemoveMutatorOnDestroy = br.ReadBoolean();
        }
    }

    private void Set_v_TargetMoabModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetMoabModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.findStrongest = br.ReadBoolean();
        }
    }

    private void Set_v_RandomRangeTravelStraightModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.RandomRangeTravelStraightModel)m[i + start];
            v.minRange = br.ReadSingle();
            v.maxRange = br.ReadSingle();
            v.speed = br.ReadSingle();
            v.speedFrames = br.ReadSingle();
        }
    }

    private void Set_v_CreateEffectOnExhaustFractionModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustFractionModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.fullscreen = br.ReadBoolean();
            v.fraction = br.ReadSingle();
            v.durationFraction = br.ReadSingle();
            v.randomRotation = br.ReadBoolean();
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_FilterWithTagsModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterWithTagsModel)m[i + start];
            v.tags = (Il2CppStringArray)m[br.ReadInt32()];
            v.inclusive = br.ReadBoolean();
        }
    }

    private void Set_v_DistributeToChildrenSetModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DistributeToChildrenSetModel)m[i + start];
            v.layers = br.ReadInt32();
        }
    }

    private void Set_v_CreateSoundOnProjectileCreatedModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.CreateSoundOnProjectileCreatedModel)m[i + start];
            v.sound1 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound2 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound3 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound4 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound5 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.type = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_AlternateProjectileModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.AlternateProjectileModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emissionModel = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.interval = br.ReadInt32();
            v.alternateAnimation = br.ReadInt32();
        }
    }

    private void Set_v_FilterBloonIfDamageTypeModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterBloonIfDamageTypeModel)m[i + start];
            v.ifCantHitBloonProperties = (BloonProperties)(br.ReadInt32());
            v.damageModel = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_RemoveMutatorOnUpgradeModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.RemoveMutatorOnUpgradeModel)m[i + start];
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.firstPath = br.ReadInt32();
            v.secondPath = br.ReadInt32();
            v.thirdPath = br.ReadInt32();
        }
    }

    private void Set_v_FollowPathModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.FollowPathModel>();
        var speedField = t.GetField("speed", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.FollowPathModel)m[i + start];
            v.path = (Il2CppStructArray<Assets.Scripts.Simulation.SMath.Vector3>)m[br.ReadInt32()];
            v.easePath = (Il2CppStructArray<Assets.Scripts.Simulation.SMath.Vector3>)m[br.ReadInt32()];
            speedField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.speedFrames = br.ReadSingle();
            v.destroyAtEndOfPath = br.ReadBoolean();
        }
    }

    private void Set_v_RotateModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.RotateModel)m[i + start];
            v.angle = br.ReadSingle();
            v.rotationFrames = br.ReadSingle();
        }
    }

    private void Set_v_FilterMoabModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterMoabModel)m[i + start];
            v.flip = br.ReadBoolean();
        }
    }

    private void Set_v_CollideOnlyWithTargetModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CollideOnlyWithTargetModel)m[i + start];
        }
    }

    private void Set_v_FlipFollowPathModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.FlipFollowPathModel)m[i + start];
            v.flipTowerDisplayX = br.ReadBoolean();
            v.flipTowerDisplayY = br.ReadBoolean();
            v.effectOnFlip = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_DamageUpModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.DamageUpModel)m[i + start];
            v.lifespanFrames = br.ReadInt32();
            v.additionalDamage = br.ReadInt32();
            v.projectileDisplay = (Assets.Scripts.Models.Effects.AssetPathModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_OrbitModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.OrbitModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.count = br.ReadInt32();
            v.range = br.ReadSingle();
        }
    }

    private void Set_v_DontDestroyOnContinueModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DontDestroyOnContinueModel)m[i + start];
        }
    }

    private void Set_v_ClearHitBloonsModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ClearHitBloonsModel)m[i + start];
            v.interval = br.ReadSingle();
            v.intervalFrames = br.ReadInt32();
        }
    }

    private void Set_v_PushBackModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.PushBackModel)m[i + start];
            v.pushAmount = br.ReadSingle();
            v.tag = br.ReadBoolean() ? null : br.ReadString();
            v.multiplierBFB = br.ReadSingle();
            v.multiplierDDT = br.ReadSingle();
            v.multiplierZOMG = br.ReadSingle();
        }
    }

    private void Set_v_ArriveAtTargetModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ArriveAtTargetModel)m[i + start];
            v.timeToTake = br.ReadSingle();
            v.curveSamples = (Il2CppStructArray<float>)m[br.ReadInt32()];
            v.filterCollisionWhileMoving = br.ReadBoolean();
            v.expireOnArrival = br.ReadBoolean();
            v.altSpeed = br.ReadSingle();
            v.stopOnTargetReached = br.ReadBoolean();
            v.curve = (Assets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve)m[br.ReadInt32()];
        }
    }

    private void Set_v_Curve_Fields(int start, int count)
    {
        var t = Il2CppType.Of<Assets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve>();
        var samplesField = t.GetField("samples", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve)m[i + start];
            v.samples = (Il2CppStructArray<float>)m[br.ReadInt32()];
        }
    }

    private void Set_v_FilterOutBloonModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterOutBloonModel)m[i + start];
            v.bloonId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_ProjectileBlockerCollisionReboundModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileBlockerCollisionReboundModel)m[i + start];
            v.clearCollidedWith = br.ReadBoolean();
        }
    }

    private void Set_v_ExpireProjectileAtScreenEdgeModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ExpireProjectileAtScreenEdgeModel)m[i + start];
        }
    }

    private void Set_v_MonkeyFanClubModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MonkeyFanClubModel)m[i + start];
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
            v.maxTier = br.ReadInt32();
            v.towerCount = br.ReadInt32();
            v.range = br.ReadSingle();
            v.reloadModifier = br.ReadSingle();
            v.immuneBloonProperties = (BloonProperties)(br.ReadInt32());
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.towerDisplay = br.ReadBoolean() ? null : br.ReadString();
            v.originDisplay = br.ReadBoolean() ? null : br.ReadString();
            v.towerOriginDisplay = br.ReadBoolean() ? null : br.ReadString();
            v.effectId = br.ReadBoolean() ? null : br.ReadString();
            v.effectLeaderId = br.ReadBoolean() ? null : br.ReadString();
            v.effectOnOtherId = br.ReadBoolean() ? null : br.ReadString();
            v.bonusPierce = br.ReadSingle();
            v.projectileRadius = br.ReadSingle();
            v.bonusDamage = br.ReadInt32();
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.handBlurEjectEffectModel =
                (Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectEffectModel)m[br.ReadInt32()];
            v.ejectX = br.ReadSingle();
            v.ejectY = br.ReadSingle();
            v.ejectZ = br.ReadSingle();
            v.otherDisplayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[br.ReadInt32()];
            v.displayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[br.ReadInt32()];
            v.leaderDisplayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[br.ReadInt32()];
            v.endDisplayModel = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectAfterTimeModel)m[br.ReadInt32()];
            v.ignoreWithMutators = br.ReadBoolean() ? null : br.ReadString();
            v.ignoreWithMutatorsList = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_CreateEffectAfterTimeModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectAfterTimeModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
            v.useRoundTime = br.ReadBoolean();
        }
    }

    private void Set_v_ShowTextOnHitModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ShowTextOnHitModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.useTowerPosition = br.ReadBoolean();
            v.text = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_CritMultiplierModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.CritMultiplierModel)m[i + start];
            v.damage = br.ReadSingle();
            v.lower = br.ReadInt32();
            v.upper = br.ReadInt32();
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.distributeToChildren = br.ReadBoolean();
        }
    }

    private void Set_v_LinkDisplayScaleToTowerRangeModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.LinkDisplayScaleToTowerRangeModel)m[i + start];
            v.displayPath = br.ReadBoolean() ? null : br.ReadString();
            v.baseTowerRange = br.ReadSingle();
            v.displayRadius = br.ReadSingle();
        }
    }

    private void Set_v_CreateEffectWhileAttackingModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.CreateEffectWhileAttackingModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.exitEffectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_SingleEmmisionTowardsTargetModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmmisionTowardsTargetModel)m[i + start];
            v.offset = br.ReadSingle();
        }
    }

    private void Set_v_SpinModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.SpinModel)m[i + start];
            v.rotationPerSecond = br.ReadSingle();
            v.rotationPerFrame = br.ReadSingle();
        }
    }

    private void Set_v_FilterOveridingMutatedTargetModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterOveridingMutatedTargetModel)m[i + start];
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.mutationOverideOrder = (Il2CppStringArray)m[br.ReadInt32()];
            v.highestPriorityMutationId = br.ReadBoolean() ? null : br.ReadString();
            v.defaultMutationId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_FilterFrozenBloonsModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterFrozenBloonsModel)m[i + start];
        }
    }

    private void Set_v_EmitOnPopModel_Fields(int start, int count)
    {
        Set_v_BloonBehaviorModelWithTowerTracking_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.Behaviors.EmitOnPopModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.pierceOverride = br.ReadSingle();
            v.ignoreSameFrameDegrade = br.ReadBoolean();
        }
    }

    private void Set_v_RemoveDamageTypeModifierModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.RemoveDamageTypeModifierModel)m[i + start];
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
            v.layers = br.ReadInt32();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_FreezeModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.FreezeModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.FreezeModel)m[i + start];
            v.speed = br.ReadSingle();
            v.layers = br.ReadInt32();
            v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>)m[br.ReadInt32()];
            v.overlayLayer = br.ReadInt32();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.percentChanceToFreeze = br.ReadSingle();
            v.enablePercentChanceToFreeze = br.ReadBoolean();
            v.damageModel = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel)m[br.ReadInt32()];
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFrames = br.ReadInt32();
            v.cascadeMutators = br.ReadBoolean();
            v.growBlockModel = (Assets.Scripts.Models.Bloons.Behaviors.GrowBlockModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_GrowBlockModel_Fields(int start, int count)
    {
        Set_v_BloonBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.Behaviors.GrowBlockModel)m[i + start];
        }
    }

    private void Set_v_FreezeNearbyWaterModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.FreezeNearbyWaterModel)m[i + start];
            v.radius = br.ReadSingle();
            v.areaHeightOffset = br.ReadSingle();
            v.freezeAsset = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_SlowBloonsZoneModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SlowBloonsZoneModel)m[i + start];
            v.zoneRadius = br.ReadSingle();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.isUnique = br.ReadBoolean();
            v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>)m[br.ReadInt32()];
            v.speedScale = br.ReadSingle();
            v.speedChange = br.ReadSingle();
            v.bindRadiusToTowerRange = br.ReadBoolean();
            v.radiusOffset = br.ReadSingle();
            v.bloonTag = br.ReadBoolean() ? null : br.ReadString();
            v.bloonTags = (Il2CppStringArray)m[br.ReadInt32()];
            v.inclusive = br.ReadBoolean();
        }
    }

    private void Set_v_AbilityBehaviorBuffModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityBehaviorBuffModel)m[i + start];
            v.showBuffIcon = br.ReadBoolean();
            v.isGlobal = br.ReadBoolean();
            v.buffLocsName = br.ReadBoolean() ? null : br.ReadString();
            v.buffIconName = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_ActivateRateSupportZoneModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ActivateRateSupportZoneModel)m[i + start];
            v.range = br.ReadSingle();
            v.rateModifier = br.ReadSingle();
            v.maxNumTowersModified = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.isUnique = br.ReadBoolean();
            v.canEffectThisTower = br.ReadBoolean();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadSingle();
            v.displayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[br.ReadInt32()];
            v.filters =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TowerFilters.TowerFilterModel>)m[br.ReadInt32()];
            v.useTowerRange = br.ReadBoolean();
        }
    }

    private void Set_v_FreezeModifierForTagsModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.FreezeModifierForTagsModel)m[i + start];
            v.tags = (Il2CppStringArray)m[br.ReadInt32()];
            v.freezeId = br.ReadBoolean() ? null : br.ReadString();
            v.freezeTimeMultiplier = br.ReadSingle();
            v.resetToUnmodified = br.ReadBoolean();
            v.preventMutation = br.ReadBoolean();
        }
    }

    private void Set_v_CarryProjectileModel_Fields(int start, int count)
    {
        Set_v_BloonBehaviorModelWithTowerTracking_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.Behaviors.CarryProjectileModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_FilterMutatedTargetModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterMutatedTargetModel)m[i + start];
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.mutationIds = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_FilterOnlyCamoInModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterOnlyCamoInModel)m[i + start];
        }
    }

    private void Set_v_CreateEffectOnExhaustedModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustedModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.fullscreen = br.ReadBoolean();
            v.randomRotation = br.ReadBoolean();
        }
    }

    private void Set_v_RefreshPierceModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.RefreshPierceModel)m[i + start];
            v.interval = br.ReadSingle();
            v.intervalFrames = br.ReadInt32();
        }
    }

    private void Set_v_DestroyProjectileIfTowerDestroyedModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DestroyProjectileIfTowerDestroyedModel)m[i + start];
        }
    }

    private void Set_v_ZeroRotationModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.ZeroRotationModel)m[i + start];
        }
    }

    private void Set_v_TargetTrackOrDefaultModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetTrackOrDefaultModel)m[i + start];
            v.radius = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
            v.useTowerRange = br.ReadBoolean();
            v.forceTargetTrack = br.ReadBoolean();
            v.useClosestTrack = br.ReadBoolean();
            v.maxTrackOffset = br.ReadSingle();
        }
    }

    private void Set_v_FilterOfftrackModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterOfftrackModel)m[i + start];
        }
    }

    private void Set_v_SyncTargetPriorityWithSubTowersModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SyncTargetPriorityWithSubTowersModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TowerCreateTowerModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TowerCreateTowerModel)m[i + start];
            v.towerModel = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.isAirBasedTower = br.ReadBoolean();
        }
    }

    private void Set_v_TowerExpireOnParentDestroyedModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TowerExpireOnParentDestroyedModel)m[i + start];
        }
    }

    private void Set_v_CreditPopsToParentTowerModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreditPopsToParentTowerModel)m[i + start];
        }
    }

    private void Set_v_IgnoreTowersBlockerModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.IgnoreTowersBlockerModel)m[i + start];
            v.filteredTowers = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_PathMovementFromScreenCenterModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.PathMovementFromScreenCenterModel)m[i + start];
            v.speed = br.ReadSingle();
            v.speedFrames = br.ReadSingle();
            v.ignoreTargetType = br.ReadBoolean();
        }
    }

    private void Set_v_Assets_Scripts_Models_Towers_Behaviors_CreateEffectOnExpireModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnExpireModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_SavedSubTowerModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SavedSubTowerModel)m[i + start];
        }
    }

    private void Set_v_EmissionRotationOffBloonDirectionModel_Fields(int start, int count)
    {
        Set_v_EmissionBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionRotationOffBloonDirectionModel)m[
                    i + start];
            v.useAirUnitPosition = br.ReadBoolean();
            v.dontSetAfterEmit = br.ReadBoolean();
        }
    }

    private void Set_v_PathMovementFromScreenCenterPatternModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PathMovementFromScreenCenterPatternModel)m[
                    i + start];
            v.radius = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_AbilityCreateTowerModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.AbilityCreateTowerModel)m[i + start];
            v.towerModel = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.isAirBasedTower = br.ReadBoolean();
        }
    }

    private void Set_v_TowerExpireModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.TowerExpireModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TowerExpireModel)m[i + start];
            v.expireOnRoundComplete = br.ReadBoolean();
            v.expireOnDefeatScreen = br.ReadBoolean();
            v.lifespanFrames = br.ReadInt32();
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
        }
    }

    private void Set_v_SwitchDisplayModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.SwitchDisplayModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.SwitchDisplayModel)m[i + start];
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFrames = br.ReadInt32();
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.excludeSubTowers = br.ReadBoolean();
            v.createEffectOnSwitchBackModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.resetOnDefeatScreen = br.ReadBoolean();
        }
    }

    private void Set_v_MutateRemoveAllAttacksOnAbilityActivateModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        var t = Il2CppType
            .Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.
                MutateRemoveAllAttacksOnAbilityActivateModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.
                    MutateRemoveAllAttacksOnAbilityActivateModel)m[i + start];
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_NecromancerZoneModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.NecromancerZoneModel)m[i + start];
            v.attackUsedForRangeModel = (Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_RotateToDefaultPositionTowerModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.RotateToDefaultPositionTowerModel)m[i + start];
            v.rotation = br.ReadSingle();
            v.onlyOnReachingTier = br.ReadInt32();
        }
    }

    private void Set_v_PrinceOfDarknessZombieBuffModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.PrinceOfDarknessZombieBuffModel)m[i + start];
            v.damageIncrease = br.ReadSingle();
            v.distanceMultiplier = br.ReadSingle();
        }
    }

    private void Set_v_NecromancerEmissionModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.NecromancerEmissionModel)m[i + start];
            v.minBloonsSpawnedPerWave = br.ReadInt32();
            v.maxBloonsSpawnedPerWave = br.ReadInt32();
            v.maxRbeSpawnedPerSecond = br.ReadInt32();
            v.maxPathRandomRange = br.ReadInt32();
            v.maxPiercePerBloon = br.ReadInt32();
            v.maxPathOffset = br.ReadInt32();
            v.maxRbeStored = br.ReadInt32();
            v.rateStackMax = br.ReadInt32();
            v.rateRbePerStack = br.ReadInt32();
            v.damageStackMax = br.ReadInt32();
            v.damageRbePerStack = br.ReadInt32();
            v.roundsBeforeDecay = br.ReadInt32();
            v.pierceMutators =
                (Dictionary<System.Int32, Assets.Scripts.Models.Towers.Behaviors.Emissions.NecromancerEmissionModel.
                    PierceMutator>)m[br.ReadInt32()];
            v.damageMutators =
                (Dictionary<System.Int32, Assets.Scripts.Models.Towers.Behaviors.Emissions.NecromancerEmissionModel.
                    DamageMutator>)m[br.ReadInt32()];
        }
    }

    private void Set_v_TravelAlongPathModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelAlongPathModel)m[i + start];
            v.range = br.ReadSingle();
            v.reverse = br.ReadBoolean();
            v.disableRotateWithPathDirection = br.ReadBoolean();
            v.lifespanFrames = br.ReadInt32();
            v.speedFrames = br.ReadSingle();
            v.rotationLerp = br.ReadSingle();
        }
    }

    private void Set_v_NecroEmissionFilterModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.NecroEmissionFilterModel)m[i + start];
            v.isPriceOfDakrnessEmission = br.ReadBoolean();
        }
    }

    private void Set_v_PrinceOfDarknessEmissionModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.PrinceOfDarknessEmissionModel)m[i + start];
            v.alternateProjectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.maxPathRandomRange = br.ReadInt32();
            v.minPiercePerBloon = br.ReadInt32();
            v.maxPathOffset = br.ReadInt32();
        }
    }

    private void Set_v_NecromancerTargetTrackWithinRangeModel_Fields(int start, int count)
    {
        Set_v_TargetTrackModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.NecromancerTargetTrackWithinRangeModel)m[
                    i + start];
        }
    }

    private void Set_v_ParallelEmissionModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.ParallelEmissionModel)m[i + start];
            v.spreadLength = br.ReadSingle();
            v.yOffset = br.ReadSingle();
            v.count = br.ReadInt32();
            v.linear = br.ReadBoolean();
            v.offsetStart = br.ReadSingle();
        }
    }

    private void Set_v_EmissionRotationOffDisplayModel_Fields(int start, int count)
    {
        Set_v_EmissionBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionRotationOffDisplayModel)m[
                    i + start];
            v.offsetRotation = br.ReadInt32();
        }
    }

    private void Set_v_FireAlternateWeaponModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.FireAlternateWeaponModel)m[i + start];
            v.weaponId = br.ReadInt32();
        }
    }

    private void Set_v_EmissionRotationOffDisplayOnEmitModel_Fields(int start, int count)
    {
        Set_v_EmissionBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionRotationOffDisplayOnEmitModel)m[
                    i + start];
            v.offsetRotation = br.ReadInt32();
        }
    }

    private void Set_v_FireWhenAlternateWeaponIsReadyModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.FireWhenAlternateWeaponIsReadyModel)m[i + start];
            v.weaponId = br.ReadInt32();
        }
    }

    private void Set_v_FilterTargetAngleFilterModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.FilterTargetAngleFilterModel)m[i + start];
            v.fieldOfView = br.ReadSingle();
            v.baseTowerRotationOffset = br.ReadSingle();
            v.shareFilterTargets = br.ReadBoolean();
            v.minTimeBetweenFilterTargetsFrames = br.ReadInt32();
        }
    }

    private void Set_v_EmissionArcRotationOffDisplayDirectionModel_Fields(int start, int count)
    {
        Set_v_EmissionBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionArcRotationOffDisplayDirectionModel)
                m[i + start];
            v.offsetRotation = br.ReadInt32();
        }
    }

    private void Set_v_CreateTowerModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTowerModel)m[i + start];
            v.tower = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.height = br.ReadSingle();
            v.positionAtTarget = br.ReadBoolean();
            v.destroySubTowersOnCreateNewTower = br.ReadBoolean();
            v.useProjectileRotation = br.ReadBoolean();
            v.useParentTargetPriority = br.ReadBoolean();
            v.carryMutatorsFromDestroyedTower = br.ReadBoolean();
        }
    }

    private void Set_v_TowerExpireOnParentUpgradedModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TowerExpireOnParentUpgradedModel)m[i + start];
        }
    }

    private void Set_v_FireFromAirUnitModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.FireFromAirUnitModel)m[i + start];
        }
    }

    private void Set_v_FighterPilotPatternFirstModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FighterPilotPatternFirstModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.offsetDistance = br.ReadSingle();
        }
    }

    private void Set_v_FighterPilotPatternCloseModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FighterPilotPatternCloseModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.offsetDistance = br.ReadSingle();
        }
    }

    private void Set_v_FighterPilotPatternLastModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FighterPilotPatternLastModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.offsetDistance = br.ReadSingle();
        }
    }

    private void Set_v_FighterPilotPatternStrongModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FighterPilotPatternStrongModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.offsetDistance = br.ReadSingle();
        }
    }

    private void Set_v_AccelerateModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AccelerateModel)m[i + start];
            v.acceleration = br.ReadSingle();
            v.accelerationFrames = br.ReadSingle();
            v.maxSpeed = br.ReadSingle();
            v.maxSpeedFrames = br.ReadSingle();
            v.turnRateChange = br.ReadSingle();
            v.turnRateChangeFrames = br.ReadSingle();
            v.maxTurnRate = br.ReadSingle();
            v.maxTurnRateFrames = br.ReadSingle();
            v.decelerate = br.ReadBoolean();
        }
    }

    private void Set_v_TargetStrongAirUnitModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongAirUnitModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_AirUnitModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.AirUnitModel)m[i + start];
            v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TowerBehaviorModel>)m[br.ReadInt32()];
            v.display = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_FighterMovementModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.FighterMovementModel)m[i + start];
            v.maxSpeed = br.ReadSingle();
            v.turningSpeed = br.ReadSingle();
            v.minDistanceToTargetBeforeFlyover = br.ReadSingle();
            v.distanceOfFlyover = br.ReadSingle();
            v.bankAngleMax = br.ReadSingle();
            v.bankSmoothness = br.ReadSingle();
            v.rollTotalTime = br.ReadSingle();
            v.rollRunUpDistance = br.ReadSingle();
            v.rollTimeBeforeNext = br.ReadSingle();
            v.rollChancePerSecondPassed = br.ReadSingle();
            v.loopTotalTime = br.ReadSingle();
            v.loopRunUpDistance = br.ReadSingle();
            v.loopTimeBeforeNext = br.ReadSingle();
            v.loopChancePerSecondPassed = br.ReadSingle();
            v.loopRadius = br.ReadSingle();
            v.loopModelScale = br.ReadSingle();
        }
    }

    private void Set_v_SubTowerFilterModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.SubTowerFilterModel)m[i + start];
            v.baseSubTowerId = br.ReadBoolean() ? null : br.ReadString();
            v.baseSubTowerIds = (Il2CppStringArray)m[br.ReadInt32()];
            v.maxNumberOfSubTowers = br.ReadSingle();
            v.checkForPreExisting = br.ReadBoolean();
        }
    }

    private void Set_v_FlagshipAttackSpeedIncreaseModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.FlagshipAttackSpeedIncreaseModel)m[i + start];
            v.attackSpeedIncrease = br.ReadSingle();
        }
    }

    private void Set_v_AddMakeshiftAreaModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.AddMakeshiftAreaModel)m[i + start];
            v.points = (Il2CppStructArray<Assets.Scripts.Simulation.SMath.Vector3>)m[br.ReadInt32()];
            v.newAreaType = (Assets.Scripts.Models.Map.AreaType)(br.ReadInt32());
            v.filterInTowerSizes = (Il2CppStringArray)m[br.ReadInt32()];
            v.filterInTowerSets = (Il2CppStringArray)m[br.ReadInt32()];
            v.filterOutSpecificTowers = (Il2CppStringArray)m[br.ReadInt32()];
            v.renderHeightOffset = br.ReadSingle();
            v.ignoreZAxisTowerCollision = br.ReadBoolean();
            v.destroyTowersOnAreaWhenSold = br.ReadBoolean();
        }
    }

    private void Set_v_EmissionRotationOffTowerDirectionModel_Fields(int start, int count)
    {
        Set_v_EmissionBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionRotationOffTowerDirectionModel)m[
                    i + start];
            v.offsetRotation = br.ReadInt32();
        }
    }

    private void Set_v_EmissionArcRotationOffTowerDirectionModel_Fields(int start, int count)
    {
        Set_v_EmissionBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionArcRotationOffTowerDirectionModel)m[
                    i + start];
            v.offsetRotation = br.ReadInt32();
        }
    }

    private void Set_v_GrappleEmissionModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.GrappleEmissionModel)m[i + start];
            v.numGrapples = br.ReadSingle();
        }
    }

    private void Set_v_MoabTakedownModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.MoabTakedownModel)m[i + start];
            v.speed = br.ReadSingle();
            v.speedFrames = br.ReadSingle();
            v.increaseMoabBloonWorth = br.ReadBoolean();
            v.multiplier = br.ReadSingle();
            v.additive = br.ReadSingle();
            v.increaseWorthTextEffectModel =
                (Assets.Scripts.Models.Bloons.Behaviors.IncreaseWorthTextEffectModel)m[br.ReadInt32()];
            v.bloonWorthMutator =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.MoabTakedownModel.BloonWorthMutator)m[
                    br.ReadInt32()];
        }
    }

    private void Set_v_IncreaseWorthTextEffectModel_Fields(int start, int count)
    {
        Set_v_BloonBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.Behaviors.IncreaseWorthTextEffectModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.displayFullPayout = br.ReadBoolean();
        }
    }

    private void Set_v_Assets_Scripts_Models_Towers_Projectiles_Behaviors_MoabTakedownModel_BloonWorthMutator_Fields(
        int start, int count)
    {
        Set_v_BehaviorMutator_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.MoabTakedownModel.BloonWorthMutator>();
        var multiplierField = t.GetField("multiplier", bindFlags);
        var additiveField = t.GetField("additive", bindFlags);
        var behaviorField = t.GetField("behavior", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.MoabTakedownModel.BloonWorthMutator)m[i + start];
            multiplierField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            additiveField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            behaviorField.SetValue(v,
                (Assets.Scripts.Models.Bloons.Behaviors.IncreaseWorthTextEffectModel)m[br.ReadInt32()]);
        }
    }

    private void Set_v_CreateRopeEffectModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateRopeEffectModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.endAssetId = br.ReadBoolean() ? null : br.ReadString();
            v.spriteSpacing = br.ReadSingle();
            v.spriteOffset = br.ReadSingle();
            v.spriteRadius = br.ReadSingle();
        }
    }

    private void Set_v_TravelTowardsEmitTowerModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelTowardsEmitTowerModel)m[i + start];
            v.lockRotation = br.ReadBoolean();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
            v.range = br.ReadSingle();
            v.speed = br.ReadSingle();
            v.speedFrames = br.ReadSingle();
            v.delayedActivation = br.ReadBoolean();
        }
    }

    private void Set_v_TargetGrapplableModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetGrapplableModel)m[i + start];
            v.canHitZomg = br.ReadBoolean();
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_PerRoundCashBonusTowerModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.PerRoundCashBonusTowerModel)m[i + start];
            v.cashPerRound = br.ReadSingle();
            v.cashRoundBonusMultiplier = br.ReadSingle();
            v.lifespan = br.ReadSingle();
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.distributeCash = br.ReadBoolean();
        }
    }

    private void Set_v_TradeEmpireBuffModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TradeEmpireBuffModel)m[i + start];
            v.cashPerRoundPerMechantship = br.ReadSingle();
            v.maxMerchantmanCapBonus = br.ReadInt32();
        }
    }

    private void Set_v_CashbackZoneModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CashbackZoneModel)m[i + start];
            v.cashbackZoneMultiplier = br.ReadSingle();
            v.cashbackMaxPercent = br.ReadSingle();
            v.groupName = br.ReadBoolean() ? null : br.ReadString();
            v.maxStacks = br.ReadInt32();
        }
    }

    private void Set_v_MerchantShipModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.MerchantShipModel)m[i + start];
        }
    }

    private void Set_v_FilterTargetAngleModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterTargetAngleModel)m[i + start];
            v.fieldOfView = br.ReadSingle();
            v.baseTowerRotationOffset = br.ReadSingle();
        }
    }

    private void Set_v_RectangleFootprintModel_Fields(int start, int count)
    {
        Set_v_FootprintModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.RectangleFootprintModel)m[i + start];
            v.xWidth = br.ReadSingle();
            v.yWidth = br.ReadSingle();
        }
    }

    private void Set_v_AttackAirUnitModel_Fields(int start, int count)
    {
        Set_v_AttackModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.AttackAirUnitModel)m[i + start];
            v.displayAirUnitModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_CirclePatternModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.CirclePatternModel)m[i + start];
            v.radius = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
            v.reverse = br.ReadBoolean();
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.displayCount = br.ReadInt32();
        }
    }

    private void Set_v_FigureEightPatternModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FigureEightPatternModel)m[i + start];
            v.radius = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
            v.rotated = br.ReadBoolean();
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.displayCount = br.ReadInt32();
        }
    }

    private void Set_v_EmissionOverTimeModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionOverTimeModel)m[i + start];
            v.count = br.ReadInt32();
            v.timeBetween = br.ReadSingle();
        }
    }

    private void Set_v_FallToGroundModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.FallToGroundModel)m[i + start];
            v.timeToTake = br.ReadSingle();
            v.expireOnContact = br.ReadBoolean();
            v.groundOffset = br.ReadSingle();
        }
    }

    private void Set_v_CheckAirUnitOverTrackModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.CheckAirUnitOverTrackModel)m[i + start];
            v.futureTime = br.ReadSingle();
            v.futureTimeFrames = br.ReadInt32();
        }
    }

    private void Set_v_TargetInFrontOfAirUnitModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetInFrontOfAirUnitModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_PathMovementModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.PathMovementModel)m[i + start];
            v.speed = br.ReadSingle();
            v.rotation = br.ReadSingle();
            v.bankRotation = br.ReadSingle();
            v.bankRotationMul = br.ReadSingle();
            v.ignoreTargetType = br.ReadBoolean();
            v.catchUpSpeed = br.ReadSingle();
            v.takeOffTime = br.ReadSingle();
            v.takeOffExponent = br.ReadSingle();
            v.takeOffAnimTime = br.ReadSingle();
            v.takeOffScale = br.ReadSingle();
            v.takeOffScaleExponent = br.ReadSingle();
            v.takeOffPitch = br.ReadSingle();
            v.takeOffPitchExponent = br.ReadSingle();
        }
    }

    private void Set_v_GroundZeroBombBuffModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.GroundZeroBombBuffModel)m[i + start];
            v.towerMutatorModel = (Assets.Scripts.Models.Towers.Mutators.TowerMutatorModel)m[br.ReadInt32()];
            v.damageIncrease = br.ReadInt32();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_CenterElipsePatternModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.CenterElipsePatternModel)m[i + start];
            v.widthRadius = br.ReadSingle();
            v.heightRadius = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
            v.reverse = br.ReadBoolean();
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.displayCount = br.ReadInt32();
            v.canSelectPoint = br.ReadBoolean();
            v.pointDisplay = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_TargetFirstAirUnitModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstAirUnitModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TargetCloseAirUnitModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseAirUnitModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TargetLastAirUnitModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastAirUnitModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_RandomTargetSpreadModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.RandomTargetSpreadModel)m[i + start];
            v.spread = br.ReadSingle();
            v.throwMarkerOffsets =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel>)m[
                    br.ReadInt32()];
        }
    }

    private void Set_v_ChipMapBasedObjectModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ChipMapBasedObjectModel)m[i + start];
            v.chipTag = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_DamageInRingRadiusModel_Fields(int start, int count)
    {
        Set_v_DamageModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageInRingRadiusModel)m[i + start];
            v.innerRingRadius = br.ReadSingle();
        }
    }

    private void Set_v_CreateSoundOnProjectileExhaustModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileExhaustModel)m[i + start];
            v.sound1 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound2 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound3 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound4 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound5 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_DamageModifierForBloonStateModel_Fields(int start, int count)
    {
        Set_v_DamageModifierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForBloonStateModel)m[i + start];
            v.bloonState = br.ReadBoolean() ? null : br.ReadString();
            v.bloonStates = (Il2CppStringArray)m[br.ReadInt32()];
            v.damageMultiplier = br.ReadSingle();
            v.damageAdditive = br.ReadSingle();
            v.mustIncludeAllStates = br.ReadBoolean();
            v.applyOverMaxDamage = br.ReadBoolean();
            v.mustBeModified = br.ReadBoolean();
        }
    }

    private void Set_v_CycleAnimationModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.CycleAnimationModel)m[i + start];
            v.minAnimationState = br.ReadInt32();
            v.maxAnimationState = br.ReadInt32();
            v.loopMode = br.ReadBoolean() ? null : br.ReadString();
            v.randomize = br.ReadBoolean();
        }
    }

    private void Set_v_EjectEffectWithOffsetsModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectEffectWithOffsetsModel)m[i + start];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.rotateToWeapon = br.ReadBoolean();
        }
    }

    private void Set_v_TargetSelectedPointModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSelectedPointModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.scale = br.ReadSingle();
            v.customName = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_ProjectileZeroRotationModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileZeroRotationModel)m[i + start];
        }
    }

    private void Set_v_WindModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.WindModel)m[i + start];
            v.distanceMin = br.ReadSingle();
            v.distanceMax = br.ReadSingle();
            v.chance = br.ReadSingle();
            v.affectMoab = br.ReadBoolean();
            v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>)m[br.ReadInt32()];
            v.overlayLayer = br.ReadInt32();
        }
    }

    private void Set_v_RandomAngleOffsetModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.RandomAngleOffsetModel)m[i + start];
            v.minOffset = br.ReadInt32();
            v.maxOffset = br.ReadInt32();
        }
    }

    private void Set_v_SelfStackingSupportCompoundingModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SelfStackingSupportCompoundingModel)m[i + start];
            v.maxStacks = br.ReadInt32();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.filters =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TowerFilters.TowerFilterModel>)m[br.ReadInt32()];
        }
    }

    private void Set_v_SupportShinobiTacticsModel_Fields(int start, int count)
    {
        Set_v_SelfStackingSupportCompoundingModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SupportShinobiTacticsModel)m[i + start];
            v.multiplier = br.ReadSingle();
        }
    }

    private void Set_v_DamagePercentOfMaxModel_Fields(int start, int count)
    {
        Set_v_DamageModifierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamagePercentOfMaxModel)m[i + start];
            v.percent = br.ReadSingle();
            v.tags = (Il2CppStringArray)m[br.ReadInt32()];
            v.damageBloonsOffscreenOnly = br.ReadBoolean();
        }
    }

    private void Set_v_SlowMinusAbilityDurationModel_Fields(int start, int count)
    {
        Set_v_SlowModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMinusAbilityDurationModel)m[i + start];
            v.abilityId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_TrackTargetWithinTimeModel_Fields(int start, int count)
    {
        Set_v_TrackTargetModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetWithinTimeModel)m[i + start];
            v.timeInFrames = br.ReadSingle();
        }
    }

    private void Set_v_TargetTrackModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetTrackModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.maxOffset = br.ReadSingle();
            v.onlyTargetPathsWithBloons = br.ReadBoolean();
        }
    }

    private void Set_v_ProjectileOverTimeModel_Fields(int start, int count)
    {
        Set_v_BloonBehaviorModelWithTowerTracking_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Bloons.Behaviors.ProjectileOverTimeModel>();
        var intervalField = t.GetField("interval", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.Behaviors.ProjectileOverTimeModel)m[i + start];
            v.projectileModel = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emissionModel = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.intervalFrames = br.ReadInt32();
            intervalField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.displayPath = br.ReadBoolean() ? null : br.ReadString();
            v.displayLifetime = br.ReadSingle();
            v.triggerImmediate = br.ReadBoolean();
            v.rotateEffectWithBloon = br.ReadBoolean();
            v.initialDelay = br.ReadSingle();
            v.initialDelayFrames = br.ReadInt32();
            v.emitOnDestroy = br.ReadBoolean();
            v.collideWithSelf = br.ReadBoolean();
        }
    }

    private void Set_v_SetTriggerOnAirUnitFireModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.SetTriggerOnAirUnitFireModel)m[i + start];
            v.triggerState = br.ReadInt32();
        }
    }

    private void Set_v_AnimateAirUnitOnFireModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.AnimateAirUnitOnFireModel)m[i + start];
            v.animationState = br.ReadInt32();
        }
    }

    private void Set_v_ResetRateOnInitialiseModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.ResetRateOnInitialiseModel)m[i + start];
            v.weaponModel = (Assets.Scripts.Models.Towers.Weapons.WeaponModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_RandomEmissionModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.RandomEmissionModel)m[i + start];
            v.angle = br.ReadSingle();
            v.count = br.ReadInt32();
            v.startOffset = br.ReadSingle();
            v.useSpeedMultiplier = br.ReadBoolean();
            v.speedMultiplierMin = br.ReadSingle();
            v.speedMultiplierMax = br.ReadSingle();
            v.ejectPointRandomness = br.ReadSingle();
            v.useMainAttackRotation = br.ReadBoolean();
        }
    }

    private void Set_v_FollowTouchSettingModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FollowTouchSettingModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_LockInPlaceSettingModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.LockInPlaceSettingModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.display = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_PatrolPointsSettingModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PatrolPointsSettingModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.pointSwitchDistance = br.ReadSingle();
            v.pointSwitchDistanceSquared = br.ReadSingle();
            v.minimumPointDistance = br.ReadSingle();
            v.minimumPointDistanceSquared = br.ReadSingle();
            v.dotSpacing = br.ReadSingle();
            v.dotOffset = br.ReadSingle();
            v.display = br.ReadBoolean() ? null : br.ReadString();
            v.lineDisplay = br.ReadBoolean() ? null : br.ReadString();
            v.lineDelayFrames = br.ReadInt32();
            v.lineDelay = br.ReadSingle();
        }
    }

    private void Set_v_RotateToTargetAirUnitModel_Fields(int start, int count)
    {
        Set_v_RotateToTargetModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetAirUnitModel)m[i + start];
        }
    }

    private void Set_v_PursuitSettingModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PursuitSettingModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.pursuitDistance = br.ReadSingle();
        }
    }

    private void Set_v_PrioritiseRotationModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PrioritiseRotationModel)m[i + start];
        }
    }

    private void Set_v_HeliMovementModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.HeliMovementModel)m[i + start];
            v.maxSpeed = br.ReadSingle();
            v.rotationSpeed = br.ReadSingle();
            v.strafeDistance = br.ReadSingle();
            v.strafeDistanceSquared = br.ReadSingle();
            v.otherHeliRepulsionRange = br.ReadSingle();
            v.otherHeliRepulsionRangeSquared = br.ReadSingle();
            v.movementForceStart = br.ReadSingle();
            v.movementForceEnd = br.ReadSingle();
            v.movementForceEndSquared = br.ReadSingle();
            v.brakeForce = br.ReadSingle();
            v.otherHeliRepulsonForce = br.ReadSingle();
            v.slowdownRadiusMax = br.ReadSingle();
            v.slowdownRadiusMaxSquared = br.ReadSingle();
            v.slowdownRadiusMin = br.ReadSingle();
            v.slowdownRadiusMinSquared = br.ReadSingle();
            v.minVelocityCapScale = br.ReadSingle();
            v.destinationYOffset = br.ReadSingle();
            v.tiltAngle = br.ReadSingle();
        }
    }

    private void Set_v_CreateEffectOnAirUnitModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnAirUnitModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.rotation = br.ReadSingle();
            v.scale = br.ReadSingle();
        }
    }

    private void Set_v_RedeployModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.RedeployModel)m[i + start];
            v.selectionObjectPath = br.ReadBoolean() ? null : br.ReadString();
            v.isSelectableGameObject = br.ReadBoolean() ? null : br.ReadString();
            v.activateSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.pickupSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.dropOffSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.isDoorGunnerActive = br.ReadBoolean();
        }
    }

    private void Set_v_KeepInBoundsModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.KeepInBoundsModel)m[i + start];
        }
    }

    private void Set_v_AngleToMapCenterModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.AngleToMapCenterModel)m[i + start];
            v.range = br.ReadSingle();
            v.offset = br.ReadSingle();
        }
    }

    private void Set_v_LivesModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.LivesModel)m[i + start];
            v.minimum = br.ReadSingle();
            v.maximum = br.ReadSingle();
            v.salvage = br.ReadSingle();
        }
    }

    private void Set_v_FindDeploymentLocationModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FindDeploymentLocationModel)m[i + start];
            v.searchRadius = br.ReadSingle();
            v.pointDistance = br.ReadSingle();
            v.towerModel = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_UsePresetTargetModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.UsePresetTargetModel)m[i + start];
        }
    }

    private void Set_v_ComancheDefenceModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.ComancheDefenceModel)m[i + start];
            v.towerModel = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.reinforcementCount = br.ReadInt32();
            v.durationFrames = br.ReadInt32();
            v.cooldownFrames = br.ReadInt32();
            v.maxActivationsPerRound = br.ReadInt32();
            v.immediate = br.ReadBoolean();
            v.sound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_MoabShoveZoneModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.MoabShoveZoneModel)m[i + start];
            v.range = br.ReadSingle();
            v.moabPushSpeedScaleCap = br.ReadSingle();
            v.bfbPushSpeedScaleCap = br.ReadSingle();
            v.zomgPushSpeedScaleCap = br.ReadSingle();
            v.filterInvisibleModel = (Assets.Scripts.Models.Towers.Filters.FilterInvisibleModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_TowerRadiusModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TowerRadiusModel)m[i + start];
        }
    }

    private void Set_v_CreateSoundOnAttachedModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnAttachedModel)m[i + start];
            v.sound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.altSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_RandomArcEmissionModel_Fields(int start, int count)
    {
        Set_v_ArcEmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.RandomArcEmissionModel)m[i + start];
            v.randomAngle = br.ReadSingle();
            v.startOffset = br.ReadSingle();
        }
    }

    private void Set_v_CheckTempleCanFireModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.CheckTempleCanFireModel)m[i + start];
        }
    }

    private void Set_v_MonkeyTempleModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.MonkeyTempleModel>();
        var tcbooMutatorField = t.GetField("tcbooMutator", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.MonkeyTempleModel)m[i + start];
            v.towerGroupCount = br.ReadInt32();
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.towerEffectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.heroEffectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.darkTransformSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.darkAltTransformSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.animation = br.ReadInt32();
            v.upgradeAnimation = br.ReadInt32();
            v.weaponDelay = br.ReadSingle();
            v.weaponDelayFrames = br.ReadInt32();
            v.templeId = br.ReadBoolean() ? null : br.ReadString();
            v.checkForThereCanOnlyBeOne = br.ReadBoolean();
            v.transformationEffect = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.theAntiBloonSacrificeEffect = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.legendOfTheNightSacrificeEffect = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.transformationAnimation = br.ReadInt32();
            v.transformationWeaponDelay = br.ReadSingle();
            v.heroOverlapYAdjustment = br.ReadSingle();
            tcbooMutatorField.SetValue(v,
                (Assets.Scripts.Models.Towers.Behaviors.MonkeyTempleModel.TCBOOMutator)m[br.ReadInt32()]);
        }
    }

    private void Set_v_TCBOOMutator_Fields(int start, int count)
    {
        Set_v_BehaviorMutator_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.MonkeyTempleModel.TCBOOMutator)m[i + start];
        }
    }

    private void Set_v_TempleTowerMutatorGroupModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorGroupModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TempleTowerMutatorGroupModel)m[i + start];
            v.cost = br.ReadInt32();
            v.towerSet = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_TowerMutatorGroupModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TowerMutatorGroupModel)m[i + start];
            v.mutators =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Mutators.TowerMutatorModel>)m[br.ReadInt32()];
        }
    }

    private void Set_v_TempleTowerMutatorGroupTierTwoModel_Fields(int start, int count)
    {
        Set_v_TempleTowerMutatorGroupModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TempleTowerMutatorGroupTierTwoModel)m[i + start];
        }
    }

    private void Set_v_TowerMutatorModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.TowerMutatorModel)m[i + start];
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.conditionalId = (Assets.Scripts.Models.Towers.Mutators.Conditions.ConditionalModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_PierceTowerMutatorModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.PierceTowerMutatorModel)m[i + start];
            v.pierce = br.ReadInt32();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_DamageTowerMutatorModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.DamageTowerMutatorModel)m[i + start];
            v.damage = br.ReadSingle();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_AddAttackTowerMutatorModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.AddAttackTowerMutatorModel)m[i + start];
            v.attackModel = (Assets.Scripts.Models.Towers.TowerBehaviorModel)m[br.ReadInt32()];
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_UseTowerRangeModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.UseTowerRangeModel)m[i + start];
        }
    }

    private void Set_v_ConditionalModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.Conditions.ConditionalModel)m[i + start];
        }
    }

    private void Set_v_CheckTempleUnderLevelModel_Fields(int start, int count)
    {
        Set_v_ConditionalModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.Conditions.Behaviors.CheckTempleUnderLevelModel)m[i + start];
            v.cost = br.ReadInt32();
            v.towerSet = br.ReadBoolean() ? null : br.ReadString();
            v.templeType = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_ReloadTimeTowerMutatorModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.ReloadTimeTowerMutatorModel)m[i + start];
            v.multiplier = br.ReadSingle();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_UseParentEjectModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.UseParentEjectModel)m[i + start];
        }
    }

    private void Set_v_ProjectileSizeTowerMutatorModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.ProjectileSizeTowerMutatorModel)m[i + start];
            v.sizeModifier = br.ReadSingle();
            v.assetSizeModifier = br.ReadSingle();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_ProjectileSpeedTowerMutatorModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.ProjectileSpeedTowerMutatorModel)m[i + start];
            v.speedModifier = br.ReadSingle();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_WindChanceTowerMutatorModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.WindChanceTowerMutatorModel)m[i + start];
            v.windChance = br.ReadSingle();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_RandomPositionModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RandomPositionModel)m[i + start];
            v.minDistance = br.ReadSingle();
            v.maxDistance = br.ReadSingle();
            v.targetRadius = br.ReadSingle();
            v.targetRadiusSquared = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
            v.pointDistance = br.ReadSingle();
            v.dontUseTowerPosition = br.ReadBoolean();
            v.areaType = br.ReadBoolean() ? null : br.ReadString();
            v.useInverted = br.ReadBoolean();
            v.ignoreTerrain = br.ReadBoolean();
            v.idealDistanceWithinTrack = br.ReadSingle();
        }
    }

    private void Set_v_RangeTowerMutatorModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.RangeTowerMutatorModel)m[i + start];
            v.rangeIncrease = br.ReadSingle();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_AddBehaviorToTowerMutatorModel_Fields(int start, int count)
    {
        Set_v_TowerMutatorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Mutators.AddBehaviorToTowerMutatorModel)m[i + start];
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
            v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TowerBehaviorModel>)m[br.ReadInt32()];
        }
    }

    private void Set_v_DiscountZoneModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.DiscountZoneModel)m[i + start];
            v.discountMultiplier = br.ReadSingle();
            v.stackLimit = br.ReadInt32();
            v.stackName = br.ReadBoolean() ? null : br.ReadString();
            v.groupName = br.ReadBoolean() ? null : br.ReadString();
            v.affectSelf = br.ReadBoolean();
            v.tierCap = br.ReadInt32();
        }
    }

    private void Set_v_BonusCashZoneModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.BonusCashZoneModel)m[i + start];
            v.multiplier = br.ReadSingle();
            v.stackName = br.ReadBoolean() ? null : br.ReadString();
            v.groupName = br.ReadBoolean() ? null : br.ReadString();
            v.stackLimit = br.ReadInt32();
        }
    }

    private void Set_v_PierceSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.PierceSupportModel)m[i + start];
            v.pierce = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.isUnique = br.ReadBoolean();
        }
    }

    private void Set_v_RangeSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.RangeSupportModel)m[i + start];
            v.multiplier = br.ReadSingle();
            v.additive = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.isUnique = br.ReadBoolean();
        }
    }

    private void Set_v_DamageSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.DamageSupportModel)m[i + start];
            v.increase = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.isUnique = br.ReadBoolean();
        }
    }

    private void Set_v_UseAttackRotationModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.UseAttackRotationModel)m[i + start];
        }
    }

    private void Set_v_RotateToMiddleOfTargetsModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToMiddleOfTargetsModel)m[i + start];
            v.onlyRotateDuringThrow = br.ReadBoolean();
        }
    }

    private void Set_v_TargetRightHandModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetRightHandModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_TargetLeftHandModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLeftHandModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_DistributeToChildrenBloonModifierModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DistributeToChildrenBloonModifierModel)m[i + start];
            v.bloonTag = br.ReadBoolean() ? null : br.ReadString();
            v.bloonTags = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_KnockbackModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.KnockbackModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.KnockbackModel)m[i + start];
            v.moabMultiplier = br.ReadSingle();
            v.heavyMultiplier = br.ReadSingle();
            v.lightMultiplier = br.ReadSingle();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespanFrames = br.ReadInt32();
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
        }
    }

    private void Set_v_OverrideCamoDetectionModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.OverrideCamoDetectionModel)m[i + start];
            v.detectCamo = br.ReadBoolean();
        }
    }

    private void Set_v_ImmunityModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ImmunityModel)m[i + start];
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_DarkshiftModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.DarkshiftModel)m[i + start];
            v.restrictToTowerRadius = br.ReadBoolean();
            v.placementZoneAssetRadius = br.ReadSingle();
            v.placementZoneAsset = br.ReadBoolean() ? null : br.ReadString();
            v.darkshiftSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.disappearEffectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.reappearEffectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_TempleTowerMutatorGroupTierOneModel_Fields(int start, int count)
    {
        Set_v_TempleTowerMutatorGroupModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.TempleTowerMutatorGroupTierOneModel)m[i + start];
        }
    }

    private void Set_v_DartlingMaintainLastPosModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.DartlingMaintainLastPosModel)m[i + start];
        }
    }

    private void Set_v_LineProjectileEmissionModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.LineProjectileEmissionModel)m[i + start];
            v.useTargetAsEndPoint = br.ReadBoolean();
            v.displayPath = (Assets.Scripts.Models.Effects.AssetPathModel)m[br.ReadInt32()];
            v.displayLength = br.ReadSingle();
            v.displayLifetime = br.ReadSingle();
            v.ignoreBlockers = br.ReadBoolean();
            v.effectAtEndModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.effectAtEndRate = br.ReadSingle();
            v.effectAtEndRateFrames = br.ReadInt32();
            v.dontUseTowerPosition = br.ReadBoolean();
            v.useTowerRotation = br.ReadBoolean();
            v.useLengthSpeed = br.ReadBoolean();
            v.lengthSpeed = br.ReadSingle();
            v.lengthPerFrame = br.ReadSingle();
            v.projectileAtEndModel = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emissionAtEndModel = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.endProjectileSharesPierce = br.ReadBoolean();
        }
    }

    private void Set_v_LineEffectModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.LineEffectModel)m[i + start];
            v.lineDisplayPath = (Assets.Scripts.Models.Effects.AssetPathModel)m[br.ReadInt32()];
            v.lineDisplayLength = br.ReadSingle();
            v.effectAtEnd = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.effectAtStart = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.isLineDisplayEndless = br.ReadBoolean();
            v.useDisplayLengthSpeed = br.ReadBoolean();
            v.displayLengthSpeed = br.ReadSingle();
            v.displayLengthPerFrame = br.ReadSingle();
            v.useWeaponEjectForDisplay = br.ReadBoolean();
            v.useRotateToPointer = br.ReadBoolean();
            v.ignoreBlockers = br.ReadBoolean();
            v.useLineProjectileEmissionShowEffect = br.ReadBoolean();
        }
    }

    private void Set_v_TargetPointerModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetPointerModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.customName = br.ReadBoolean() ? null : br.ReadString();
            v.setOnAttached = br.ReadBoolean();
        }
    }

    private void Set_v_RotateToPointerModel_Fields(int start, int count)
    {
        Set_v_AttackBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToPointerModel)m[i + start];
            v.rate = br.ReadSingle();
            v.rateFrames = br.ReadSingle();
            v.rotateTower = br.ReadBoolean();
            v.weaponEjectZ = br.ReadSingle();
        }
    }

    private void Set_v_CollideExtraPierceReductionModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CollideExtraPierceReductionModel)m[i + start];
            v.bloonTag = br.ReadBoolean() ? null : br.ReadString();
            v.extraAmount = br.ReadInt32();
            v.destroyProjectileIfPierceNotEnough = br.ReadBoolean();
        }
    }

    private void Set_v_CreateProjectileOnExhaustPierceModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExhaustPierceModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.pierceInterval = br.ReadSingle();
            v.count = br.ReadInt32();
            v.minimumTimeDifferenceInFrames = br.ReadInt32();
            v.destroyProjectile = br.ReadBoolean();
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.displayLifetime = br.ReadSingle();
            v.displayFullscreen = br.ReadBoolean();
        }
    }

    private void Set_v_CreateProjectileOnBlockerCollideModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnBlockerCollideModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.displayLifetime = br.ReadSingle();
        }
    }

    private void Set_v_TravelCurvyModel_Fields(int start, int count)
    {
        Set_v_TravelStraitModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelCurvyModel)m[i + start];
            v.turnRate = br.ReadSingle();
            v.maxTurnAngle = br.ReadSingle();
            v.turnRatePerFrame = br.ReadSingle();
        }
    }

    private void Set_v_TravelStraightSlowdownModel_Fields(int start, int count)
    {
        Set_v_TravelStraitModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraightSlowdownModel)m[i + start];
            v.slowdownSpeed = br.ReadSingle();
            v.slowdownSpeedFrames = br.ReadSingle();
            v.minSpeed = br.ReadSingle();
            v.minSpeedFrames = br.ReadSingle();
            v.maxDistance = br.ReadSingle();
            v.canReducePierce = br.ReadBoolean();
            v.startingPierce = br.ReadInt32();
            v.endPierce = br.ReadInt32();
        }
    }

    private void Set_v_EjectAnimationModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectAnimationModel)m[i + start];
            v.animationState = br.ReadInt32();
        }
    }

    private void Set_v_TargetIndependantModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetIndependantModel)m[i + start];
            v.isSelectable = br.ReadBoolean();
            v.targetProvider =
                (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_RateBasedAnimationOffsetModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.RateBasedAnimationOffsetModel)m[i + start];
            v.baseRate = br.ReadSingle();
            v.offset = br.ReadSingle();
        }
    }

    private void Set_v_CanBuffIndicatorModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CanBuffIndicatorModel)m[i + start];
            v.isDisabled = br.ReadBoolean();
        }
    }

    private void Set_v_LoadAlchemistBrewInfoModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.LoadAlchemistBrewInfoModel)m[i + start];
            v.addBerserkerBrewToProjectileModel =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddBerserkerBrewToProjectileModel)m[br.ReadInt32()];
            v.addAcidicMixtureToProjectileModel =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddAcidicMixtureToProjectileModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_AddBerserkerBrewToProjectileModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddBerserkerBrewToProjectileModel)m[i + start];
            v.cap = br.ReadInt32();
            v.ignoreList = br.ReadBoolean() ? null : br.ReadString();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
            v.damageUp = br.ReadSingle();
            v.pierceUp = br.ReadSingle();
            v.rateUp = br.ReadSingle();
            v.rangeUp = br.ReadSingle();
            v.rebuffBlockTime = br.ReadSingle();
            v.rebuffBlockTimeFrames = br.ReadInt32();
            v.weapBehaviors =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>)m[br.ReadInt32()];
            v.towerBehaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TowerBehaviorModel>)m[br.ReadInt32()];
            v.ignoreMutationsByOrder = (Il2CppStringArray)m[br.ReadInt32()];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.buffLocsName = br.ReadBoolean() ? null : br.ReadString();
            v.buffIconName = br.ReadBoolean() ? null : br.ReadString();
            v.mutatorsToRemove = br.ReadBoolean() ? null : br.ReadString();
            v.mutatorsToRemoveList = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_BerserkerBrewModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.BerserkerBrewModel)m[i + start];
        }
    }

    private void Set_v_BerserkerBrewCheckModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.BerserkerBrewCheckModel)m[i + start];
            v.maxCount = br.ReadInt32();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_AddAcidicMixtureToProjectileModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddAcidicMixtureToProjectileModel)m[i + start];
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.cap = br.ReadInt32();
            v.towerBehaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TowerBehaviorModel>)m[br.ReadInt32()];
            v.weapBehaviors =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>)m[br.ReadInt32()];
            v.projBehaviors =
                (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Projectiles.ProjectileBehaviorModel>)m[
                    br.ReadInt32()];
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.ignoreList = br.ReadBoolean() ? null : br.ReadString();
            v.buffLocsName = br.ReadBoolean() ? null : br.ReadString();
            v.buffIconName = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_AcidicMixtureCheckModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.AcidicMixtureCheckModel)m[i + start];
            v.maxCount = br.ReadInt32();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_AcidicMixtureModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.AcidicMixtureModel)m[i + start];
        }
    }

    private void Set_v_RemovePermaBrewModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.RemovePermaBrewModel)m[i + start];
        }
    }

    private void Set_v_AcidPoolModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AcidPoolModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        var lifespanIfMissesField = t.GetField("lifespanIfMisses", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AcidPoolModel)m[i + start];
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFrames = br.ReadInt32();
            lifespanIfMissesField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFramesIfMisses = br.ReadInt32();
            v.radiusIfMisses = br.ReadSingle();
            v.pierce = br.ReadSingle();
        }
    }

    private void Set_v_ScaleProjectileModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ScaleProjectileModel)m[i + start];
            v.samples = (Il2CppStructArray<float>)m[br.ReadInt32()];
            v.curve = (Assets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve)m[br.ReadInt32()];
        }
    }

    private void Set_v_TargetFriendlyModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFriendlyModel)m[i + start];
            v.ignoreList = br.ReadBoolean() ? null : br.ReadString();
            v.isSelectable = br.ReadBoolean();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.mustHaveWeapon = br.ReadBoolean();
        }
    }

    private void Set_v_BrewTargettingModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.BrewTargettingModel)m[i + start];
            v.towerIgnoreList = (Il2CppStringArray)m[br.ReadInt32()];
            v.ignoreMutationsByOrder = (Il2CppStringArray)m[br.ReadInt32()];
            v.isSelectable = br.ReadBoolean();
        }
    }

    private void Set_v_UnstableConcoctionSplashModel_Fields(int start, int count)
    {
        Set_v_EmitOnPopModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Bloons.Behaviors.UnstableConcoctionSplashModel)m[i + start];
            v.baseIdToBloonDmg = (Dictionary<System.String, System.Single>)m[br.ReadInt32()];
            v.defaultBloonDmg = br.ReadSingle();
            v.baseIdToMoabDmg = (Dictionary<System.String, System.Single>)m[br.ReadInt32()];
            v.bossToMoabDmg = br.ReadSingle();
            v.defaultMoabDmg = br.ReadSingle();
        }
    }

    private void Set_v_DamageModifierUnstableConcoctionModel_Fields(int start, int count)
    {
        Set_v_DamageModifierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierUnstableConcoctionModel)m[i + start];
        }
    }

    private void Set_v_CreateEffectOnAbilityEndModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateEffectOnAbilityEndModel)m[i + start];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.lifespan = br.ReadSingle();
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_MorphTowerModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MorphTowerModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MorphTowerModel)m[i + start];
            v.isUnique = br.ReadBoolean();
            v.priority = br.ReadInt32();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.mutateAll = br.ReadBoolean();
            v.mutateSelf = br.ReadBoolean();
            v.towerModel = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.secondaryTowerModel = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.effectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.effectOnTransitionBackModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.maxTier = br.ReadInt32();
            v.maxCost = br.ReadSingle();
            v.maxTowers = br.ReadInt32();
            v.affectList = br.ReadBoolean() ? null : br.ReadString();
            v.resetOnDefeatScreen = br.ReadBoolean();
            v.ignoreWithMutators = br.ReadBoolean() ? null : br.ReadString();
            v.ignoreWithMutatorsList = (Il2CppStringArray)m[br.ReadInt32()];
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
            v.lifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_IncreaseRangeModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.IncreaseRangeModel)m[i + start];
            v.lifespanFrames = br.ReadInt32();
            v.multiplier = br.ReadSingle();
            v.addative = br.ReadSingle();
            v.endOnDefeatScreen = br.ReadBoolean();
        }
    }

    private void Set_v_TargetTrackOrDefaultAcidPoolModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetTrackOrDefaultAcidPoolModel)m[i + start];
            v.radius = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
            v.useTowerRange = br.ReadBoolean();
            v.isActive = br.ReadBoolean();
        }
    }

    private void Set_v_IncreaseBloonWorthModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.IncreaseBloonWorthModel)m[i + start];
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.cash = br.ReadSingle();
            v.cashMultiplier = br.ReadSingle();
            v.filter = (Assets.Scripts.Models.Towers.Filters.FilterModel)m[br.ReadInt32()];
            v.charges = br.ReadInt32();
            v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>)m[br.ReadInt32()];
            v.overlayLayer = br.ReadInt32();
        }
    }

    private void Set_v_MorphBloonModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.MorphBloonModel)m[i + start];
            v.bloonId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_EndOfRoundClearBypassModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.EndOfRoundClearBypassModel)m[i + start];
            v.gameModes = br.ReadBoolean() ? null : br.ReadString();
            v.gameModesList = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_SetSpriteFromPierceModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SetSpriteFromPierceModel)m[i + start];
            v.sprites = (Il2CppStringArray)m[br.ReadInt32()];
            v.loopMode = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_FadeProjectileModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.FadeProjectileModel)m[i + start];
            v.startFadingAt = br.ReadSingle();
            v.startFadingAtFrames = br.ReadInt32();
        }
    }

    private void Set_v_HeightOffsetProjectileModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.HeightOffsetProjectileModel)m[i + start];
            v.samples = (Il2CppStructArray<float>)m[br.ReadInt32()];
            v.curve = (Assets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve)m[br.ReadInt32()];
        }
    }

    private void Set_v_RandomRotationWeaponBehaviorModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.RandomRotationWeaponBehaviorModel)m[i + start];
        }
    }

    private void Set_v_ActivateAbilityAfterIntervalModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.ActivateAbilityAfterIntervalModel)m[i + start];
            v.abilityModel = (Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel)m[br.ReadInt32()];
            v.interval = br.ReadSingle();
            v.intervalFrames = br.ReadInt32();
        }
    }

    private void Set_v_AgeRandomModel_Fields(int start, int count)
    {
        Set_v_AgeModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AgeRandomModel)m[i + start];
            v.minLifespan = br.ReadSingle();
            v.maxLifespan = br.ReadSingle();
            v.minLifespanFrames = br.ReadInt32();
            v.maxLifespanFrames = br.ReadInt32();
        }
    }

    private void Set_v_StartOfRoundRateBuffModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.StartOfRoundRateBuffModel)m[i + start];
            v.modifier = br.ReadSingle();
            v.duration = br.ReadSingle();
            v.durationFrames = br.ReadInt32();
            v.mutator = (Assets.Scripts.Models.Towers.Behaviors.StartOfRoundRateBuffModel.RateMutator)m[br.ReadInt32()];
        }
    }

    private void Set_v_RateMutator_Fields(int start, int count)
    {
        Set_v_BehaviorMutator_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.StartOfRoundRateBuffModel.RateMutator>();
        var startOfRoundRateBuffModelField = t.GetField("startOfRoundRateBuffModel", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.StartOfRoundRateBuffModel.RateMutator)m[i + start];
            startOfRoundRateBuffModelField.SetValue(v,
                (Assets.Scripts.Models.Towers.Behaviors.StartOfRoundRateBuffModel)m[br.ReadInt32()]);
        }
    }

    private void Set_v_CloseTargetTrackModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.CloseTargetTrackModel)m[i + start];
            v.radius = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
            v.maxOffset = br.ReadSingle();
        }
    }

    private void Set_v_FarTargetTrackModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FarTargetTrackModel)m[i + start];
            v.radius = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
            v.maxOffset = br.ReadSingle();
            v.donutRadius = br.ReadSingle();
        }
    }

    private void Set_v_SmartTargetTrackModel_Fields(int start, int count)
    {
        Set_v_TargetSupplierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.SmartTargetTrackModel)m[i + start];
            v.radius = br.ReadSingle();
            v.isSelectable = br.ReadBoolean();
            v.maxOffset = br.ReadSingle();
        }
    }

    private void Set_v_CreateProjectileOnIntervalModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnIntervalModel)m[i + start];
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.intervalFrames = br.ReadInt32();
            v.onlyIfHasTarget = br.ReadBoolean();
            v.range = br.ReadSingle();
            v.targetType.id = br.ReadString();
            v.targetType.actionOnCreate = br.ReadBoolean();
        }
    }

    private void Set_v_CreateLightningEffectModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateLightningEffectModel)m[i + start];
            v.lifeSpan = br.ReadSingle();
            v.displayPaths = (Il2CppStringArray)m[br.ReadInt32()];
            v.displayLengths = (Il2CppStructArray<float>)m[br.ReadInt32()];
        }
    }

    private void Set_v_LightningModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.LightningModel)m[i + start];
            v.splits = br.ReadInt32();
            v.emissionModel = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.splitRange = br.ReadSingle();
            v.delay = br.ReadSingle();
            v.delayFrames = br.ReadInt32();
        }
    }

    private void Set_v_IgnoreInsufficientPierceModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.IgnoreInsufficientPierceModel)m[i + start];
        }
    }

    private void Set_v_SpiritOfTheForestModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SpiritOfTheForestModel)m[i + start];
            v.objectToPlace1FarPath = br.ReadBoolean() ? null : br.ReadString();
            v.objectToPlace2FarPath = br.ReadBoolean() ? null : br.ReadString();
            v.objectToPlace3FarPath = br.ReadBoolean() ? null : br.ReadString();
            v.objectToPlace4FarPath = br.ReadBoolean() ? null : br.ReadString();
            v.objectsToPlaceFarPath = (Il2CppStringArray)m[br.ReadInt32()];
            v.objectToPlace1MiddlePath = br.ReadBoolean() ? null : br.ReadString();
            v.objectToPlace2MiddlePath = br.ReadBoolean() ? null : br.ReadString();
            v.objectToPlace3MiddlePath = br.ReadBoolean() ? null : br.ReadString();
            v.objectToPlace4MiddlePath = br.ReadBoolean() ? null : br.ReadString();
            v.objectsToPlaceMiddlePath = (Il2CppStringArray)m[br.ReadInt32()];
            v.objectToPlace1ClosePath = br.ReadBoolean() ? null : br.ReadString();
            v.objectToPlace2ClosePath = br.ReadBoolean() ? null : br.ReadString();
            v.objectToPlace3ClosePath = br.ReadBoolean() ? null : br.ReadString();
            v.objectToPlace4ClosePath = br.ReadBoolean() ? null : br.ReadString();
            v.objectsToPlaceClosePath = (Il2CppStringArray)m[br.ReadInt32()];
            v.damageOverTimeZoneModelFar =
                (Assets.Scripts.Models.Towers.Behaviors.DamageOverTimeZoneModel)m[br.ReadInt32()];
            v.damageOverTimeZoneModelMiddle =
                (Assets.Scripts.Models.Towers.Behaviors.DamageOverTimeZoneModel)m[br.ReadInt32()];
            v.damageOverTimeZoneModelClose =
                (Assets.Scripts.Models.Towers.Behaviors.DamageOverTimeZoneModel)m[br.ReadInt32()];
            v.middleRange = br.ReadSingle();
            v.closeRange = br.ReadSingle();
            v.spacingMin = br.ReadSingle();
            v.spacingMax = br.ReadSingle();
            v.scaleMin = br.ReadSingle();
            v.scaleMax = br.ReadSingle();
            v.rotationMin = br.ReadSingle();
            v.rotationMax = br.ReadSingle();
            v.offsetMin = br.ReadSingle();
            v.offsetMax = br.ReadSingle();
            v.circleRadius = br.ReadSingle();
            v.generateRadius = br.ReadSingle();
            v.time = br.ReadSingle();
            v.timeFrames = br.ReadInt32();
            v.amountPerFrame = br.ReadSingle();
            v.minScale = br.ReadSingle();
            v.maxScale = br.ReadSingle();
            v.scaleTime = br.ReadSingle();
            v.minScaleVector = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            v.maxScaleVector3 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }
    }

    private void Set_v_DamageOverTimeZoneModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.DamageOverTimeZoneModel)m[i + start];
            v.behaviorModel = (Assets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel)m[br.ReadInt32()];
            v.range = br.ReadSingle();
            v.isGlobal = br.ReadBoolean();
            v.filterInvisible = br.ReadBoolean();
            v.onlyAffectOnscreen = br.ReadBoolean();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_JungleVineEffectModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.JungleVineEffectModel)m[i + start];
            v.displayFrontAssetId = br.ReadBoolean() ? null : br.ReadString();
            v.displayBackAssetId = br.ReadBoolean() ? null : br.ReadString();
            v.fullscreen = br.ReadBoolean();
            v.destroyAfterPopTime = br.ReadSingle();
            v.sound1 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound2 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound3 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.sound4 = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.ageingDestroyModel = (Assets.Scripts.Models.Behaviors.AgeingDestroyModel)m[br.ReadInt32()];
            v.backEffectDisplayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[br.ReadInt32()];
            v.bloonEffectDisplayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[br.ReadInt32()];
            v.frontEffectDisplayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_AgeingDestroyModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Behaviors.AgeingDestroyModel)m[i + start];
            v.time = br.ReadSingle();
            v.timeFrames = br.ReadInt32();
            v.useRoundTime = br.ReadBoolean();
        }
    }

    private void Set_v_JungleVineLimitProjectileModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.JungleVineLimitProjectileModel)m[i + start];
            v.limit = br.ReadInt32();
            v.delayInFrames = br.ReadInt32();
        }
    }

    private void Set_v_FilterOutOffscreenModel_Fields(int start, int count)
    {
        Set_v_FilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Filters.FilterOutOffscreenModel)m[i + start];
            v.includeBloonRadius = br.ReadBoolean();
        }
    }

    private void Set_v_CashPerBananaFarmInRangeModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v =
                (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CashPerBananaFarmInRangeModel)m[i + start];
            v.baseCash = br.ReadSingle();
            v.cash = br.ReadSingle();
            v.rangeIncrease = br.ReadSingle();
            v.textAssetId = br.ReadBoolean() ? null : br.ReadString();
            v.textLifespan = br.ReadSingle();
        }
    }

    private void Set_v_BonusLivesOnAbilityModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.BonusLivesOnAbilityModel)m[i + start];
            v.amount = br.ReadSingle();
        }
    }

    private void Set_v_LifeBasedAttackSpeedModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.LifeBasedAttackSpeedModel)m[i + start];
            v.ratePerLife = br.ReadSingle();
            v.lifeCap = br.ReadInt32();
            v.baseRateIncrease = br.ReadSingle();
            v.saveId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_DamageModifierWrathModel_Fields(int start, int count)
    {
        Set_v_DamageModifierModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierWrathModel)m[i + start];
            v.rbeThreshold = br.ReadInt32();
            v.damage = br.ReadInt32();
            v.maxDamageBoost = br.ReadInt32();
        }
    }

    private void Set_v_PoplustSupportModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.PoplustSupportModel)m[i + start];
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.maxStacks = br.ReadInt32();
            v.ratePercentIncrease = br.ReadSingle();
            v.piercePercentIncrease = br.ReadSingle();
        }
    }

    private void Set_v_DamageBasedAttackSpeedModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.DamageBasedAttackSpeedModel)m[i + start];
            v.damageThreshold = br.ReadSingle();
            v.increasePerThreshold = br.ReadSingle();
            v.maxStacks = br.ReadInt32();
            v.maxTimeInFramesWithoutDamage = br.ReadInt32();
        }
    }

    private void Set_v_DruidVengeanceEffectModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.DruidVengeanceEffectModel)m[i + start];
            v.damageModifierWrathModel =
                (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierWrathModel)m[br.ReadInt32()];
            v.smallGlowEffectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.mediumGlowEffectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.epicGlowEffectModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.defaultProjectilePath = br.ReadBoolean() ? null : br.ReadString();
            v.weakProjectilePath = br.ReadBoolean() ? null : br.ReadString();
            v.mediumProjectilePath = br.ReadBoolean() ? null : br.ReadString();
            v.epicProjectilePath = br.ReadBoolean() ? null : br.ReadString();
            v.smallGlowEffectStacks = br.ReadInt32();
            v.mediumGlowEffectStacks = br.ReadInt32();
            v.epicGlowEffectStacks = br.ReadInt32();
        }
    }

    private void Set_v_PlayAnimationIndexModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.PlayAnimationIndexModel)m[i + start];
            v.placeAnimation = br.ReadInt32();
            v.upgradeAnimation = br.ReadInt32();
        }
    }

    private void Set_v_CreateProjectileOnTowerDestroyModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CreateProjectileOnTowerDestroyModel)m[i + start];
            v.projectileModel = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emissionModel = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.useTowerRotation = br.ReadBoolean();
            v.setAgeZeroOnSell = br.ReadBoolean();
        }
    }

    private void Set_v_OverclockModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.OverclockModel)m[i + start];
            v.lifespanFrames = br.ReadInt32();
            v.rateModifier = br.ReadSingle();
            v.villageRangeModifier = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.mutatorSaveId = br.ReadBoolean() ? null : br.ReadString();
            v.maxStacks = br.ReadInt32();
            v.selectionObjectPath = br.ReadBoolean() ? null : br.ReadString();
            v.buffDisplayPath = br.ReadBoolean() ? null : br.ReadString();
            v.initialEffect = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.buffDisplayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[br.ReadInt32()];
            v.buffLocsName = br.ReadBoolean() ? null : br.ReadString();
            v.buffIconName = br.ReadBoolean() ? null : br.ReadString();
            v.tierBasedDurationMultiplier = (Il2CppStructArray<float>)m[br.ReadInt32()];
        }
    }

    private void Set_v_OverclockPermanentModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.OverclockPermanentModel)m[i + start];
            v.rateModifier = br.ReadSingle();
            v.villageRangeModifier = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.maxStacks = br.ReadInt32();
            v.buffLocsName = br.ReadBoolean() ? null : br.ReadString();
            v.buffIconName = br.ReadBoolean() ? null : br.ReadString();
            v.mutatorsByStack =
                (Dictionary<System.Int32, Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.
                    OverclockPermanentModel.OverclockPermanentMutator>)m[br.ReadInt32()];
        }
    }

    private void Set_v_SlowOnPopModel_Fields(int start, int count)
    {
        Set_v_SlowModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowOnPopModel)m[i + start];
        }
    }

    private void Set_v_CollectCreatedProjectileModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CollectCreatedProjectileModel)m[i + start];
            v.projectileId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_EatBloonModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.EatBloonModel)m[i + start];
            v.rbeCapacity = br.ReadSingle();
            v.rbeCashMultiplier = br.ReadSingle();
            v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[br.ReadInt32()];
            v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[br.ReadInt32()];
            v.animationStateOpen = br.ReadInt32();
            v.animationStateClosed = br.ReadInt32();
            v.timeUntilClose = br.ReadSingle();
            v.timeUntilCloseFrames = br.ReadInt32();
            v.effectOnEatModel = (Assets.Scripts.Models.Effects.EffectModel)m[br.ReadInt32()];
            v.bloonTrapOpenSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
            v.bloonTrapCloseSound = (Assets.Scripts.Models.Audio.SoundModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_LimitProjectileModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.LimitProjectileModel)m[i + start];
            v.projectileId = br.ReadBoolean() ? null : br.ReadString();
            v.limit = br.ReadInt32();
            v.delayInFrames = br.ReadInt32();
            v.limitByDestroyedPriorProjectile = br.ReadBoolean();
            v.globalForPlayer = br.ReadBoolean();
        }
    }

    private void Set_v_CreateTypedTowerModel_Fields(int start, int count)
    {
        Set_v_ProjectileBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTypedTowerModel)m[i + start];
            v.crushingTower = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.boomTower = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.coldTower = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.energyTower = (Assets.Scripts.Models.Towers.TowerModel)m[br.ReadInt32()];
            v.crushingDisplay = br.ReadBoolean() ? null : br.ReadString();
            v.boomDisplay = br.ReadBoolean() ? null : br.ReadString();
            v.coldDisplay = br.ReadBoolean() ? null : br.ReadString();
            v.energyDisplay = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_CreateTypedTowerCurrentIndexModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.CreateTypedTowerCurrentIndexModel)m[i + start];
        }
    }

    private void Set_v_ShowCashIconInsteadModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.ShowCashIconInsteadModel)m[i + start];
        }
    }

    private void Set_v_BananaCentralBuffModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.BananaCentralBuffModel)m[i + start];
            v.multiplier = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_SendToBankModel_Fields(int start, int count)
    {
        Set_v_EmissionModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.SendToBankModel)m[i + start];
        }
    }

    private void Set_v_EmissionsPerRoundFilterModel_Fields(int start, int count)
    {
        Set_v_WeaponBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.EmissionsPerRoundFilterModel)m[i + start];
            v.count = br.ReadInt32();
            v.allowSpawnOnInitialise = br.ReadBoolean();
            v.ignoreInSandbox = br.ReadBoolean();
        }
    }

    private void Set_v_BankModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.BankModel)m[i + start];
            v.capacity = br.ReadSingle();
            v.interest = br.ReadSingle();
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.fullBankAssetId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.autoCollect = br.ReadBoolean();
            v.collectAnimation = br.ReadInt32();
        }
    }

    private void Set_v_ImfLoanModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ImfLoanModel)m[i + start];
            v.amount = br.ReadSingle();
            v.incomeRecoveryRate = br.ReadSingle();
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
            v.lifespan = br.ReadSingle();
            v.imfLoanCollection = (Assets.Scripts.Models.SimulationBehaviors.ImfLoanCollectionModel)m[br.ReadInt32()];
        }
    }

    private void Set_v_SimulationBehaviorModel_Fields(int start, int count)
    {
        Set_v_Model_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.SimulationBehaviors.SimulationBehaviorModel)m[i + start];
        }
    }

    private void Set_v_ImfLoanCollectionModel_Fields(int start, int count)
    {
        Set_v_SimulationBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.SimulationBehaviors.ImfLoanCollectionModel)m[i + start];
            v.collectionRate = br.ReadSingle();
            v.amount = br.ReadSingle();
        }
    }

    private void Set_v_CentralMarketBuffModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorBuffModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CentralMarketBuffModel)m[i + start];
            v.multiplier = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.maxStackCount = br.ReadInt32();
        }
    }

    private void Set_v_BonusLivesPerRoundModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.BonusLivesPerRoundModel)m[i + start];
            v.amount = br.ReadInt32();
            v.lifespan = br.ReadSingle();
            v.assetId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_VisibilitySupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.VisibilitySupportModel)m[i + start];
            v.isUnique = br.ReadBoolean();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_AddBehaviorToBloonInZoneModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.AddBehaviorToBloonInZoneModel)m[i + start];
            v.zoneRadius = br.ReadSingle();
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
            v.isUnique = br.ReadBoolean();
            v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Bloons.BloonBehaviorModel>)m[br.ReadInt32()];
            v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>)m[br.ReadInt32()];
            v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>)m[br.ReadInt32()];
            v.overlayLayer = br.ReadInt32();
        }
    }

    private void Set_v_FilterInSetModel_Fields(int start, int count)
    {
        Set_v_TowerFilterModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.TowerFilters.FilterInSetModel)m[i + start];
            v.towerSets = (Il2CppStringArray)m[br.ReadInt32()];
        }
    }

    private void Set_v_ProjectileSpeedSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.ProjectileSpeedSupportModel)m[i + start];
            v.multiplier = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.isUnique = br.ReadBoolean();
        }
    }

    private void Set_v_FreeUpgradeSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.FreeUpgradeSupportModel)m[i + start];
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.upgrade = br.ReadInt32();
        }
    }

    private void Set_v_AbilityCooldownScaleSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.AbilityCooldownScaleSupportModel)m[i + start];
            v.isUnique = br.ReadBoolean();
            v.abilityCooldownSpeedScale = br.ReadSingle();
            v.affectsOnlyWater = br.ReadBoolean();
            v.mutatorPriority = br.ReadInt32();
        }
    }

    private void Set_v_DamageTypeSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.DamageTypeSupportModel)m[i + start];
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.immuneBloonProperties = (BloonProperties)(br.ReadInt32());
            v.isUnique = br.ReadBoolean();
        }
    }

    private void Set_v_SupportRemoveFilterOutTagModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.SupportRemoveFilterOutTagModel)m[i + start];
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
            v.removeScriptWithSupportMutatorId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_CallToArmsModel_Fields(int start, int count)
    {
        Set_v_AbilityBehaviorModel_Fields(start, count);
        var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CallToArmsModel>();
        var lifespanField = t.GetField("lifespan", bindFlags);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CallToArmsModel)m[i + start];
            v.multiplier = br.ReadSingle();
            v.useRadius = br.ReadBoolean();
            v.buffLocsName = br.ReadBoolean() ? null : br.ReadString();
            v.buffIconName = br.ReadBoolean() ? null : br.ReadString();
            v.lifespanFrames = br.ReadInt32();
            lifespanField.SetValue(v, br.ReadSingle().ToIl2Cpp());
        }
    }

    private void Set_v_AddBehaviorToTowerSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.AddBehaviorToTowerSupportModel)m[i + start];
            v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TowerBehaviorModel>)m[br.ReadInt32()];
            v.mutationId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_CashIncreaseModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.CashIncreaseModel)m[i + start];
            v.increase = br.ReadSingle();
            v.multiplier = br.ReadSingle();
        }
    }

    private void Set_v_MonkeyCityModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.MonkeyCityModel)m[i + start];
            v.roundsTillMultiplier = br.ReadInt32();
            v.towerId = br.ReadBoolean() ? null : br.ReadString();
            v.multiplier = br.ReadSingle();
            v.mutatorId = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    private void Set_v_MonkeyCityIncomeSupportModel_Fields(int start, int count)
    {
        Set_v_SupportModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.MonkeyCityIncomeSupportModel)m[i + start];
            v.isUnique = br.ReadBoolean();
            v.incomeModifier = br.ReadSingle();
        }
    }

    private void Set_v_MonkeyopolisModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.MonkeyopolisModel)m[i + start];
            v.filterTower = br.ReadBoolean() ? null : br.ReadString();
            v.valueRequiredForCrate = br.ReadInt32();
            v.cashFromCrate = br.ReadInt32();
        }
    }

    private void Set_v_MonkeyopolisUpgradeCostModel_Fields(int start, int count)
    {
        Set_v_TowerBehaviorModel_Fields(start, count);
        for (var i = 0; i < count; i++)
        {
            var v = (Assets.Scripts.Models.Towers.Behaviors.MonkeyopolisUpgradeCostModel)m[i + start];
            v.costPerFarm = br.ReadInt32();
            v.path = br.ReadInt32();
            v.towerFilter = br.ReadBoolean() ? null : br.ReadString();
        }
    }

    #endregion

    public Assets.Scripts.Models.Towers.TowerModel Load(byte[] bytes)
    {
        using (var s = new MemoryStream(bytes))
        {
            using (var reader = new BinaryReader(s))
            {
                this.br = reader;
                var totalCount = br.ReadInt32();
                m = new object[totalCount];

                //##  Step 1: create empty collections
                CreateArraySet<Assets.Scripts.Models.Model>();
                Read_a_Int32_Array();
                Read_a_AreaType_Array();
                CreateArraySet<Assets.Scripts.Models.Towers.Mods.ApplyModModel>();
                CreateArraySet<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
                CreateArraySet<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
                CreateArraySet<Assets.Scripts.Models.Towers.Filters.FilterModel>();
                Read_a_String_Array();
                CreateArraySet<Assets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel>();
                CreateArraySet<Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel>();
                CreateArraySet<Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>();
                CreateArraySet<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
                Read_a_TargetType_Array();
                CreateArraySet<Assets.Scripts.Models.Towers.TowerFilters.TowerFilterModel>();
                CreateArraySet<Assets.Scripts.Models.Bloons.BloonBehaviorModel>();
                CreateArraySet<Assets.Scripts.Models.Towers.Projectiles.DamageModifierModel>();
                Read_a_Vector3_Array();
                Read_a_Single_Array();
                CreateArraySet<Assets.Scripts.Models.Towers.TowerBehaviorModel>();
                CreateArraySet<Assets.Scripts.Models.Towers.Mutators.TowerMutatorModel>();
                CreateArraySet<Assets.Scripts.Models.Towers.Projectiles.ProjectileBehaviorModel>();
                CreateListSet<Assets.Scripts.Models.Model>();
                Read_l_String_List();
                CreateDictionarySet<System.String, Assets.Scripts.Models.Effects.AssetPathModel>();
                Read_String_v_Single_Dictionary();

                //##  Step 2: create empty objects
                Create_Records<Assets.Scripts.Models.Towers.TowerModel>();
                Create_Records<Assets.Scripts.Utils.SpriteReference>();
                Create_Records<Assets.Scripts.Models.Towers.Mods.ApplyModModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnPlaceModel>();
                Create_Records<Assets.Scripts.Models.Effects.EffectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnSellModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SubmergeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.ProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RemoveBloonModifiersModel>();
                Create_Records<Assets.Scripts.Models.GenericBehaviors.DisplayModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectEffectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.IgnoreThrowMarkerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.WeaponRateMinModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterOutTagModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileFilterModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddTagToBloonModel>();
                Create_Records<Assets.Scripts.Models.Effects.AssetPathModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.AttackFilterModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterInvisibleModel>();
                Create_Records<Assets.Scripts.Models.Audio.SoundModel>();
                Create_Records<Assets.Scripts.Models.GenericBehaviors.BuffIndicatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnSellModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnUpgradeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SubmergeEffectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.LinkProjectileRadiusToTowerRangeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnTowerPlaceModel>();
                Create_Records<Assets.Scripts.Models.Audio.BlankSoundModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnUpgradeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionWithOffsetsModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.
                    EmissionCamoIfTargetIsCamoModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.AlternateAnimationModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterInvisibleSubIntelModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstSharedRangeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastSharedRangeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseSharedRangeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongSharedRangeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.SubmergedTargetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.PreEmptiveStrikeLauncherModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterAllModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.InstantModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.
                    CreateEffectProjectileAfterTimeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileExpireModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExpireModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterAllExceptTargetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterWithTagModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.InstantDamageEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForTagModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForBloonTypeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ActivateAttackModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CircleFootprintModel>();
                Create_Records<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.
                    EmissionRotationOffProjectileDirectionModel>();
                Create_Records<
                    Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExhaustFractionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.ArcEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SubCommanderSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.TowerFilters.FilterInBaseTowerIdModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterGlueLevelModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddBehaviorToBloonModel>();
                Create_Records<Assets.Scripts.Models.Bloons.Behaviors.DamageOverTimeCustomModel>();
                Create_Records<Assets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnContactModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowModifierForTagModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RemoveMutatorsFromBloonModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.
                    CreateSoundOnProjectileCollisionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnContactModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddBonusDamagePerHitToBloonModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToParentModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateEffectOnAbilityModel>();
                Create_Records<Assets.Scripts.Models.Bloons.Behaviors.EmitOnDestroyModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowForBloonModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.EmitOnDamageModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMaimMoabModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMaimMoabModel.Mutator>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RetargetOnContactModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.
                    CreateEffectFromCollisionToCollisionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.
                    CheckTargetsWithoutOffsetsModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstPrioCamoModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastPrioCamoModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetClosePrioCamoModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongPrioCamoModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetEliteTargettingModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SwitchTargetSupplierOnUpgradeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.PickupModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnPickupModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CashModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTextEffectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.OffsetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RandomPositionBasicModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TargetSupplierSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TargetSupplierSupportModel.MutatorTower>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.RateSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.LeakDangerAttackSpeedModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.TurboModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetMoabModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RandomRangeTravelStraightModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustFractionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterWithTagsModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DistributeToChildrenSetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.CreateSoundOnProjectileCreatedModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.AlternateProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterBloonIfDamageTypeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.RemoveMutatorOnUpgradeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.FollowPathModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RotateModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterMoabModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CollideOnlyWithTargetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.FlipFollowPathModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.DamageUpModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.OrbitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DontDestroyOnContinueModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ClearHitBloonsModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.PushBackModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ArriveAtTargetModel>();
                Create_Records<Assets.Scripts.Simulation.Towers.Projectiles.Behaviors.Curve>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterOutBloonModel>();
                Create_Records<
                    Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileBlockerCollisionReboundModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ExpireProjectileAtScreenEdgeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MonkeyFanClubModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectAfterTimeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ShowTextOnHitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.CritMultiplierModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.LinkDisplayScaleToTowerRangeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.
                    CreateEffectWhileAttackingModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmmisionTowardsTargetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.SpinModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterOveridingMutatedTargetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterFrozenBloonsModel>();
                Create_Records<Assets.Scripts.Models.Bloons.Behaviors.EmitOnPopModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RemoveDamageTypeModifierModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.FreezeModel>();
                Create_Records<Assets.Scripts.Models.Bloons.Behaviors.GrowBlockModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.FreezeNearbyWaterModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SlowBloonsZoneModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.
                    ActivateRateSupportZoneModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.FreezeModifierForTagsModel>();
                Create_Records<Assets.Scripts.Models.Bloons.Behaviors.CarryProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterMutatedTargetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterOnlyCamoInModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustedModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RefreshPierceModel>();
                Create_Records<
                    Assets.Scripts.Models.Towers.Projectiles.Behaviors.DestroyProjectileIfTowerDestroyedModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.ZeroRotationModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetTrackOrDefaultModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterOfftrackModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SyncTargetPriorityWithSubTowersModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TowerCreateTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TowerExpireOnParentDestroyedModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreditPopsToParentTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.IgnoreTowersBlockerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.PathMovementFromScreenCenterModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnExpireModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SavedSubTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.
                    EmissionRotationOffBloonDirectionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.
                    PathMovementFromScreenCenterPatternModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.AbilityCreateTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TowerExpireModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.SwitchDisplayModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.
                    MutateRemoveAllAttacksOnAbilityActivateModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.NecromancerZoneModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.RotateToDefaultPositionTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.PrinceOfDarknessZombieBuffModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.NecromancerEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelAlongPathModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.NecroEmissionFilterModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.PrinceOfDarknessEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.
                    NecromancerTargetTrackWithinRangeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.ParallelEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.
                    EmissionRotationOffDisplayModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.FireAlternateWeaponModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.
                    EmissionRotationOffDisplayOnEmitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.FireWhenAlternateWeaponIsReadyModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.FilterTargetAngleFilterModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.
                    EmissionArcRotationOffDisplayDirectionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TowerExpireOnParentUpgradedModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.FireFromAirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FighterPilotPatternFirstModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FighterPilotPatternCloseModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FighterPilotPatternLastModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.
                    FighterPilotPatternStrongModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AccelerateModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongAirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.AirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.FighterMovementModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.SubTowerFilterModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.FlagshipAttackSpeedIncreaseModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.AddMakeshiftAreaModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.
                    EmissionRotationOffTowerDirectionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.
                    EmissionArcRotationOffTowerDirectionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.GrappleEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.MoabTakedownModel>();
                Create_Records<Assets.Scripts.Models.Bloons.Behaviors.IncreaseWorthTextEffectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.MoabTakedownModel.
                    BloonWorthMutator>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateRopeEffectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelTowardsEmitTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetGrapplableModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.PerRoundCashBonusTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TradeEmpireBuffModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CashbackZoneModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.MerchantShipModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterTargetAngleModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.RectangleFootprintModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.AttackAirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.CirclePatternModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FigureEightPatternModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionOverTimeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.FallToGroundModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.CheckAirUnitOverTrackModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetInFrontOfAirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.PathMovementModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.GroundZeroBombBuffModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.CenterElipsePatternModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstAirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseAirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastAirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.RandomTargetSpreadModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ChipMapBasedObjectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageInRingRadiusModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.
                    CreateSoundOnProjectileExhaustModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForBloonStateModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.CycleAnimationModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectEffectWithOffsetsModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSelectedPointModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileZeroRotationModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.WindModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.RandomAngleOffsetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SupportShinobiTacticsModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamagePercentOfMaxModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowMinusAbilityDurationModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetWithinTimeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetTrackModel>();
                Create_Records<Assets.Scripts.Models.Bloons.Behaviors.ProjectileOverTimeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.SetTriggerOnAirUnitFireModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.AnimateAirUnitOnFireModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.ResetRateOnInitialiseModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.RandomEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FollowTouchSettingModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.LockInPlaceSettingModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PatrolPointsSettingModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetAirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PursuitSettingModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PrioritiseRotationModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.HeliMovementModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnAirUnitModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.RedeployModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.KeepInBoundsModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.AngleToMapCenterModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.LivesModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FindDeploymentLocationModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.UsePresetTargetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.ComancheDefenceModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.MoabShoveZoneModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TowerRadiusModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnAttachedModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.RandomArcEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.CheckTempleCanFireModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.MonkeyTempleModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.MonkeyTempleModel.TCBOOMutator>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TempleTowerMutatorGroupTierTwoModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.PierceTowerMutatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.DamageTowerMutatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.AddAttackTowerMutatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.UseTowerRangeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.Conditions.Behaviors.CheckTempleUnderLevelModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.ReloadTimeTowerMutatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.UseParentEjectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.ProjectileSizeTowerMutatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.ProjectileSpeedTowerMutatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.WindChanceTowerMutatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RandomPositionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.RangeTowerMutatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Mutators.AddBehaviorToTowerMutatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.DiscountZoneModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.BonusCashZoneModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.PierceSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.RangeSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.DamageSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.UseAttackRotationModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToMiddleOfTargetsModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetRightHandModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLeftHandModel>();
                Create_Records<
                    Assets.Scripts.Models.Towers.Projectiles.Behaviors.DistributeToChildrenBloonModifierModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.KnockbackModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.OverrideCamoDetectionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ImmunityModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.DarkshiftModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.TempleTowerMutatorGroupTierOneModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.DartlingMaintainLastPosModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.LineProjectileEmissionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.LineEffectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetPointerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToPointerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CollideExtraPierceReductionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.
                    CreateProjectileOnExhaustPierceModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.
                    CreateProjectileOnBlockerCollideModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelCurvyModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraightSlowdownModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectAnimationModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetIndependantModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.RateBasedAnimationOffsetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CanBuffIndicatorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.LoadAlchemistBrewInfoModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddBerserkerBrewToProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.BerserkerBrewModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.BerserkerBrewCheckModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddAcidicMixtureToProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.AcidicMixtureCheckModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.AcidicMixtureModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.RemovePermaBrewModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AcidPoolModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ScaleProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFriendlyModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.BrewTargettingModel>();
                Create_Records<Assets.Scripts.Models.Bloons.Behaviors.UnstableConcoctionSplashModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.
                    DamageModifierUnstableConcoctionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.
                    CreateEffectOnAbilityEndModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MorphTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.IncreaseRangeModel>();
                Create_Records<
                    Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetTrackOrDefaultAcidPoolModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.IncreaseBloonWorthModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.MorphBloonModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.EndOfRoundClearBypassModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.SetSpriteFromPierceModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.FadeProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.HeightOffsetProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.RandomRotationWeaponBehaviorModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.ActivateAbilityAfterIntervalModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AgeRandomModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.StartOfRoundRateBuffModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.StartOfRoundRateBuffModel.RateMutator>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.CloseTargetTrackModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FarTargetTrackModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.SmartTargetTrackModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnIntervalModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateLightningEffectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.LightningModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.IgnoreInsufficientPierceModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SpiritOfTheForestModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.DamageOverTimeZoneModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.JungleVineEffectModel>();
                Create_Records<Assets.Scripts.Models.Behaviors.AgeingDestroyModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.JungleVineLimitProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Filters.FilterOutOffscreenModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.
                    CashPerBananaFarmInRangeModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.BonusLivesOnAbilityModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.LifeBasedAttackSpeedModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierWrathModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.PoplustSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.DamageBasedAttackSpeedModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.DruidVengeanceEffectModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.PlayAnimationIndexModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateProjectileOnTowerDestroyModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.OverclockModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.OverclockPermanentModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.SlowOnPopModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CollectCreatedProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.EatBloonModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.LimitProjectileModel>();
                Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTypedTowerModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.CreateTypedTowerCurrentIndexModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.ShowCashIconInsteadModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.BananaCentralBuffModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.SendToBankModel>();
                Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.EmissionsPerRoundFilterModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.BankModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ImfLoanModel>();
                Create_Records<Assets.Scripts.Models.SimulationBehaviors.ImfLoanCollectionModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CentralMarketBuffModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.BonusLivesPerRoundModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.VisibilitySupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.AddBehaviorToBloonInZoneModel>();
                Create_Records<Assets.Scripts.Models.Towers.TowerFilters.FilterInSetModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.ProjectileSpeedSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.FreeUpgradeSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.AbilityCooldownScaleSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.DamageTypeSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.SupportRemoveFilterOutTagModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CallToArmsModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.AddBehaviorToTowerSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.CashIncreaseModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.MonkeyCityModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.MonkeyCityIncomeSupportModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.MonkeyopolisModel>();
                Create_Records<Assets.Scripts.Models.Towers.Behaviors.MonkeyopolisUpgradeCostModel>();

                Set_v_TowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SpriteReference_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ApplyModModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectOnPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SubmergeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AttackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_WeaponModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SingleEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AgeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RemoveBloonModifiersModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DisplayModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EjectEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_IgnoreThrowMarkerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_WeaponRateMinModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterOutTagModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ProjectileFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddTagToBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AssetPathModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AttackFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterInvisibleModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SoundModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BuffIndicatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SubmergeEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LinkProjectileRadiusToTowerRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnTowerPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BlankSoundModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionWithOffsetsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionCamoIfTargetIsCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TravelStraitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TrackTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AlternateAnimationModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RotateToTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterInvisibleSubIntelModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetFirstSharedRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetLastSharedRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetCloseSharedRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetStrongSharedRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SubmergedTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PreEmptiveStrikeLauncherModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterAllModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_InstantModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectProjectileAfterTimeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_Assets_Scripts_Models_Towers_Projectiles_Behaviors_CreateEffectOnExpireModel_Fields(
                    br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnProjectileExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateProjectileOnExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterAllExceptTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterWithTagModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_InstantDamageEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageModifierForTagModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageModifierForBloonTypeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ActivateAttackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetStrongModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CircleFootprintModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_UpgradePathModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ThrowMarkerOffsetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionRotationOffProjectileDirectionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateProjectileOnExhaustFractionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ArcEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SubCommanderSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterInBaseTowerIdModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterGlueLevelModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SlowModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddBehaviorToBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageOverTimeCustomModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageOverTimeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateProjectileOnContactModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SlowModifierForTagModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RemoveMutatorsFromBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnProjectileCollisionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectOnContactModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetFirstModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetLastModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetCloseModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddBonusDamagePerHitToBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RotateToParentModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmitOnDestroyModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SlowForBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmitOnDamageModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SlowMaimMoabModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_Assets_Scripts_Models_Towers_Projectiles_Behaviors_SlowMaimMoabModel_Mutator_Fields(
                    br.ReadInt32(), br.ReadInt32());
                Set_v_RetargetOnContactModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectFromCollisionToCollisionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CheckTargetsWithoutOffsetsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetFirstPrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetLastPrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetClosePrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetStrongPrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetEliteTargettingModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SwitchTargetSupplierOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PickupModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnPickupModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CashModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateTextEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_OffsetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RandomPositionBasicModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetSupplierSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MutatorTower_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RateSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LeakDangerAttackSpeedModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TurboModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetMoabModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RandomRangeTravelStraightModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectOnExhaustFractionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterWithTagsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DistributeToChildrenSetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnProjectileCreatedModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AlternateProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterBloonIfDamageTypeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RemoveMutatorOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FollowPathModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RotateModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterMoabModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CollideOnlyWithTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FlipFollowPathModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageUpModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_OrbitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DontDestroyOnContinueModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ClearHitBloonsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PushBackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ArriveAtTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_Curve_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterOutBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ProjectileBlockerCollisionReboundModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ExpireProjectileAtScreenEdgeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MonkeyFanClubModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectAfterTimeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ShowTextOnHitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CritMultiplierModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LinkDisplayScaleToTowerRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectWhileAttackingModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SingleEmmisionTowardsTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SpinModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterOveridingMutatedTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterFrozenBloonsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmitOnPopModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RemoveDamageTypeModifierModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FreezeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_GrowBlockModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FreezeNearbyWaterModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SlowBloonsZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ActivateRateSupportZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FreezeModifierForTagsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CarryProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterMutatedTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterOnlyCamoInModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectOnExhaustedModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RefreshPierceModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DestroyProjectileIfTowerDestroyedModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ZeroRotationModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetTrackOrDefaultModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterOfftrackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SyncTargetPriorityWithSubTowersModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TowerCreateTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TowerExpireOnParentDestroyedModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreditPopsToParentTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_IgnoreTowersBlockerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PathMovementFromScreenCenterModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_Assets_Scripts_Models_Towers_Behaviors_CreateEffectOnExpireModel_Fields(br.ReadInt32(),
                    br.ReadInt32());
                Set_v_SavedSubTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionRotationOffBloonDirectionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PathMovementFromScreenCenterPatternModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AbilityCreateTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TowerExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SwitchDisplayModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MutateRemoveAllAttacksOnAbilityActivateModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_NecromancerZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RotateToDefaultPositionTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PrinceOfDarknessZombieBuffModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_NecromancerEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TravelAlongPathModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_NecroEmissionFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PrinceOfDarknessEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_NecromancerTargetTrackWithinRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ParallelEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionRotationOffDisplayModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FireAlternateWeaponModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionRotationOffDisplayOnEmitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FireWhenAlternateWeaponIsReadyModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterTargetAngleFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionArcRotationOffDisplayDirectionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TowerExpireOnParentUpgradedModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FireFromAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FighterPilotPatternFirstModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FighterPilotPatternCloseModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FighterPilotPatternLastModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FighterPilotPatternStrongModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AccelerateModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetStrongAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FighterMovementModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SubTowerFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FlagshipAttackSpeedIncreaseModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddMakeshiftAreaModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionRotationOffTowerDirectionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionArcRotationOffTowerDirectionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_GrappleEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MoabTakedownModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_IncreaseWorthTextEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_Assets_Scripts_Models_Towers_Projectiles_Behaviors_MoabTakedownModel_BloonWorthMutator_Fields(
                    br.ReadInt32(), br.ReadInt32());
                Set_v_CreateRopeEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TravelTowardsEmitTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetGrapplableModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PerRoundCashBonusTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TradeEmpireBuffModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CashbackZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MerchantShipModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterTargetAngleModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RectangleFootprintModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AttackAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CirclePatternModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FigureEightPatternModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionOverTimeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FallToGroundModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CheckAirUnitOverTrackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetInFrontOfAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PathMovementModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_GroundZeroBombBuffModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CenterElipsePatternModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetFirstAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetCloseAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetLastAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RandomTargetSpreadModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ChipMapBasedObjectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageInRingRadiusModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnProjectileExhaustModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageModifierForBloonStateModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CycleAnimationModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EjectEffectWithOffsetsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetSelectedPointModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ProjectileZeroRotationModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_WindModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RandomAngleOffsetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SupportShinobiTacticsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamagePercentOfMaxModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SlowMinusAbilityDurationModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TrackTargetWithinTimeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetTrackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ProjectileOverTimeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SetTriggerOnAirUnitFireModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AnimateAirUnitOnFireModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ResetRateOnInitialiseModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RandomEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FollowTouchSettingModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LockInPlaceSettingModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PatrolPointsSettingModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RotateToTargetAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PursuitSettingModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PrioritiseRotationModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_HeliMovementModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectOnAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RedeployModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_KeepInBoundsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AngleToMapCenterModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LivesModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FindDeploymentLocationModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_UsePresetTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ComancheDefenceModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MoabShoveZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TowerRadiusModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateSoundOnAttachedModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RandomArcEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CheckTempleCanFireModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MonkeyTempleModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TCBOOMutator_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TempleTowerMutatorGroupTierTwoModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PierceTowerMutatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageTowerMutatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddAttackTowerMutatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_UseTowerRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CheckTempleUnderLevelModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ReloadTimeTowerMutatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_UseParentEjectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ProjectileSizeTowerMutatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ProjectileSpeedTowerMutatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_WindChanceTowerMutatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RandomPositionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RangeTowerMutatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddBehaviorToTowerMutatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DiscountZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BonusCashZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PierceSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RangeSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_UseAttackRotationModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RotateToMiddleOfTargetsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetRightHandModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetLeftHandModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DistributeToChildrenBloonModifierModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_KnockbackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_OverrideCamoDetectionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ImmunityModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DarkshiftModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TempleTowerMutatorGroupTierOneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DartlingMaintainLastPosModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LineProjectileEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LineEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetPointerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RotateToPointerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CollideExtraPierceReductionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateProjectileOnExhaustPierceModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateProjectileOnBlockerCollideModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TravelCurvyModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TravelStraightSlowdownModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EjectAnimationModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetIndependantModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RateBasedAnimationOffsetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CanBuffIndicatorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LoadAlchemistBrewInfoModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddBerserkerBrewToProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BerserkerBrewModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BerserkerBrewCheckModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddAcidicMixtureToProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AcidicMixtureCheckModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AcidicMixtureModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RemovePermaBrewModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AcidPoolModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ScaleProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetFriendlyModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BrewTargettingModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_UnstableConcoctionSplashModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageModifierUnstableConcoctionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateEffectOnAbilityEndModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MorphTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_IncreaseRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_TargetTrackOrDefaultAcidPoolModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_IncreaseBloonWorthModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MorphBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EndOfRoundClearBypassModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SetSpriteFromPierceModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FadeProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_HeightOffsetProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RandomRotationWeaponBehaviorModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ActivateAbilityAfterIntervalModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AgeRandomModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_StartOfRoundRateBuffModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_RateMutator_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CloseTargetTrackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FarTargetTrackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SmartTargetTrackModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateProjectileOnIntervalModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateLightningEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LightningModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_IgnoreInsufficientPierceModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SpiritOfTheForestModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageOverTimeZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_JungleVineEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AgeingDestroyModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_JungleVineLimitProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterOutOffscreenModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CashPerBananaFarmInRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BonusLivesOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LifeBasedAttackSpeedModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageModifierWrathModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PoplustSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageBasedAttackSpeedModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DruidVengeanceEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_PlayAnimationIndexModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateProjectileOnTowerDestroyModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_OverclockModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_OverclockPermanentModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SlowOnPopModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CollectCreatedProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EatBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_LimitProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateTypedTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CreateTypedTowerCurrentIndexModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ShowCashIconInsteadModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BananaCentralBuffModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SendToBankModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_EmissionsPerRoundFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BankModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ImfLoanModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ImfLoanCollectionModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CentralMarketBuffModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_BonusLivesPerRoundModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_VisibilitySupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddBehaviorToBloonInZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FilterInSetModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_ProjectileSpeedSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_FreeUpgradeSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AbilityCooldownScaleSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_DamageTypeSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_SupportRemoveFilterOutTagModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CallToArmsModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_AddBehaviorToTowerSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_CashIncreaseModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MonkeyCityModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MonkeyCityIncomeSupportModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MonkeyopolisModel_Fields(br.ReadInt32(), br.ReadInt32());
                Set_v_MonkeyopolisUpgradeCostModel_Fields(br.ReadInt32(), br.ReadInt32());

                //##  Step 4: link object collections e.g Product[]. Note: requires object data e.g dictionary<string, value> where string = model.name
                LinkArray<Assets.Scripts.Models.Model>();
                LinkArray<Assets.Scripts.Models.Towers.Mods.ApplyModModel>();
                LinkArray<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
                LinkArray<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
                LinkArray<Assets.Scripts.Models.Towers.Filters.FilterModel>();
                LinkArray<Assets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel>();
                LinkArray<Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel>();
                LinkArray<Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>();
                LinkArray<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
                LinkArray<Assets.Scripts.Models.Towers.TowerFilters.TowerFilterModel>();
                LinkArray<Assets.Scripts.Models.Bloons.BloonBehaviorModel>();
                LinkArray<Assets.Scripts.Models.Towers.Projectiles.DamageModifierModel>();
                LinkArray<Assets.Scripts.Models.Towers.TowerBehaviorModel>();
                LinkArray<Assets.Scripts.Models.Towers.Mutators.TowerMutatorModel>();
                LinkArray<Assets.Scripts.Models.Towers.Projectiles.ProjectileBehaviorModel>();
                LinkList<Assets.Scripts.Models.Model>();
                LinkDictionary<Assets.Scripts.Models.Effects.AssetPathModel>();

                var resIndex = br.ReadInt32();
                UnityEngine.Debug.Assert(br.BaseStream.Position == br.BaseStream.Length);
                return (Assets.Scripts.Models.Towers.TowerModel)m[resIndex];
            }
        }
    }
}