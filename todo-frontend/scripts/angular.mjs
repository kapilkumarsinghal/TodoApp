import { spawnSync } from 'node:child_process';
import { fileURLToPath } from 'node:url';

const angularCli = fileURLToPath(new URL('../node_modules/@angular/cli/bin/ng.js', import.meta.url));
const result = spawnSync(process.execPath, [angularCli, ...process.argv.slice(2)], {
  env: {
    ...process.env,
    NG_BUILD_CACHE_STORE: process.env.NG_BUILD_CACHE_STORE || 'sqlite',
  },
  stdio: 'inherit',
});

if (result.error) {
  console.error(result.error);
  process.exitCode = 1;
} else {
  process.exitCode = result.status ?? 1;
}
