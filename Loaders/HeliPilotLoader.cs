using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.Api;
using Il2Cpp;

namespace UltimateCrosspathing.Loaders;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Runtime.Serialization;
using Il2CppSystem.Reflection;
using Il2CppSystem;
using Il2CppAssets.Scripts.Simulation.SMath;
using System.IO;

public class HeliPilotLoader : ModByteLoader<Il2CppAssets.Scripts.Models.Towers.TowerModel> {
	
	BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static; 
	BinaryReader br = null;
	
	// NOTE: was a collection per type but it prevented inheriance e.g list of Products would required class type id
	protected override string BytesFileName => "HeliPilots.bytes";
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
	private void LinkModelDictionary<T>() where T : Il2CppAssets.Scripts.Models.Model {
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
			var arr = new Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.TargetType>(arrCount);
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = new Il2CppAssets.Scripts.Models.Towers.TargetType {id = br.ReadString(), isActionable = br.ReadBoolean()};
			}
			m[mIndex++] = arr;
		}
	}
	private void Read_a_AreaType_Array() {
		var arrSetCount = br.ReadInt32();
		var count = arrSetCount;
		for (var i = 0; i < count; i++) {
			var arrCount = br.ReadInt32();
			var arr = new Il2CppAssets.Scripts.Models.Map.AreaType[arrCount];
			for (var j = 0; j < arr.Length; j++) {
				arr[j] = (Il2CppAssets.Scripts.Models.Map.AreaType)br.ReadInt32();
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
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Model>();
		var _nameField = t.GetField("_name", bindFlags);
		var childDependantsField = t.GetField("childDependants", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Model)m[i+start];
			_nameField.SetValue(v,br.ReadBoolean() ? null : String.Intern(br.ReadString()));
			childDependantsField.SetValue(v,(List<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()]);
		}
	}
	
	private void Set_v_TowerModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.TowerModel)m[i+start];
			v.display = ModContent.CreatePrefabReference(br.ReadString());
			v.baseId = br.ReadBoolean() ? null : br.ReadString();
			v.cost = br.ReadSingle();
			v.radius = br.ReadSingle();
			v.radiusSquared = br.ReadSingle();
			v.range = br.ReadSingle();
			v.ignoreBlockers = br.ReadBoolean();
			v.isGlobalRange = br.ReadBoolean();
			v.tier = br.ReadInt32();
			v.tiers = (Il2CppStructArray<int>) m[br.ReadInt32()];
			v.towerSet = (Il2CppAssets.Scripts.Models.TowerSets.TowerSet) (br.ReadInt32());
			var whoCares = m[br.ReadInt32()];
			v.areaTypes = null;
			v.icon = ModContent.CreateSpriteReference(br.ReadString());
			v.portrait = ModContent.CreateSpriteReference(br.ReadString());
			v.instaIcon = ModContent.CreateSpriteReference(br.ReadString());
			v.mods = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel>) m[br.ReadInt32()];
			v.ignoreTowerForSelection = br.ReadBoolean();
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.footprint = (Il2CppAssets.Scripts.Models.Towers.Behaviors.FootprintModel) m[br.ReadInt32()];
			v.dontDisplayUpgrades = br.ReadBoolean();
			v.emoteSpriteSmall = ModContent.CreateSpriteReference(br.ReadString());
			v.emoteSpriteLarge = ModContent.CreateSpriteReference(br.ReadString());
			v.doesntRotate = br.ReadBoolean();
			v.upgrades = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>) m[br.ReadInt32()];
			v.appliedUpgrades = (Il2CppStringArray) m[br.ReadInt32()];
			v.targetTypes = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.TargetType>) m[br.ReadInt32()];
			v.paragonUpgrade = (Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel) m[br.ReadInt32()];
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
			v.dontAddMutatorsFromParent = br.ReadBoolean();
			v.displayScale = br.ReadSingle();
			v.showBuffs = br.ReadBoolean();
		}
	}
	
	private void Set_v_ApplyModModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel)m[i+start];
			v.mod = br.ReadBoolean() ? null : br.ReadString();
			v.target = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_TowerBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.TowerBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_CreateEffectOnPlaceModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnPlaceModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_EffectModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Effects.EffectModel)m[i+start];
			v.assetId = ModContent.CreatePrefabReference(br.ReadString());
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
	
	private void Set_v_CreateEffectOnSellModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnSellModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateSoundOnSellModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnSellModel)m[i+start];
			v.sound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SoundModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Audio.SoundModel)m[i+start];
			v.assetId = ModContent.CreateAudioSourceReference(br.ReadString());
		}
	}
	
	private void Set_v_CreateSoundOnUpgradeModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnUpgradeModel)m[i+start];
			v.sound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound1 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound3 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound4 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound5 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound6 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound7 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound8 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_FootprintModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.FootprintModel)m[i+start];
			v.doesntBlockTowerPlacement = br.ReadBoolean();
			v.ignoresPlacementCheck = br.ReadBoolean();
			v.ignoresTowerOverlap = br.ReadBoolean();
		}
	}
	
	private void Set_v_RectangleFootprintModel_Fields(int start, int count) {
		Set_v_FootprintModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.RectangleFootprintModel)m[i+start];
			v.xWidth = br.ReadSingle();
			v.yWidth = br.ReadSingle();
		}
	}
	
	private void Set_v_CreateSoundOnTowerPlaceModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnTowerPlaceModel)m[i+start];
			v.sound1 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound1 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateEffectOnUpgradeModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnUpgradeModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.createOnAirUnit = br.ReadBoolean();
		}
	}
	
	private void Set_v_AttackModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackModel)m[i+start];
			v.weapons = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>) m[br.ReadInt32()];
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.range = br.ReadSingle();
			v.targetProvider = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel) m[br.ReadInt32()];
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
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>();
		var animationOffsetField = t.GetField("animationOffset", bindFlags);
		var rateField = t.GetField("rate", bindFlags);
		var customStartCooldownField = t.GetField("customStartCooldown", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel)m[i+start];
			v.animation = br.ReadInt32();
			animationOffsetField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.emission = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel) m[br.ReadInt32()];
			v.ejectX = br.ReadSingle();
			v.ejectY = br.ReadSingle();
			v.ejectZ = br.ReadSingle();
			v.projectile = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.fireWithoutTarget = br.ReadBoolean();
			v.fireBetweenRounds = br.ReadBoolean();
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>) m[br.ReadInt32()];
			rateField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.useAttackPosition = br.ReadBoolean();
			v.startInCooldown = br.ReadBoolean();
			customStartCooldownField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.animateOnMainAttack = br.ReadBoolean();
			v.isStunned = br.ReadBoolean();
		}
	}
	
	private void Set_v_EmissionModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel)m[i+start];
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionBehaviorModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_EmissionWithOffsetsModel_Fields(int start, int count) {
		Set_v_EmissionModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionWithOffsetsModel)m[i+start];
			v.throwMarkerOffsetModels = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel>) m[br.ReadInt32()];
			v.projectileCount = br.ReadInt32();
			v.rotateProjectileWithTower = br.ReadBoolean();
			v.randomRotationCone = br.ReadSingle();
		}
	}
	
	private void Set_v_WeaponBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_ThrowMarkerOffsetModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel)m[i+start];
			v.ejectX = br.ReadSingle();
			v.ejectY = br.ReadSingle();
			v.ejectZ = br.ReadSingle();
			v.rotation = br.ReadSingle();
		}
	}
	
	private void Set_v_ProjectileModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel)m[i+start];
			v.display = ModContent.CreatePrefabReference(br.ReadString());
			v.id = br.ReadBoolean() ? null : br.ReadString();
			v.maxPierce = br.ReadSingle();
			v.pierce = br.ReadSingle();
			v.scale = br.ReadSingle();
			v.ignoreBlockers = br.ReadBoolean();
			v.usePointCollisionWithBloons = br.ReadBoolean();
			v.canCollisionBeBlockedByMapLos = br.ReadBoolean();
			v.filters = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()];
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
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel)m[i+start];
		}
	}
	
	private void Set_v_FilterInvisibleModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterInvisibleModel)m[i+start];
			v.isActive = br.ReadBoolean();
			v.ignoreBroadPhase = br.ReadBoolean();
		}
	}
	
	private void Set_v_ProjectileBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileBehaviorModel)m[i+start];
			v.collisionPass = br.ReadInt32();
		}
	}
	
	private void Set_v_DamageModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel)m[i+start];
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
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel>();
		var speedField = t.GetField("speed", bindFlags);
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel)m[i+start];
			speedField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_ProjectileFilterModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileFilterModel)m[i+start];
			v.filters = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_DisplayModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.GenericBehaviors.DisplayModel)m[i+start];
			v.display = ModContent.CreatePrefabReference(br.ReadString());
			v.layer = br.ReadInt32();
			v.positionOffset = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
			v.scale = br.ReadSingle();
			v.ignoreRotation = br.ReadBoolean();
			v.animationChanges = (List<Il2CppAssets.Scripts.Models.GenericBehaviors.AnimationChange>) m[br.ReadInt32()];
			v.delayedReveal = br.ReadSingle();
		}
	}
	
	private void Set_v_FireFromAirUnitModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.FireFromAirUnitModel)m[i+start];
		}
	}
	
	private void Set_v_AnimateAirUnitOnFireModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.AnimateAirUnitOnFireModel)m[i+start];
			v.animationState = br.ReadInt32();
		}
	}
	
	private void Set_v_TargetSupplierModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetSupplierModel)m[i+start];
			v.isOnSubTower = br.ReadBoolean();
		}
	}
	
	private void Set_v_AttackBehaviorModel_Fields(int start, int count) {
		Set_v_Model_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_FollowTouchSettingModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FollowTouchSettingModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_LockInPlaceSettingModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.LockInPlaceSettingModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
			v.display = ModContent.CreatePrefabReference(br.ReadString());
		}
	}
	
	private void Set_v_PatrolPointsSettingModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PatrolPointsSettingModel>();
		var lineDelayField = t.GetField("lineDelay", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PatrolPointsSettingModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
			v.pointSwitchDistance = br.ReadSingle();
			v.pointSwitchDistanceSquared = br.ReadSingle();
			v.minimumPointDistance = br.ReadSingle();
			v.minimumPointDistanceSquared = br.ReadSingle();
			v.dotSpacing = br.ReadSingle();
			v.dotOffset = br.ReadSingle();
			v.display = ModContent.CreatePrefabReference(br.ReadString());
			v.lineDisplay = ModContent.CreatePrefabReference(br.ReadString());
			lineDelayField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_RotateToTargetAirUnitModel_Fields(int start, int count) {
		Set_v_RotateToTargetModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetAirUnitModel)m[i+start];
		}
	}
	
	private void Set_v_AttackFilterModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.AttackFilterModel)m[i+start];
			v.filters = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_AttackAirUnitModel_Fields(int start, int count) {
		Set_v_AttackModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackAirUnitModel)m[i+start];
			v.displayAirUnitModel = (Il2CppAssets.Scripts.Models.GenericBehaviors.DisplayModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SingleEmissionModel_Fields(int start, int count) {
		Set_v_EmissionModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel)m[i+start];
		}
	}
	
	private void Set_v_TrackTargetModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetModel>();
		var turnRateField = t.GetField("turnRate", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetModel)m[i+start];
			v.distance = br.ReadSingle();
			v.trackNewTargets = br.ReadBoolean();
			v.constantlyAquireNewTarget = br.ReadBoolean();
			v.maxSeekAngle = br.ReadSingle();
			v.ignoreSeekAngle = br.ReadBoolean();
			v.overrideRotation = br.ReadBoolean();
			v.useLifetimeAsDistance = br.ReadBoolean();
			turnRateField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_CreateProjectileOnContactModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnContactModel)m[i+start];
			v.projectile = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.emission = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel) m[br.ReadInt32()];
			v.passOnCollidedWith = br.ReadBoolean();
			v.dontCreateAtBloon = br.ReadBoolean();
			v.passOnDirectionToContact = br.ReadBoolean();
		}
	}
	
	private void Set_v_AgeModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel)m[i+start];
			v.rounds = br.ReadInt32();
			v.useRoundTime = br.ReadBoolean();
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.endOfRoundClearBypassModel = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.EndOfRoundClearBypassModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_DamageModifierModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.DamageModifierModel)m[i+start];
		}
	}
	
	private void Set_v_DamageModifierForTagModel_Fields(int start, int count) {
		Set_v_DamageModifierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForTagModel)m[i+start];
			v.tag = br.ReadBoolean() ? null : br.ReadString();
			v.tags = (Il2CppStringArray) m[br.ReadInt32()];
			v.damageMultiplier = br.ReadSingle();
			v.damageAddative = br.ReadSingle();
			v.mustIncludeAllTags = br.ReadBoolean();
			v.applyOverMaxDamage = br.ReadBoolean();
		}
	}
	
	private void Set_v_DamageModifierForBloonTypeModel_Fields(int start, int count) {
		Set_v_DamageModifierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForBloonTypeModel)m[i+start];
			v.bloonId = br.ReadBoolean() ? null : br.ReadString();
			v.damageMultiplier = br.ReadSingle();
			v.damageAdditive = br.ReadSingle();
			v.includeChildren = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateEffectOnContactModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnContactModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreateSoundOnProjectileCollisionModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileCollisionModel)m[i+start];
			v.sound1 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound3 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound4 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound5 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_TargetFirstAirUnitModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstAirUnitModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_MoabShoveZoneModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.MoabShoveZoneModel)m[i+start];
			v.range = br.ReadSingle();
			v.moabPushSpeedScaleCap = br.ReadSingle();
			v.bfbPushSpeedScaleCap = br.ReadSingle();
			v.zomgPushSpeedScaleCap = br.ReadSingle();
			v.filterInvisibleModel = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterInvisibleModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_AirUnitModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.AirUnitModel)m[i+start];
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.TowerBehaviorModel>) m[br.ReadInt32()];
			v.display = ModContent.CreatePrefabReference(br.ReadString());
			v.displayScale = br.ReadSingle();
		}
	}
	
	private void Set_v_HeliMovementModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.HeliMovementModel)m[i+start];
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
	
	private void Set_v_CreateEffectOnAirUnitModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnAirUnitModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
			v.rotation = br.ReadSingle();
			v.scale = br.ReadSingle();
		}
	}
	
	private void Set_v_FilterMutatedTargetModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterMutatedTargetModel)m[i+start];
			v.mutationId = br.ReadBoolean() ? null : br.ReadString();
			v.mutationIds = (Il2CppStringArray) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_FilterOutTagModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterOutTagModel)m[i+start];
			v.tag = br.ReadBoolean() ? null : br.ReadString();
			v.disableWhenSupportMutatorIDs = (Il2CppStringArray) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_ProjectileBehaviorWithOverlayModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileBehaviorWithOverlayModel)m[i+start];
			v.overlayType = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_WindModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorWithOverlayModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.WindModel)m[i+start];
			v.distanceMin = br.ReadSingle();
			v.distanceMax = br.ReadSingle();
			v.chance = br.ReadSingle();
			v.affectMoab = br.ReadBoolean();
			v.distanceScaleForTags = br.ReadSingle();
			v.distanceScaleForTagsTags = br.ReadBoolean() ? null : br.ReadString();
			v.distanceScaleForTagsTagsList = (Il2CppStringArray) m[br.ReadInt32()];
			v.speedMultiplier = br.ReadSingle();
		}
	}
	
	private void Set_v_IgnoreThrowMarkerModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.IgnoreThrowMarkerModel)m[i+start];
		}
	}
	
	private void Set_v_UpgradePathModel_Fields(int start, int count) {
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
		var towerField = t.GetField("tower", bindFlags);
		var upgradeField = t.GetField("upgrade", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel)m[i+start];
			towerField.SetValue(v,br.ReadBoolean() ? null : br.ReadString());
			upgradeField.SetValue(v,br.ReadBoolean() ? null : br.ReadString());
		}
	}
	
	private void Set_v_ComancheDefenceModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.ComancheDefenceModel)m[i+start];
			v.towerModel = (Il2CppAssets.Scripts.Models.Towers.TowerModel) m[br.ReadInt32()];
			v.reinforcementCount = br.ReadInt32();
			v.durationFrames = br.ReadInt32();
			v.cooldownFrames = br.ReadInt32();
			v.maxActivationsPerRound = br.ReadInt32();
			v.immediate = br.ReadBoolean();
			v.sound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_CreditPopsToParentTowerModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreditPopsToParentTowerModel)m[i+start];
		}
	}
	
	private void Set_v_PursuitSettingModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PursuitSettingModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
			v.pursuitDistance = br.ReadSingle();
		}
	}
	
	private void Set_v_RotateToTargetModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetModel)m[i+start];
			v.onlyRotateDuringThrow = br.ReadBoolean();
			v.useThrowMarkerHeight = br.ReadBoolean();
			v.rotateOnlyOnThrow = br.ReadBoolean();
			v.additionalRotation = br.ReadInt32();
			v.rotateTower = br.ReadBoolean();
			v.useMainAttackRotation = br.ReadBoolean();
		}
	}
	
	private void Set_v_CircleFootprintModel_Fields(int start, int count) {
		Set_v_FootprintModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CircleFootprintModel)m[i+start];
			v.radius = br.ReadSingle();
		}
	}
	
	private void Set_v_AbilityModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel>();
		var cooldownSpeedScaleField = t.GetField("cooldownSpeedScale", bindFlags);
		var animationOffsetField = t.GetField("animationOffset", bindFlags);
		var cooldownField = t.GetField("cooldown", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel)m[i+start];
			v.displayName = br.ReadBoolean() ? null : br.ReadString();
			v.description = br.ReadBoolean() ? null : br.ReadString();
			v.icon = ModContent.CreateSpriteReference(br.ReadString());
			v.behaviors = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Model>) m[br.ReadInt32()];
			v.activateOnPreLeak = br.ReadBoolean();
			v.activateOnLeak = br.ReadBoolean();
			v.addedViaUpgrade = br.ReadBoolean() ? null : br.ReadString();
			v.livesCost = br.ReadInt32();
			v.maxActivationsPerRound = br.ReadInt32();
			v.animation = br.ReadInt32();
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
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.AbilityBehaviorModel)m[i+start];
		}
	}
	
	private void Set_v_RedeployModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.RedeployModel)m[i+start];
			v.selectionObjectPath = ModContent.CreatePrefabReference(br.ReadString());
			v.isSelectableGameObject = ModContent.CreatePrefabReference(br.ReadString());
			v.activateSound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.pickupSound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.dropOffSound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.isDoorGunnerActive = br.ReadBoolean();
		}
	}
	
	private void Set_v_ActivateAttackModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ActivateAttackModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ActivateAttackModel)m[i+start];
			v.attacks = (Il2CppReferenceArray<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>) m[br.ReadInt32()];
			v.processOnActivate = br.ReadBoolean();
			v.cancelIfNoTargets = br.ReadBoolean();
			v.turnOffExisting = br.ReadBoolean();
			v.endOnRoundEnd = br.ReadBoolean();
			v.endOnDefeatScreen = br.ReadBoolean();
			v.isOneShot = br.ReadBoolean();
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_FilterAllModel_Fields(int start, int count) {
		Set_v_FilterModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Filters.FilterAllModel)m[i+start];
		}
	}
	
	private void Set_v_KeepInBoundsModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.KeepInBoundsModel)m[i+start];
		}
	}
	
	private void Set_v_CreateProjectileOnExpireModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExpireModel)m[i+start];
			v.projectile = (Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel) m[br.ReadInt32()];
			v.emission = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionModel) m[br.ReadInt32()];
			v.useRotation = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateSoundOnPickupModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnPickupModel)m[i+start];
			v.sound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_PickupModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.PickupModel>();
		var delayField = t.GetField("delay", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.PickupModel)m[i+start];
			v.collectRadius = br.ReadSingle();
			delayField.SetValue(v,br.ReadSingle().ToIl2Cpp());
		}
	}
	
	private void Set_v_CashModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CashModel)m[i+start];
			v.minimum = br.ReadSingle();
			v.maximum = br.ReadSingle();
			v.bonusMultiplier = br.ReadSingle();
			v.salvage = br.ReadSingle();
			v.noTransformCash = br.ReadBoolean();
			v.distributeSalvage = br.ReadBoolean();
			v.forceCreateProjectile = br.ReadBoolean();
			v.isDoubleable = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateTextEffectModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTextEffectModel)m[i+start];
			v.assetId = ModContent.CreatePrefabReference(br.ReadString());
			v.lifespan = br.ReadSingle();
			v.useTowerPosition = br.ReadBoolean();
		}
	}
	
	private void Set_v_Assets_Scripts_Models_Towers_Projectiles_Behaviors_CreateEffectOnExpireModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel)m[i+start];
			v.assetId = ModContent.CreatePrefabReference(br.ReadString());
			v.lifespan = br.ReadSingle();
			v.fullscreen = br.ReadBoolean();
			v.randomRotation = br.ReadBoolean();
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_AngleToMapCenterModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.AngleToMapCenterModel)m[i+start];
			v.range = br.ReadSingle();
			v.offset = br.ReadSingle();
		}
	}
	
	private void Set_v_CreateSoundOnProjectileCreatedModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.CreateSoundOnProjectileCreatedModel)m[i+start];
			v.sound1 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound3 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound4 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.sound5 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.type = br.ReadBoolean() ? null : br.ReadString();
		}
	}
	
	private void Set_v_LivesModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.LivesModel)m[i+start];
			v.minimum = br.ReadSingle();
			v.maximum = br.ReadSingle();
			v.salvage = br.ReadSingle();
		}
	}
	
	private void Set_v_FindDeploymentLocationModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FindDeploymentLocationModel)m[i+start];
			v.searchRadius = br.ReadSingle();
			v.pointDistance = br.ReadSingle();
			v.towerModel = (Il2CppAssets.Scripts.Models.Towers.TowerModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_TowerExpireModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		var t = Il2CppType.Of<Il2CppAssets.Scripts.Models.Towers.Behaviors.TowerExpireModel>();
		var lifespanField = t.GetField("lifespan", bindFlags);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.TowerExpireModel)m[i+start];
			v.expireOnRoundComplete = br.ReadBoolean();
			v.expireOnDefeatScreen = br.ReadBoolean();
			lifespanField.SetValue(v,br.ReadSingle().ToIl2Cpp());
			v.rounds = br.ReadInt32();
		}
	}
	
	private void Set_v_Assets_Scripts_Models_Towers_Behaviors_CreateEffectOnExpireModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnExpireModel)m[i+start];
			v.effectModel = (Il2CppAssets.Scripts.Models.Effects.EffectModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SavedSubTowerModel_Fields(int start, int count) {
		Set_v_TowerBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.SavedSubTowerModel)m[i+start];
		}
	}
	
	private void Set_v_TargetFirstModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetLastModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetCloseModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_TargetStrongModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongModel)m[i+start];
			v.isSelectable = br.ReadBoolean();
		}
	}
	
	private void Set_v_CreateTowerModel_Fields(int start, int count) {
		Set_v_ProjectileBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTowerModel)m[i+start];
			v.tower = (Il2CppAssets.Scripts.Models.Towers.TowerModel) m[br.ReadInt32()];
			v.height = br.ReadSingle();
			v.positionAtTarget = br.ReadBoolean();
			v.destroySubTowersOnCreateNewTower = br.ReadBoolean();
			v.useProjectileRotation = br.ReadBoolean();
			v.useParentTargetPriority = br.ReadBoolean();
			v.carryMutatorsFromDestroyedTower = br.ReadBoolean();
		}
	}
	
	private void Set_v_UsePresetTargetModel_Fields(int start, int count) {
		Set_v_TargetSupplierModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.UsePresetTargetModel)m[i+start];
		}
	}
	
	private void Set_v_CreateSoundOnAbilityModel_Fields(int start, int count) {
		Set_v_AbilityBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel)m[i+start];
			v.sound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
			v.heroSound2 = (Il2CppAssets.Scripts.Models.Audio.SoundModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_ResetRateOnInitialiseModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.ResetRateOnInitialiseModel)m[i+start];
			v.weaponModel = (Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel) m[br.ReadInt32()];
		}
	}
	
	private void Set_v_SetTriggerOnAirUnitFireModel_Fields(int start, int count) {
		Set_v_WeaponBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.SetTriggerOnAirUnitFireModel)m[i+start];
			v.triggerState = br.ReadInt32();
		}
	}
	
	private void Set_v_RandomEmissionModel_Fields(int start, int count) {
		Set_v_EmissionModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.RandomEmissionModel)m[i+start];
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
	
	private void Set_v_PrioritiseRotationModel_Fields(int start, int count) {
		Set_v_AttackBehaviorModel_Fields(start, count);
		for (var i=0; i<count; i++) {
			var v = (Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PrioritiseRotationModel)m[i+start];
		}
	}
	
	#endregion
	
	protected override Il2CppAssets.Scripts.Models.Towers.TowerModel Load(byte[] bytes) {
		using (var s = new MemoryStream(bytes)) {
			using (var reader = new BinaryReader(s)) {
				this.br = reader;
				var totalCount = br.ReadInt32();
				m = new object[totalCount];
				
				//##  Step 1: create empty collections
				CreateArraySet<Il2CppAssets.Scripts.Models.Model>();
				Read_a_Int32_Array();
				Read_a_AreaType_Array();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel>();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel>();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
				Read_a_String_Array();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.TowerBehaviorModel>();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				Read_a_TargetType_Array();
				CreateArraySet<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>();
				CreateListSet<Il2CppAssets.Scripts.Models.Model>();
				
				//##  Step 2: create empty objects
				Create_Records<Il2CppAssets.Scripts.Models.Towers.TowerModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnPlaceModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Effects.EffectModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnSellModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnSellModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Audio.SoundModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnUpgradeModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.RectangleFootprintModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateSoundOnTowerPlaceModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnUpgradeModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.EmissionWithOffsetsModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.ProjectileModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterInvisibleModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.TravelStraitModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.ProjectileFilterModel>();
				Create_Records<Il2CppAssets.Scripts.Models.GenericBehaviors.DisplayModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.FireFromAirUnitModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.AnimateAirUnitOnFireModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FollowTouchSettingModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.LockInPlaceSettingModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PatrolPointsSettingModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetAirUnitModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.AttackFilterModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackAirUnitModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.SingleEmissionModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.TrackTargetModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnContactModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.AgeModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForTagModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.DamageModifierForBloonTypeModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnContactModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnProjectileCollisionModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstAirUnitModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.MoabShoveZoneModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.AirUnitModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.HeliMovementModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnAirUnitModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterMutatedTargetModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterOutTagModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.WindModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.IgnoreThrowMarkerModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.ComancheDefenceModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreditPopsToParentTowerModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PursuitSettingModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.RotateToTargetModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CircleFootprintModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.AbilityModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.RedeployModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.ActivateAttackModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Filters.FilterAllModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.KeepInBoundsModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateProjectileOnExpireModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateSoundOnPickupModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.PickupModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CashModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTextEffectModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.AngleToMapCenterModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.CreateSoundOnProjectileCreatedModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.LivesModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.FindDeploymentLocationModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.TowerExpireModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnExpireModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.SavedSubTowerModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetFirstModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetLastModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetCloseModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.TargetStrongModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateTowerModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.UsePresetTargetModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors.CreateSoundOnAbilityModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.ResetRateOnInitialiseModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.SetTriggerOnAirUnitFireModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.RandomEmissionModel>();
				Create_Records<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors.PrioritiseRotationModel>();
				
				Set_v_TowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ApplyModModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_EffectModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnSellModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SoundModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RectangleFootprintModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnTowerPlaceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnUpgradeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AttackModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_WeaponModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_EmissionWithOffsetsModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ThrowMarkerOffsetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ProjectileModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterInvisibleModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TravelStraitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ProjectileFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DisplayModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FireFromAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AnimateAirUnitOnFireModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FollowTouchSettingModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_LockInPlaceSettingModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_PatrolPointsSettingModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RotateToTargetAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AttackFilterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AttackAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SingleEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TrackTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateProjectileOnContactModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AgeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageModifierForTagModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_DamageModifierForBloonTypeModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnContactModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnProjectileCollisionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetFirstAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_MoabShoveZoneModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_HeliMovementModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateEffectOnAirUnitModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterMutatedTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterOutTagModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_WindModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_IgnoreThrowMarkerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_UpgradePathModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ComancheDefenceModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreditPopsToParentTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_PursuitSettingModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RotateToTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CircleFootprintModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RedeployModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ActivateAttackModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FilterAllModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_KeepInBoundsModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateProjectileOnExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnPickupModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_PickupModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CashModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateTextEffectModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_Assets_Scripts_Models_Towers_Projectiles_Behaviors_CreateEffectOnExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_AngleToMapCenterModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnProjectileCreatedModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_LivesModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_FindDeploymentLocationModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TowerExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_Assets_Scripts_Models_Towers_Behaviors_CreateEffectOnExpireModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SavedSubTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetFirstModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetLastModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetCloseModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_TargetStrongModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateTowerModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_UsePresetTargetModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_CreateSoundOnAbilityModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_ResetRateOnInitialiseModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_SetTriggerOnAirUnitFireModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_RandomEmissionModel_Fields(br.ReadInt32(), br.ReadInt32());
				Set_v_PrioritiseRotationModel_Fields(br.ReadInt32(), br.ReadInt32());
				
				//##  Step 4: link object collections e.g Product[]. Note: requires object data e.g dictionary<string, value> where string = model.name
				LinkArray<Il2CppAssets.Scripts.Models.Model>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Mods.ApplyModModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors.ThrowMarkerOffsetModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Filters.FilterModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Weapons.WeaponBehaviorModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.TowerBehaviorModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Upgrades.UpgradePathModel>();
				LinkArray<Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.AttackModel>();
				LinkList<Il2CppAssets.Scripts.Models.Model>();
				
				var resIndex = br.ReadInt32();
				UnityEngine.Debug.Assert(br.BaseStream.Position == br.BaseStream.Length);
				return (Il2CppAssets.Scripts.Models.Towers.TowerModel) m[resIndex];
			}
		}
	}
}
