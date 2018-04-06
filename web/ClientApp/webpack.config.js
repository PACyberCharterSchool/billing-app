const path = require('path');

module.exports = {
  entry: {
    app: './src/main.ts',
    vendor: './src/vendor.ts',
    polyfills: './src/polyfills.ts'
  },
  output: {
    filename: '[name].js',
    path: path.resolve(__dirname, 'dist')
  }
};
