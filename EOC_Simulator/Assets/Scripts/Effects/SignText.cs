using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Effects
{
    public class SignText : MonoBehaviour
    {
        [SerializeField] private string signText;
        [SerializeField] private List<TMP_Text> signTmpTexts = new();
        private void Awake()
        {
            foreach (var tmpText in signTmpTexts)
            {
                tmpText.text = signText;
            }
        }
    }
}
