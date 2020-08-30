using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace vtb.Utils.Tests
{
    public class CheckTests
    {
        [Test]
        public void NotEmpty_Will_Not_Throw_When_Valid_String()
        {
            Assert.DoesNotThrow(() => Check.NotEmpty("test", "testField"));
        }

        [Test]
        public void NotEmpty_Will_Not_Throw_When_Valid_Array_Of_Strings()
        {
            Assert.DoesNotThrow(() => Check.NotEmpty(new[] {"test1", "test2"}, "testField"));
        }

        [Test]
        public void NotEmpty_Will_Throw_When_Empty_String()
        {
            var e = Assert.Throws<ArgumentException>(() => Check.NotEmpty("", "testField"));
            Assert.AreEqual("testField", e.Message);
        }

        [Test]
        public void NotEmpty_Will_Throw_When_Whitespace()
        {
            var e = Assert.Throws<ArgumentException>(() => Check.NotEmpty("\n", "testField"));
            Assert.AreEqual("testField", e.Message);
        }

        [Test]
        public void NotEmpty_Will_Throw_When_Empty_Array()
        {
            var e = Assert.Throws<ArgumentException>(() => Check.NotEmpty(Array.Empty<string>(), "testField"));
            Assert.AreEqual("testField", e.Message);
        }

        [Test]
        public void NotEmpty_Will_Throw_When_Null_Array()
        {
            var e = Assert.Throws<ArgumentException>(() => Check.NotEmpty(default(string[]), "testField"));
            Assert.AreEqual("testField", e.Message);
        }

        [Test]
        public void NotNull_Will_Not_Throw_When_Valid()
        {
            Assert.DoesNotThrow(() => Check.NotNull("", "testField"));
        }

        [Test]
        public void NotNull_Will_Throw_ArgumentNullException_When_Null()
        {
            var e = Assert.Throws<ArgumentNullException>(() => Check.NotNull(null, "testField"));
            Assert.AreEqual("testField", e.ParamName);
        }

        [Test]
        public void GuidNotEmpty_Will_Not_Throw_When_Valid()
        {
            Assert.DoesNotThrow(() => Check.GuidNotEmpty(Guid.NewGuid(), "testField"));
        }

        [Test]
        public void GuidNotEmpty_Will_Throw_ArgumentNullException_When_Null()
        {
            var e = Assert.Throws<ArgumentNullException>(() => Check.GuidNotEmpty(null, "testField"));
            Assert.AreEqual("testField", e.ParamName);
        }

        [Test]
        public void GuidNotEmpty_Will_Throw_ArgumentException_When_Empty()
        {
            var e = Assert.Throws<ArgumentException>(() => Check.GuidNotEmpty(Guid.Empty, "testField"));
            Assert.AreEqual("testField", e.Message);
        }

        [Test]
        public void NotDefault_Will_Not_Throw_When_Valid_DateTime()
        {
            Assert.DoesNotThrow(() => Check.NotDefault(DateTime.Now, "test_field"));
        }

        [Test]
        public void NotDefault_Will_Throw_ArgumentException_When_Default_DateTime()
        {
            var e = Assert.Throws<ArgumentException>(() => Check.NotDefault(default, "testField"));
            Assert.AreEqual("testField", e.Message);
        }

        [Test]
        public void EntityFound_Will_Not_Throw_When_Found()
        {
            var id = Guid.NewGuid();
            var entity = new SampleEntity {Id = id};

            Assert.DoesNotThrow(() => Check.EntityFound(id, entity));
        }

        [Test]
        public void Entity_Will_Throw_ArgumentException_When_Empty_Id()
        {
            var e = Assert.Throws<ArgumentException>(() => Check.EntityFound(Guid.Empty, new SampleEntity()));
            Assert.AreEqual("id", e.Message);
        }

        [Test]
        public void Entity_Will_Throw_ArgumentException_When_Null_Entity()
        {
            var id = Guid.NewGuid();
            var e = Assert.Throws<KeyNotFoundException>(() => Check.EntityFound<SampleEntity>(id, null));

            var entityType = typeof(SampleEntity);
            Assert.AreEqual($"Entity {entityType}#{id} was not found.", e.Message);
        }

        [TestCase(3, 2, true)]
        [TestCase(1, 2, false)]
        [TestCase(2, 2, false)]
        public void GreaterThan_Throw_ArgumentException_When_Value_Is_Lower_Or_Equal(int testedValue,
            int referenceValue, bool result)
        {
            if (result)
            {
                Assert.DoesNotThrow(() => Check.GreaterThan(testedValue, referenceValue, nameof(testedValue)));
            }
            else
            {
                var e = Assert.Throws<ArgumentException>(() =>
                    Check.GreaterThan(testedValue, referenceValue, nameof(testedValue)));
                Assert.AreEqual("testedValue", e.Message);
            }
        }

        private class SampleEntity
        {
            public Guid Id { get; set; }
        }
    }
}