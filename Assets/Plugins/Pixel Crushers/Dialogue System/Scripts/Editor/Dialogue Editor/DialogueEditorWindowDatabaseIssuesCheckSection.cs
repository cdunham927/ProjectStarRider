﻿// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
#if USE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace PixelCrushers.DialogueSystem.DialogueEditor
{

    /// <summary>
    /// This part of the Dialogue Editor window handles the Database tab's Check For Issues section.
    /// </summary>
    public partial class DialogueEditorWindow
    {

        private string issuesReport = string.Empty;

        private void DrawCheckForIssuesSection()
        {
            EditorWindowTools.StartIndentedSection();

            EditorGUI.BeginDisabledGroup(database == null);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Check Sequences", "Check for syntax errors in dialogue entries' Sequence fields.")))
            {
                if (EditorUtility.DisplayDialog("Check Sequences", "This will check all dialogue entries' Sequence fields for syntax errors. Continue?", "OK", "Cancel"))
                {
                    CheckSequences();
                }
            }
            if (GUILayout.Button(new GUIContent("Find Orphan Nodes", "Report dialogue entries that don't have links to them.")))
            {
                if (EditorUtility.DisplayDialog("Find Orphan Nodes", "Report dialogue entries that don't have links to them. Continue?", "OK", "Cancel"))
                {
                    CheckForOrphanNodes();
                }
            }
            if (GUILayout.Button(new GUIContent("Find Undefined Variables", "Report instances where a variable is referenced but it's not defined in the dialogue database's Variable section.")))
            {
                if (EditorUtility.DisplayDialog("Find Undefined Variables", "Report instances where a variable is referenced but it's not defined in the dialogue database's Variable section. This is generally fine because you don't need to predefine variables in Lua, but you may want to know anyway. Continue?", "OK", "Cancel"))
                {
                    CheckUndefinedVariables();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Find Duplicate Fields & Titles", "Report actors, items/quests, locations, conversations, and dialogue entries that have duplicate fields and duplicate conversation titles.")))
            {
                if (EditorUtility.DisplayDialog("Find Duplicate Fields & Titles", "This will identify actors, items/quests, locations, conversations, and dialogue entries that have duplicate fields and duplicate conversation titles. Continue?", "OK", "Cancel"))
                {
                    CheckDuplicateFields();
                }
            }
            if (GUILayout.Button(new GUIContent("Check Entrytag Audio", "Check if each non-blank entry has a corresponding entrytag audio file in Addressables or Resources.")))
            {
                if (EditorUtility.DisplayDialog("Check Entrytag Audio", "This will identify non-blank entries that do not have a corresponding entrytag audio file in Resources folders or local Addressables. Set the entrytag format in the Export Database foldout.\n\nSince it must check through the Asset Database, it may take some time. Continue?", "OK", "Cancel"))
                {
                    CheckEntrytagAudio();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(issuesReport))
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextArea(issuesReport);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy to Clipboard", GUILayout.Width(200)))
                {
                    GUIUtility.systemCopyBuffer = issuesReport;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorWindowTools.EndIndentedSection();
        }

        private void CheckSequences()
        {
            try
            {
                issuesReport = "Sequence Syntax Errors:\n";
                int numErrors = 0;
                var parser = new SequenceParser();
                var numConversations = database.conversations.Count;
                for (int i = 0; i < database.conversations.Count; i++)
                {
                    var progress = (float)i / (float)numConversations;
                    var conversation = database.conversations[i];
                    var foundErrorInConversation = false;
                    if (EditorUtility.DisplayCancelableProgressBar("Checking Sequences", conversation.Title, progress)) return;
                    foreach (var entry in conversation.dialogueEntries)
                    {
                        var sequence = entry.Sequence;
                        if (string.IsNullOrEmpty(sequence)) continue;
                        try
                        {
                            var sanitizedSequence = sequence.Replace(@"{{end}}", "0").Replace(@"{{default}}", "");
                            if (sanitizedSequence.Contains("[var=") || sanitizedSequence.Contains("[lua") ||
                                sanitizedSequence.Contains(@"{{"))
                            {
                                if (!foundErrorInConversation)
                                {
                                    foundErrorInConversation = true;
                                    issuesReport += $"\nConversation [{conversation.id}]: {conversation.Title}\n";
                                }
                                issuesReport += $"[{entry.id}] '{entry.subtitleText}'\nCan't check sequence because variable content may change at runtime:\n{sequence}\n\n";
                            }
                            else
                            {
                                parser.Parse(sanitizedSequence, true);
                            }
                        }
                        catch (Exception)
                        {
                            numErrors++;
                            if (!foundErrorInConversation)
                            {
                                foundErrorInConversation = true;
                                issuesReport += $"\nConversation [{conversation.id}]: {conversation.Title}\n";
                            }
                            issuesReport += $"[{entry.id}] '{entry.subtitleText}'\n{sequence}\n\n";
                        }
                    }
                }
                issuesReport += $"{numErrors} errors found.";
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void CheckForOrphanNodes()
        {
            try
            {
                issuesReport = "Orphaned Dialogue Entries:\n";
                float progress = 0;
                if (EditorUtility.DisplayCancelableProgressBar("Chekc Orphaned Dialogue Entries", "Checking...", progress)) return;

                // Make a list of all entries:
                var allEntries = new Dictionary<int, List<int>>();
                foreach (var conversation in database.conversations)
                {
                    var list = new List<int>();
                    allEntries[conversation.id] = list;
                    foreach (var entry in conversation.dialogueEntries)
                    {
                        if (entry.id == 0) continue; // Skip <START>
                        list.Add(entry.id);
                    }
                }

                // Remove entries that are linked from something:
                foreach (var conversation in database.conversations)
                {
                    foreach (var entry in conversation.dialogueEntries)
                    {
                        foreach (var link in entry.outgoingLinks)
                        {
                            List<int> list;
                            if (allEntries.TryGetValue(link.destinationConversationID, out list))
                            {
                                list.Remove(link.destinationDialogueID);
                            }
                        }
                    }
                }

                // Report remaining entries:
                int numOrphans = 0;
                foreach (var kvp in allEntries)
                {
                    var conversationID = kvp.Key;
                    var list = kvp.Value;
                    if (list.Count > 0)
                    {
                        numOrphans += list.Count;
                        var conversation = database.GetConversation(conversationID);
                        issuesReport += $"\nConversation [{conversationID}]: {conversation.Title}\n";
                        foreach (var entryID in list)
                        {
                            var entry = conversation.GetDialogueEntry(entryID);
                            issuesReport += $"[{entryID}] '{entry.subtitleText}'\n";
                        }
                    }
                }
                issuesReport += $"{numOrphans} orphans found.";
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void CheckUndefinedVariables()
        {
            try
            {
                issuesReport = "Undefined Variables:\n";
                float progress = 0;
                int count = 0;
                if (EditorUtility.DisplayCancelableProgressBar("Check Undefined Variables", "", progress)) return;

                // Make a quick lookup hash of all defined variables:
                var definedVariables = new HashSet<string>();
                database.variables.ForEach(variable => definedVariables.Add(DialogueLua.StringToTableIndex(variable.Name)));

                // Go through all quests and dialogue entries:
                var total = database.items.Count + database.conversations.Count;
                var undefinedVariables = new Dictionary<string, string>();
                foreach (var quest in database.items)
                {
                    if (quest.IsItem) continue;
                    var questName = quest.Name;
                    foreach (var field in quest.fields)
                    {
                        count++;
                        progress = (float)count / (float)total;
                        if (EditorUtility.DisplayCancelableProgressBar("Check Undefined Variables", questName, progress)) return;
                        CheckUndefinedVariablesInField(questName, field, definedVariables, undefinedVariables);
                    }
                }
                foreach (var conversation in database.conversations)
                {
                    var conversationTitle = conversation.Title;
                    count++;
                    progress = (float)count / (float)total;
                    if (EditorUtility.DisplayCancelableProgressBar("Check Undefined Variables", conversationTitle, progress)) return;
                    foreach (var entry in conversation.dialogueEntries)
                    {
                        var entryID = $"{conversationTitle}[{entry.id}]";
                        foreach (var field in entry.fields)
                        {
                            CheckUndefinedVariablesInField(entryID, field, definedVariables, undefinedVariables);
                        }
                    }
                }

                // Report defined variables:
                foreach (var kvp in undefinedVariables)
                {
                    issuesReport += $"{kvp.Value}\n";
                }
                issuesReport += $"{undefinedVariables.Count} undefined variables found.";
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void CheckDuplicateFields()
        {
            try
            {
                issuesReport = "Duplicate Fields:\n";
                if (EditorUtility.DisplayCancelableProgressBar("Find Duplicate Fields", "", 0)) return;

                var report = string.Empty;
                foreach (var actor in database.actors)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Find Duplicate Fields", "Actors", 0.2f)) return;
                    ResetLastFieldsChecked();
                    CheckFields(actor.fields);
                    if (string.IsNullOrEmpty(duplicateFieldsReport)) continue;
                    report += $"Actor[{actor.Name}] {duplicateFieldsReport}\n";
                }
                foreach (var item in database.items)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Find Duplicate Fields", "Items/Quests", 0.4f)) return;
                    ResetLastFieldsChecked();
                    CheckFields(item.fields);
                    if (string.IsNullOrEmpty(duplicateFieldsReport)) continue;
                    if (item.IsItem)
                    {
                        report += $"Item[{item.Name}] {duplicateFieldsReport}\n";
                    }
                    else
                    {
                        report += $"Quest[{item.Name}] {duplicateFieldsReport}\n"; 
                    }
                }
                foreach (var location in database.locations)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Find Duplicate Fields", "Locations", 0.6f)) return;
                    ResetLastFieldsChecked();
                    CheckFields(location.fields);
                    if (string.IsNullOrEmpty(duplicateFieldsReport)) continue;
                    report += $"Location[{location.Name}] {duplicateFieldsReport}\n";
                }
                var conversationTitles = new HashSet<string>();
                foreach (var conversation in database.conversations)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Find Duplicate Fields", "Conversations", 0.8f)) return;
                    var conversationTitle = conversation.Title;
                    if (conversationTitles.Contains(conversationTitle))
                    {
                        report += $"More than one conversation has the title '{conversation.Title}'\n";
                    }
                    conversationTitles.Add(conversationTitle);
                    ResetLastFieldsChecked();
                    CheckFields(conversation.fields);
                    if (!string.IsNullOrEmpty(duplicateFieldsReport))
                    {
                        report += $"Conversation[{conversation.id}] '{conversation.Title}' {duplicateFieldsReport}\n";
                    }
                    foreach (var entry in conversation.dialogueEntries)
                    {
                        ResetLastFieldsChecked();
                        CheckFields(entry.fields);
                        if (string.IsNullOrEmpty(duplicateFieldsReport)) continue;
                        report += $"Conversation[{conversation.id}] '{conversation.Title}' entry [{entry.id}] {duplicateFieldsReport}\n";
                    }
                }
                issuesReport += report;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static Regex VarTagRegex = new Regex(@"\[var=[^\]]+\]");

        private void CheckUndefinedVariablesInField(string assetName, Field field, 
            HashSet<string> definedVariables, 
            Dictionary<string, string> undefinedVariables)
        {
            foreach (Match match in VarTagRegex.Matches(field.value))
            {
                var varName = DialogueLua.StringToTableIndex(match.Value.Substring(5, match.Value.Length - 6));
                if (!definedVariables.Contains(varName))
                {
                    if (!undefinedVariables.ContainsKey(varName))
                    {
                        undefinedVariables.Add(varName, $"{varName}: In ");
                    }
                    else
                    {
                        undefinedVariables[varName] += ", ";
                    }
                    undefinedVariables[varName] += $"{assetName}.{field.title}";
                }
            }
        }

        private void CheckEntrytagAudio()
        {
            try
            {
                issuesReport = "Missing Entrytag Audio:\n";

                // Get all audio clip paths in Resources folders:
                var guids = AssetDatabase.FindAssets("t:AudioClip");
                var paths = new List<string>();
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!path.Contains("/Resources/")) continue;
                    var extPos = path.LastIndexOf('.');
                    paths.Add(path.Substring(0, extPos));
                }

#if USE_ADDRESSABLES
                var addressableKeys = GetAllAddressableAudioClipsKeys();
#endif

                // Check all entries:
                int numErrors = 0;
                var numConversations = database.conversations.Count;
                for (int i = 0; i < database.conversations.Count; i++)
                {
                    var progress = (float)i / (float)numConversations;
                    var conversation = database.conversations[i];
                    var foundErrorInConversation = false;
                    if (EditorUtility.DisplayCancelableProgressBar("Checking Entrytag Audio", conversation.Title, progress)) return;
                    foreach (var entry in conversation.dialogueEntries)
                    {
                        if (entry.id == 0 || (string.IsNullOrEmpty(entry.DialogueText) && string.IsNullOrEmpty(entry.MenuText))) continue;
                        var entrytag = database.GetEntrytag(conversation, entry, entrytagFormat);
                        var slashEntrytag = "/" + entrytag;
                        var foundClip = false;

                        // Search Resources:
                        foreach (var path in paths)
                        {
                            if (path.EndsWith(slashEntrytag))
                            {
                                foundClip = true;
                                break;
                            }
                        }

#if USE_ADDRESSABLES
                        // Search Addressables:
                        if (!foundClip)
                        {
                            foundClip = addressableKeys.Contains(entrytag);
                        }
#endif

                        if (!foundClip)
                        {
                            numErrors++;
                            if (!foundErrorInConversation)
                            {
                                foundErrorInConversation = true;
                                issuesReport += $"\nConversation [{conversation.id}]: {conversation.Title}\n";
                            }
                            issuesReport += $"[{entry.id}] '{entry.subtitleText}' missing entrytag audio: {entrytag}\n";
                        }
                    }
                }
                issuesReport += $"{numErrors} errors found.";
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

#if USE_ADDRESSABLES
        private List<string> GetAllAddressableAudioClipsKeys()
        {
            var addresses = new List<string>();
            var addressableGroups = GetAllAddressablesGroups();
            for (int i = 0; i < addressableGroups.Count; ++i)
            {
                var result = new List<UnityEditor.AddressableAssets.Settings.AddressableAssetEntry>();
                addressableGroups[i].GatherAllAssets(result, true, true, true);
                for (int j = 0; j < result.Count; ++j)
                {
                    if (result[j].TargetAsset is AudioClip)
                    {
                        addresses.Add(result[j].address);
                    }
                }
            }
            return addresses;
        }

        private List<UnityEditor.AddressableAssets.Settings.AddressableAssetGroup> GetAllAddressablesGroups()
        {
            var paths = AssetDatabase.GetAllAssetPaths();
            var assetsPaths = new List<string>();
            for (int i = 0; i < paths.Length; ++i)
            {
                if (paths[i].EndsWith(".asset"))
                {
                    assetsPaths.Add(paths[i]);
                }
            }

            var groups = new List<UnityEditor.AddressableAssets.Settings.AddressableAssetGroup>();
            for (int i = 0; i < assetsPaths.Count; ++i)
            {
                var group = AssetDatabase.LoadAssetAtPath<UnityEditor.AddressableAssets.Settings.AddressableAssetGroup>(assetsPaths[i]);
                if (group != null)
                {
                    groups.Add(group);
                }
            }

            return groups;
        }
#endif

    }
}
