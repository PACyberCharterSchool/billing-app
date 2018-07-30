import { writeFile } from 'fs';
import { argv } from 'yargs';

// This is good for local dev environments, when it's better to
// store a projects environment variables in a .gitignore'd file
require('dotenv').config();

// Would be passed to script like this:
// `ts-node set-env.ts --environment=dev`
// we get it from yargs's argv object
const environment = argv.environment;
const isProd = environment === 'prod';

const targetPath = environment === 'dev' ? `./src/environments/environment.ts` : `./src/environments/environment.${environment}.ts`;

const envConfigFile = `
export const environment = {
  production: ${isProd},
  apiHostName: "${process.env.API_HOST}",
  apiHostPort: "${process.env.API_PORT}",
  apiBaseUrl: "http://${process.env.API_HOST}:${process.env.API_PORT}/api"
};
`;

writeFile(targetPath, envConfigFile, function (err) {
  if (err) {
    console.log(err);
  }

  console.log(`Output generated at ${targetPath}`);
});
