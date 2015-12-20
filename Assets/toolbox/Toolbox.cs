using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Toolbox
{
    // From: http://wiki.unity3d.com/index.php/Toolbox
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopenning the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);

                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }


        private static bool applicationIsQuitting = false;
        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }

    [System.Serializable]
    public class Language
    {
        public string current;
        public string lastLang;
    }
    public static class MathfExt
    {
        public static Vector2 MakeRandomForce()
        {
            const float max = 15.0f;
            var f = new Vector2(Random.Range(-max, max), Random.Range(-max, max));
            f = f * 2.0f;
            return f;
        }

        public static float RoundToNearestMultiple(float f, float multiple)
        {
            return Mathf.Round(f/multiple)*multiple;
        }
        public static Vector2 MakeRandom2D()
        {
            return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        }

        public static Vector2 To2D(Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public static Vector3 From2D(Vector2 v2)
        {
            return new Vector3(v2.x, v2.y, 0.0f);
        }


    }

    public static class GameObjectExt
    {
#if UNITY_EDITOR
        public static IEnumerable<GameObject> SceneRoots()
        {
            var prop = new HierarchyProperty(HierarchyType.GameObjects);
            var expanded = new int[0];
            while (prop.Next(expanded))
            {
                yield return prop.pptrValue as GameObject;
            }
        }

        /// <summary>
        /// Breadthfirst enumeration of all game objects
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Transform> AllSceneObjects()
        {
            var queue = new Queue<Transform>();

            foreach (var root in SceneRoots())
            {
                var tf = root.transform;
                yield return tf;
                queue.Enqueue(tf);
            }

            while (queue.Count > 0)
            {
                foreach (Transform child in queue.Dequeue())
                {
                    yield return child;
                    queue.Enqueue(child);
                }
            }
        }
#endif
        public static void Show(this GameObject go, bool b)
        {
            go.GetComponent<SpriteRenderer>().enabled = b;
        }

        /// <summary>
        /// Gets or add a component. Usage example:
        /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
        /// </summary>
        public static T GetOrAddComponent<T>(this Component child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.gameObject.AddComponent<T>();
            }
            return result;
        }

        public static GameObject FindOrCreateTempContainer(this Transform root, string name)
        {

            var trans = root.FindChild(name);
            if (trans == null)
            {
                var ret = new GameObject(name);
                ret.transform.parent = root;
                return ret;
            }
            else
            {
                return trans.gameObject;
            }
        }

        public static T InstantiateAtTransform<T>(this T prefab, Transform tr) where T:Component
        {
            var instance = Object.Instantiate(prefab);
            instance.transform.parent = tr;
            instance.transform.position = tr.position;
            instance.transform.rotation = tr.rotation;
            return instance;
        }

        /// <summary>
        /// Prefab contains position and rotation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T InstantiateInTransform<T>(this T prefab, Transform parent) where T : Component
        {
            var instance = Object.Instantiate(prefab);
            instance.transform.parent = parent;
            instance.transform.position = prefab.transform.position;
            instance.transform.rotation = prefab.transform.rotation;
            return instance;
        }

        public static Rect GetCameraWorldRect( this GameObject go)
        {
            var dist = (go.transform.position - Camera.main.transform.position).z;
            var leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
            var rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
            var topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
            var bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
            var camRect = new Rect(new Vector2(leftBorder, topBorder),
                new Vector2(rightBorder - leftBorder, bottomBorder - topBorder));
            return camRect;
        }


        public static void SafeDestroy(ref Component obj)
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
            obj = null;
        }

        public static void SafeDestroy(ref GameObject obj)
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
            obj = null;
        }

        public static void SafeDestroy(ref ParticleSystem obj)
        {
            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
            }
            obj = null;
        }


        public static void DestroyChildren(GameObject abc)
        {
            while (abc.transform.childCount > 0)
            {
                Object.Destroy(abc.transform.GetChild(0).gameObject);
            }
        }
    }

    public static class CoroutineUtils
    {

        /**
         * Usage: StartCoroutine(CoroutineUtils.Chain(...))
         * For example:
         *     StartCoroutine(CoroutineUtils.Chain(
         *         CoroutineUtils.Do(() => Debug.Log("A")),
         *         CoroutineUtils.WaitForSeconds(2),
         *         CoroutineUtils.Do(() => Debug.Log("B"))));
         */

        public static IEnumerator Chain(params IEnumerator[] actions)
        {
            foreach (IEnumerator action in actions)
            {
                yield return GameManager.Instance.StartCoroutine(action); 
            }
        }

        /**
         * Usage: StartCoroutine(CoroutineUtils.DelaySeconds(action, delay))
         * For example:
         *     StartCoroutine(CoroutineUtils.DelaySeconds(
         *         () => DebugUtils.Log("2 seconds past"),
         *         2);
         */

        public static IEnumerator DelaySeconds(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        public static IEnumerator WaitForSeconds(float time)
        {
            yield return new WaitForSeconds(time);
        }

        public static IEnumerator Do(Action action)
        {
            action();
            yield return 0;
        }
    }

    public class BaseBehaviour : MonoBehaviour
    {
        // Any shared stuff that everyone should have.
    }

    public class BaseNetworkBehaviour : NetworkBehaviour
    {
        // Any shared stuff that everyone should have.
    }

    public class Base2DBehaviour : BaseBehaviour
    {
        private Rect? _camRect;

        protected Vector3 MakeRandomPos()
        {
            if (!_camRect.HasValue)
            {
                _camRect = GameObjectExt.GetCameraWorldRect(this.gameObject);
            }

            var camRect = _camRect.Value;
            return new Vector3(Random.Range(camRect.xMin, camRect.xMax),
                Random.Range(camRect.yMin, camRect.yMax), 0.0f);
        }

        protected Vector3 MakeRandomCentralPos()
        {
            if (!_camRect.HasValue)
            {
                _camRect = GameObjectExt.GetCameraWorldRect(this.gameObject);
            }
            var camRect = _camRect.Value;
            return new Vector3(Random.Range(camRect.xMin / 2, camRect.xMax / 2),
                Random.Range(camRect.yMin / 2, camRect.yMax / 2), 0.0f);
        }






    }
}

