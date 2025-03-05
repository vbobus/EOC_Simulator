// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This component hooks up the elements of a Standard UI quest track template,
    /// which is used by the Unity UI Quest Tracker.
    /// Add it to your quest track template and assign the properties.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class StandardUIQuestTrackTemplate : MonoBehaviour
    {

        [Header("Quest Heading")]
        [Tooltip("The heading - name or description depends on tracker setting")]
        public TMP_Text  description;

        public StandardUIQuestTemplateAlternateDescriptions alternateDescriptions = new StandardUIQuestTemplateAlternateDescriptions();

        [Header("Quest Entries")]
        [Tooltip("(Optional) If set, holds instantiated quest entries")]
        public Transform entryContainer;

        [Tooltip("Used for quest entries")]
        public TMP_Text entryDescription;

        public StandardUIQuestTemplateAlternateDescriptions alternateEntryDescriptions = new StandardUIQuestTemplateAlternateDescriptions();

        protected List<GameObject> m_instances = null;

        public bool arePropertiesAssigned { get { return (description != null) && (entryDescription != null); } }

        protected int numEntries = 0;

        public virtual void Initialize()
        {
            // description.gameObject.SetActive(false);
            // alternateDescriptions.SetActive(false);
            entryDescription.gameObject.SetActive(false);
            // alternateEntryDescriptions.SetActive(false);
            if (entryContainer != null)
            {
                entryContainer.gameObject.SetActive(false);
                if (m_instances != null)
                {
                    for (int i = 0; i < m_instances.Count; i++)
                    {
                        if (m_instances[i] != null) Destroy(m_instances[i].gameObject);
                    }
                }
                m_instances = new List<GameObject>();
            }
            numEntries = 0;
        }

        public virtual void SetDescription(string text, QuestState questState)
        {
            if (text == null) return;
            switch (questState)
            {
                case QuestState.Active:
                case QuestState.ReturnToNPC:
                    SetFirstValidTextElement(text, description);
                    break;
                case QuestState.Success:
                    SetFirstValidTextElement(text, alternateDescriptions.successDescription, description);
                    break;
                case QuestState.Failure:
                    SetFirstValidTextElement(text, alternateDescriptions.failureDescription, description);
                    break;
                default:
                    return;
            }
        }

        public virtual void AddEntryDescription(string text, QuestState entryState)
        {
            if (entryContainer == null)
            {
                // No container, so make entryDescription a big multi-line string:
                // alternateEntryDescriptions.SetActive(false);
                if (entryDescription != null)
                {
                    if (numEntries == 0)
                    {
                        entryDescription.gameObject.SetActive(true);
                        entryDescription.text = text;
                    }
                    else
                    {
                        entryDescription.text += "\n" + text;
                    }
                }
            }
            else
            {
                // Instantiate into container:
                if (numEntries == 0)
                {
                    // entryContainer.gameObject.SetActive(true);
                    if (entryDescription) entryDescription.gameObject.SetActive(false);
                    // alternateEntryDescriptions.SetActive(false);
                }
                switch (entryState)
                {
                    case QuestState.Active:
                        InstantiateFirstValidTextElement(text, entryContainer, entryDescription);
                        break;
                    case QuestState.Success:
                        InstantiateFirstValidTextElement(text, entryContainer, alternateEntryDescriptions.successDescription, entryDescription);
                        break;
                    case QuestState.Failure:
                        InstantiateFirstValidTextElement(text, entryContainer, alternateEntryDescriptions.failureDescription, entryDescription);
                        break;
                }
            }
            numEntries++;
        }

        protected void SetFirstValidTextElement(string text, params TMP_Text[] textElements)
        {
            for (int i = 0; i < textElements.Length; i++)
            {
                if (textElements[i] != null && textElements[i].gameObject != null)
                {
                    textElements[i].gameObject.SetActive(true);
                    textElements[i].text = text;
                    return;
                }
            }
        }

        protected void InstantiateFirstValidTextElement(string text, Transform container, params TMP_Text[] textElements)
        {
            for (int i = 0; i < textElements.Length; i++)
            {
                if (textElements[i] != null && textElements[i].gameObject != null)
                {
                    textElements[i].text = text;
                    var go = Instantiate(textElements[i].gameObject) as GameObject;
                    go.transform.SetParent(container.transform, false);
                    go.SetActive(true);
                    m_instances.Add(go);
                    return;
                }
            }
        }

    }

}
