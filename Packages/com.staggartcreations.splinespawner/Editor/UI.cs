// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using UnityEditor;
using UnityEngine;

namespace sc.splines.spawner.editor
{
    public static class UI
    {
        public static readonly Color RedColor = new Color(1f, 0.31f, 0.34f);
        public static readonly Color OrangeColor= new Color(1f, 0.68f, 0f);
        public static readonly Color GreenColor = new Color(0.33f, 1f, 0f);
        
        public static string iconPrefix => EditorGUIUtility.isProSkin ? "d_" : string.Empty;
        
        private static Texture FoldoutIcon => _foldoutIcon ??= EditorGUIUtility.IconContent("IN_foldout").image;
        private static Texture FoldoutOnIcon => _foldoutOnIcon ??= EditorGUIUtility.IconContent("IN_foldout_on").image;

        private static Texture _foldoutIcon;
        private static Texture _foldoutOnIcon;
        
        public static Texture CreateIcon(string data)
        {
            byte[] bytes = System.Convert.FromBase64String(data);

            Texture2D icon = new Texture2D(32, 32, TextureFormat.RGBA32, false, false);
            icon.LoadImage(bytes, true);
            
            return icon;
        }
        
        public static void DrawH2(string text)
        {
            Rect backgroundRect = EditorGUILayout.GetControlRect();
            backgroundRect.height = 25f;
            
            var labelRect = backgroundRect;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;

            // Background
            float backgroundTint = (EditorGUIUtility.isProSkin ? 0.1f : 1f);
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, new GUIContent(text), Styles.H2);
            
            EditorGUILayout.Space(backgroundRect.height * 0.5f);
        }

        
        private const float HeaderHeight = 27f;
        public static bool DrawFoldout(bool state, string title, Action drawAction, string subtitle = "")
        {
            DrawSplitter();

            Rect backgroundRect = GUILayoutUtility.GetRect(1f, HeaderHeight);
            
            var labelRect = backgroundRect;
            labelRect.xMin += 8f;
            labelRect.xMax -= 20f + 16 + 5;

            var foldoutRect = backgroundRect;
            foldoutRect.xMin -= 12f;
            foldoutRect.width = HeaderHeight;
            foldoutRect.height = HeaderHeight;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            float backgroundTint = (EditorGUIUtility.isProSkin ? 0.1f : 1f);
            if (backgroundRect.Contains(Event.current.mousePosition)) backgroundTint *= EditorGUIUtility.isProSkin ? 1.5f : 0.9f;
            
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));
            
            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            if (subtitle != string.Empty)
            {
                Rect subtitleRect = labelRect;
                subtitleRect.xMin += EditorStyles.boldLabel.CalcSize(new GUIContent(title)).x + 3f;
                EditorGUI.LabelField(subtitleRect, subtitle, EditorStyles.miniLabel);

            }

            // Foldout
            GUI.Label(foldoutRect, state ? FoldoutOnIcon : FoldoutIcon, EditorStyles.miniLabel);
            
            // Handle events
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (backgroundRect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                    {
                        state = !state;
                        //if(clickAction != null) clickAction.Invoke();
                    }

                    e.Use();
                }
            }

            if(state)
            {
                EditorGUILayout.Space();
                
                drawAction.Invoke();
                
                EditorGUILayout.Space();
            }

            return state;
        }
        
        public static void DrawSplitter(bool isBoxed = false)
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f);
            float xMin = rect.xMin;

            // Splitter rect should be full-width
            rect.xMin = 0f;
            rect.width += 4f;
            
            if (isBoxed)
            {
                rect.xMin = xMin == 7.0f ? 4.0f : EditorGUIUtility.singleLineHeight;
                rect.width -= 1;
            }

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, !EditorGUIUtility.isProSkin
                ? new Color(0.6f, 0.6f, 0.6f, 1.333f)
                : new Color(0.12f, 0.12f, 0.12f, 1.333f));
        }
        
        public static class Styles
        {
            private static GUIStyle _Section;
            public static GUIStyle Section
            {
                get
                {
                    if (_Section == null)
                    {
                        _Section = new GUIStyle()
                        {
                            margin = new RectOffset(0, 0, -5, 5),
                            padding = new RectOffset(10, 10, 5, 5),
                            clipping = TextClipping.Clip,
                        };
                    }

                    return _Section;
                }
            }
            
            private static GUIStyle _H2;
            public static GUIStyle H2
            {
                get
                {
                    if (_H2 == null)
                    {
                        _H2 = new GUIStyle(GUI.skin.label)
                        {
                            richText = true,
                            alignment = TextAnchor.MiddleLeft,
                            wordWrap = true,
                            fontSize = 14,
                            fontStyle = FontStyle.Bold,
                            padding = new RectOffset(10, 0, 0, 0)
                        };
                    }

                    return _H2;
                }
            }
            
            private static GUIStyle _Button;
            public static GUIStyle Button
            {
                get
                {
                    if (_Button == null)
                    {
                        _Button = new GUIStyle(GUI.skin.button)
                        {
                            alignment = TextAnchor.MiddleLeft,
                            stretchWidth = true,
                            richText = true,
                            wordWrap = true,
                            padding = new RectOffset()
                            {
                                left = 14,
                                right = 14,
                                top = 8,
                                bottom = 8
                            }
                        };
                    }

                    return _Button;
                }
            }
            
            private static GUIStyle _UpdateText;
            public static GUIStyle UpdateText
            {
                get
                {
                    if (_UpdateText == null)
                    {
                        _UpdateText = new GUIStyle("Button")
                        {
                            //fontSize = 10,
                            alignment = TextAnchor.MiddleLeft,
                            stretchWidth = false,
                        };
                    }

                    return _UpdateText;
                }
            }

            private static GUIStyle _Header;
            public static GUIStyle Header
            {
                get
                {
                    if (_Header == null)
                    {
                        _Header = new GUIStyle(GUI.skin.label)
                        {
                            richText = true,
                            alignment = TextAnchor.MiddleCenter,
                            wordWrap = true,
                            fontSize = 18,
                            fontStyle = FontStyle.Normal
                        };
                    }

                    return _Header;
                }
            }
            
            private static GUIStyle _Footer;
            public static GUIStyle Footer
            {
                get
                {
                    if (_Footer == null)
                    {
                        _Footer = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                        {
                            richText = true,
                            alignment = TextAnchor.MiddleCenter,
                            wordWrap = true,
                            fontSize = 12
                        };
                    }

                    return _Footer;
                }
            }
        }
    }
}