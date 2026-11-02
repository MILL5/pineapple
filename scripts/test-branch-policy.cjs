// Exercise the actual inline Actions script, without GitHub credentials.
const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');

const workflow = fs.readFileSync('.github/workflows/branch-policy.yml', 'utf8');
const script = workflow.split('          script: |\n')[1];
assert.ok(script, 'The workflow must contain its branch validation script');

const cases = [
  ['develop', 'main', true, true],
  ['feature/example', 'develop', true, true],
  ['feature/nested/example', 'develop', true, true],
  ['feature/example', 'main', true, false],
  ['main', 'develop', true, false],
  ['develop', 'develop', true, false],
  ['feature/', 'develop', true, false],
  ['bugfix/example', 'develop', true, false],
  ['dependabot/nuget/example', 'develop', true, false],
  ['develop', 'main', false, false],
  ['feature/example', 'develop', false, false],
  ['feature/example', 'other', true, false],
];

for (const [source, target, sameRepository, expected] of cases) {
  let failed = false;
  vm.runInNewContext(script, {
    context: {payload: {pull_request: {
      head: {ref: source, repo: {full_name: sameRepository ? 'MILL5/pineapple' : 'fork/pineapple'}},
      base: {ref: target, repo: {full_name: 'MILL5/pineapple'}},
    }}},
    core: {setFailed() { failed = true; }, info() {}},
  });
  assert.equal(!failed, expected, `${source} -> ${target} (same repository: ${sameRepository})`);
}
console.log(`Branch policy: ${cases.length} positive/negative cases passed`);
