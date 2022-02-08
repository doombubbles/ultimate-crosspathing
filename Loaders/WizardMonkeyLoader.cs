using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using BTD_Mod_Helper.Extensions;
using UltimateCrosspathing;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Runtime.Serialization;
using Il2CppSystem.Reflection;
using Il2CppSystem;
using Assets.Scripts.Simulation.SMath;
using System.IO;

public class WizardMonkeyLoader : TowersLoader {
	
	BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static; 
	BinaryReader br = null;
	
	// NOTE: was a collection per type but it prevented inheriance e.g list of Products would required class type id
	
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
	private void Read_l_String_List() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new List<string>(arrCount);
			for (var j = 0; j < arrCount; j++) {
				arr.Add( br.ReadBoolean() ? null : br.ReadString() );
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
			v.isParagon = br.ReadBoolean();
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
	
	private void Set_v_SyncTargetPriorityWithSubTowersModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.SyncTargetPriorityWithSubTowersModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
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
	
	private void Set_v_BlankSoundModel_Fields(int start, int count) {
		Set_v_SoundModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Audio.BlankSoundModel)m[i+start];
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
		}
	}
	
	private void Set_v_EmissionModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[i+start];
			v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SingleEmissionModel_Fields(int start, int count) {
		Set_v_EmissionModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel)m[i+start];
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
	
	private void Set_v_TrackTargetModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetModel>();
		var turnRateField = t.GetField("turnRate", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetModel)m[i+start];
			v.distance = br.ReadSingle();
			v.trackNewTargets = br.ReadBoolean();
			v.constantlyAquireNewTarget = br.ReadBoolean();
			v.maxSeekAngle = br.ReadSingle();
			v.ignoreSeekAngle = br.ReadBoolean();
			v.overrideRotation = br.ReadBoolean();
			v.useLifetimeAsDistance = br.ReadBoolean();
			v.turnRatePerFrame = br.ReadSingle();
			turnRateField.SetValue(v,br.ReadSingle().ToIl2Cpp());
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
	
	private void Set_v_CreateProjectileOnContactModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnContactModel)m[i+start];
			v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.emission = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel) m[br.ReadInt32()];
			v.passOnCollidedWith = br.ReadBoolean();
			v.dontCreateAtBloon = br.ReadBoolean();
			v.passOnDirectionToContact = br.ReadBoolean();
		}
	}
	
	private void Set_v_AgeModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel)m[i+start];
			v.rounds = br.ReadInt32();
			v.lifespanFrames = br.ReadInt32();
			v.useRoundTime = br.ReadBoolean();
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.endOfRoundClearBypassModel = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.EndOfRoundClearBypassModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateEffectOnContactModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnContactModel)m[i+start];
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateSoundOnProjectileCollisionModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileCollisionModel)m[i+start];
			v.sound1 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound3 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound4 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound5 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateEffectOnExhaustedModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustedModel)m[i+start];
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.assetId = br.ReadBoolean() ? null : br.ReadString();
			v.lifespan = br.ReadSingle();
			v.fullscreen = br.ReadBoolean();
			v.randomRotation = br.ReadBoolean();
		}
	}
	
	private void Set_v_RefreshPierceModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.RefreshPierceModel)m[i+start];
			v.interval = br.ReadSingle();
			v.intervalFrames = br.ReadInt32();
		}
	}
	
	private void Set_v_ClearHitBloonsModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.ClearHitBloonsModel)m[i+start];
			v.interval = br.ReadSingle();
			v.intervalFrames = br.ReadInt32();
		}
	}
	
	private void Set_v_InstantModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.InstantModel)m[i+start];
			v.destroyIfInvalid = br.ReadBoolean();
		}
	}
	
	private void Set_v_DestroyProjectileIfTowerDestroyedModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.DestroyProjectileIfTowerDestroyedModel)m[i+start];
		}
	}
	
	private void Set_v_WeaponBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_CreateSoundOnProjectileCreatedModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.CreateSoundOnProjectileCreatedModel)m[i+start];
			v.sound1 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound3 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound4 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound5 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.type = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_ZeroRotationModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.ZeroRotationModel)m[i+start];
		}
	}
	
	private void Set_v_TargetTrackOrDefaultModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetTrackOrDefaultModel)m[i+start];
			v.radius = br.ReadSingle();
			v.isSelectable = br.ReadBoolean();
			v.useTowerRange = br.ReadBoolean();
			v.forceTargetTrack = br.ReadBoolean();
			v.useClosestTrack = br.ReadBoolean();
			v.maxTrackOffset = br.ReadSingle();
		}
	}
	
	private void Set_v_FilterOfftrackModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Filters.FilterOfftrackModel)m[i+start];
		}
	}
	
	private void Set_v_AddBehaviorToBloonModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddBehaviorToBloonModel)m[i+start];
			v.mutationId = br.ReadBoolean() ? null : br.ReadString();
			v.lifespan = br.ReadSingle();
			v.layers = br.ReadInt32();
			v.lifespanFrames = br.ReadInt32();
			v.filter = (Assets.Scripts.Models.Towers.Filters.FilterModel) m[br.ReadInt32()];
			v.filters = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
			v.behaviors = (Il2CppReferenceArray<Assets.Scripts.Models.Bloons.BloonBehaviorModel>) m[br.ReadInt32()];
			v.overlays = (Dictionary<System.String, Assets.Scripts.Models.Effects.AssetPathModel>) m[br.ReadInt32()];
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
	
	private void Set_v_BloonBehaviorModelWithTowerTracking_Fields(int start, int count) {
		Set_v_BloonBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Bloons.BloonBehaviorModelWithTowerTracking)m[i+start];
		}
	}
	
	private void Set_v_BloonBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Bloons.BloonBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_DamageOverTimeModel_Fields(int start, int count) {
		Set_v_BloonBehaviorModelWithTowerTracking_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel>();
		var intervalField = t.GetField("interval", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel)m[i+start];
			v.damage = br.ReadSingle();
			v.payloadCount = br.ReadInt32();
			v.immuneBloonProperties = (BloonProperties) (br.ReadInt32());
			v.intervalFrames = br.ReadInt32();
			intervalField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.displayPath = br.ReadBoolean() ? null : br.ReadString();
			v.displayLifetime = br.ReadSingle();
			v.triggerImmediate = br.ReadBoolean();
			v.rotateEffectWithBloon = br.ReadBoolean();
			v.initialDelay = br.ReadSingle();
			v.initialDelayFrames = br.ReadInt32();
			v.damageOnDestroy = br.ReadBoolean();
			v.overrideDistributionBlocker = br.ReadBoolean();
			v.damageModifierModels = (Il2CppReferenceArray<Assets.Scripts.Models.Towers.Projectiles.DamageModifierModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_AssetPathModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Effects.AssetPathModel)m[i+start];
			v.assetPath = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_TowerCreateTowerModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.TowerCreateTowerModel)m[i+start];
			v.towerModel = (Assets.Scripts.Models.Towers.TowerModel) m[br.ReadInt32()];
			v.isAirBasedTower = br.ReadBoolean();
		}
	}
	
	private void Set_v_TowerExpireOnParentDestroyedModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.TowerExpireOnParentDestroyedModel)m[i+start];
		}
	}
	
	private void Set_v_CreditPopsToParentTowerModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CreditPopsToParentTowerModel)m[i+start];
		}
	}
	
	private void Set_v_IgnoreTowersBlockerModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.IgnoreTowersBlockerModel)m[i+start];
			v.filteredTowers = (Il2CppStringArray) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_PathMovementFromScreenCenterModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.PathMovementFromScreenCenterModel)m[i+start];
			v.speed = br.ReadSingle();
			v.speedFrames = br.ReadSingle();
			v.ignoreTargetType = br.ReadBoolean();
		}
	}
	
	private void Set_v_Assets_Scripts_Models_Towers_Behaviors_CreateEffectOnExpireModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnExpireModel)m[i+start];
			v.effectModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SavedSubTowerModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.SavedSubTowerModel)m[i+start];
		}
	}
	
	private void Set_v_EmissionBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_EmissionRotationOffBloonDirectionModel_Fields(int start, int count) {
		Set_v_EmissionBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionRotationOffBloonDirectionModel)m[i+start];
			v.useAirUnitPosition = br.ReadBoolean();
			v.dontSetAfterEmit = br.ReadBoolean();
		}
	}
	
	private void Set_v_PathMovementFromScreenCenterPatternModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PathMovementFromScreenCenterPatternModel)m[i+start];
			v.radius = br.ReadSingle();
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
			v.disabledByAnotherTower = br.ReadBoolean();
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
	
	private void Set_v_CreateSoundOnAbilityModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel)m[i+start];
			v.sound = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound2 = (Assets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
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
	
	private void Set_v_AbilityCreateTowerModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.AbilityCreateTowerModel)m[i+start];
			v.towerModel = (Assets.Scripts.Models.Towers.TowerModel) m[br.ReadInt32()];
			v.isAirBasedTower = br.ReadBoolean();
		}
	}
	
	private void Set_v_TowerExpireModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.TowerExpireModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.TowerExpireModel)m[i+start];
			v.expireOnRoundComplete = br.ReadBoolean();
			v.expireOnDefeatScreen = br.ReadBoolean();
			v.lifespanFrames = br.ReadInt32();
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
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
	
	private void Set_v_AlternateProjectileModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.AlternateProjectileModel)m[i+start];
			v.projectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.emissionModel = (Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel) m[br.ReadInt32()];
			v.interval = br.ReadInt32();
			v.alternateAnimation = br.ReadInt32();
		}
	}
	
	private void Set_v_SwitchDisplayModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.SwitchDisplayModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.SwitchDisplayModel)m[i+start];
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.lifespanFrames = br.ReadInt32();
			v.display = br.ReadBoolean() ? null : br.ReadString();
			v.excludeSubTowers = br.ReadBoolean();
			v.createEffectOnSwitchBackModel = (Assets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.resetOnDefeatScreen = br.ReadBoolean();
		}
	}
	
	private void Set_v_MutateRemoveAllAttacksOnAbilityActivateModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MutateRemoveAllAttacksOnAbilityActivateModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MutateRemoveAllAttacksOnAbilityActivateModel)m[i+start];
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.lifespanFrames = br.ReadInt32();
		}
	}
	
	private void Set_v_RemoveBloonModifiersModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.RemoveBloonModifiersModel)m[i+start];
			v.cleanseRegen = br.ReadBoolean();
			v.cleanseCamo = br.ReadBoolean();
			v.cleanseLead = br.ReadBoolean();
			v.cleanseFortified = br.ReadBoolean();
			v.cleanseOnlyIfDamaged = br.ReadBoolean();
			v.bloonTagExcludeList = (List<System.String>) m[br.ReadInt32()];
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
	
	private void Set_v_FilterOnlyCamoInModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Filters.FilterOnlyCamoInModel)m[i+start];
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
	
	private void Set_v_NecromancerZoneModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.NecromancerZoneModel)m[i+start];
			v.attackUsedForRangeModel = (Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_RotateToDefaultPositionTowerModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.RotateToDefaultPositionTowerModel)m[i+start];
			v.rotation = br.ReadSingle();
			v.onlyOnReachingTier = br.ReadInt32();
		}
	}
	
	private void Set_v_TowerBehaviorBuffModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.TowerBehaviorBuffModel)m[i+start];
			v.buffLocsName = br.ReadBoolean() ? null : br.ReadString();
			v.buffIconName = br.ReadBoolean() ? null : br.ReadString();
			v.maxStackSize = br.ReadInt32();
			v.isGlobalRange = br.ReadBoolean();
		}
	}
	
	private void Set_v_PrinceOfDarknessZombieBuffModel_Fields(int start, int count) {
		Set_v_TowerBehaviorBuffModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.PrinceOfDarknessZombieBuffModel)m[i+start];
			v.damageIncrease = br.ReadSingle();
			v.distanceMultiplier = br.ReadSingle();
		}
	}
	
	private void Set_v_BuffIndicatorModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Assets.Scripts.Models.GenericBehaviors.BuffIndicatorModel>();
		var _fullNameField = t.GetField("_fullName", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.GenericBehaviors.BuffIndicatorModel)m[i+start];
			v.buffName = br.ReadBoolean() ? null : br.ReadString();
			v.iconName = br.ReadBoolean() ? null : br.ReadString();
			v.stackable = br.ReadBoolean();
			v.maxStackSize = br.ReadInt32();
			v.globalRange = br.ReadBoolean();
			v.onlyShowBuffIfMutated = br.ReadBoolean();
			_fullNameField.SetValue(v,br.ReadBoolean() ? null : br.ReadString());
		}
	}
	
	private void Set_v_NecromancerEmissionModel_Fields(int start, int count) {
		Set_v_EmissionModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.NecromancerEmissionModel)m[i+start];
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
			v.pierceMutators = (Dictionary<System.Int32, Assets.Scripts.Models.Towers.Behaviors.Emissions.NecromancerEmissionModel.PierceMutator>) m[br.ReadInt32()];
			v.damageMutators = (Dictionary<System.Int32, Assets.Scripts.Models.Towers.Behaviors.Emissions.NecromancerEmissionModel.DamageMutator>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_TravelAlongPathModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelAlongPathModel)m[i+start];
			v.range = br.ReadSingle();
			v.reverse = br.ReadBoolean();
			v.disableRotateWithPathDirection = br.ReadBoolean();
			v.lifespanFrames = br.ReadInt32();
			v.speedFrames = br.ReadSingle();
			v.rotationLerp = br.ReadSingle();
		}
	}
	
	private void Set_v_NecroEmissionFilterModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Weapons.Behaviors.NecroEmissionFilterModel)m[i+start];
			v.isPriceOfDakrnessEmission = br.ReadBoolean();
		}
	}
	
	private void Set_v_PrinceOfDarknessEmissionModel_Fields(int start, int count) {
		Set_v_EmissionModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Emissions.PrinceOfDarknessEmissionModel)m[i+start];
			v.alternateProjectile = (Assets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.maxPathRandomRange = br.ReadInt32();
			v.minPiercePerBloon = br.ReadInt32();
			v.maxPathOffset = br.ReadInt32();
		}
	}
	
	private void Set_v_TargetTrackModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetTrackModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
			v.maxOffset = br.ReadSingle();
			v.onlyTargetPathsWithBloons = br.ReadBoolean();
		}
	}
	
	private void Set_v_NecromancerTargetTrackWithinRangeModel_Fields(int start, int count) {
		Set_v_TargetTrackModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.NecromancerTargetTrackWithinRangeModel)m[i+start];
		}
	}
	
	#endregion
	
	public override Assets.Scripts.Models.Towers.TowerModel Load(byte[] bytes) {
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
				Read_a_String_Array();
				CreateArraySet<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
				CreateArraySet<Assets.Scripts.Models.Bloons.BloonBehaviorModel>();
				CreateArraySet<Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel>();
				CreateArraySet<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				Read_a_TargetType_Array();
				CreateListSet<Assets.Scripts.Models.Model>();
				Read_l_String_List();
				CreateDictionarySet<System.String, Assets.Scripts.Models.Effects.AssetPathModel>();
				
				//##  Step 2: create empty objects
				Create_Records<Assets.Scripts.Models.Towers.TowerModel>();
				Create_Records<Assets.Scripts.Utils.SpriteReference>();
				Create_Records<Assets.Scripts.Models.Towers.Mods.ApplyModModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnPlaceModel>();
				Create_Records<Assets.Scripts.Models.Effects.EffectModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.SyncTargetPriorityWithSubTowersModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnUpgradeModel>();
				Create_Records<Assets.Scripts.Models.Audio.SoundModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnSellModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnSellModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateSoundOnTowerPlaceModel>();
				Create_Records<Assets.Scripts.Models.Audio.BlankSoundModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnUpgradeModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>();
				Create_Records<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.ProjectileModel>();
				Create_Records<Assets.Scripts.Models.Towers.Filters.FilterInvisibleModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileFilterModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetModel>();
				Create_Records<Assets.Scripts.Models.GenericBehaviors.DisplayModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForTagModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.AttackFilterModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstPrioCamoModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastPrioCamoModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetClosePrioCamoModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongPrioCamoModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnContactModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnContactModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileCollisionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExhaustedModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RefreshPierceModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.ClearHitBloonsModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.InstantModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.DestroyProjectileIfTowerDestroyedModel>();
				Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.CreateSoundOnProjectileCreatedModel>();
				Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.ZeroRotationModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetTrackOrDefaultModel>();
				Create_Records<Assets.Scripts.Models.Towers.Filters.FilterOfftrackModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.AddBehaviorToBloonModel>();
				Create_Records<Assets.Scripts.Models.Bloons.Behaviors.DamageOverTimeModel>();
				Create_Records<Assets.Scripts.Models.Effects.AssetPathModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.TowerCreateTowerModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.TowerExpireOnParentDestroyedModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreditPopsToParentTowerModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.IgnoreTowersBlockerModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.PathMovementFromScreenCenterModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CreateEffectOnExpireModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.SavedSubTowerModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors.EmissionRotationOffBloonDirectionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PathMovementFromScreenCenterPatternModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.CircleFootprintModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateEffectOnAbilityModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.AbilityCreateTowerModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.TowerExpireModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.ArcEmissionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.AlternateProjectileModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.SwitchDisplayModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.MutateRemoveAllAttacksOnAbilityActivateModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.RemoveBloonModifiersModel>();
				Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.EjectEffectModel>();
				Create_Records<Assets.Scripts.Models.Towers.Filters.FilterOnlyCamoInModel>();
				Create_Records<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.NecromancerZoneModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.RotateToDefaultPositionTowerModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.PrinceOfDarknessZombieBuffModel>();
				Create_Records<Assets.Scripts.Models.GenericBehaviors.BuffIndicatorModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.NecromancerEmissionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Projectiles.Behaviors.TravelAlongPathModel>();
				Create_Records<Assets.Scripts.Models.Towers.Weapons.Behaviors.NecroEmissionFilterModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Emissions.PrinceOfDarknessEmissionModel>();
				Create_Records<Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.NecromancerTargetTrackWithinRangeModel>();
				
				Set_v_TowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SpriteReference_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ApplyModModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_EffectModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SyncTargetPriorityWithSubTowersModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SoundModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnTowerPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_BlankSoundModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AttackModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_WeaponModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SingleEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterInvisibleModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TravelStraitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ProjectileFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TrackTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DisplayModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageModifierForTagModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RotateToTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AttackFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetFirstPrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetLastPrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetClosePrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetStrongPrioCamoModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateProjectileOnContactModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AgeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnContactModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnProjectileCollisionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnExhaustedModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RefreshPierceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ClearHitBloonsModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_InstantModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DestroyProjectileIfTowerDestroyedModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnProjectileCreatedModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ZeroRotationModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetTrackOrDefaultModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterOfftrackModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AddBehaviorToBloonModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageOverTimeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AssetPathModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TowerCreateTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TowerExpireOnParentDestroyedModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreditPopsToParentTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_IgnoreTowersBlockerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_PathMovementFromScreenCenterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_Assets_Scripts_Models_Towers_Behaviors_CreateEffectOnExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SavedSubTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_EmissionRotationOffBloonDirectionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_PathMovementFromScreenCenterPatternModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CircleFootprintModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AbilityCreateTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TowerExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ArcEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AlternateProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SwitchDisplayModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_MutateRemoveAllAttacksOnAbilityActivateModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RemoveBloonModifiersModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_EjectEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterOnlyCamoInModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_UpgradePathModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_NecromancerZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RotateToDefaultPositionTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_PrinceOfDarknessZombieBuffModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_BuffIndicatorModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_NecromancerEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TravelAlongPathModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_NecroEmissionFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_PrinceOfDarknessEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_NecromancerTargetTrackWithinRangeModel_Fields(br.ReadInt32(), br.ReadInt32());
				
				//##  Step 4: link object collections e.g Product[]. Note: requires object data e.g dictionary<string, value> where string = model.name
				LinkArray<Assets.Scripts.Models.Model>();
				LinkArray<Assets.Scripts.Models.Towers.Mods.ApplyModModel>();
				LinkArray<Assets.Scripts.Models.Towers.Weapons.WeaponModel>();
				LinkArray<Assets.Scripts.Models.Towers.Filters.FilterModel>();
				LinkArray<Assets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
				LinkArray<Assets.Scripts.Models.Bloons.BloonBehaviorModel>();
				LinkArray<Assets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel>();
				LinkArray<Assets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				LinkList<Assets.Scripts.Models.Model>();
				LinkDictionary<Assets.Scripts.Models.Effects.AssetPathModel>();
				
				var resIndex = br.ReadInt32();
				UnityEngine.Debug.Assert(br.BaseStream.Position == br.BaseStream.Length);
				return (Assets.Scripts.Models.Towers.TowerModel) m[resIndex];
			}
		}
	}
}
