using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class PreloadSigningAlias
{

	static PreloadSigningAlias ()
	{
		PlayerSettings.Android.keystorePass = "yoho62978";
		PlayerSettings.Android.keyaliasName = "release";
		PlayerSettings.Android.keyaliasPass = "yoho62978";
	}

}