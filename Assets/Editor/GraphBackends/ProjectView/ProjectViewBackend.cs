using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using RelationsInspector.Extensions;

namespace RelationsInspector.Backend.ProjectView
{
	// Asset reference. The graph nodes are of this type.
	public class AssetRef
	{
		public int instanceID;
		public string name;
		public Texture2D icon;
		public bool highlight;
		public bool isFolder;
	}

	[Title("Project view")]
	[Version("1.0.0")]
	[Description("Shows the asset hierarchy.\n" + "Allows to filter and highlight certain types of nodes.")]
	public class ProjectViewBackend : MinimalBackend<AssetRef, string>
	{
		enum FilterMode { All, None, OfTypeScript, OfTypePrefab, OfTypeTexture, ReferencedByScene }
		static FilterMode includeFilterMode = FilterMode.All;
		static FilterMode markFilterMode = FilterMode.None;
		HashSet<int> sceneRefAssetIds;

		protected Dictionary<AssetRef, List<AssetRef>> assetTree;

		public override void Awake( GetAPI getAPI )
		{
			bool needSceneRefAssetIds = ( markFilterMode == FilterMode.ReferencedByScene || includeFilterMode == FilterMode.ReferencedByScene );
			sceneRefAssetIds = needSceneRefAssetIds ?
				GetActiveSceneDependencies().Select( obj => obj.GetInstanceID() ).ToHashSet() :
				new HashSet<int>();

			base.Awake( getAPI );
		}

		public override IEnumerable<AssetRef> Init( object target )
		{
			var asAssetRef = target as AssetRef;
			if ( asAssetRef == null )
				yield break;

			assetTree = new Dictionary<AssetRef, List<AssetRef>>();
			PopulateAssetSubtree( asAssetRef, GetFilterCondition( includeFilterMode ), GetFilterCondition( markFilterMode ) );


			yield return asAssetRef;
		}

		public override IEnumerable<Relation<AssetRef, string>> GetRelations( AssetRef entity )
		{
			return assetTree[ entity ]
				.Select( childRef => new Relation<AssetRef, string>( entity, childRef, string.Empty ) );
		}

		public override Rect OnGUI()
		{
			GUILayout.BeginHorizontal( EditorStyles.toolbar );
			{
				// tree creation
				if ( GUILayout.Button( "Create tree", EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) ) )
					api.ResetTargets( new object[] { GetAssetRef( "Assets" ) } );

				// tree filtering
				GUILayout.Space(10);
				EditorGUIUtility.labelWidth = 90;
				EditorGUI.BeginChangeCheck();
				includeFilterMode = (FilterMode) EditorGUILayout.EnumPopup( "Include assets", includeFilterMode, GUILayout.Width(250) );
				if ( EditorGUI.EndChangeCheck() )
				{
					api.Rebuild();
				}

				// marking nodes referenced by the scene
				GUILayout.Space( 10 );
				EditorGUIUtility.labelWidth = 90;
				EditorGUI.BeginChangeCheck();
				markFilterMode = (FilterMode) EditorGUILayout.EnumPopup( "Mark assets", markFilterMode, GUILayout.Width( 250 ) );
				if ( EditorGUI.EndChangeCheck() )
				{
					api.Rebuild();
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			return BackendUtil.GetMaxRect();
		}
		
		public override Rect DrawContent( AssetRef entity, EntityDrawContext drawContext )
		{
			//if ( !DoMark( entity ) )
			if(!entity.highlight)
				return DrawUtil.DrawContent( new GUIContent( entity.name, entity.icon ), drawContext );

			var bgColor = drawContext.style.backgroundColor;
			var targetBgColor = drawContext.style.targetBackgroundColor;
			drawContext.style.backgroundColor = drawContext.style.targetBackgroundColor = new Color( 0.7f, 0.7f, 1, 1 );
			var rect = DrawUtil.DrawContent( new GUIContent( entity.name, entity.icon ), drawContext );
			drawContext.style.backgroundColor = bgColor;
			drawContext.style.targetBackgroundColor = targetBgColor;
			return rect;
		}

		public override void OnEntitySelectionChange( AssetRef[] selection )
		{
			Selection.instanceIDs = selection.Select( assetRef => assetRef.instanceID ).ToArray();
		}

		System.Func<AssetRef, bool> GetFilterCondition( FilterMode filterMode )
		{
			switch ( filterMode )
			{
				case FilterMode.All:
				default:
					return ar => true;

				case FilterMode.None:
					return ar => false;

				case FilterMode.OfTypeScript:
					return ar => IsOfType( ar, typeof( MonoScript ) );

				case FilterMode.OfTypePrefab:
					return ar => IsOfType( ar, typeof( GameObject ) );

				case FilterMode.OfTypeTexture:
					return ar => IsOfType( ar, typeof( Texture ) );

				case FilterMode.ReferencedByScene:
					return ar => sceneRefAssetIds.Contains( ar.instanceID );
			}
		}

		bool AnyNodeSatisfies( AssetRef assetRef, Dictionary<AssetRef, List<AssetRef>> tree, System.Func<AssetRef, bool> condition )
		{
			return condition( assetRef ) || tree[ assetRef ].Any( af => AnyNodeSatisfies( af, tree, condition ) );
		}

		bool IsOfType( AssetRef assetRef, System.Type type )
		{
			var asset = EditorUtility.InstanceIDToObject( assetRef.instanceID );
			if ( asset == null )
				return false;

			return type.IsAssignableFrom( asset.GetType() );
		}

		Object[] GetActiveSceneDependencies()
		{
#if UNITY_5_3
			string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
#else
			string sceneName = EditorApplication.currentScene;
#endif
			var sceneAsset = AssetDatabase.LoadAssetAtPath( sceneName, typeof(Object) );
			if ( sceneAsset == null )
			{
				return new Object[ 0 ];
			}

			return EditorUtility
				.CollectDependencies( new[] { sceneAsset } )
				.Where( o => !IgnoreDependencyFromPath( AssetDatabase.GetAssetPath( o ) ) )
				.ToArray();
		}

		static bool IgnoreDependencyFromPath( string path )
		{
			return
				path.StartsWith( "Library" ) ||
				path.StartsWith( "Resources/unity_builtin_extra" ) ||
				path.EndsWith( "dll" );
		}

		void PopulateAssetSubtree( AssetRef root, System.Func<AssetRef, bool> doIncludeRef, System.Func<AssetRef,bool> doMarkRef )
		{
			var children = GetChildAssetRefs( root ).ToList();

			assetTree[ root ] = children;
			foreach ( var child in children )
				PopulateAssetSubtree( child, doIncludeRef, doMarkRef );

			// remove children that are:
			// empty folders
			// part of the filter relation
			assetTree[ root ].RemoveWhere( ar => ar.isFolder ? !assetTree[ar].Any() : !doIncludeRef( ar ) );

			// highlight root if:
			// it's a folder containing any highlighted children
			// it's an asset that is part of the highlight relation
			root.highlight = root.isFolder ? assetTree[ root ].Any( child => child.highlight ) : doMarkRef( root );
		}

		#region AssetRef utility

		public static AssetRef GetAssetRef( string assetPath )
		{
			var assetGUID = AssetDatabase.AssetPathToGUID( assetPath );
			var guidToInstanceID = typeof( AssetDatabase ).GetMethod( "GetInstanceIDFromGUID", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic );
			int instanceID = (int) guidToInstanceID.Invoke( null, new object[] { assetGUID } );
			return new AssetRef() { instanceID = instanceID, name = assetPath };
		}

		public static IEnumerable<AssetRef> GetChildAssetRefs( AssetRef entity )
		{
			var hierarchyProperty = new HierarchyProperty( HierarchyType.Assets );
			hierarchyProperty.Reset();
			if ( !hierarchyProperty.Find( entity.instanceID, null ) )
				yield break;

			int entityDepth = hierarchyProperty.depth;

			while ( hierarchyProperty.NextWithDepthCheck( null, entityDepth + 1 ) )
			{
				if ( hierarchyProperty.depth != entityDepth + 1 )
					continue;

				yield return new AssetRef()
				{
					instanceID = hierarchyProperty.instanceID,
					icon = hierarchyProperty.icon,
					isFolder = hierarchyProperty.isFolder,
					name = hierarchyProperty.name
				};
			}
		}

		#endregion
	}
}
