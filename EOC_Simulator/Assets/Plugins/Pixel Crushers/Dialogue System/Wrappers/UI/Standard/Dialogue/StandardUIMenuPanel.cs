﻿// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.Wrappers
{

    /// <summary>
    /// This wrapper class keeps references intact if you switch between the 
    /// compiled assembly and source code versions of the original class.
    /// </summary>
    [HelpURL("http://www.pixelcrushers.com/dialogue_system/manual2x/html/standard_u_i_menu_panel.html")]
    [AddComponentMenu("Pixel Crushers/Dialogue System/UI/Standard UI/Dialogue/Standard UI Menu Panel")]
    public class StandardUIMenuPanel : PixelCrushers.DialogueSystem.StandardUIMenuPanel
    {
        
        protected override void Start()
        {
            base.Start();
            ClearResponseButtons();
        }

        public void ClearButtons()
        {
            ClearResponseButtons();
        }
    }

}
