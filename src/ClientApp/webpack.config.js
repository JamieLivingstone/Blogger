const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const Dotenv = require('dotenv-webpack');

module.exports = (env, argv) => {
  const isProduction = argv.mode === 'production';

  return {
    mode: argv.mode,
    entry: ['./src/index.js', './src/styles/main.scss'],
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
          test: /\.scss$/,
          use: [
            {
              loader: isProduction ? MiniCssExtractPlugin.loader : 'style-loader'
            },
            {
              loader: 'css-loader'
            },
            {
              loader: 'sass-loader'
            }
          ]
        }
      ]
    },
    plugins: [
      new HtmlWebpackPlugin({
        cache: true,
        template: './src/index.html',
        minify: isProduction
      }),
      new Dotenv()
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
