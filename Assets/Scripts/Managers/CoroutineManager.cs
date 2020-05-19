using System.Collections;
using Boo.Lang;
using UnityEngine;
using UnityScript.Lang;

namespace Managers
{
    public class CoroutineManager : MonoBehaviour
    {
        public List<Coroutine> crl=new List<Coroutine>();

        public Coroutine newCoroutine(IEnumerator e)
        {
            /*if (CRL.Contains() CRL.Add();
            print(CRL.Count);
            print(CRL.Count);
            return CRL[CRL.Count-1];*/

            return StartCoroutine(e);
        }
    }
}