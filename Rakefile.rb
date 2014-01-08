require 'rake'
require 'albacore'

task :teamcity => [:build]

msbuild :build do |msb|
	msb.properties :configuration => :Release
	msb.targets [:Clean, :Build]
	msb.solution = 'src/RxFileSystemWatcher.sln'
end

task :nuget => [:test] do
	sh "src/.nuget/nuget pack src/RxFileSystemWatcher/RxFileSystemWatcher.csproj /OutputDirectory " + ENV["NuGetDevFeed"] + " -Prop Configuration=Release"
end

nunit :test => [:build] do |cmd|
	cmd.command = 'src/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe'
	cmd.assemblies = ['src/Tests/bin/Release/Tests.dll']
end