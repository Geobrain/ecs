//  Project : ecs
// Contacts : Pix - ask@pixeye.games

//  Project  : ACTORS
//  Contacts : Pixeye - ask@pixeye.games

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pixeye.Framework
{
	public class PostHandleComponents : EndNameEditAction
	{
		#if !ACTORS_COMPONENTS_STRUCTS
		public const string PATH_TO_TEMPLATE = @"Assets\Framework\Editor\Templates\TmpComponent.txt";
		#else
	  public const string PATH_TO_TEMPLATE = @"Assets\Framework\Editor\Templates\TmpComponentStruct.txt";
		#endif
		const int MENU_ITEM_PRIORITY = 0;
		static Texture2D scriptIcon;

		public override void OnEnable()
		{
			base.OnEnable();
			scriptIcon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
		}
		[MenuItem("Assets/Create/Actors/Generate/Component", false, MENU_ITEM_PRIORITY)]
		public static void CreateSample()
		{
			var path = HelperFramework.GetPathLibrary();
			if (path == string.Empty)
				path = PATH_TO_TEMPLATE;
			#if !ACTORS_COMPONENTS_STRUCTS
			else path = string.Format($"{path}/Editor/Templates/TmpComponent.txt");
			#else
		 	else path = string.Format($"{path}/Editor/Templates/TmpComponentStruct.txt");
			#endif
			CreateFromTemplate("ComponentDefault.cs", path);
		}

		public static void CreateFromTemplate(string name, string pathName)
		{
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
					0,
					CreateInstance<PostHandleComponents>(),
					name,
					scriptIcon,
					pathName);
		}

		public static Object CreateScript(string pathName, string templatePath)
		{
			var filePath = Path.GetFullPath(pathName);

			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
			var className                = fileNameWithoutExtension.Replace(" ", string.Empty);
			var templateContents         = string.Empty;

			if (!File.Exists(templatePath)) return (MonoScript) AssetDatabase.LoadAssetAtPath(pathName, typeof(MonoScript));
			using (var t = new StreamReader(templatePath))
			{
				t.ReadLine();
				templateContents = t.ReadToEnd();
			}

			templateContents = templateContents.Replace("##NAMESPACE##", string.Format($"{Environment.NewLine}namespace {DataFramework.nameSpace}"));
			templateContents = templateContents.Replace("##NAME##", className);


			templateContents = templateContents.Replace("##NAMESPACE2##", string.Format($"{DataFramework.nameSpace}"));
			templateContents = templateContents.Replace("##NAME2##", string.Format($"{className.Replace("Component", "")}"));
			var encoding = new UTF8Encoding(true, false);

			using (var tc = new StreamWriter(filePath, false, encoding))
			{
				tc.WriteLine(templateContents);
			}

			AssetDatabase.ImportAsset(pathName);
			AssetDatabase.Refresh();

			return (MonoScript) AssetDatabase.LoadAssetAtPath(pathName, typeof(MonoScript));
		}

		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			var o = CreateScript(pathName, resourceFile);
			AssetDatabase.SetLabels(o, new[] {"Component"});
			ProjectWindowUtil.ShowCreatedAsset(o);
			AssetDatabase.Refresh();
		}
	}
}