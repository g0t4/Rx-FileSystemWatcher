require 'rake'
require 'albacore'

task :teamcity => [:build]

msbuild :build do |msb|
	msb.properties :configuration => :Release
	msb.targets [:Clean, :Build]
	msb.solution = 'src\\RxFileSystemWatcher.sln'
end

task :nuget do
	sh "src\\.nuget\\nuget pack src\\RxFileSystemWatcher\\RxFileSystemWatcher.csproj /OutputDirectory " + ENV["NuGetDevFeed"] + " -Prop Configuration=Release"
end