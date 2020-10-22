using System;
using System.Collections.Generic;
using Prototype.Element;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.Rendering
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class PixelImageRenderer : MonoBehaviour
    {
        [SerializeField]
        private PixelImageAsset _imageAsset;
        public PixelImage Image { get; private set; }
        
        public Vector2Int RenderGridSize { get; private set; }
        
        public Mesh Mesh { get; private set; }

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            Mesh = new Mesh();
            _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.sharedMesh = Mesh;

            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial = PixelAssetManager.Instance.PixelImageMaterial;    
        }

        private void Update()
        {
            if(_imageAsset && Image is null)
                UpdatePixelImage(_imageAsset.Image);
        }

        public void UpdatePixelImage(PixelImage image)
        {
            Image = image;

            if (image.Size != RenderGridSize)
            {
                RenderGridSize = image.Size;
                
                Mesh.Clear();
                Mesh = CreateImageMesh(image);
                _meshFilter.sharedMesh = Mesh;
            }

            var uvs = Mesh.uv;
            var colors = Mesh.colors;

            for (var y = 0; y < RenderGridSize.y; y++)
            {
                for (var x = 0; x < RenderGridSize.x; x++)
                {
                    var baseIdx = y * RenderGridSize.x * 4 + x * 4;
                    
                    var pixelType = image.Pixels[x, y]?.Type;
                    if (pixelType is null)
                    {
                        colors[baseIdx + 0]
                            = colors[baseIdx + 1]
                            = colors[baseIdx + 2]
                            = colors[baseIdx + 3] 
                            = Color.white.Transparent();
                    }
                    else
                    {
                        var rect = pixelType.sprite.rect.Scale(pixelType.sprite.texture.texelSize);
                        uvs[baseIdx + 0] = new Vector2(rect.xMin, rect.yMin);
                        uvs[baseIdx + 1] = new Vector2(rect.xMax, rect.yMin);
                        uvs[baseIdx + 2] = new Vector2(rect.xMax, rect.yMax);
                        uvs[baseIdx + 3] = new Vector2(rect.xMin, rect.yMax);
                        colors[baseIdx + 0]
                            = colors[baseIdx + 1]
                            = colors[baseIdx + 2]
                            = colors[baseIdx + 3] 
                            = Color.white;
                    }
                }
            }
            Mesh.SetColors(colors);
            Mesh.SetUVs(0, uvs);
            Mesh.RecalculateBounds();
            Mesh.UploadMeshData(false);
        }

        /// <summary>
        /// Generate grid mesh in x,y ∈ [-.5, .5]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Mesh CreateImageMesh(PixelImage image)
        {
            var vertices = ObjectPool<List<Vector3>>.Get();
            var indices = ObjectPool<List<int>>.Get();
            var uvs = ObjectPool<List<Vector2>>.Get();
            var colors = ObjectPool<List<Color>>.Get();
            Vector2 pixelSize = Vector2.one / image.Size;
            Vector2 offset = Vector2.one * -.5f;
            for (var y = 0; y < image.Size.y; y++)
            {
                for (var x = 0; x < image.Size.x; x++)
                {
                    var pos = new Vector2(x, y);
                    var baseIdx = vertices.Count;
                    vertices.Add((new Vector2(0, 0) + pos) * pixelSize + offset);
                    vertices.Add((new Vector2(1, 0) + pos) * pixelSize + offset);
                    vertices.Add((new Vector2(1, 1) + pos) * pixelSize + offset);
                    vertices.Add((new Vector2(0, 1) + pos) * pixelSize + offset);
                    indices.Add(0 + baseIdx);
                    indices.Add(2 + baseIdx);
                    indices.Add(1 + baseIdx);
                    indices.Add(0 + baseIdx);
                    indices.Add(3 + baseIdx);
                    indices.Add(2 + baseIdx);
                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(1, 1));
                    uvs.Add(new Vector2(0, 1));
                    colors.Add(Color.white.Transparent());
                    colors.Add(Color.white.Transparent());
                    colors.Add(Color.white.Transparent());
                    colors.Add(Color.white.Transparent());
                }
            }
            var mesh = new Mesh();
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(indices, 0);
            mesh.SetUVs(0, uvs);
            mesh.SetColors(colors);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            vertices.Clear();
            indices.Clear();
            uvs.Clear();
            colors.Clear();
            
            ObjectPool.Release(vertices);
            ObjectPool.Release(indices);
            ObjectPool.Release(uvs);
            ObjectPool.Release(colors);

            return mesh;
        }
    }
}