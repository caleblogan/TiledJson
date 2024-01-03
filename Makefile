publish:
	dotnet pack -c Release
	dotnet nuget push ./bin/Release/*.nupkg -k $(NUGET_KEY) -s nuget.org