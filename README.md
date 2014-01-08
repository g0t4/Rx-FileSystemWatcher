## Contents

- ObservableFileSystemWatcher - an observable wrapper around the FileSystemWatcher type
- FileDropWatcher - an observable stream of events to detect when a file has been dropped in a directory

## Install via nuget

	Install-Package ReactiveFileSystemWatcher

## Building from the CLI

- Install ruby - [with chocolatey](http://chocolatey.org/packages/ruby) `cinst ruby`
- `gem install albacore`
- `rake build`

## Testing

This project has a suite of integration tests to verify the behavior of monitoring the file system. The tests are a great way to understand the behavior.

To run the tests use `rake test`

## Packaging

`rake nuget`