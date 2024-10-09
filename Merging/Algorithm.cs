using System;
using System.Collections.Generic;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppSystem.Reflection;
using Array = Il2CppSystem.Array;
using Exception = System.Exception;
using Math = System.Math;
using Object = Il2CppSystem.Object;
using Type = Il2CppSystem.Type;

namespace UltimateCrosspathing.Merging
{
    public static class DeepMerging
    {
        private static readonly HashSet<string> DontMerge =
        [
            "animation",
            "offsetX",
            "offsetY",
            "offsetZ",
            "ejectX",
            "ejectY",
            "ejectZ",
            "rate",
            "rateFrames",
            "isPowerTower",
            "isGeraldoItem"
        ];

        private static readonly HashSet<string> Multiplicative =
        [
            "pierce",
            "range"
        ];

        private static readonly Dictionary<string, string> StringOverrides = new()
        {
            { "fcddee8a92f5d2e4d8605a8924566620", "69bf8d5932f2bea4f9ce36f861240d2e" }, //DartMonkey-340
            { "0ddd8752be0d3554cb0db6abe6686e8e", "69bf8d5932f2bea4f9ce36f861240d2e" } //DartMonkey-043
        };

        private static readonly Dictionary<Tuple<string, Type>, bool> BetterBooleans = new()
        {
            { new Tuple<string, Type>("isActive", Il2CppType.Of<FilterModel>()), false },
            { new Tuple<string, Type>("ignoreBlockers", Il2CppType.Of<ProjectileModel>()), true },
            { new Tuple<string, Type>("isSharedRangeEnabled", Il2CppType.Of<TargetSupplierModel>()), true },
        };


        private static Object DeepMerge(Object left, Object right, Object ancestor, History history,
            bool shallow = false)
        {
            if (right == null)
            {
                return left;
            }

            if (left == null)
            {
                return right;
            }

            history.Push(left, right, ancestor);

            try
            {
                // Without this, there is inconsistent handling of the WeaponModels rate and rateFrames fields
                if (left.IsType<WeaponModel>(out var leftWeapon)
                    && right.IsType<WeaponModel>(out var rightWeapon)
                    && ancestor != null && ancestor.IsType<WeaponModel>(out var ancestorWeapon))
                {
                    leftWeapon.Rate *= rightWeapon.Rate / ancestorWeapon.Rate;
                }

                if (left.IsType<Model>(out var leftModel) && right.IsType<Model>(out var rightModel) &&
                    !(ModelsAreTheSame(leftModel, rightModel, false, history) || shallow))
                {
                    return MergeDifferentModels(left, right, ancestor, history);
                }

                var leftFields = left.GetIl2CppType().GetFields();
                foreach (var fieldInfo in leftFields)
                {
                    var fieldName = fieldInfo.Name;

                    if (DontMerge.Any(s => fieldName.Contains(s))) continue;

                    MergeField(fieldInfo, left, right, ancestor, history, shallow);
                }

                var leftProperties = left.GetIl2CppType().GetProperties();
                foreach (var propertyInfo in leftProperties)
                {
                    var propertyName = propertyInfo.Name;

                    if (propertyName.ToUpper()[0] == propertyName[0] || DontMerge.Any(s => propertyName.Contains(s)))
                    {
                        //skip capitalized ones, they seem like weird ones
                        continue;
                    }

                    MergeField(propertyInfo, left, right, ancestor, history, shallow);
                }

                return left;
            }
            finally
            {
                history.Pop();
            }
        }

        public static void MergeField(MemberInfo memberInfo, Object left, Object right, Object ancestor,
            History history = null, bool shallow = false)
        {
            var memberType = memberInfo.Type();
            var leftValue = memberInfo.GetValue(left);
            var rightValue = memberInfo.GetValue(right);

            if (history == null)
            {
                history = new History();
                history.Push(left, right, ancestor);
            }

            Log($"{memberInfo.Name} ({memberType.Name})", history.depth);

            Object ancestorValue = null;
            if (ancestor != null && ancestor.GetIl2CppType().GetMember(memberInfo.Name).Length > 0)
            {
                ancestorValue = memberInfo.GetValue(ancestor);
            }

            if (leftValue == null && rightValue == null)
            {
                return;
            }

            if (memberType.IsArray)
            {
                memberInfo.SetValue(left,
                    MergeArray(memberInfo, leftValue, rightValue, ancestorValue, history, shallow));
            }
            else if (memberType.IsType<float>())
            {
                memberInfo.SetValue(left, MergeFloat(memberInfo, leftValue, rightValue, ancestorValue));
            }
            else if (memberType.IsType<int>())
            {
                memberInfo.SetValue(left, MergeInt(memberInfo, leftValue, rightValue, ancestorValue));
            }
            else if (memberType.IsType<bool>())
            {
                memberInfo.SetValue(left, MergeBool(memberInfo, leftValue, rightValue, history));
            }
            else if (memberType.IsType<Model>())
            {
                memberInfo.SetValue(left, DeepMerge(leftValue, rightValue, ancestorValue, history));
            }
            else if (memberType.IsType<string>())
            {
                var fieldInfo = memberInfo.TryCast<FieldInfo>();
                if (fieldInfo == null || (!fieldInfo.IsLiteral && !fieldInfo.IsInitOnly))
                {
                    memberInfo.SetValue(left, MergeString(memberInfo, leftValue, rightValue, ancestorValue));
                }
            }
            else if (memberType.IsType<BloonProperties>())
            {
                var leftProps = leftValue.Unbox<BloonProperties>();
                var rightProps = rightValue.Unbox<BloonProperties>();
                var result = (int)(leftProps & rightProps);
                memberInfo.SetValue(left, result.ToIl2Cpp());
            }
        }

        private static Object MergeArray(MemberInfo memberInfo, Object left, Object right, Object ancestor,
            History history, bool shallow = false)
        {
            if (left == null && right != null)
            {
                return right;
            }

            if (right == null && left != null)
            {
                return left;
            }

            history.depth++;
            var elementType = memberInfo.Type().GetElementType();
            var stuff = new List<Object>();

            var leftStuff = new List<Object>();
            foreach (var o in left.Cast<Array>())
            {
                leftStuff.Add(o);
            }

            var rightStuff = new List<Object>();
            foreach (var o in right.Cast<Array>())
            {
                rightStuff.Add(o);
            }

            var ancestorStuff = new List<Object>();
            if (ancestor != null)
            {
                foreach (var o in ancestor.Cast<Array>())
                {
                    ancestorStuff.Add(o);
                }
            }


            if (elementType.IsType<Model>())
            {
                foreach (var leftThing in leftStuff)
                {
                    var leftModel = leftThing.Cast<Model>();

                    var rightModel = rightStuff
                        .Select(rightThing => rightThing.Cast<Model>())
                        .FirstOrDefault(model => ModelsAreTheSame(leftModel, model, true, history));

                    if (rightModel != null && !shallow)
                    {
                        var ancestorModel = ancestorStuff
                            .Select(ancestorThing => ancestorThing.Cast<Model>())
                            .FirstOrDefault(model => ModelsAreTheSame(leftModel, model, true, history));

                        Log($"{leftModel.name} ({leftModel.GetIl2CppType().Name})", history.depth);

                        DeepMerge(leftModel, rightModel, ancestorModel, history);
                    }

                    stuff.Add(leftModel);


                    if (leftModel.IsType<AttackModel>(out var attackModel)) // Newly added attacks 
                    {
                        var leftTowerModel = history.GetLeft<TowerModel>();
                        var rightTowerModel = history.GetRight<TowerModel>();
                        if (Math.Abs(rightTowerModel.range - attackModel.range) < 1e7 &&
                            leftTowerModel.range > rightTowerModel.range)
                        {
                            attackModel.range = Math.Max(leftTowerModel.range, attackModel.range);
                        }
                    }
                }

                foreach (var rightThing in rightStuff)
                {
                    var rightModel = rightThing.Cast<Model>();

                    var leftModel = leftStuff
                        .Select(leftThing => leftThing.Cast<Model>())
                        .FirstOrDefault(model => ModelsAreTheSame(model, rightModel, true, history));

                    if (leftModel == null)
                    {
                        stuff.Add(rightModel);
                        var peek = history.left.Peek();
                        if (peek != null && peek.IsType<Model>(out var model))
                        {
                            model.AddChildDependant(rightModel);
                        }


                        if (rightModel.IsType<AttackModel>(out var attackModel)) // Newly added attacks 
                        {
                            var leftTowerModel = history.GetLeft<TowerModel>();
                            var rightTowerModel = history.GetRight<TowerModel>();
                            if (Math.Abs(rightTowerModel.range - attackModel.range) < 1e7 &&
                                leftTowerModel.range > rightTowerModel.range)
                            {
                                attackModel.range = Math.Max(leftTowerModel.range, attackModel.range);
                            }
                        }
                    }
                }
            }
            else if (memberInfo.Name == "collisionPasses")
            {
                stuff = rightStuff.Count > leftStuff.Count ? rightStuff : leftStuff;
            }
            else
            {
                //what to do with arrays that aren't just more models?

                if (leftStuff.Count == ancestorStuff.Count && rightStuff.Count != ancestorStuff.Count)
                {
                    stuff = rightStuff;
                }
                else
                {
                    stuff = leftStuff;
                }
            }


            var result = Array.CreateInstance(memberInfo.Type().GetElementType(), stuff.Count);
            for (var i = 0; i < stuff.Count; i++)
            {
                result.SetValue(stuff[i], i);
            }

            history.depth--;
            return result;
        }

        /// <summary>
        /// Merge two integers with respect to a common ancestor
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <param name="ancestorValue"></param>
        /// <returns></returns>
        private static Object MergeInt(MemberInfo memberInfo, Object leftValue, Object rightValue, Object ancestorValue)
        {
            if (leftValue == null)
            {
                return rightValue;
            }

            if (rightValue == null)
            {
                return leftValue;
            }

            var leftInt = leftValue.Unbox<int>();
            var rightInt = rightValue.Unbox<int>();


            if (ancestorValue != null && leftInt != rightInt)
            {
                var ancestorInt = ancestorValue.Unbox<int>();

                if (Multiplicative.Any(s => memberInfo.Name.Contains(s)))
                {
                    leftInt = leftInt * rightInt / ancestorInt;
                }
                else
                {
                    leftInt += rightInt - ancestorInt;
                }
            }

            return leftInt.ToIl2Cpp();
        }

        /// <summary>
        /// Merge two floats with respect to a common ancestor
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <param name="ancestorValue"></param>
        /// <returns></returns>
        private static Object MergeFloat(MemberInfo memberInfo, Object leftValue, Object rightValue,
            Object ancestorValue)
        {
            if (leftValue == null)
            {
                return rightValue;
            }

            if (rightValue == null)
            {
                return leftValue;
            }

            var leftFloat = leftValue.Unbox<float>();
            var rightFloat = rightValue.Unbox<float>();

            if (ancestorValue != null && Math.Abs(leftFloat - rightFloat) > 1e-7)
            {
                var ancestorFloat = ancestorValue.Unbox<float>();

                if (Multiplicative.Any(s => memberInfo.Name.Contains(s)))
                {
                    leftFloat *= rightFloat / ancestorFloat;
                }
                else
                {
                    leftFloat += rightFloat - ancestorFloat;
                }
            }

            return leftFloat.ToIl2Cpp();
        }


        /// <summary>
        /// Merge two booleans with respect to a common ancestor
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        private static Object MergeBool(MemberInfo memberInfo, Object leftValue, Object rightValue, History history)
        {
            if (leftValue == null)
            {
                return rightValue;
            }

            if (rightValue == null)
            {
                return leftValue;
            }

            var fieldName = memberInfo.Name;
            var leftBool = leftValue.Unbox<bool>();
            var rightBool = rightValue.Unbox<bool>();

            if (leftBool != rightBool)
            {
                foreach (var ((name, type), value) in BetterBooleans)
                {
                    if (fieldName.Contains((string) name))
                    {
                        if (history.GetLeft<Model>().GetIl2CppType().IsSubclassOf(type))
                        {
                            return value.ToIl2Cpp();
                        }
                    }
                }
            }

            return leftBool.ToIl2Cpp();
        }


        /// <summary>
        /// Merge two strings with respect to a common ancestor
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <param name="ancestorValue"></param>
        /// <returns></returns>
        private static Object MergeString(MemberInfo memberInfo, Object leftValue, Object rightValue,
            Object ancestorValue)
        {
            try
            {
                if (leftValue == null)
                {
                    return rightValue;
                }

                if (rightValue == null)
                {
                    return leftValue;
                }

                var leftString = leftValue.ToString();
                var rightString = rightValue.ToString();


                if (StringOverrides.ContainsKey(leftString) && StringOverrides[leftString] == rightString)
                {
                    return leftString;
                }

                if (StringOverrides.ContainsKey(rightString) && StringOverrides[rightString] == leftString)
                {
                    return rightString;
                }

                if (ancestorValue != null)
                {
                    var ancestorString = ancestorValue.ToString();

                    if (leftString == ancestorString && rightString != ancestorString)
                    {
                        return rightValue;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to merge strings for {memberInfo.Name}", e);
            }


            return leftValue;
        }


        /// <summary>
        /// Attempt to merge two different Model objects that are not the same type
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="ancestor"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        private static Object MergeDifferentModels(Object left, Object right, Object ancestor, History history)
        {
            var leftModel = left.Cast<Model>();
            var rightModel = right.Cast<Model>();
            Model ancestorModel = null;
            if (ancestor != null)
            {
                ancestorModel = ancestor.Cast<Model>();
            }

            if (leftModel.IsType<EmissionModel>(out var leftEmission) &&
                rightModel.IsType<EmissionModel>(out var rightEmission))
            {
                if (rightModel.IsType<SendToBankModel>())
                {
                    return rightModel;
                }

                var leftCount = GetCountForEmissionModel(left, leftEmission);
                var rightCount = GetCountForEmissionModel(right, rightEmission);

                //Ring of Fire type things
                if (history.GetLeft<TowerModel>()?.GetBehavior<LinkProjectileRadiusToTowerRangeModel>() != null)
                {
                    var leftWeapon = history.GetLeft<WeaponModel>();
                    var ancestorWeapon = history.GetAncestor<WeaponModel>();
                    if (leftWeapon != null && ancestorWeapon != null)
                    {
                        var ancestorCount = GetCountForEmissionModel(ancestor, ancestorModel.Cast<EmissionModel>());

                        leftWeapon.projectile.GetDamageModel().damage =
                            (float)Math.Round(
                                leftWeapon.projectile.GetDamageModel().damage * rightCount / ancestorCount);

                        history.GetLeft<TowerModel>().GetBehavior<LinkProjectileRadiusToTowerRangeModel>()
                            .projectileModel = leftWeapon.projectile;
                    }

                    return leftModel;
                }

                return rightCount > leftCount ? rightModel : leftModel;
            }

            if (leftModel.IsType<ProjectileModel>(out var leftProjectile) &&
                rightModel.IsType<ProjectileModel>(out var rightProjectile) &&
                ancestorModel.IsType<ProjectileModel>(out var ancestorProjectile))
            {
                // Try out a shallow merge
                return DeepMerge(leftModel, rightModel, ancestor, history, true);

                /*if (leftProjectile.id == ancestorProjectile.id && leftProjectile.display ==
                                                               ancestorProjectile.display
                                                               && rightProjectile.id != ancestorProjectile.id &&
                                                               rightProjectile.display !=
                                                               ancestorProjectile.display)
                {
                    ModHelper.Msg<UltimateCrosspathingMod>("overriding projectile hmmmm");
                    return rightModel;
                }*/
            }

            //ModHelper.Msg<UltimateCrosspathingMod>($"Default merge for {leftModel.GetIl2CppType().Name} and {rightModel.GetIl2CppType().Name}");
            return leftModel;
        }

        private static Object ShallowMerge(Object left, Object right, Object ancestor, History history)
        {
            var leftFields = left.GetIl2CppType().GetFields();
            foreach (var memberInfo in leftFields)
            {
                var leftValue = memberInfo.GetValue(left);
                var rightValue = memberInfo.GetValue(right);
                var fieldName = memberInfo.Name;

                if (DontMerge.Any(s => fieldName.Contains(s))) continue;
                if (memberInfo.Type().IsArray)
                {
                    memberInfo.SetValue(left, MergeArray(memberInfo, leftValue, rightValue, ancestor, history, true));
                }
                else
                {
                    MergeField(memberInfo, left, right, ancestor, history);
                }
            }

            var leftProperties = left.GetIl2CppType().GetProperties();
            foreach (var memberInfo in leftProperties)
            {
                var leftValue = memberInfo.GetValue(left);
                var rightValue = memberInfo.GetValue(right);
                var propertyName = memberInfo.Name;

                if (propertyName.ToUpper()[0] == propertyName[0] || DontMerge.Any(s => propertyName.Contains(s)))
                {
                    //skip capitalized ones, they seem like weird ones
                    continue;
                }

                if (memberInfo.Type().IsArray)
                {
                    memberInfo.SetValue(left, MergeArray(memberInfo, left, right, ancestor, history, true));
                }
                else
                {
                    MergeField(memberInfo, left, right, ancestor, history);
                }
            }

            return left;
        }

        private static int GetCountForEmissionModel(Object methodInfo, EmissionModel emissionModel)
        {
            var count = 1;
            if (emissionModel.IsType<LineProjectileEmissionModel>())
            {
                count = 10000; // Lines should always take priority
            }

            var maybeCount = emissionModel.GetIl2CppType().GetProperty("count");
            if (maybeCount != null)
            {
                var value = maybeCount.GetValue(methodInfo);
                if (value != null) count = value.Unbox<int>();
            }

            var maybeCount2 = emissionModel.GetIl2CppType().GetField("count");
            if (maybeCount2 != null)
            {
                var value = maybeCount2.GetValue(methodInfo);
                if (value != null) count = value.Unbox<int>();
            }

            return count;
        }

        private static void Log(string msg, int depth)
        {
            for (var i = 0; i < depth; i++)
            {
                msg = "|  " + msg;
            }
            //ModHelper.Msg<UltimateCrosspathingMod>(msg);
        }

        private static bool ModelsAreTheSame(Model leftModel, Model rightModel, bool array, History history)
        {
            if (leftModel.IsType<AbilityModel>(out var leftAbility) &&
                rightModel.IsType<AbilityModel>(out var rightAbility))
            {
                return leftAbility.displayName == rightAbility.displayName;
            }

            if (leftModel.IsType<TowerBehaviorModel>() && rightModel.IsType<TowerBehaviorModel>() &&
                leftModel.name.Contains("Create") && rightModel.name.Contains("Create") &&
                leftModel.name.Contains("On") && rightModel.name.Contains("On") &&
                leftModel.GetIl2CppType().Name == rightModel.GetIl2CppType().Name)
            {
                return true;
            }

            if (leftModel.IsType<ProjectileModel>() && rightModel.IsType<ProjectileModel>())
            {
                var leftProjectile = leftModel.Cast<ProjectileModel>();
                var rightProjectile = rightModel.Cast<ProjectileModel>();
                return leftProjectile.id == rightProjectile.id || array;
            }

            if (leftModel.IsType<AttackModel>(out var leftAttack) &&
                rightModel.IsType<AttackModel>(out var rightAttack))
            {
                var lineLeft = leftAttack.weapons.Any(weapon => weapon.emission.IsType<LineProjectileEmissionModel>());
                var line = rightAttack.weapons.Any(weapon => weapon.emission.IsType<LineProjectileEmissionModel>());
                if (line != lineLeft)
                {
                    return false;
                }

                if (leftAttack.HasBehavior<RotateToTargetModel>() != rightAttack.HasBehavior<RotateToTargetModel>())
                {
                    return false;
                }

                var leftDisplay = leftAttack.GetBehavior<DisplayModel>();
                var rightDisplay = rightAttack.GetBehavior<DisplayModel>();
                if (leftDisplay != null && rightDisplay != null)
                {
                    if (leftDisplay.display != rightDisplay.display)
                    {
                        return false;
                    }
                }
                else if (!(leftDisplay == null && rightDisplay == null))
                {
                    /*var ancestor = history.GetAncestor<TowerModel>();
                    var ancestorWeapon = ancestor.GetWeapon();
                    if (ancestorWeapon != null)
                    {
                        var ancestorDisplay = ancestorWeapon.projectile.display;
                        var match = leftAttack.weapons[0].projectile.display == ancestorDisplay 
                                    || rightAttack.weapons[0].projectile.display == ancestorDisplay;
                    }*/
                    return false;
                }
            }

            return rightModel.name == leftModel.name
                   && rightModel.GetIl2CppType().Name == leftModel.GetIl2CppType().Name;
        }

        /*private static bool IsType<T>(this Object objectBase)
        {
            return objectBase.GetIl2CppType().IsType<T>();
        }*/

        private static bool IsType<T>(this Type typ)
        {
            if (typeof(T).IsEnum)
            {
                return typ.FullName.StartsWith(typeof(T).FullName!);
            }
            var ty = Il2CppType.From(typeof(T));
            return ty.IsAssignableFrom(typ);
        }

        public static Type Type(this MemberInfo memberInfo)
        {
            if (memberInfo.GetIl2CppType().IsType<FieldInfo>())
            {
                return memberInfo.Cast<FieldInfo>().FieldType;
            }

            if (memberInfo.GetIl2CppType().IsType<PropertyInfo>())
            {
                return memberInfo.Cast<PropertyInfo>().PropertyType;
            }

            return null;
        }

        public static Object GetValue(this MemberInfo memberInfo, Object obj)
        {
            if (memberInfo.GetIl2CppType().IsType<FieldInfo>())
            {
                return memberInfo.Cast<FieldInfo>().GetValue(obj);
            }

            if (memberInfo.GetIl2CppType().IsType<PropertyInfo>())
            {
                return memberInfo.Cast<PropertyInfo>().GetValue(obj);
            }

            return null;
        }

        public static void SetValue(this MemberInfo memberInfo, Object obj, Object newValue)
        {
            if (memberInfo.GetIl2CppType().IsType<FieldInfo>())
            {
                memberInfo.Cast<FieldInfo>().SetValue(obj, newValue);
            }

            if (memberInfo.GetIl2CppType().IsType<PropertyInfo>())
            {
                memberInfo.Cast<PropertyInfo>().SetValue(obj, newValue, null);
            }
        }

        public class History
        {
            public readonly Stack<Object> left;
            public readonly Stack<Object> right;
            public readonly Stack<Object> ancestor;

            public int depth;

            public History()
            {
                left = new Stack<Object>();
                right = new Stack<Object>();
                ancestor = new Stack<Object>();
                depth = -1;
            }

            public void Push(Object l, Object r, Object a)
            {
                left.Push(l);
                right.Push(r);
                ancestor.Push(a);
                depth++;
            }

            public Object Pop()
            {
                var pop = left.Pop();
                right.Pop();
                ancestor.Pop();
                depth--;
                return pop;
            }

            public T GetLeft<T>() where T : Object
            {
                return (from o in left where o.IsType<T>() select o.Cast<T>()).FirstOrDefault();
            }

            public T GetRight<T>() where T : Object
            {
                return (from o in right where o.IsType<T>() select o.Cast<T>()).FirstOrDefault();
            }

            public T GetAncestor<T>() where T : Object
            {
                return (from o in ancestor where o.IsType<T>() select o.Cast<T>()).FirstOrDefault();
            }
        }
    }
}
