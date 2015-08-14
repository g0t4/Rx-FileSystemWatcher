require 'rake'
require 'albacore'

task :teamcity => [:build]

build :build do |b|
	b.prop 'Configuration', 'Release'
	b.target = [:Clean, :Build]
	b.sln = 'src/RxFileSystemWatcher.sln'
end

task :nuget => [:test] do
	sh "src/.nuget/nuget pack src/RxFileSystemWatcher/RxFileSystemWatcher.csproj -Prop Configuration=Release"
end

test_runner  :test => [:build] do |tests|
	tests.exe = 'src/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe'
	tests.files = ['src/Tests/bin/Release/Tests.dll']
end