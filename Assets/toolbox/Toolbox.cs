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

    public class TypedEventHandler
    {
        [Serializable]
        public delegate void EventHandler<T>(T sender, EventArgs args);
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
            var instance = Object.Instantiate<T>(prefab);
            instance.transform.parent = tr.parent;
            instance.transform.position = tr.position;
            instance.transform.rotation = tr.rotation;
            return instance;
        }

        // Must be defined inside a class called Fahrenheit:
        public static T QC<T>(this GameObject go )  where T:MonoBehaviour
        {
            return go.GetComponent<T>();
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
            var instance = Object.Instantiate<T>(prefab);
            instance.transform.parent = parent;
            instance.transform.position = prefab.transform.position;
            instance.transform.rotation = prefab.transform.rotation;
            return instance;
        }

        public static T Instantiate<T>(this T prefab) where T : Component
        {
            var instance = Object.Instantiate(prefab);
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

        public static IEnumerator DelaySeconds(float delay, Action action)
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

        // Use like this:
        // yield return StartCoroutine(CoroutineUtils.UntilTrue(() => (lives > 3)));
        public static IEnumerator UntilTrue(System.Func<bool> fn)
        {
            while (!fn())
            {
                yield return null;
            }
        }

        // yield return StartCoroutine(CoroutineUtils.WhileAnimating(animation));
        public static IEnumerator WhileAnimating(Animation animation)
        {
            while (animation.isPlaying)
            {
                yield return null;
            }
        }

        // A wait function using GameTime instead of Time
        public static IEnumerator WaitForGameSecondsCoroutine(float time)
        {
            time = Mathf.Max(time, 0);
            float timeElapsed = 0;
            while (timeElapsed < time)
            {
                timeElapsed += Time.deltaTime;

                yield return 0;
            }
        }

        public static Coroutine WaitForGameSeconds(float time)
        {
            return GameManager.Instance.StartCoroutine(WaitForGameSecondsCoroutine(time));
        }

        // yield return StartCoroutine(CoroutineUtils.OnNextFrame(() => { doSomething() } ));
        public static IEnumerator OnNextFrame(System.Action fn)
        {
            yield return null;
            fn();
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

