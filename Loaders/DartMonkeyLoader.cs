using System.IO;
using Assets.Scripts.Simulation.SMath;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Reflection;
using Il2CppSystem.Runtime.Serialization;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;

namespace UltimateCrosspathing.Loaders;

public class DartMonkeyLoader : ModByteLoader<Assets.Scripts.Models.Towers.TowerModel> {
	
	BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static; 
	BinaryReader br = null;
	
	// NOTE: was a collection per type but it prevented inheriance e.g list of Products would required class type id
	protected override string BytesFileName => "DartMonkeys.bytes";
	int mIndex = 1; // first element is null
	#region Read array
	
	private void LinkArray<T>() where T : Il2CppObjectBase {
		var setCount = br.ReadInt32();
		for (var i = 0; i < setCount; i++) {
			var arrIndex = br.ReadInt32();
			var arr = (Il2CppReferenceArray<T>)m[arrIndex];
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = (T) m[br.ReadInt32()];
			}
		}
	}
	private void LinkList<T>() where T : Il2CppObjectBase {
		var setCount = br.ReadInt32();
		for (var i = 0; i < setCount; i++) {
			var arrIndex = br.ReadInt32();
			var arr = (List<T>)m[arrIndex];
			for (var j = 0; j < arr.Capacity; j++) {
				arr.Add( (T) m[br.ReadInt32()] );
			}
		}
	}
	private void LinkDictionary<T>() where T : Il2CppObjectBase {
		var setCount = br.ReadInt32();
		for (var i = 0; i < setCount; i++) {
			var arrIndex = br.ReadInt32();
			var arr = (Dictionary<string, T>)m[arrIndex];
			var arrCount = br.ReadInt32();
			for (var j = 0; j < arrCount; j++) {
				var key = br.ReadString();
				var valueIndex = br.ReadInt32();
				arr[key] = (T) m[valueIndex];
			}
		}
	}
	private void LinkModelDictionary<T>() where T : Assets.Scripts.Models.Model {
		var setCount = br.ReadInt32();
		for (var i = 0; i < setCount; i++) {
			var arrIndex = br.ReadInt32();
			var arr = (Dictionary<string, T>)m[arrIndex];
			var arrCount = br.ReadInt32();
			for (var j = 0; j < arrCount; j++) {
				var valueIndex = br.ReadInt32();
				var obj = (T)m[valueIndex];
				arr[obj.name] = obj;
			}
		}
	}
	private void Read_a_Int32_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppStructArray<int>(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = br.ReadInt32();
			}
			m[mIndex++] = arr;
		}
	}
	private void Read_a_String_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppStringArray(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = br.ReadBoolean() ? null : br.ReadString();
			}
			m[mIndex++] = arr;
		}
	}
	private void Read_a_TargetType_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppReferenceArray<Assets.Scripts.Models.Towers.TargetType>(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = new Assets.Scripts.Models.Towers.TargetType {id = br.ReadString(), isActionable = br.ReadBoolean()};
			}
			m[mIndex++] = arr;
		}
	}
	private void Read_a_AreaType_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppStructArray<Assets.Scripts.Models.Map.AreaType>(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = (Assets.Scripts.Models.Map.AreaType)br.ReadInt32();
			}
			m[mIndex++] = arr;
		}
	}
	#endregion
	
	#region Read object records
	
	private void CreateArraySet<T>() where T : Il2CppObjectBase {
		var arrCount = br.ReadInt32();
		for(var i = 0; i < arrCount; i++) {
			m[mIndex++] = new Il2CppReferenceArray<T>(br.ReadInt32());;
		}
	}
	
	private void CreateListSet<T>() where T : Il2CppObjectBase {
		var arrCount = br.ReadInt32();
		for (var i = 0; i < arrCount; i++) {
			m[mIndex++] = new List<T>(br.ReadInt32()); // set capactity
		}
	}
	
	private void CreateDictionarySet<K, T>() {
		var arrCount = br.ReadInt32();
		for (var i = 0; i < arrCount; i++) {
			m[mIndex++] = new Dictionary<K, T>(br.ReadInt32());// set capactity
		}
	}
	
	private void Create_Records<T>() where T : Il2CppObjectBase {
		var count = br.ReadInt32();
		var t = Il2CppType.Of<T>();
		for (var i = 0; i < count; i++) {
			m[mIndex++] = FormatterServices.GetUninitializedObject(t).Cast<T>();
		}
	}
	#endregion
	
	#region Link object records
	
	private void Set_v_Model_Fields(int start, int count) {
		var t = Il2CppType.Of<Assets.Scripts.Models.Model>();
		var _nameField = t.GetField("_name", bindFlags);
		var childDependantsField = t.GetField("childDependants", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Model)m[i+start];
			_nameField.SetValue(v,br.ReadBoolean() ? null : String.Intern(br.ReadString()));
			childDependantsField.SetValue(v,(List<Assets.Scripts.Models.Model>) m[br.ReadInt32()]);
		}
	}
	
	private void Set_v_TowerModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.TowerModel>();
		var towerSizeField = t.GetField("towerSize", bindFlags);
		var cachedThrowMarkerHeightField = t.GetField("cachedThrowMarkerHeight", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.TowerModel)m[i+start];
			v.display = br.ReadBoolean() ? null : br.ReadString();
			v.baseId = br.ReadBoolean() ? null : br.ReadString();
			v.cost = br.ReadSingle();
			v.radius = br.ReadSingle();
			v.radiusSquared = br.ReadSingle();
			v.range = br.ReadSingle();
			v.ignoreBlockers = br.ReadBoolean();
			v.isGlobalRange = br.ReadBoolean();
			v.tier = br.ReadInt32();
			v.tiers = (Il2CppStructArray<int>) m[br.ReadInt32()];
			v.towerSet = br.ReadBoolean() ? null : br.ReadString();
			v.areaTypes = (Il2CppStructArray<Assets.Scripts.Models.Map.AreaType>) m[br.ReadInt32()];
			v.icon = (Assets.Scripts.Utils.SpriteReference) m[br.ReadInt32()];
			v.portrait = (Assets.Scripts.Utils.SpriteReference) m[br.ReadInt32()];
			v.instaIcon = (Assets.Scripts.Utils.SpriteReference) m[br.ReadInt32()];
			v.mods = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Mods.ApplyModModel>) m[br.ReadInt32()];
			v.ignoreTowerForSelection = br.ReadBoolean();
			v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.footprint = (Assets.Scripts.Models.Towers.Behaviors.FootprintModel) m[br.ReadInt32()];
			v.dontDisplayUpgrades = br.ReadBoolean();
			v.emoteSpriteSmall = (Assets.Scripts.Utils.SpriteReference) m[br.ReadInt32()];
			v.emoteSpriteLarge = (Assets.Scripts.Utils.SpriteReference) m[br.ReadInt32()];
			v.doesntRotate = br.ReadBoolean();
			v.upgrades = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>) m[br.ReadInt32()];
			v.appliedUpgrades = (Il2CppStringArray) m[br.ReadInt32()];
			v.targetTypes = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.TargetType>) m[br.ReadInt32()];
			v.paragonUpgrade = (Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel) m[br.ReadInt32()];
			v.isSubTower = br.ReadBoolean();
			v.isBakable = br.ReadBoolean();
			v.powerName = br.ReadBoolean() ? null : br.ReadString();
			v.showPowerTowerBuffs = br.ReadBoolean();
			v.animationSpeed = br.ReadSingle();
			v.towerSelectionMenuThemeId = br.ReadBoolean() ? null : br.ReadString();
			v.ignoreCoopAreas = br.ReadBoolean();
			v.canAlwaysBeSold = br.ReadBoolean();
			v.blockSelling = br.ReadBoolean();
			v.isParagon = br.ReadBoolean();
			v.ignoreMaxSellPercent = br.ReadBoolean();
			v.isStunned = br.ReadBoolean();
			v.geraldoItemName = br.ReadBoolean() ? null : br.ReadString();
			v.sellbackModifierAdd = br.ReadSingle();
			v.skinName = br.ReadBoolean() ? null : br.ReadString();
			towerSizeField.SetValue(v,br.ReadInt32().ToIl2Cpp());
			cachedThrowMarkerHeightField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_ar_Sprite_Fields(int start, int count) {
		Set_v_AssetReference_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Utils.AssetReference<UnityEngine.Sprite>)m[i+start];
		}
	}
	
	private void Set_v_AssetReference_Fields(int start, int count) {
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Utils.AssetReference)m[i+start];
		}
	}
	
	private void Set_v_SpriteReference_Fields(int start, int count) {
		Set_ar_Sprite_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Utils.SpriteReference>();
		var guidRefField = t.GetField("guidRef", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Utils.SpriteReference)m[i+start];
			guidRefField.SetValue(v,br.ReadBoolean() ? null : br.ReadString());
		}
	}
	
	private void Set_v_ApplyModModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Mods.ApplyModModel)m[i+start];
			v.mod = br.ReadBoolean() ? null : br.ReadString();
			v.target = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_TowerBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.TowerBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_CreateEffectOnPlaceModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnPlaceModel)m[i+start];
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_EffectModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Effects.EffectModel)m[i+start];
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
	
	private void Set_v_CreateSoundOnUpgradeModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnUpgradeModel)m[i+start];
			v.sound = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound1 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound3 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound4 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound5 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound6 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound7 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound8 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SoundModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Audio.SoundModel)m[i+start];
			v.assetId = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_CreateSoundOnSellModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnSellModel)m[i+start];
			v.sound = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateEffectOnSellModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnSellModel)m[i+start];
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateSoundOnTowerPlaceModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnTowerPlaceModel)m[i+start];
			v.sound1 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound1 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound2 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateEffectOnUpgradeModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnUpgradeModel)m[i+start];
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_AttackModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel)m[i+start];
			v.weapons = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Weapons.WeaponModel>) m[br.ReadInt32()];
			v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.range = br.ReadSingle();
			v.targetProvider = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel) m[br.ReadInt32()];
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
	
	private void Set_v_WeaponModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
		var rateField = t.GetField("rate", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Weapons.WeaponModel)m[i+start];
			v.animation = br.ReadInt32();
			v.animationOffset = br.ReadSingle();
			v.animationOffsetFrames = br.ReadInt32();
			v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel) m[br.ReadInt32()];
			v.ejectX = br.ReadSingle();
			v.ejectY = br.ReadSingle();
			v.ejectZ = br.ReadSingle();
			v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.rateFrames = br.ReadInt32();
			v.fireWithoutTarget = br.ReadBoolean();
			v.fireBetweenRounds = br.ReadBoolean();
			v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>) m[br.ReadInt32()];
			rateField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.useAttackPosition = br.ReadBoolean();
			v.startInCooldown = br.ReadBoolean();
			v.customStartCooldown = br.ReadSingle();
			v.customStartCooldownFrames = br.ReadInt32();
			v.animateOnMainAttack = br.ReadBoolean();
			v.isStunned = br.ReadBoolean();
		}
	}
	
	private void Set_v_EmissionModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[i+start];
			v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_ArcEmissionModel_Fields(int start, int count) {
		Set_v_EmissionModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Emissions.ArcEmissionModel>();
		var CountField = t.GetField("Count", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.ArcEmissionModel)m[i+start];
			v.angle = br.ReadSingle();
			v.offsetStart = br.ReadSingle();
			v.offset = br.ReadSingle();
			v.sliceSize = br.ReadSingle();
			v.ignoreTowerRotation = br.ReadBoolean();
			v.useProjectileRotation = br.ReadBoolean();
			CountField.SetValue(v,br.ReadInt32().ToIl2Cpp());
		}
	}
	
	private void Set_v_ProjectileModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[i+start];
			v.display = br.ReadBoolean() ? null : br.ReadString();
			v.id = br.ReadBoolean() ? null : br.ReadString();
			v.maxPierce = br.ReadSingle();
			v.pierce = br.ReadSingle();
			v.scale = br.ReadSingle();
			v.ignoreBlockers = br.ReadBoolean();
			v.usePointCollisionWithBloons = br.ReadBoolean();
			v.canCollisionBeBlockedByMapLos = br.ReadBoolean();
			v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
			v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.collisionPasses = (Il2CppStructArray<int>) m[br.ReadInt32()];
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
	
	private void Set_v_FilterModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Filters.FilterModel)m[i+start];
		}
	}
	
	private void Set_v_FilterInvisibleModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Filters.FilterInvisibleModel)m[i+start];
			v.isActive = br.ReadBoolean();
			v.ignoreBroadPhase = br.ReadBoolean();
		}
	}
	
	private void Set_v_ProjectileBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.ProjectileBehaviorModel)m[i+start];
			v.collisionPass = br.ReadInt32();
		}
	}
	
	private void Set_v_DamageModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel)m[i+start];
			v.damage = br.ReadSingle();
			v.maxDamage = br.ReadSingle();
			v.distributeToChildren = br.ReadBoolean();
			v.overrideDistributeBlocker = br.ReadBoolean();
			v.createPopEffect = br.ReadBoolean();
			v.immuneBloonProperties = (BloonProperties) (br.ReadInt32());
			v.immuneBloonPropertiesOriginal = (BloonProperties) (br.ReadInt32());
		}
	}
	
	private void Set_v_TravelStraitModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		var speedField = t.GetField("speed", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel)m[i+start];
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.lifespanFrames = br.ReadInt32();
			speedField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.speedFrames = br.ReadSingle();
		}
	}
	
	private void Set_v_ProjectileFilterModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileFilterModel)m[i+start];
			v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_DisplayModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.GenericBehaviors.DisplayModel)m[i+start];
			v.display = br.ReadBoolean() ? null : br.ReadString();
			v.layer = br.ReadInt32();
			v.positionOffset = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
			v.scale = br.ReadSingle();
			v.ignoreRotation = br.ReadBoolean();
			v.animationChanges = (List<Assets.Scripts.Models.GenericBehaviors.AnimationChange>) m[br.ReadInt32()];
			v.delayedReveal = br.ReadSingle();
		}
	}
	
	private void Set_v_SingleEmissionModel_Fields(int start, int count) {
		Set_v_EmissionModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel)m[i+start];
		}
	}
	
	private void Set_v_AttackBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.AttackBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_RotateToTargetModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetModel)m[i+start];
			v.onlyRotateDuringThrow = br.ReadBoolean();
			v.useThrowMarkerHeight = br.ReadBoolean();
			v.rotateOnlyOnThrow = br.ReadBoolean();
			v.additionalRotation = br.ReadInt32();
			v.rotateTower = br.ReadBoolean();
			v.useMainAttackRotation = br.ReadBoolean();
		}
	}
	
	private void Set_v_AttackFilterModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.AttackFilterModel)m[i+start];
			v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_TargetCamoModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCamoModel)m[i+start];
		}
	}
	
	private void Set_v_TargetSupplierModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel)m[i+start];
			v.isOnSubTower = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetFirstPrioCamoModel_Fields(int start, int count) {
		Set_v_TargetCamoModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstPrioCamoModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetLastPrioCamoModel_Fields(int start, int count) {
		Set_v_TargetCamoModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastPrioCamoModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetClosePrioCamoModel_Fields(int start, int count) {
		Set_v_TargetCamoModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetClosePrioCamoModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetStrongPrioCamoModel_Fields(int start, int count) {
		Set_v_TargetCamoModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongPrioCamoModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_FootprintModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.FootprintModel)m[i+start];
			v.doesntBlockTowerPlacement = br.ReadBoolean();
			v.ignoresPlacementCheck = br.ReadBoolean();
			v.ignoresTowerOverlap = br.ReadBoolean();
		}
	}
	
	private void Set_v_CircleFootprintModel_Fields(int start, int count) {
		Set_v_FootprintModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CircleFootprintModel)m[i+start];
			v.radius = br.ReadSingle();
		}
	}
	
	private void Set_v_UpgradePathModel_Fields(int start, int count) {
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
		var towerField = t.GetField("tower", bindFlags);
		var upgradeField = t.GetField("upgrade", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel)m[i+start];
			towerField.SetValue(v,br.ReadBoolean() ? null : br.ReadString());
			upgradeField.SetValue(v,br.ReadBoolean() ? null : br.ReadString());
		}
	}
	
	private void Set_v_ShowTextOnHitModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ShowTextOnHitModel)m[i+start];
			v.assetId = br.ReadBoolean() ? null : br.ReadString();
			v.lifespan = br.ReadSingle();
			v.useTowerPosition = br.ReadBoolean();
			v.text = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_WeaponBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_CritMultiplierModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.CritMultiplierModel)m[i+start];
			v.damage = br.ReadSingle();
			v.lower = br.ReadInt32();
			v.upper = br.ReadInt32();
			v.display = br.ReadBoolean() ? null : br.ReadString();
			v.distributeToChildren = br.ReadBoolean();
		}
	}
	
	private void Set_v_AbilityModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel>();
		var cooldownSpeedScaleField = t.GetField("cooldownSpeedScale", bindFlags);
		var animationOffsetField = t.GetField("animationOffset", bindFlags);
		var cooldownField = t.GetField("cooldown", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel)m[i+start];
			v.displayName = br.ReadBoolean() ? null : br.ReadString();
			v.description = br.ReadBoolean() ? null : br.ReadString();
			v.icon = (Assets.Scripts.Utils.SpriteReference) m[br.ReadInt32()];
			v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Model>) m[br.ReadInt32()];
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
			v.activateOnLivesLost = br.ReadBoolean();
			v.sharedCooldown = br.ReadBoolean();
			v.dontShowStacked = br.ReadBoolean();
			v.animateOnMainAttackDisplay = br.ReadBoolean();
			v.restrictAbilityAfterMaxRoundTimer = br.ReadBoolean();
			cooldownSpeedScaleField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			animationOffsetField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			cooldownField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_AbilityBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_MonkeyFanClubModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MonkeyFanClubModel)m[i+start];
			v.lifespan = br.ReadSingle();
			v.lifespanFrames = br.ReadInt32();
			v.maxTier = br.ReadInt32();
			v.towerCount = br.ReadInt32();
			v.range = br.ReadSingle();
			v.reloadModifier = br.ReadSingle();
			v.immuneBloonProperties = (BloonProperties) (br.ReadInt32());
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
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.handBlurEjectEffectModel = (Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectEffectModel) m[br.ReadInt32()];
			v.ejectX = br.ReadSingle();
			v.ejectY = br.ReadSingle();
			v.ejectZ = br.ReadSingle();
			v.otherDisplayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel) m[br.ReadInt32()];
			v.displayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel) m[br.ReadInt32()];
			v.leaderDisplayModel = (Assets.Scripts.Models.GenericBehaviors.DisplayModel) m[br.ReadInt32()];
			v.endDisplayModel = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectAfterTimeModel) m[br.ReadInt32()];
			v.ignoreWithMutators = br.ReadBoolean() ? null : br.ReadString();
			v.ignoreWithMutatorsList = (Il2CppStringArray) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_EjectEffectModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectEffectModel)m[i+start];
			v.assetId = br.ReadBoolean() ? null : br.ReadString();
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.lifespan = br.ReadSingle();
			v.fullscreen = br.ReadBoolean();
			v.rotateToWeapon = br.ReadBoolean();
			v.useEjectPoint = br.ReadBoolean();
			v.useEmittedFrom = br.ReadBoolean();
			v.useMainAttackRotation = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateEffectAfterTimeModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectAfterTimeModel)m[i+start];
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.lifespan = br.ReadSingle();
			v.lifespanFrames = br.ReadInt32();
			v.useRoundTime = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateEffectOnAbilityModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateEffectOnAbilityModel)m[i+start];
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.randomRotation = br.ReadBoolean();
			v.centerEffect = br.ReadBoolean();
			v.destroyOnEnd = br.ReadBoolean();
			v.useAttackTransform = br.ReadBoolean();
			v.canSave = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateSoundOnAbilityModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel)m[i+start];
			v.sound = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound2 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_DamageModifierModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.DamageModifierModel)m[i+start];
		}
	}
	
	private void Set_v_DamageModifierForTagModel_Fields(int start, int count) {
		Set_v_DamageModifierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForTagModel)m[i+start];
			v.tag = br.ReadBoolean() ? null : br.ReadString();
			v.tags = (Il2CppStringArray) m[br.ReadInt32()];
			v.damageMultiplier = br.ReadSingle();
			v.damageAddative = br.ReadSingle();
			v.mustIncludeAllTags = br.ReadBoolean();
			v.applyOverMaxDamage = br.ReadBoolean();
		}
	}
	
	private void Set_v_ProjectileBlockerCollisionReboundModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileBlockerCollisionReboundModel)m[i+start];
			v.clearCollidedWith = br.ReadBoolean();
		}
	}
	
	private void Set_v_KnockbackModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.KnockbackModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.KnockbackModel)m[i+start];
			v.moabMultiplier = br.ReadSingle();
			v.heavyMultiplier = br.ReadSingle();
			v.lightMultiplier = br.ReadSingle();
			v.mutationId = br.ReadBoolean() ? null : br.ReadString();
			v.lifespanFrames = br.ReadInt32();
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_CreateProjectileOnExhaustFractionModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExhaustFractionModel)m[i+start];
			v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel) m[br.ReadInt32()];
			v.fraction = br.ReadSingle();
			v.durationfraction = br.ReadSingle();
			v.canCreateInBetweenRounds = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateEffectOnExhaustFractionModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustFractionModel)m[i+start];
			v.assetId = br.ReadBoolean() ? null : br.ReadString();
			v.lifespan = br.ReadSingle();
			v.fullscreen = br.ReadBoolean();
			v.fraction = br.ReadSingle();
			v.durationFraction = br.ReadSingle();
			v.randomRotation = br.ReadBoolean();
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_ExpireProjectileAtScreenEdgeModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ExpireProjectileAtScreenEdgeModel)m[i+start];
		}
	}
	
	#endregion
	
	protected override Assets.Scripts.Models.Towers.TowerModel Load(byte[] bytes) {
		using (var s = new MemoryStream(bytes)) {
			using (var reader = new BinaryReader(s)) {
				this.br = reader;
				var totalCount = br.ReadInt32();
				m = new object[totalCount];
				
				//##  Step 1: create empty collections
				CreateArraySet<Assets.Scripts.Models.Model>();
				Read_a_Int32_Array();
				Read_a_AreaType_Array();
				CreateArraySet<Assets.Scripts.Models.Towers.Mods.ApplyModModel>();
				CreateArraySet<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
				CreateArraySet<Assets.Scripts.Models.Towers.Filters.FilterModel>();
				CreateArraySet<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				Read_a_String_Array();
				Read_a_TargetType_Array();
				CreateArraySet<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
				CreateListSet<Assets.Scripts.Models.Model>();
				
				//##  Step 2: create empty objects
				Create_Records<Assets.Scripts.Models.Towers.TowerModel>();
				Create_Records<Assets.Scripts.Utils.SpriteReference>();
				Create_Records<Assets.Scripts.Models.Towers.Mods.ApplyModModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnPlaceModel>();
				Create_Records<Assets.Scripts.Models.Effects.EffectModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnUpgradeModel>();
				Create_Records<Assets.Scripts.Models.Audio.SoundModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnSellModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnSellModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnTowerPlaceModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnUpgradeModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>();
				Create_Records<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.ArcEmissionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.ProjectileModel>();
				Create_Records<Assets.Scripts.Models.Towers.Filters.FilterInvisibleModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileFilterModel>();
				Create_Records<Assets.Scripts.Models.GenericBehaviors.DisplayModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.AttackFilterModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstPrioCamoModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastPrioCamoModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetClosePrioCamoModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongPrioCamoModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CircleFootprintModel>();
				Create_Records<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ShowTextOnHitModel>();
				Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.CritMultiplierModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MonkeyFanClubModel>();
				Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectEffectModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectAfterTimeModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateEffectOnAbilityModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForTagModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileBlockerCollisionReboundModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.KnockbackModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExhaustFractionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustFractionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ExpireProjectileAtScreenEdgeModel>();
				
				Set_v_TowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SpriteReference_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ApplyModModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_EffectModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SoundModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnTowerPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AttackModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_WeaponModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ArcEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterInvisibleModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TravelStraitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ProjectileFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DisplayModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SingleEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RotateToTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AttackFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetFirstPrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetLastPrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetClosePrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetStrongPrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CircleFootprintModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_UpgradePathModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ShowTextOnHitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CritMultiplierModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_MonkeyFanClubModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_EjectEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectAfterTimeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageModifierForTagModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ProjectileBlockerCollisionReboundModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_KnockbackModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateProjectileOnExhaustFractionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnExhaustFractionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ExpireProjectileAtScreenEdgeModel_Fields(br.ReadInt32(), br.ReadInt32());
				
				//##  Step 4: link object collections e.g Product[]. Note: requires object data e.g dictionary<string, value> where string = model.name
				LinkArray<Assets.Scripts.Models.Model>();
				LinkArray<Assets.Scripts.Models.Towers.Mods.ApplyModModel>();
				LinkArray<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
				LinkArray<Assets.Scripts.Models.Towers.Filters.FilterModel>();
				LinkArray<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				LinkArray<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
				LinkList<Assets.Scripts.Models.Model>();
				
				var resIndex = br.ReadInt32();
				UnityEngine.Debug.Assert(br.BaseStream.Position == br.BaseStream.Length);
				return (Assets.Scripts.Models.Towers.TowerModel) m[resIndex];
			}
		}
	}
}