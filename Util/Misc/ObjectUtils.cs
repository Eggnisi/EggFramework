#region

//文件创建者：Egg
//创建时间：07-28 04:27

#endregion

namespace EggFramework.Util
{
    public static class ObjectUtils
    {
        /// <summary>
        /// Compares two objects for equality, handling UnityEngine.Object references correctly.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>True if the objects are considered equal, false otherwise.</returns>
        public static bool AnyEquals(object a, object b)
        {
            //regardless calling ReferenceEquals, unity is still doing magic and this is the only true solution (I've found)
            if (a is UnityEngine.Object or null && b is UnityEngine.Object or null)
            {
                return (UnityEngine.Object)a == (UnityEngine.Object)b;
            }

            //while '==' is reference equals, we still use '==' for when one is unity object and the other is not
            return a == b || Equals(a, b) || ReferenceEquals(a, b);
        }
    }
}