# NuGet References Validator

## Summary

A Visual Studio extension to assist in spotting conflicting NuGet package references in solutions with multiple projects. Once a solution has been loaded, NuGet package references are analyzed and compared. Any conflict results in an error tool window entry.

## Usage

1. Install this extension via Visual Studio (Tools -> Extensions and Updates -> Search for _NuGet References Validator_).
2. Load a solution with multiple projects that are referencing NuGet packages.
3. Check the error tool window.
4. If a conflict has been detected, double click the entry and consolidate the relevant packages.
5. Currently related projects or simply the solution have to be reloaded to trigger a new validation run (see roadmap).

## Requirements

* Visual Studio 2015
* Visual Studio 2017

## Roadmap

* Automatic validation run when NuGet packages have changed.
