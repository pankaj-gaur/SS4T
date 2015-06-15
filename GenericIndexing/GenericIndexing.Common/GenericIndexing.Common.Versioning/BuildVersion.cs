using System;

namespace GenericIndexing.Common.Versioning
{
	/// <summary>
	/// Class represents the build version number
	/// </summary>
    public static class BuildVersion
	{
		private const string dot = ".";
		private const string major = "0"; //for major app releases
		private const string minor = "0"; //minor functionality releases
        //This changes for I6
		private const string revision = "14"; //build revision version increments
		
		public const string VersionString = major + dot + minor + dot + revision;
	}
}
