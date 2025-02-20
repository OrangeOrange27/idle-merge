using System;
using DevSettings.Interfaces;
using UnityEngine;

namespace DevSettings
{
	[Serializable]
	public class BuildInfo : IBuildInfo
	{
		[SerializeField] private string _buildPresetName;
		[SerializeField] private string _branchName;
		[SerializeField] private int _buildNumber;

		public string BuildPresetName => _buildPresetName;
		public string BranchName => _branchName;
		public int BuildNumber => _buildNumber;

		public BuildInfo(string buildPresetName, string branchName, int buildNumber)
		{
			_buildPresetName = buildPresetName;
			_branchName = branchName;
			_buildNumber = buildNumber;
		}
	}
}
