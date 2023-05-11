# Contributing to Modot

Thank you for your interest in contributing to **Modot**!
Any contribution, big or small, is appreciated, and goes a long way towards making **Modot** a more usable and robust framework.

## Navigating the repository

### Branches

- `stable` is the default branch. Nothing is directly committed to `stable` - instead, commits are merged from other branches into it.
- `development` is the feature branch. Most changes and new additions are either committed into this branch or merged from other branches or forks, before being merged into `stable`.
- `csharp` and `gdscript` are the branches for **Modot**'s C# and GDScript versions respectively. Development specific to either of these versions takes place in these branches.

## Issues

### Creating issues

If you spot bugs or otherwise unintended behaviour in **Modot**, you are welcome to [open a new issue](https://github.com/Carnagion/Modot/issues/new) using the relevant template.
Don't forget to include a clear description of the issue and context where applicable.

### Working on issues

If you find an [existing issue](https://github.com/Carnagion/Modot/issues) you would like to work on, feel free to open a pull request with a fix for it.
Don't forget to inform other contributors of your decision, to prevent multiple contributors working separately on the same issue at the same time.

## Making changes

You can [fork the repository](https://github.com/Carnagion/Modot/fork) and make any new changes or additions on the forked versions.
If the contribution is related to a bugfix or feature request, it is recommended to [open a related issue](https://github.com/Carnagion/Modot/issues/new) to inform users and other contributors of progress towards it.

## Pull Requests

When you are finished with any new additions or changes, [open a pull request](https://github.com/Carnagion/Modot/pulls).
Include a clear and detailed list of all changes, as well as links to relevant issues if applicable.

Also enable the checkbox for [maintainer edits](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/allowing-changes-to-a-pull-request-branch-created-from-a-fork) to your pull request.
This allows maintainers to suggest additional changes or revisions to your pull request if necessary.

Finally, don't forget to [resolve related conversations and issues](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/reviewing-changes-in-pull-requests/commenting-on-a-pull-request#resolving-conversations) once your pull request has been merged.

## Style

### Git commits and branches

- Branch names should be `kebab-case`.
- Commits should be capitalized and use the imperative tone.

### Coding conventions

Use the rules defined in the provided [`.editorconfig` file](https://github.com/Carnagion/Modot/blob/stable/.editorconfig).