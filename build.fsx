#r "paket:
nuget FSharp.Core
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
"

#load "./.fake/build.fsx/intellisense.fsx"

//Temporary fix until this is resolved : https://github.com/mono/mono/issues/9315
#if !FAKE
#r "Facades/netstandard"
#r "netstandard"
#endif
//Temporary fix until this is resolved : https://github.com/mono/mono/issues/9315

open System.IO
open Fake.Core
open Fake.IO.Globbing.Operators
open Fake.DotNet

//*********************************************************/
// *** Target implementations
//*********************************************************/

let targetClean _ =
  Trace.log " --- Cleaning stuff --- "
  DotNet.exec id "clean" ""
  |> ignore

let targetBuild _ =
  Trace.log " --- Building the solution --- "
  DotNet.build id ""

let targetTest _ =
  Trace.log " --- Testing projects in parallal --- "

  let setDotNetOptions (projectDirectory:string) : (DotNet.TestOptions-> DotNet.TestOptions) =
    fun (dotNetTestOptions:DotNet.TestOptions) ->
      { dotNetTestOptions with
          Common        = { dotNetTestOptions.Common with WorkingDirectory = projectDirectory}
          Configuration = DotNet.BuildConfiguration.Release
      }

  //Looks overkill for only one csproj but just add 2 or 3 csproj and this will scale a lot better
  !!("test/**/*.Tests.csproj")
  |> Seq.toArray
  |> Array.Parallel.iter (
    fun fullCsProjName ->
      let projectDirectory = Path.GetDirectoryName(fullCsProjName)
      DotNet.test (setDotNetOptions projectDirectory) ""
    )

let targetPack _ =
  Trace.log " --- Packaging nugets app --- "
  DotNet.pack id "" //--output FOLDERHERE"

let targetPush _ =
  Trace.log " --- Pushing nuget --- "

  let source = Environment.environVarOrDefault "NUGET_FEED_TO_PUSH" "NUGET_FEED_TO_PUSH"
  let apiKey = Environment.environVarOrDefault "SOURCE_NUGET_API_KEY" "SOURCE_NUGET_API_KEY"

  let getNugetPushArgs source apiKey=
    sprintf "nuget push -s %s -k %s" source apiKey

  let nugetPushArgs = getNugetPushArgs source apiKey
  printfn "Pushing nuget with : 'dotnet %s'" nugetPushArgs
  let processResult = DotNet.exec id nugetPushArgs ""
  if processResult.OK then
    printfn "Nuget pushed to '%s'" source
  else
    failwithf "Could not push nuget, error messages :\n%A" processResult



//*********************************************************/
// *** Define Targets ***
//*********************************************************/
Target.create "Clean" targetClean
Target.create "Build" targetBuild
Target.create "Test" targetTest
Target.create "Pack" targetPack
Target.create "Push" targetPush
Target.createFinal "Done" (fun _ -> Trace.log " --- Fake script is done --- ")

//*********************************************************/
//                   TARGETS ORDERING
//*********************************************************/
open Fake.Core.TargetOperators

// *** Define Dependencies ***
"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Pack"
  =?> ("Push", not BuildServer.isLocalBuild)
  ==> "Done"

// *** Start Build ***
Target.runOrDefault "Done"
