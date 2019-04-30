const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = (env, argv) => {
  const isProduction = argv.mode === 'production';

  return {
    mode: argv.mode,
    entry: path.join(__dirname, '/src/index.js'),
    output: {
      filename: '[name].js',
      path: path.join(__dirname, '/dist')
    },
    module: {
      rules: [
        {
          test: /\.js$/,
          loader: 'babel-loader',
          query: { compact: isProduction }
        },
        {
          test: /\.css$/,
          use: ['style-loader', 'css-loader']
        }
      ]
    },
    plugins: [
      new HtmlWebpackPlugin({
        cache: true,
        template: './src/index.html',
        minify: isProduction
      })
    ],
    devServer: {
      contentBase: path.join(__dirname, 'dist'),
      compress: true,
      port: 3000,
      open: false,
      publicPath: '/',
      historyApiFallback: true
    },
    resolve: {
      extensions: ['.js', '.jsx', '.css']
    }
  };
};
