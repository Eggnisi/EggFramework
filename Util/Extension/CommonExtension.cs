#region

//文件创建者：Egg
//创建时间：08-17 10:21

#endregion

using UnityEngine;

namespace EggFramework
{
    public static class CommonExtension
    {
        public static void SetPosition(this MonoBehaviour self, Vector3 pos) => self.transform.position = pos;
        public static void SetPosition(this GameObject self, Vector3 pos) => self.transform.position = pos;
        public static Vector2 GetPosition2D(this MonoBehaviour self) => self.transform.position;
        public static Vector2 GetPosition2D(this GameObject self) => self.transform.position;
        public static Vector3 GetPosition(this MonoBehaviour self) => self.transform.position;
        public static Vector3 GetPosition(this GameObject self) => self.transform.position;

        public static void SetXPosition(this GameObject self, float value)
        {
            self.transform.position = new Vector3(value, self.transform.position.y,
                self.transform.position.z);
        }

        public static void SetYPosition(this GameObject self, float value)
        {
            self.transform.position = new Vector3(self.transform.position.x, value,
                self.transform.position.z);
        }

        public static void SetZPosition(this GameObject self, float value)
        {
            self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y,
                value);
        }

        public static void SetXPosition(this MonoBehaviour self, float value)
        {
            self.transform.position = new Vector3(value, self.transform.position.y,
                self.transform.position.z);
        }

        public static void SetYPosition(this MonoBehaviour self, float value)
        {
            self.transform.position = new Vector3(self.transform.position.x, value,
                self.transform.position.z);
        }

        public static void SetZPosition(this MonoBehaviour self, float value)
        {
            self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y,
                value);
        }

        public static void Show(this GameObject self) => self.SetActive(true);
        public static void Show(this MonoBehaviour self) => self.gameObject.SetActive(true);
        public static void Hide(this GameObject self) => self.SetActive(false);
        public static void Hide(this MonoBehaviour self) => self.gameObject.SetActive(false);
        public static bool IsActive(this MonoBehaviour self) => self.gameObject.activeSelf;
        public static bool IsActive(this GameObject self) => self.activeSelf;
        public static void DestroyChild(this GameObject self) => self.transform.DestroyChild();

        public static void DestroyChild(this Transform self)
        {
            for (int i = self.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(self.GetChild(i).gameObject);
            }
        }

        public static void DestroyChildImmediately(this GameObject self) => self.transform.DestroyChildImmediately();

        public static void DestroyChildImmediately(this Transform self)
        {
            for (int i = self.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(self.GetChild(i).gameObject);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            var component             = self.GetComponent<T>();
            if (!component) component = self.AddComponent<T>();
            return component;
        }

        public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component =>
            GetOrAddComponent<T>(self.gameObject);

        public static T GetComponentInParent<T>(this MonoBehaviour self) where T : Component =>
            GetComponentInParent<T>(self.gameObject);

        public static T GetComponentInParent<T>(this GameObject self) where T : Component
        {
            var component             = self.GetComponent<T>();
            if (!component) component = self.GetComponentInParent<T>();
            return component;
        }
    }
}