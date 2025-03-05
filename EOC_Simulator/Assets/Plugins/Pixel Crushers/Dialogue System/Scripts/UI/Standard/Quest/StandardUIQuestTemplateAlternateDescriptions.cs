// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using TMPro;

namespace PixelCrushers.DialogueSystem
{

    [Serializable]
    public class StandardUIQuestTemplateAlternateDescriptions
    {

        [Tooltip("(Optional) If set, use if state is success.")]
        public TMP_Text successDescription;

        [Tooltip("(Optional) If set, use if state is failure.")]
        public TMP_Text failureDescription;

        public void SetActive(bool value)
        {
            successDescription.gameObject.SetActive(value);
            failureDescription.gameObject.SetActive(value);
        }

    }

}
