// Karma configuration
// Generated on Fri Oct 27 2017 10:21:38 GMT-0600 (Mountain Daylight Time)

var webpackConfig = require("./webpack.config.js");
//webpackConfig.entry = {};

module.exports = function(config) {
  config.set({

    // base path that will be used to resolve all patterns (eg. files, exclude)
    basePath: '',


    // frameworks to use
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
    frameworks: ['jasmine'],
    //frameworks: ['jasmine', 'requirejs'],


    // list of files / patterns to load in the browser
    files: [
        './app/bundle.js',
        './app/**/*.spec.js'
    ],
      //{pattern: 'lib/requirejs/requirejs.js', included: false},
    //'test-main.js',
    //  {pattern: 'lib/angular/angular.js', included: false },
    //  { pattern: 'bundle.js', included: false },
    //  { pattern: 'app/**/*.spec.js', included: false }


    // list of files to exclude
    exclude: [
        './app/bundle.js'
    ],


    // preprocess matching files before serving them to the browser
    // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
    preprocessors: {
        './bundle.js': ['webpack'],
        './app/**/*.spec.js': ['webpack']
    },

    webpack: webpackConfig,
    webpackMiddleware: {
        noInfo: true
    },

    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress'],


    // web server port
    port: 9876,


    // enable / disable colors in the output (reporters and logs)
    colors: true,


    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,


    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: true,


    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['Chrome'],


    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: false,

    // Concurrency level
    // how many browser should be started simultaneous
    concurrency: Infinity
  })
}
