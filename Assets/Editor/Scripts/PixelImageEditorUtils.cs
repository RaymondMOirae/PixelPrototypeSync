using System.Collections.Generic;
using Prototype.Element;
using Prototype.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace Prototype.Editor
{
    public static class PixelImageEditorUtils
    {
        public static Mesh CreateImageMesh(PixelImage image)
        {
            var vertices = Utils.ObjectPool<List<Vector3>>.Get();
            var indices = Utils.ObjectPool<List<int>>.Get();
            var uvs = Utils.ObjectPool<List<Vector2>>.Get();
            var colors = Utils.ObjectPool<List<Color>>.Get();
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

        public static void UpdateImageMesh(PixelImage image, Mesh mesh)
        {
            var uvs = mesh.uv;
            var colors = mesh.colors;

            for (var y = 0; y < image.Size.y; y++)
            {
                for (var x = 0; x < image.Size.x; x++)
                {
                    var baseIdx = y * image.Size.x * 4 + x * 4;
                    
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
            mesh.SetColors(colors);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);
        }

        public static void RenderImage(Mesh imageMesh, RenderTexture target)
        {
            var cmd = CommandBufferPool.Get();
            cmd.SetRenderTarget(target);
            cmd.ClearRenderTarget(true, true, Color.white.Transparent());
            cmd.SetGlobalTexture("_MainTex", PixelAssetManager.Instance.PixelTexture);
            cmd.DrawMesh(imageMesh, Matrix4x4.identity, ShaderPool.Get("Prototype/Editor/CanvasGridCode"), 0, 1);
            
            Graphics.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}