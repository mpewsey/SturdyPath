using GdUnit4;
using Godot;

namespace MPewsey.SturdyPath.Tests
{
    [TestSuite]
    public class TestSturdyPathRef
    {
        private const string TestResourcePath = "uid://rxhphhkir3js";

        private SturdyPathRef SturdyPathRef { get; set; }

        [Before]
        public void Before()
        {
            SturdyPathRef = ResourceLoader.Load<SturdyPathRef>(TestResourcePath);

            Assertions.AssertThat(string.IsNullOrEmpty(SturdyPathRef.ResPath)).IsFalse();
            Assertions.AssertThat(SturdyPathRef.ResPath.StartsWith("res://")).IsTrue();

            Assertions.AssertThat(string.IsNullOrEmpty(SturdyPathRef.UidPath)).IsFalse();
            Assertions.AssertThat(SturdyPathRef.UidPath.StartsWith("uid://")).IsTrue();
        }

        [TestCase]
        public void TestSetResPath()
        {
            var resource = new SturdyPathRef(SturdyPathRef.ResPath);

            Assertions.AssertThat(string.IsNullOrEmpty(resource.ResPath)).IsFalse();
            Assertions.AssertThat(resource.ResPath.StartsWith("res://")).IsTrue();
            Assertions.AssertThat(resource.ResPath).IsEqual(SturdyPathRef.ResPath);

            Assertions.AssertThat(string.IsNullOrEmpty(resource.UidPath)).IsFalse();
            Assertions.AssertThat(resource.UidPath.StartsWith("uid://")).IsTrue();
            Assertions.AssertThat(resource.UidPath).IsEqual(SturdyPathRef.UidPath);
        }

        [TestCase]
        public void TestGetLoadPathWithValidUid()
        {
            Assertions.AssertThat(SturdyPathRef.GetLoadPath()).IsEqual(SturdyPathRef.UidPath);
        }

        [TestCase]
        public void TestGetLoadPathWithInvalidUid()
        {
            var resource = new SturdyPathRef(SturdyPathRef.ResPath);
            resource.UidPath = "uid://bad_uid";
            Assertions.AssertThat(resource.GetLoadPath()).IsEqual(resource.ResPath);
        }

        [TestCase]
        public void TestLoad()
        {
            var resource = SturdyPathRef.Load();
            Assertions.AssertThat(resource == SturdyPathRef).IsTrue();
        }

        [TestCase]
        public void TestTypedLoad()
        {
            var resource = SturdyPathRef.Load<SturdyPathRef>();
            Assertions.AssertThat(resource == SturdyPathRef).IsTrue();
        }

        [TestCase]
        public void TestRefreshResourcePath()
        {
            var resource = new SturdyPathRef();
            resource.UidPath = SturdyPathRef.UidPath;
            Assertions.AssertThat(resource.RefreshResourcePath()).IsTrue();
            Assertions.AssertThat(resource.ResPath).IsEqual(SturdyPathRef.ResPath);
        }

        [TestCase]
        public void TestRefreshResourcePathWithBadUidReturnsFalse()
        {
            var resource = new SturdyPathRef();
            resource.UidPath = "uid://bad_uid";
            Assertions.AssertThat(resource.RefreshResourcePath()).IsFalse();
        }

        [TestCase]
        public void TestOpenResourceInEditorForInvalidResourceReturnsFalse()
        {
            var resource = new SturdyPathRef();
            Assertions.AssertThat(resource.OpenResourceInEditor()).IsFalse();
        }
    }
}