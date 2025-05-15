# ChangeLogVersioningTask

## Parameters

### `CrashOnFailure`
Should it crash build if it fails to find the version or CHANGELOG-file?

Accepted `true` or `false`. Defaults to `true`.

### `ChangeLogDirectory`
Directory to look for the CHANGELOG-file.

Defaults to `$(MSBuildProjectDirectory)` which is the directory of the current project
begin built.

### `ChangeLogFileName`
File name of the CHANGELOG-file.

Defaults to `CHANGELOG.md`.

## Why?
For some reason unknown to me some projects prefer to have [CHANGELOG.md](https://keepachangelog.com/en/1.1.0/) instead
of using GitVersion and properly written merge messages..

For other reasons I have to work on projects where this is the norm.

So this was born.

