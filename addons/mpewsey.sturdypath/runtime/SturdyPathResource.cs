using Godot;

namespace MPewsey.SturdyPath
{
    [Tool]
    [GlobalClass]
    public partial class SturdyPathResource : Resource
    {
        private const string ResPathExtensions = "*.tscn,*.tres,*.scn,*.res";

        [Export] public bool OpenResource { get => false; set => CallDeferred(MethodName.OpenResourceInEditor, value); }

        [Export] public bool RefreshResPath { get => false; set => UpdateResPath(value); }

        private string _resPath;
        [Export(PropertyHint.File, ResPathExtensions)] public string ResPath { get => _resPath; set => SetResPath(value); }

        [Export] public string UidPath { get; set; }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (name == PropertyName.UidPath)
                property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly);
        }

        private void SetResPath(string value)
        {
            _resPath = value;
            UidPath = GetUidPath(value);
        }

        public bool UpdateResPath(bool run = true)
        {
            if (!run)
                return false;

            if (!ResourceLoader.Exists(UidPath))
            {
                GD.PrintErr($"Could not update res:// path. uid:// path does not exist: {UidPath}");
                return true;
            }

            var resource = ResourceLoader.Load(UidPath);
            ResPath = resource.ResourcePath;
            GD.PrintRich("[color=#00ff00]res:// path updated.[/color]");
            return true;
        }

        public bool OpenResourceInEditor(bool run = true)
        {
            if (!run)
                return false;

#if TOOLS
            if (Engine.IsEditorHint())
            {
                var path = GetLoadPath();

                if (!ResourceLoader.Exists(path))
                {
                    GD.PrintErr("Target resource does not exist.");
                    return true;
                }

                var resource = ResourceLoader.Load(path);

                if (resource is PackedScene)
                {
                    EditorInterface.Singleton.OpenSceneFromPath(resource.ResourcePath);
                    GD.PrintRich($"[color=#00ff00]Opened scene: {resource.ResourcePath}[/color]");
                    return true;
                }

                EditorInterface.Singleton.EditResource(resource);
                GD.PrintRich($"[color=#00ff00]Opened resource: {resource.ResourcePath}[/color]");
                return true;
            }
#endif

            GD.PrintErr("Resources can only be opened in the Godot editor.");
            return false;
        }

        private static string GetUidPath(string path)
        {
            var id = ResourceLoader.GetResourceUid(path);
            return id == -1 ? null : ResourceUid.IdToText(id);
        }

        private string GetLoadPath(string typeHint = null)
        {
            return ResourceLoader.Exists(UidPath, typeHint) ? UidPath : ResPath;
        }

        public Resource Load(string typeHint = null, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse)
        {
            return ResourceLoader.Load(GetLoadPath(typeHint), typeHint, cacheMode);
        }

        public T Load<T>(string typeHint = null, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse) where T : class
        {
            return ResourceLoader.Load<T>(GetLoadPath(typeHint), typeHint, cacheMode);
        }
    }
}
