﻿using System.Linq;
using Core2D;
using Xunit;

namespace Core2D.UnitTests
{
    public class LineSegmentTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = _factory.CreateLineSegment(_factory.CreatePointShape(), true, true);

            var target = segment.GetPoints();
            var count = target.Count();

            Assert.Equal(1, count);
            Assert.Contains(segment.Point, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToXamlString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateLineSegment(_factory.CreatePointShape(), true, true);

            var actual = target.ToXamlString();

            Assert.Equal("L0,0", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToSvgString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateLineSegment(_factory.CreatePointShape(), true, true);

            var actual = target.ToSvgString();

            Assert.Equal("L0,0", actual);
        }
    }
}
