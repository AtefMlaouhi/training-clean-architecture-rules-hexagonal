# Husky Rules

This document explains the Git hook rules currently enforced in this repository and how to extend them safely.

## Current Configuration

The Node tooling is defined in `package.json`:

- `prepare` script: `husky`
- dev dependency: `husky`
- dev dependency: `validate-branch-name`

The active hook is:

- `.husky/pre-push`

Current hook command:

```bash
npx --no-install validate-branch-name
```

## What This Rule Does

On every `git push`, Husky runs `validate-branch-name`.

- If the current branch name is accepted by the rule set, push continues.
- If the branch name is rejected, push is blocked.

This prevents non-compliant branch names from reaching remote branches.

## Installation and Activation

Hooks are installed when you run:

```bash
npm install
```

because `npm install` triggers the `prepare` script.

If hooks are missing, run:

```bash
npm run prepare
```

## Troubleshooting

### "npx --no-install" fails

Cause: required package is not present in `node_modules`.

Fix:

```bash
npm install
```

### Push blocked by branch-name rule

Cause: current branch name does not pass `validate-branch-name` policy.

Fix: rename your branch, then push again:

```bash
git branch -m <new-branch-name>
git push --set-upstream origin <new-branch-name>
```

## Optional Extension Rules

If you want stronger enforcement, add additional hooks.

### Example `commit-msg` hook for Conventional Commits

```bash
npx --no-install commitlint --edit "$1"
```

### Example `pre-commit` hook for fast validation

```bash
dotnet test --filter "FullyQualifiedName!~IntegrationTests"
```

## Team Recommendation

Keep hooks fast and deterministic.

- Use `pre-commit` for lightweight checks.
- Use `pre-push` for branch policy and longer checks.
- Keep CI as the final source of truth for full validation.
