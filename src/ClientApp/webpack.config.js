const path = require("path");
const HtmlWebpackPlugin = require("html-webpack-plugin");

module.exports = (env, argv) => {
  const isProduction = argv.mode === "production";

  return {
    mode: argv.mode,
    entry: path.join(__dirname, "/src/app.js"),
    output: {
      filename: "[name].js",
      path: path.join(__dirname, "/dist")
    },
    module: {
      rules: [
        {
          test: /\.js$/,
          loader: "babel-loader",
          query: { compact: isProduction }
        }
      ]
    },
    plugins: [
      new HtmlWebpackPlugin({
        cache: true,
        template: "./src/index.html",
        minify: isProduction
      })
    ],
    devServer: {
      contentBase: path.join(__dirname, "dist"),
      compress: true,
      port: 3000,
      open: true
    }
  };
};
