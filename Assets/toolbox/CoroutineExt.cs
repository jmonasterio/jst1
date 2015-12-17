using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shared
{
    // See: https://www.google.com/webhp?sourceid=chrome-instant&rlz=1C1CHWA_enUS646US646&ion=1&espv=2&ie=UTF-8#q=unit3d%20too%20many%20coroutines
    public static class MonoBehaviorExt
    {
        public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour obj, IEnumerator coroutine)
        {
            Coroutine<T> coroutineObject = new Coroutine<T>();
            coroutineObject.coroutine = obj.StartCoroutine(coroutineObject.InternalRoutine(coroutine));
            return coroutineObject;
        }
    }

    public class Coroutine<T>
    {
        public T Value
        {
            get
            {
                if (e != null)
                {
                    throw e;
                }
                return returnVal;
            }
        }

        private T returnVal;
        private Exception e;
        public Coroutine coroutine;

        public IEnumerator InternalRoutine(IEnumerator coroutine)
        {
            while (true)
            {
                try
                {
                    if (!coroutine.MoveNext())
                    {
                        yield break;
                    }
                }
                catch (Exception e)
                {
                    this.e = e;
                    yield break;
                }
                object yielded = coroutine.Current;
                if (yielded != null && yielded.GetType() == typeof (T))
                {
                    returnVal = (T) yielded;
                    yield break;
                }
                else
                {
                    yield return coroutine.Current;
                }
            }
        }
    }

#if TEST
    class Test : MonoBehaviour
    {
        IEnumerator Start1()
        {
            var routine = StartCoroutine<int>(TestNewRoutine()); //Start our new routine
            yield return routine.coroutine; // wait as we normally can
            Debug.Log(routine.returnVal); // print the result now that it is finished.
        }

        IEnumerator TestNewRoutine
            ()
        {
            yield return null;
            yield return new WaitForSeconds(2f);
            yield return 10;
        }

        IEnumerator Start2()
        {
            var routine = StartCoroutine<int>(TestNewRoutineGivesException());
            yield return routine.coroutine;
            try
            {
                Debug.Log(routine.Value);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                // do something
                Debug.Break();
            }
        }

        IEnumerator TestNewRoutineGivesException()
        {
            yield return null;
            yield return new WaitForSeconds(2f);
            throw new Exception("Bad thing!");
        }
    }
#endif
}


