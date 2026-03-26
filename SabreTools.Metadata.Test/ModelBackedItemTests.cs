using SabreTools.Data.Models.Metadata;
using Xunit;

namespace SabreTools.Metadata.Test
{
    public class ModelBackedItemTests
    {
        #region Private Testing Classes

        /// <summary>
        /// Testing implementation of DictionaryBase
        /// </summary>
        private class TestDictionaryBase : DictionaryBase
        {
            public const string NameKey = "__NAME__";
        }

        /// <summary>
        /// Testing implementation of ModelBackedItem
        /// </summary>
        private class TestModelBackedItem : ModelBackedItem<TestDictionaryBase>
        {
            #region Comparision Methods

            /// <inheritdoc/>
            public override bool Equals(ModelBackedItem? other)
            {
                // If other is null
                if (other is null)
                    return false;

                // If the type is mismatched
                if (other is not TestModelBackedItem otherItem)
                    return false;

                // Compare internal models
                return _internal.EqualTo(otherItem._internal);
            }

            /// <inheritdoc/>
            public override bool Equals(ModelBackedItem<TestDictionaryBase>? other)
            {
                // If other is null
                if (other is null)
                    return false;

                // If the type is mismatched
                if (other is not TestModelBackedItem otherItem)
                    return false;

                // Compare internal models
                return _internal.EqualTo(otherItem._internal);
            }

            #endregion
        }

        /// <summary>
        /// Alternate testing implementation of ModelBackedItem
        /// </summary>
        private class TestModelAltBackedItem : ModelBackedItem<TestDictionaryBase>
        {
            #region Comparision Methods

            /// <inheritdoc/>
            public override bool Equals(ModelBackedItem? other)
            {
                // If other is null
                if (other is null)
                    return false;

                // If the type is mismatched
                if (other is not TestModelAltBackedItem otherItem)
                    return false;

                // Compare internal models
                return _internal.EqualTo(otherItem._internal);
            }

            /// <inheritdoc/>
            public override bool Equals(ModelBackedItem<TestDictionaryBase>? other)
            {
                // If other is null
                if (other is null)
                    return false;

                // If the type is mismatched
                if (other is not TestModelAltBackedItem otherItem)
                    return false;

                // Compare internal models
                return _internal.EqualTo(otherItem._internal);
            }

            #endregion
        }

        #endregion

        #region Equals

        [Fact]
        public void Equals_NullOther_False()
        {
            ModelBackedItem self = new TestModelBackedItem();
            ModelBackedItem? other = null;

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_MismatchedType_False()
        {
            ModelBackedItem self = new TestModelBackedItem();
            ModelBackedItem? other = new TestModelAltBackedItem();

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_MismatchedTypeAlt_False()
        {
            ModelBackedItem self = new TestModelAltBackedItem();
            ModelBackedItem? other = new TestModelBackedItem();

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_DifferentModels_False()
        {
            ModelBackedItem<TestDictionaryBase> self = new TestModelBackedItem();
            self.Write(TestDictionaryBase.NameKey, "self");

            ModelBackedItem<TestDictionaryBase>? other = new TestModelBackedItem();
            other.Write(TestDictionaryBase.NameKey, "other");

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_EqualModels_True()
        {
            ModelBackedItem<TestDictionaryBase> self = new TestModelBackedItem();
            self.Write(TestDictionaryBase.NameKey, "name");

            ModelBackedItem<TestDictionaryBase>? other = new TestModelBackedItem();
            other.Write(TestDictionaryBase.NameKey, "name");

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        #endregion

        #region Remove

        [Fact]
        public void Remove_NullItem_False()
        {
            TestModelBackedItem? modelBackedItem = null;
            string? fieldName = TestDictionaryBase.NameKey;
            bool? actual = modelBackedItem?.Remove(fieldName);
            Assert.Null(actual);
        }

        [Fact]
        public void Remove_EmptyFieldName_False()
        {
            var modelBackedItem = new TestModelBackedItem();
            string? fieldName = string.Empty;
            bool actual = modelBackedItem.Remove(fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void Remove_MissingKey_True()
        {
            var modelBackedItem = new TestModelBackedItem();
            string? fieldName = TestDictionaryBase.NameKey;
            bool actual = modelBackedItem.Remove(fieldName);
            Assert.True(actual);
            Assert.Null(modelBackedItem.ReadString(fieldName));
        }

        [Fact]
        public void Remove_ValidKey_True()
        {
            var modelBackedItem = new TestModelBackedItem();
            modelBackedItem.Write(TestDictionaryBase.NameKey, "value");
            string? fieldName = TestDictionaryBase.NameKey;
            bool actual = modelBackedItem.Remove(fieldName);
            Assert.True(actual);
            Assert.Null(modelBackedItem.ReadString(fieldName));
        }

        #endregion

        #region Replace

        [Fact]
        public void Replace_NullFrom_False()
        {
            TestModelBackedItem? from = null;
            var to = new TestModelBackedItem();
            string? fieldName = TestDictionaryBase.NameKey;
            bool actual = to.Replace(from, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void Replace_NullTo_False()
        {
            TestModelBackedItem? from = null;
            TestModelBackedItem? to = new TestModelBackedItem();
            string? fieldName = TestDictionaryBase.NameKey;
            bool actual = to.Replace(from, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void Replace_EmptyFieldName_False()
        {
            TestModelBackedItem? from = new TestModelBackedItem();
            TestModelBackedItem? to = new TestModelBackedItem();
            string? fieldName = string.Empty;
            bool actual = to.Replace(from, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void Replace_MissingKey_False()
        {
            TestModelBackedItem? from = new TestModelBackedItem();
            TestModelBackedItem? to = new TestModelBackedItem();
            string? fieldName = TestDictionaryBase.NameKey;
            bool actual = to.Replace(from, fieldName);
            Assert.False(actual);
        }

        [Fact]
        public void Replace_ValidKey_True()
        {
            TestModelBackedItem? from = new TestModelBackedItem();
            from.Write(TestDictionaryBase.NameKey, "value");
            TestModelBackedItem? to = new TestModelBackedItem();
            string? fieldName = TestDictionaryBase.NameKey;
            bool actual = to.Replace(from, fieldName);
            Assert.True(actual);
            Assert.Equal("value", to.ReadString(TestDictionaryBase.NameKey));
        }

        #endregion

        #region WriteWithValidation

        [Fact]
        public void WriteWithValidation_NullItem_False()
        {
            TestModelBackedItem? modelBackedItem = null;
            string? fieldName = TestDictionaryBase.NameKey;
            object value = "value";
            bool? actual = modelBackedItem?.WriteWithValidation(fieldName, value);
            Assert.Null(actual);
        }

        [Fact]
        public void WriteWithValidation_EmptyFieldName_False()
        {
            TestModelBackedItem? modelBackedItem = new TestModelBackedItem();
            string? fieldName = string.Empty;
            object value = "value";
            bool actual = modelBackedItem.WriteWithValidation(fieldName, value);
            Assert.False(actual);
        }

        [Fact]
        public void WriteWithValidation_MissingKey_False()
        {
            TestModelBackedItem? modelBackedItem = new TestModelBackedItem();
            string? fieldName = Rom.SHA1Key;
            object value = "value";
            bool actual = modelBackedItem.WriteWithValidation(fieldName, value);
            Assert.False(actual);
        }

        [Fact]
        public void WriteWithValidation_InvalidKey_True()
        {
            TestModelBackedItem? modelBackedItem = new TestModelBackedItem();
            modelBackedItem.Write(TestDictionaryBase.NameKey, "old");
            string? fieldName = "INVALID";
            object value = "value";
            bool actual = modelBackedItem.WriteWithValidation(fieldName, value);
            Assert.False(actual);
            Assert.Null(modelBackedItem.ReadString(fieldName));
        }

        [Fact]
        public void WriteWithValidation_ValidKey_True()
        {
            TestModelBackedItem? modelBackedItem = new TestModelBackedItem();
            modelBackedItem.Write(TestDictionaryBase.NameKey, "old");
            string? fieldName = TestDictionaryBase.NameKey;
            object value = "value";
            bool actual = modelBackedItem.WriteWithValidation(fieldName, value);
            Assert.True(actual);
            Assert.Equal(value, modelBackedItem.ReadString(fieldName));
        }

        #endregion
    }
}
