using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Prototype.Element;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests
{
    public class TestDiagonalConvertion
    {
        [Test]
        public void Size100Grid()
        {
            for (var y = -100; y <= 100; y++)
            for (var x = -100; x <= 100; x++)
            {
                var original = new Vector2Int(x, y);
                var diagonal = PixelWeaponAnalyser.ToDiagonal(original);
                var convertBack = PixelWeaponAnalyser.FromDiagonal(diagonal);
                Assert.AreEqual(convertBack, original);
            }
        }

        [Test]
        public void RandomTest()
        {
            var rand = new System.Random();
            for (var i = 0; i < 10000; i++)
            {
                var original = new Vector2Int(
                    rand.Next(-int.MaxValue / 2, int.MaxValue / 2),
                    rand.Next(-int.MaxValue / 2, int.MaxValue / 2));
                var diagonal = PixelWeaponAnalyser.ToDiagonal(original);
                var convertBack = PixelWeaponAnalyser.FromDiagonal(diagonal);
                Assert.AreEqual(convertBack, original);
                
            }
        }
    }
}
