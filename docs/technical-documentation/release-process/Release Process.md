# Release Process

The release process for a new version can roughly be divided in the following steps:

* Preparation
* Build of a versioned image
* Release of a new helm chart version
* Merge upstream to eclipse-tractusx

The process builds on the development flow which takes place within the forks from eclipse-tractusx, located in the catenax-ng organization.

## Preparation

It's recommended to do step 1-3 in one preparatory pull request to main, or dev respectively.

### 1. Update changelog file

The changelog file tracks all notable changes since the last released version.
During development every developer should extend the changelog under the 'Unreleased' section when raising a pull request to main or dev.
Once a new version is ready to be released, the changelog of the version gets finalized and the release version gets set for the, up to then, unreleased changes.
In the released version, the changelog is structured as following:

* Changes
* Features
* Technical Support
* Bug Fixes

In case of breaking change, the breaking change will get highlighted with a breaking change tag => ![Tag](https://img.shields.io/static/v1?label=&message=BreakingChange&color=yellow&style=flat)

### 2. Update dependencies file

In order to have an up-to-date list, of the used third-party libraries, the dependencies file needs to be updated.

This can be done by running the following statement:

```bash
dotnet list src package --include-transitive > DEPENDENCIES-PREP
cat DEPENDENCIES-PREP | grep ">" | grep -Pv "\s(Org|Microsoft|NuGet|System|runtime|docker|Docker|NETStandard)" | sed -E -e "s/\s+> ([a-zA-Z\.\-]+).+\s([0-9]+\.[0-9]+\.[0-9]+)\s*/nuget\/nuget\/\-\/\1\/\2/g" > DEPENDENCIES
awk -i inplace '!seen[$0]++' DEPENDENCIES
```

Only commit the updated dependencies file, not the 'DEPENDENCIES-PREP' file.

### 3. Version bump

To update the version please adjust the version in the `src` directory within the `Directory.Build.props` file.

TODO (EG): we might add something here

### 4. Merge from dev into main branch

The merge from dev into main, via pull request, needs to happen before releasing.

## Build of a versioned image

It's important to pull the latest state from main of every repository.
Then a tag for the released version (e.g. v0.10.0) needs to be created and pushed.
The push of the tag triggers the release workflow action (available in every repository) which creates the versioned image/s.

## Release of a new helm chart version

TODO (EG): depending on our process we need to add something here

## Merge upstream to eclipse-tractusx

Once a new version has been released, it should be merged upstream to eclipse-tractusx and tagged.

- https://github.com/eclipse-tractusx/policy-hub

## NOTICE

This work is licensed under the [Apache-2.0](https://www.apache.org/licenses/LICENSE-2.0).

- SPDX-License-Identifier: Apache-2.0
- SPDX-FileCopyrightText: 2021-2023 Contributors to the Eclipse Foundation
- Source URL: https://github.com/eclipse-tractusx/policy-hub
