using System.Collections.Generic;
using Prototype.Element;
using Prototype.Settings;
using Prototype.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace Prototype.Rendering
{
    public class PixelImageRenderHelper
    {
        public readonly Vector2Int Size;
        public Mesh Mesh { get; }
        public RenderTexture RenderTexture { get; }
        public Texture2D Texture { get; }
        readonly List<Vector2> _uvs = new List<Vector2>();
        readonly List<Color> _colors = new List<Color>();

        public PixelImageRenderHelper(Vector2Int size)
        {
            Size = size;
            RenderTexture = new RenderTexture(size.x * 8, size.y * 8, 0)
            {
                filterMode = FilterMode.Point,
                useMipMap = true,
                autoGenerateMips = false,
            };
            RenderTexture.Create();
            Texture = new Texture2D(size.x * 8, size.y * 8)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };
            Texture.Apply(true);
            Mesh = CreateMesh();
        }

        public void UpdateTexture(PixelImage image)
        {
            UpdateMesh(image);
            
            var cmd = CommandBufferPool.Get();
            cmd.SetRenderTarget(RenderTexture);
            cmd.ClearRenderTarget(true, true, Color.white.Transparent());
            cmd.SetGlobalTexture("_MainTex", PixelAssets.Current.PixelTexture);
            cmd.DrawMesh(Mesh, Matrix4x4.identity, ShaderPool.Get("Prototype/Editor/CanvasGridCode"), 0, 1);

            cmd.GenerateMips(RenderTexture);
            cmd.CopyTexture(RenderTexture, Texture);
            // cmd.CopyTexture(RenderTexture, 0, 0, Texture, 0, 0);
            // cmd.CopyTexture(RenderTexture, 0, 1, Texture, 0, 1);
            
            Graphics.ExecuteCommandBuffer(cmd);
                
            // Texture.Apply(true);
            CommandBufferPool.Release(cmd);
        }

        Mesh CreateMesh()
        {
            var vertices = Utils.ObjectPool<List<Vector3>>.Get();
            var indices = Utils.ObjectPool<List<int>>.Get();
            Vector2 pixelSize = Vector2.one / Size;
            Vector2 offset = Vector2.one * -.5f;
            for (var y = 0; y < Size.y; y++)
            {
                for (var x = 0; x < Size.x; x++)
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
                    _uvs.Add(new Vector2(0, 0));
                    _uvs.Add(new Vector2(1, 0));
                    _uvs.Add(new Vector2(1, 1));
                    _uvs.Add(new Vector2(0, 1));
                    _colors.Add(Color.white.Transparent());
                    _colors.Add(Color.white.Transparent());
                    _colors.Add(Color.white.Transparent());
                    _colors.Add(Color.white.Transparent());
                }
            }
            var mesh = new Mesh();
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(indices, 0);
            mesh.SetUVs(0, _uvs);
            mesh.SetColors(_colors);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            vertices.Clear();
            indices.Clear();
            ObjectPool.Release(vertices);
            ObjectPool.Release(indices);

            return mesh;
        }
        
        void UpdateMesh(PixelImage image)
        {
            Assert.AreEqual(image.Size, Size);
            
            for (var y = 0; y < image.Size.y; y++)
            {
                for (var x = 0; x < image.Size.x; x++)
                {
                    var baseIdx = y * image.Size.x * 4 + x * 4;
                    
                    var pixelType = image.Pixels[x, y]?.Type;
                    if (pixelType is null)
                    {
                        _colors[baseIdx + 0]
                            = _colors[baseIdx + 1]
                                = _colors[baseIdx + 2]
                                    = _colors[baseIdx + 3] 
                                        = Color.white.Transparent();
                    }
                    else
                    {
                        var rect = pixelType.sprite.rect.Scale(pixelType.sprite.texture.texelSize);
                        _uvs[baseIdx + 0] = new Vector2(rect.xMin, rect.yMin);
                        _uvs[baseIdx + 1] = new Vector2(rect.xMax, rect.yMin);
                        _uvs[baseIdx + 2] = new Vector2(rect.xMax, rect.yMax);
                        _uvs[baseIdx + 3] = new Vector2(rect.xMin, rect.yMax);
                        _colors[baseIdx + 0]
                            = _colors[baseIdx + 1] 
                                = _colors[baseIdx + 2]
                                    = _colors[baseIdx + 3] 
                                        = Color.white;
                    }
                }
            }
            Mesh.SetColors(_colors);
            Mesh.SetUVs(0, _uvs);
            Mesh.RecalculateBounds();
            Mesh.UploadMeshData(false);
        }
        
        
    }
}