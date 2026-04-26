// MIT License
// 
// Copyright (c) 2021 Fletcher Cole
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using NaughtyAttributes;
#endif

namespace FletcherLibraries
{
    /// <summary>
    ///     Component with a button that will combine the sprites for all subobjects into one big texture.
    ///     </summary>
    /// <remarks>
    ///     Directions:
    ///     1) Put this component on an empty game object in your scene, then either: 
    ///         a) drag game objects with static SpriteRenderer sprites under this object as subobjects, or 
    ///         b) drag the static sprite game objects in the scene into the "CombineFromSpecificObjects" field on this component.
    ///     2) Set "PixelsPerUnit" to the intended resolution of the output texture relative to world units
    ///         (should usually match the PixelsPerUnit in the import settings for the sprites being combined, at least for pixel art games).
    ///     3) Set "Path" to the path under your project's Assets folder where you would like the generated textures to be stored.
    ///     4) Press "Combine Sprites" to generate the texture!
    ///     
    ///     Note that for SpriteRenderers that use different materials or tint colors, different combined sprites will need to be generated.
    ///     The color and material for the first SpriteRenderer subobject/object in list is used for the SpriteRenderer on the object 
    ///     that gets created when "CreateObjectInScene" is enabled.
    ///     </remarks>
    public class SimpleSpriteCombine : MonoBehaviour
    {
#if UNITY_EDITOR
        [Tooltip("Pixels Per Unit to use, should usually match for all sprites (check their import settings)")]
        public float PixelsPerUnit = 20.0f;
        public int EdgePadding = 2;
        public TextureFormat NewTextureFormat = TextureFormat.RGBA32;
        public string Path = "Sprites/CombinedSprites";
        public Color FillColor = Color.clear;
        [Tooltip("Whether to setup an object in this scene with the generated texture as its sprite")]
        public bool CreateObjectInScene = true;
        [Tooltip("If there's an animator, we skip this object as sprites on it may be getting animated")]
        public bool SkipObjectsWithAnimators = true;
        public bool SkipDisabledObjects = false;
        [Tooltip("When enabled, don't worry about ordering when drawing multiple sprites on the same pixels. May be required on very large textures (> 40000x40000-ish size)")]
        public bool IgnoreSortingLayers = false;
        [Tooltip("Whether to combine sprites from objects that are subobjects of this object")]
        public bool CombineFromSubobjects = true;
        [Tooltip("Combine sprites from these specific objects. Useful if you don't want to make all the sprites subobjects of this object")]
        public Transform[] CombineFromSpecificObjects;

        [Button("Combine Sprites")]
        public void CombineSprites()
        {
            List<SpriteRenderer> rendererList = getAllSpriteRenderers();
            if (rendererList.Count == 0)
            {
                Debug.Log("No sprites found to combine");
                return;
            }

            Vector2 min, max;
            determineNewTextureSize(rendererList, out min, out max);

            IntegerVector textureSize = max - min;
            textureSize.X += this.EdgePadding * 2;
            textureSize.Y += this.EdgePadding * 2;
            Texture2D createdTexture = new Texture2D(textureSize.X, textureSize.Y, this.NewTextureFormat, false);
            int x, y;
            for (x = 0; x < textureSize.X; ++x)
                for (y = 0; y < textureSize.Y; ++y)
                    createdTexture.SetPixel(x, y, this.FillColor);

            Vector2 pixelCenter = (max + min) / 2.0f;
            Vector2 unitsCenter = pixelCenter / this.PixelsPerUnit;

            // Add sprites to the created texture
            Vector2 curr;
            IntegerVector spriteSize;
            Sprite currSprite;
            Texture2D spriteTexture;
            IntegerVector textureRectOffset;
            int textX, baseTextY, textY;
            TextureImporterSettings tis = new TextureImporterSettings();
            Color pixelColor;

            Dictionary<int, IntegerVector> sortingLayersByPixel = new Dictionary<int, IntegerVector>();
            int pixelKeyXMod = Mathf.RoundToInt(Mathf.Pow(10, Mathf.Floor(Mathf.Log10(createdTexture.height) + 1)));
            Debug.Log("Pixel Key Mod: " + pixelKeyXMod + ", texture dimensions " + createdTexture.width + ", " + createdTexture.height);
            bool prevPixelDataExists = false;
            IntegerVector sortingData;
            int rendererSortingLayer = 0;

            for (int i = 0; i < rendererList.Count; ++i)
            {
                curr.x = rendererList[i].transform.position.x * this.PixelsPerUnit - min.x + this.EdgePadding;
                curr.y = rendererList[i].transform.position.y * this.PixelsPerUnit - min.y + this.EdgePadding;

                currSprite = rendererList[i].sprite;
                spriteSize = currSprite.rect.size;

                // Make sure texture for the sprite we're reading is readable and has FullRect mesh type
                TextureImporter currImporter = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(currSprite.texture));
                currImporter.ReadTextureSettings(tis);
                bool needReadWriteSet = !currImporter.isReadable;
                bool needMeshTypeSet = tis.spriteMeshType != SpriteMeshType.FullRect;
                if (needMeshTypeSet)
                {
                    tis.spriteMeshType = SpriteMeshType.FullRect;
                    currImporter.SetTextureSettings(tis);
                }
                if (needReadWriteSet)
                    currImporter.isReadable = true;
                if (needReadWriteSet || needMeshTypeSet)
                    currImporter.SaveAndReimport();

                // Copy the pixels from the sprite's texture to the created texture
                spriteTexture = currSprite.texture;
                textureRectOffset = currSprite.textureRect.position;

                bool flipX = rendererList[i].flipX;
                bool flipY = rendererList[i].flipY;
                textX = !flipX ? Mathf.RoundToInt(curr.x - ((float)spriteSize.X) / 2.0f) : Mathf.RoundToInt(curr.x + ((float)spriteSize.X) / 2.0f - 0.5f);
                baseTextY = !flipY ? Mathf.RoundToInt(curr.y - ((float)spriteSize.Y) / 2.0f) : Mathf.RoundToInt(curr.y + ((float)spriteSize.Y) / 2.0f - 0.5f);
                textY = baseTextY;

                for (x = textureRectOffset.X; x < textureRectOffset.X + spriteSize.X; ++x)
                {
                    for (y = textureRectOffset.Y; y < textureRectOffset.Y + spriteSize.Y; ++y)
                    {
                        // Check if we already drew a pixel here, and if so if it is in a higher layer than the one we're about to draw
                        int pixelKey = textX * pixelKeyXMod + textY;
                        bool prevPixelOk = true;
                        if (!this.IgnoreSortingLayers)
                        {
                            prevPixelDataExists = sortingLayersByPixel.TryGetValue(pixelKey, out sortingData);
                            if (prevPixelDataExists)
                            {
                                rendererSortingLayer = SortingLayer.GetLayerValueFromID(rendererList[i].sortingLayerID);
                                if (rendererSortingLayer < sortingData.X || (rendererSortingLayer == sortingData.X && rendererList[i].sortingOrder <= sortingData.Y))
                                    prevPixelOk = false;
                            }
                        }

                        if (prevPixelOk)
                        {
                            pixelColor = spriteTexture.GetPixel(x, y);
                            if (pixelColor.a > 0) // If this pixel is in a transparent part of the sprite, don't draw it as it may be drawing over another sprite
                            {
                                // Draw the pixel
                                createdTexture.SetPixel(textX, textY, pixelColor);

                                if (!this.IgnoreSortingLayers)
                                {
                                    // Store the sorting layer data for this sprite at the position of this pixel in case we try to draw to this pixel again
                                    if (prevPixelDataExists)
                                    {
                                        sortingData = new IntegerVector(rendererSortingLayer, rendererList[i].sortingOrder);
                                        sortingLayersByPixel[pixelKey] = sortingData;
                                    }
                                    else
                                    {
                                        sortingData = new IntegerVector(SortingLayer.GetLayerValueFromID(rendererList[i].sortingLayerID), rendererList[i].sortingOrder);
                                        sortingLayersByPixel.Add(pixelKey, sortingData);
                                    }
                                }
                            }
                        }
                        textY += !flipY ? 1 : -1;
                    }

                    textX += !flipX ? 1 : -1;
                    textY = baseTextY;
                }

                // Return the sprite's texture settings to what they were before if necessary
                if (needMeshTypeSet)
                {
                    tis.spriteMeshType = SpriteMeshType.Tight;
                    currImporter.SetTextureSettings(tis);
                }
                if (needReadWriteSet)
                    currImporter.isReadable = false;
                if (needReadWriteSet || needMeshTypeSet)
                    currImporter.SaveAndReimport();
            }

            // Write the texture to streaming assets folder
            string assetName = this.gameObject.scene.name + StringExtensions.UNDERSCORE + this.gameObject.name;
            string filename = assetName + ".png";
            string directoryPath = System.IO.Path.Combine(Application.dataPath, this.Path);
            string path = System.IO.Path.Combine(directoryPath, filename);
            Debug.Log("Writing combined texture (position at " + unitsCenter.x + ", " + unitsCenter.y + ") " + path);
            byte[] textureBytes = createdTexture.EncodeToPNG();
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            File.WriteAllBytes(path, textureBytes);
            AssetDatabase.Refresh();

            // Set texture import settings
            string assetsPath = "Assets/" + this.Path + "/" + filename;
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetsPath);
            importer.spritePixelsPerUnit = this.PixelsPerUnit;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Point;
            importer.ReadTextureSettings(tis);
            tis.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(tis);
            importer.SaveAndReimport();

            // Set up object in scene with new sprite
            if (this.CreateObjectInScene)
            {
                GameObject sceneObject = GameObject.Find(assetName);
                if (sceneObject == null)
                    sceneObject = new GameObject(assetName);
                SpriteRenderer sceneObjectRenderer = sceneObject.GetComponent<SpriteRenderer>();
                if (sceneObjectRenderer == null)
                    sceneObjectRenderer = sceneObject.AddComponent<SpriteRenderer>();
                
                sceneObject.transform.position = new Vector3(unitsCenter.x, unitsCenter.y, this.transform.position.z);
                sceneObjectRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetsPath);
                sceneObjectRenderer.sortingLayerID = rendererList[0].sortingLayerID;
                sceneObjectRenderer.sortingOrder = rendererList[0].sortingOrder;
                sceneObjectRenderer.sharedMaterial = rendererList[0].sharedMaterial;
                sceneObjectRenderer.color = rendererList[0].color;

                // Highlight the scene object
                Selection.activeObject = sceneObject;
            }

            // Ping object in project hierarchy
            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(assetsPath));

            // Mark scene as dirty to make sure changes don't get wiped
            EditorSceneManager.MarkAllScenesDirty();
        }

        [Button("Toggle Child Renderers")]
        public void ToggleChildRenderers()
        {
            List<SpriteRenderer> rendererList = getAllSpriteRenderers();
            for (int i = 0; i < rendererList.Count; ++i)
            {
                rendererList[i].enabled = !rendererList[i].enabled;
            }
        }

        private List<SpriteRenderer> getAllSpriteRenderers()
        {
            List<SpriteRenderer> rendererList = new List<SpriteRenderer>();

            if (this.CombineFromSubobjects)
                getSpriteRenderers(this.transform, rendererList);

            if (this.CombineFromSpecificObjects != null)
            {
                for (int i = 0; i < this.CombineFromSpecificObjects.Length; ++i)
                {
                    getSpriteRenderers(this.CombineFromSpecificObjects[i], rendererList);
                }
            }
            
            return rendererList;
        }

        private void getSpriteRenderers(Transform obj, List<SpriteRenderer> rendererList)
        {
            if (this.SkipObjectsWithAnimators && obj.GetComponent<Animator>() != null)
                return;
            
            // Add our renderer
            SpriteRenderer objRenderer = obj.GetComponent<SpriteRenderer>();
            if (objRenderer != null && (!this.SkipDisabledObjects || (objRenderer.enabled && objRenderer.gameObject.activeInHierarchy)))
                rendererList.Add(objRenderer);
            
            // Add renderers on subobjects
            for (int i = 0; i < obj.childCount; ++i)
            {
                getSpriteRenderers(obj.GetChild(i), rendererList);
            }
        }

        private void determineNewTextureSize(List<SpriteRenderer> rendererList, out Vector2 min, out Vector2 max)
        {
            min.x = min.y = max.x = max.y = 0.0f;
            bool initialized = false;
            Vector2 curr, spriteSize, currMin, currMax;
            Sprite currSprite;

            for (int i = 0; i < rendererList.Count; ++i)
            {
                curr.x = rendererList[i].transform.position.x * this.PixelsPerUnit;
                curr.y = rendererList[i].transform.position.y * this.PixelsPerUnit;

                currSprite = rendererList[i].sprite;
                spriteSize = currSprite.rect.size;
                currMin.x = curr.x - spriteSize.x / 2.0f;
                currMin.y = curr.y - spriteSize.y / 2.0f;
                currMax.x = curr.x + spriteSize.x / 2.0f;
                currMax.y = curr.y + spriteSize.y / 2.0f;
                
                if (!initialized)
                {
                    min.x = currMin.x;
                    min.y = currMin.y;
                    max.x = currMax.x;
                    max.y = currMax.y;
                    initialized = true;
                }
                else
                {
                    if (currMin.x < min.x) min.x = currMin.x;
                    if (currMin.y < min.y) min.y = currMin.y;
                    if (currMax.x > max.x) max.x = currMax.x;
                    if (currMax.y > max.y) max.y = currMax.y;
                }
            }
        }
#endif
    }
}
