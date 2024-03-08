using System.Text.Json;
using System.Text.Json.Serialization;
using NGame.Assets.Common.Assets;
using Singulink.IO;

namespace NGame.Assets.UsageFinder.AssetOverviews;



internal class JsonAsset : Asset;



internal interface IAssetOverviewCreator
{
	AssetOverview Create(IAbsoluteDirectoryPath assetListPaths);
}



internal class AssetOverviewCreator(
	IEnumerable<JsonConverter> jsonConverters
) : IAssetOverviewCreator
{
	private record AssetFileInfo(PathInfo MainPathInfo, PathInfo? SatellitePathInfo);


	public AssetOverview Create(IAbsoluteDirectoryPath solutionDirectory)
	{
		var options = new JsonSerializerOptions();

		foreach (var jsonConverter in jsonConverters)
		{
			options.Converters.Add(jsonConverter);
		}

		var pathToAssetLists = DirectoryPath.ParseRelative(
			Path.Combine(".ngame", "assets")
		);

		var assetsDirectory = solutionDirectory.Combine(pathToAssetLists);

		var assetEntries = assetsDirectory
			.GetChildFiles("*.g.txt")
			.SelectMany(ReadAssetListFile)
			.Select(x => CreateAssetEntry(x, options));


		return new AssetOverview(assetEntries);
	}


	private IEnumerable<AssetFileInfo> ReadAssetListFile(IAbsoluteFilePath assetListFile)
	{
		var fileLines = File.ReadAllLines(assetListFile.PathDisplay);
		var projectDirectory = DirectoryPath.ParseAbsolute(fileLines[0]);

		var fileLineSet =
			fileLines
				.Skip(1)
				.ToHashSet();

		return
			fileLineSet
				.Select(x =>
					CreateAssetFileInfo(x, fileLineSet, projectDirectory)
				);
	}


	private static AssetFileInfo CreateAssetFileInfo(
		string assetFilePath,
		IReadOnlySet<string> allFilePaths,
		IAbsoluteDirectoryPath projectDirectory
	)
	{
		var mainPathInfo = CreatePathInfo(assetFilePath, projectDirectory);

		var satelliteFilePath =
			assetFilePath[..^AssetConventions.AssetFileEnding.Length];

		var satellitePathInfo =
			allFilePaths.Contains(satelliteFilePath)
				? CreatePathInfo(satelliteFilePath, projectDirectory)
				: null;

		return new AssetFileInfo(mainPathInfo, satellitePathInfo);
	}


	private static PathInfo CreatePathInfo(
		string fileLine,
		IAbsoluteDirectoryPath projectDirectory
	)
	{
		var sourcePath = projectDirectory.CombineFile(fileLine);
		var targetPath = FilePath.ParseRelative(fileLine);
		return new PathInfo(sourcePath, targetPath);
	}


	private static AssetEntry CreateAssetEntry(
		AssetFileInfo assetFileInfo,
		JsonSerializerOptions options
	)
	{
		var mainPathInfo = assetFileInfo.MainPathInfo;
		var allText = File.ReadAllText(mainPathInfo.SourcePath.PathDisplay);
		var jsonAsset = JsonSerializer.Deserialize<JsonAsset>(allText, options)!;

		return new AssetEntry(
			jsonAsset.Id,
			mainPathInfo,
			jsonAsset.PackageName,
			assetFileInfo.SatellitePathInfo
		);
	}
}
