using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Filters;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Weapons;
using BTD_Mod_Helper.Extensions;
using Il2CppSystem;
using Il2CppSystem.Reflection;
using MelonLoader;
using UnhollowerRuntimeLib;
using Array = Il2CppSystem.Array;
using Boolean = Il2CppSystem.Boolean;
using Console = System.Console;
using Exception = System.Exception;
using Math = System.Math;
using Object = Il2CppSystem.Object;
using Type = Il2CppSystem.Type;

namespace UltimateCrosspathing
{
    public static class DeepMerging
    {
        private static readonly HashSet<string> DONT_MERGE = new HashSet<string>
        {
            "animation",
            "offsetX",
            "offsetY",
            "offsetZ",
            "ejectX",
            "ejectY",
            "ejectZ",
            "rate",
            "rateFrames",
            "isPowerTower"
        };

        private static readonly HashSet<string> MULTIPLICATIVE = new HashSet<string>
        {
            "pierce",
            "range"
        };

        private static readonly Dictionary<string, string> STRING_OVERRIDES = new Dictionary<string, string>
        {
            { "fcddee8a92f5d2e4d8605a8924566620", "69bf8d5932f2bea4f9ce36f861240d2e" }, //DartMonkey-340
            { "0ddd8752be0d3554cb0db6abe6686e8e", "69bf8d5932f2bea4f9ce36f861240d2e" } //DartMonkey-043
        };

        private static readonly Dictionary<(string, Type), bool> BETTER_BOOLEANS = new Dictionary<(string, Type), bool>
        {
            { ("isActive", Il2CppType.Of<FilterModel>()), false },
            { ("ignoreBlockers", Il2CppType.Of<ProjectileModel>()), true }
        };


        private static Object DeepMerge(Object left, Object right, Object ancestor, History history)
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

            // Without this, there is inconsistent handling of the WeaponModels rate and rateFrames fields
            if (left.IsType<WeaponModel>(out var leftWeapon)
                && right.IsType<WeaponModel>(out var rightWeapon)
                && ancestor != null && ancestor.IsType<WeaponModel>(out var ancestorWeapon))
            {
                leftWeapon.Rate *= rightWeapon.Rate / ancestorWeapon.Rate;
            }

            if (left.IsType<Model>(out var leftModel) && right.IsType<Model>(out var rightModel) &&
                !ModelsAreTheSame(leftModel, rightModel, false))
            {
                return MergeDifferentModels(left, right, ancestor, history);
            }

            var leftFields = left.GetIl2CppType().GetFields();
            foreach (var fieldInfo in leftFields)
            {
                var fieldName = fieldInfo.Name;

                if (DONT_MERGE.Any(s => fieldName.Contains(s))) continue;

                MergeField(fieldInfo, left, right, ancestor, history);
            }

            var leftProperties = left.GetIl2CppType().GetProperties();
            foreach (var propertyInfo in leftProperties)
            {
                var propertyName = propertyInfo.Name;

                if (propertyName.ToUpper()[0] == propertyName[0] || DONT_MERGE.Any(s => propertyName.Contains(s)))
                {
                    //skip capitalized ones, they seem like weird ones
                    continue;
                }

                MergeField(propertyInfo, left, right, ancestor, history);
            }

            history.Pop();

            return left;
        }

        public static void MergeField(MemberInfo memberInfo, Object left, Object right, Object ancestor,
            History history = null)
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
            if (ancestor != null)
            {
                ancestorValue = memberInfo.GetValue(ancestor);
            }

            if (leftValue == null && rightValue == null)
            {
                return;
            }

            if (memberType.IsArray)
            {
                memberInfo.SetValue(left, MergeArray(memberInfo, leftValue, rightValue, ancestorValue, history));
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
                memberInfo.SetValue(left, MergeBool(memberInfo, leftValue, rightValue));
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

                memberInfo.SetValue(left, new Int32 { m_value = result }.BoxIl2CppObject());
            }
        }

        private static Object MergeArray(MemberInfo memberInfo, Object left, Object right, Object ancestor,
            History history)
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


            if (memberInfo.Name.Contains("damageTypes") &&
                (leftStuff.Contains("Normal") || rightStuff.Contains("Normal")))
            {
                MelonLogger.Msg("It thinks damage types are real");
                stuff.Add("Normal");
            }
            else
            {
                if (elementType.IsType<Model>())
                {
                    foreach (var leftThing in leftStuff)
                    {
                        var leftModel = leftThing.Cast<Model>();

                        var rightModel = rightStuff
                            .Select(rightThing => rightThing.Cast<Model>())
                            .FirstOrDefault(model => ModelsAreTheSame(leftModel, model, true));

                        if (rightModel != null)
                        {
                            var ancestorModel = ancestorStuff
                                .Select(ancestorThing => ancestorThing.Cast<Model>())
                                .FirstOrDefault(model => ModelsAreTheSame(leftModel, model, true));

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
                                attackModel.range = leftTowerModel.range;
                            }
                        }
                    }

                    foreach (var rightThing in rightStuff)
                    {
                        var rightModel = rightThing.Cast<Model>();

                        var leftModel = leftStuff
                            .Select(leftThing => leftThing.Cast<Model>())
                            .FirstOrDefault(model => ModelsAreTheSame(model, rightModel, true));

                        if (leftModel == null)
                        {
                            stuff.Add(rightModel);
                            var peek = history.left.Peek();
                            if (peek != null && peek.IsType<Model>(out var model))
                            {
                                model.AddChildDependant(rightModel);
                            }
                        }
                    }
                }
                else
                {
                    //what to do with arrays that aren't just more models?
                    //int arrays are 

                    if (leftStuff.Count == ancestorStuff.Count && rightStuff.Count != ancestorStuff.Count)
                    {
                        stuff = rightStuff;
                    }
                    else
                    {
                        stuff = leftStuff;
                    }
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

                if (MULTIPLICATIVE.Any(s => memberInfo.Name.Contains(s)))
                {
                    leftInt = leftInt * rightInt / ancestorInt;
                }
                else
                {
                    leftInt += rightInt - ancestorInt;
                }
            }

            return new Int32 { m_value = leftInt }.BoxIl2CppObject();
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

                if (MULTIPLICATIVE.Any(s => memberInfo.Name.Contains(s)))
                {
                    leftFloat *= rightFloat / ancestorFloat;
                }
                else
                {
                    leftFloat += rightFloat - ancestorFloat;
                }
            }

            return new Single { m_value = leftFloat }.BoxIl2CppObject();
        }


        /// <summary>
        /// Merge two booleans with respect to a common ancestor
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        private static Object MergeBool(MemberInfo memberInfo, Object leftValue, Object rightValue)
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
            var fieldType = memberInfo.Type();
            var leftBool = leftValue.Unbox<bool>();
            var rightBool = rightValue.Unbox<bool>();

            if (leftBool != rightBool)
            {
                var better = BETTER_BOOLEANS.FirstOrDefault(pair =>
                    fieldName.Contains(pair.Key.Item1) && fieldType.IsAssignableFrom(pair.Key.Item2));
                if (!better.Equals(default))
                {
                    return new Boolean { m_value = better.Value }.BoxIl2CppObject();
                }
            }

            return new Boolean { m_value = leftBool }.BoxIl2CppObject();
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


                if (STRING_OVERRIDES.ContainsKey(leftString) && STRING_OVERRIDES[leftString] == rightString)
                {
                    return leftString;
                }

                if (STRING_OVERRIDES.ContainsKey(rightString) && STRING_OVERRIDES[rightString] == leftString)
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
            try
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

                if (leftModel.IsType<ProjectileModel>() && rightModel.IsType<ProjectileModel>() &&
                    ancestorModel != null)
                {
                    var leftProjectile = leftModel.Cast<ProjectileModel>();
                    var rightProjectile = rightModel.Cast<ProjectileModel>();
                    var ancestorProjectile = ancestorModel.Cast<ProjectileModel>();

                    if (leftProjectile.id == ancestorProjectile.id && leftProjectile.display ==
                                                                   ancestorProjectile.display
                                                                   && rightProjectile.id != ancestorProjectile.id &&
                                                                   rightProjectile.display !=
                                                                   ancestorProjectile.display)
                    {
                        // MelonLogger.Msg("overriding projectile hmmmm");
                        return rightModel;
                    }
                }

                //MelonLogger.Msg($"Default merge for {leftModel.GetIl2CppType().Name} and {rightModel.GetIl2CppType().Name}");
                return leftModel;
            }
            catch (Exception e)
            {
                throw new Exception("Failed merging different models", e);
            }
            finally
            {
                history.Pop();
            }
        }

        private static int GetCountForEmissionModel(Object methodInfo, EmissionModel emissionModel)
        {
            var count = 1;
            if (emissionModel.IsType<LineProjectileEmissionModel>())
            {
                count = 10000;  // Lines should always take priority
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
            //MelonLogger.Msg(msg);
        }

        private static bool ModelsAreTheSame(Model leftModel, Model rightModel, bool array)
        {
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
                if (leftProjectile.id != rightProjectile.id && !array)
                {
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
            var ty = Il2CppType.From(typeof(T));
            return ty.IsAssignableFrom(typ);
        }

        private static Type Type(this MemberInfo memberInfo)
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

        private static Object GetValue(this MemberInfo memberInfo, Object obj)
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

        private static void SetValue(this MemberInfo memberInfo, Object obj, Object newValue)
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