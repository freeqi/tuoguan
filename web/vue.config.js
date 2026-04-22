/* eslint-disable no-unused-vars */
const DllConfig = require('./webpack.dll.config')
// const TOOLS = require('./src/libs/tools')
const webpack = require('webpack')
const AddAssetHtmlPlugin = require('add-asset-html-webpack-plugin')
const path = require('path')
const CompressionWebpackPlugin = require('compression-webpack-plugin')

const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin
const UglifyJsPlugin = require('uglifyjs-webpack-plugin') // 去掉console

const d = new Date();
const year = d.getFullYear();
const month = d.getMonth()>8?d.getMonth() + 1:'0'+(d.getMonth() + 1)
const date = d.getDate()>9?d.getDate():'0'+ d.getDate()
const AllData = month +'_'+ date +'_'+ d.getHours() + d.getMinutes();

const resolve = dir => {
  return path.join(__dirname, dir)
}
const productionGzipExtensions = ['js', 'css', 'html']
const productionDllPlugin = false
const productionRuntime = true
// 项目部署基础
// 默认情况下，我们假设你的应用将被部署在域的根目录下,
// 例如：https://www.my-app.com/
// 默认：'/'
// 如果您的应用程序部署在子路径中，则需要在这指定子路径
// 例如：https://www.foobar.com/my-app/
// 需要将它改为'/my-app/'

const BASE_URL = process.env.NODE_ENV === 'production'
  ? '/' // 生产
  : '/'
module.exports = {
  // 自定义配置
  configureWebpack: webpackConfig => {
    if (process.env.NODE_ENV === 'production') {
      // 对打包文件进行分析
      // code splitting
      let splitChunks = {
        chunks: 'all',
        cacheGroups: {
          libs: {
            name: 'chunk-libs',
            test: /[\\/]node_modules[\\/]/,
            priority: 10,
            chunks: 'initial' // 只打包初始时依赖的第三方
          },
          iviewUI: {
            name: 'chunk-iview', // 单独将 iview 拆包
            priority: 20, // 权重需大于其它缓存组
            test: /[\\/]node_modules[\\/]iview[\\/]/
          },
          commons: {
            name: 'chunk-commons',
            test: resolve('src/components'), // 可自定义拓展你的规则
            minChunks: 2, // 最小共用次数
            priority: 5,
            reuseExistingChunk: true
          }
        }
      }
      let DllPlugin = [
        ...Object.keys(DllConfig.entry).map(name => {
          // 对第三方插件进行提前打包
          return new webpack.DllReferencePlugin({
            context: process.cwd(),
            manifest: require(`./public/vendor/${name}.dll.manifest.json`)
          })
        }),
        // 将 dll 注入到 生成的 html 模板中
        new AddAssetHtmlPlugin({
          // dll文件位置
          filepath: path.resolve(__dirname, './public/vendor/*.js'),
          // dll 引用路径
          publicPath: './vendor',
          // dll最终输出的目录
          outputPath: './vendor'
        })
      ]
      let plugins = [
        new BundleAnalyzerPlugin({
          analyzerMode: 'static'
        }),
        new CompressionWebpackPlugin({
          test: new RegExp('\\.(' + productionGzipExtensions.join('|') + ')$'),
          threshold: 20480,
          minRatio: 0.8,
          deleteOriginalAssets: false // 不删除源文件
        })
      ]
      let hashedModules = [
        new webpack.HashedModuleIdsPlugin()
      ]
      webpackConfig.plugins.push(...plugins)
      productionDllPlugin && webpackConfig.plugins.push(...DllPlugin)
      webpackConfig.optimization.splitChunks = splitChunks
      productionRuntime && (webpackConfig.optimization.runtimeChunk = 'single')
      productionRuntime && webpackConfig.plugins.push(...hashedModules)
      // webpackConfig.optimization.minimizer.push(new UglifyJsPlugin({
      //   uglifyOptions: {
      //     output: {
      //       // 删除注释
      //       comments: true
      //     },
      //     chunkFilter: () => true,
      //     compress: {
      //       // 删除console
      //       drop_console: true,
      //       ie8: false,
      //       // 删除debugger
      //       drop_debugger: true
      //     }
      //   },
      //   // cache: true,
      //   parallel: true
      // }))
    }
    webpackConfig.devtool = 'source-map'
  },
  // Project deployment base
  // By default we assume your app will be deployed at the root of a domain,
  // e.g. https://www.my-app.com/
  // If your app is deployed at a sub-path, you will need to specify that
  // sub-path here. For example, if your app is deployed at
  // https://www.foobar.com/my-app/
  // then change this to '/my-app/'
  // cy "baseUrl" option in vue.config.js is deprecated now, please use "publicPath" instead.
  // baseUrl: BASE_URL,
  publicPath: BASE_URL,
  // tweak internal webpack configuration.
  // see https://github.com/vuejs/vue-cli/blob/dev/docs/webpack.md
  // 如果你不需要使用eslint，把lintOnSave设为false即可
  lintOnSave: false,
  chainWebpack: config => {
    config.resolve.alias
      .set('@', resolve('src')) // key,value自行定义，比如.set('@@', resolve('src/components'))
      .set('_c', resolve('src/components'))
  },
  // 打包时不生成.map文件
  productionSourceMap: false,
  // outputDir: 'dist/group_web('+AllData+')',
  // 这里写你调用接口的基础路径，来解决跨域，如果设置了代理，那你本地开发环境的axios的baseUrl要写为 '' ，即空字符串
  // devServer: {
  //   proxy: 'http://restapi.amap.com/v3/geocode/geo/'
  // }
}
