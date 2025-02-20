using System;
using DevSettings;
using UnityEngine;

namespace AddressablesRelated
{
	public static class RemoteAssetsStages
	{
		public static string RemoteAssetsStage
		{
			get
			{
				var branchName = new DevSettingsProvider().GetBuildInfo().BranchName;
				branchName = Truncate(branchName.Replace("refs/heads/", ""), 30);

				var unityVersion = Application.unityVersion;
				return string.IsNullOrEmpty(branchName) ? "local" : $"{branchName}/{unityVersion}";
			}
		}

		private static string Truncate(string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value)) { return value; }

			return value.Substring(0, Math.Min(value.Length, maxLength));
		}
	}
}
