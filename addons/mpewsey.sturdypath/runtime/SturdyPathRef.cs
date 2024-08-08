using Godot;

namespace MPewsey.SturdyPath
{
    /// <summary>
    /// Holds weak resource path references in a way that prevents path invalidation
    /// when files are moved within a Godot project.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class SturdyPathRef : Resource
    {
        /// <summary>
        /// The valid resource path extensions.
        /// </summary>
        private const string ResPathExtensions = "*.tscn,*.tres,*.scn,*.res";

        /// <summary>
        /// This property is used as a Godot inspector button only and should not be used via script.
        /// When set to true, it opens the target resource in the Godot editor.
        /// The property always returns false.
        /// </summary>
        [Export] public bool _OpenResource { get => false; set => CallDeferred(MethodName.OpenResourceInEditor, value); }

        /// <summary>
        /// This property is used as a Godot inspector button only and should not be used via script.
        /// When set to true, it refreshes the Res path to its current destination based on the current Uid path.
        /// The property always returns false.
        /// </summary>
        [Export] public bool _RefreshResPath { get => false; set => RefreshResPath(value); }

        private string _resPath;
        /// <summary>
        /// The target resource path, beginning with with res://. This path may become invalid if the target resource file is moved.
        /// </summary>
        [Export(PropertyHint.File, ResPathExtensions)] public string ResPath { get => _resPath; set => SetResPath(value); }

        /// <summary>
        /// The Uid path for the target resource, beginning with uid://. This path should never become invalid unless the target resource file is deleted.
        /// </summary>
        [Export] public string UidPath { get; set; }

        public SturdyPathRef()
        {

        }

        /// <summary>
        /// Creates a new sturdy path.
        /// </summary>
        /// <param name="resPath">The resource path, beginning with res://.</param>
        public SturdyPathRef(string resPath)
        {
            ResPath = resPath;
        }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (name == PropertyName.UidPath)
                property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly);
        }

        /// <summary>
        /// Sets the resource path to the specified value and sets the corresponding Uid path.
        /// </summary>
        /// <param name="value">The resource path.</param>
        private void SetResPath(string value)
        {
            _resPath = value;
            UidPath = GetUidPath(value);
        }

        /// <summary>
        /// Returns the Uid path corresponding to the specified resource path.
        /// </summary>
        /// <param name="resPath">The resource path, beginning with res://.</param>
        private static string GetUidPath(string resPath)
        {
            var id = ResourceLoader.GetResourceUid(resPath);
            return id == -1 ? null : ResourceUid.IdToText(id);
        }

        /// <summary>
        /// Updates the resource path to its current destination based on the current Uid path.
        /// Returns true if successful.
        /// </summary>
        /// <param name="run">Performs the operation only when set to true.</param>
        public bool RefreshResPath(bool run = true)
        {
            if (!run)
                return false;

            if (!ResourceLoader.Exists(UidPath))
            {
                GD.PrintErr($"Could not update resource path. Uid path does not exist: {UidPath}");
                return false;
            }

            ResPath = ResourceLoader.Load(UidPath).ResourcePath;
            GD.PrintRich("[color=#00ff00]Resource path updated.[/color]");
            return true;
        }

        /// <summary>
        /// Opens the target resource in the editor if it exists. Returns true if successful.
        /// This operation only works within the Godot editor.
        /// </summary>
        /// <param name="run">Performs the operation only when set to true.</param>
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
                    return false;
                }

                var resource = ResourceLoader.Load(path);

                switch (resource)
                {
                    case PackedScene:
                        EditorInterface.Singleton.OpenSceneFromPath(resource.ResourcePath);
                        GD.PrintRich($"[color=#00ff00]Opened scene: {resource.ResourcePath}[/color]");
                        break;
                    default:
                        EditorInterface.Singleton.EditResource(resource);
                        GD.PrintRich($"[color=#00ff00]Opened resource: {resource.ResourcePath}[/color]");
                        break;
                }

                return true;
            }
#endif

            GD.PrintErr("Resources can only be opened in the Godot editor.");
            return false;
        }

        /// <summary>
        /// Returns the controlling resource load path. If the Uid path exists, then it is returned.
        /// Otherwise, the Res path is returned.
        /// </summary>
        /// <param name="typeHint">The resource type hint.</param>
        private string GetLoadPath(string typeHint = null)
        {
            return ResourceLoader.Exists(UidPath, typeHint) ? UidPath : ResPath;
        }

        /// <summary>
        /// Loads the target resource and returns it.
        /// See the Godot ResourceLoader.Load method for an explaination of parameters.
        /// </summary>
        public Resource Load(string typeHint = null, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse)
        {
            return ResourceLoader.Load(GetLoadPath(typeHint), typeHint, cacheMode);
        }

        /// <summary>
        /// Loads the target resource and returns it.
        /// See the Godot ResourceLoader.Load method for an explaination of parameters.
        /// </summary>
        public T Load<T>(string typeHint = null, ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse) where T : class
        {
            return ResourceLoader.Load<T>(GetLoadPath(typeHint), typeHint, cacheMode);
        }
    }
}
