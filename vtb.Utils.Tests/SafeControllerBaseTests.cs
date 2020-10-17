using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace vtb.Utils.Tests
{
    public class SafeControllerBaseTests
    {
        private TestController _controller;
        
        [SetUp]
        public void SetUp()
        {
            _controller = new TestController();
        }

        [Test]
        public async Task Will_Catch_Any_Exception()
        {
            var result1 = await _controller.Passthrough(async () => throw new Exception());
            result1.Should().BeOfType(typeof(StatusCodeResult));

            var result2 = await _controller.Passthrough(async () => throw new InvalidOperationException());
            result2.Should().BeOfType(typeof(StatusCodeResult));
        }

        [Test]
        public async Task Will_Map_Exception_To_Response()
        {
            var result = await _controller.Passthrough(async () => throw new KeyNotFoundException());
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Test]
        public async Task Will_Map_Exception_To_Overriden_Response()
        {
            var result = await _controller.Passthrough(async () => throw new KeyNotFoundException(), new Dictionary<Type, Func<IActionResult>>()
            {
                { typeof(KeyNotFoundException), _controller.Conflict }
            });

            result.Should().BeOfType(typeof(ConflictResult));
        }

        [Test]
        public async Task Will_Map_Exception_To_Custom_Response()
        {
            var result = await _controller.Passthrough(async () => throw new InvalidOperationException(), new Dictionary<Type, Func<IActionResult>>()
            {
                { typeof(InvalidOperationException), _controller.Conflict }
            });

            result.Should().BeOfType(typeof(ConflictResult));
        }

        public class TestController : SafeControllerBase
        {
            public TestController() : base()
            {
                _exceptionToResponseMap.Add(typeof(KeyNotFoundException), NotFound);
            }

            internal Task<IActionResult> Passthrough(Func<Task<IActionResult>> func, Dictionary<Type, Func<IActionResult>> overrides = null)
                => SafeInvoke(func, overrides);
        }
    }
}