using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Effects
{
    public class SignText : MonoBehaviour
    {
        [SerializeField] private bool changeText = true;
        [SerializeField] private string signText;
        [SerializeField] private List<TMP_Text> signTmpTexts = new();
        private void Awake()
        {
            if (!changeText) return;
            foreach (var tmpText in signTmpTexts)
            {
                tmpText.text = signText;
            }
        }
    }
}
