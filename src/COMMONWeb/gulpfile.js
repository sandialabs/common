/// <binding BeforeBuild='copy' />

"use strict";

var gulp = require("gulp");
var rename = require("gulp-rename");

var paths = {
    webroot: "./",
    npm: "./node_modules/",
    lib: "./lib/"
};

gulp.task("copy", function () {
    var npm = {
        "dygraphs": "dist/**/*.{js,css,map}",
        "angular-animate": "*.{js,css,map}",
        "angular-aria": "*.{js,map}",
        "angular-material": "*.{js,css}",
        "chart.js": "dist/*.js",
        "angular-chart.js": "dist/*.js",
        "angular-treasure-overlay-spinner": "dist/*.{js,css}",
        "moment": "**/*.{js,css,map}",
        "systemjs": "dist/*.js",
        "jquery": "dist/*.{js,css,map}",
        "bootstrap": "dist/**/*.{js,map,css,ttf,svg,woff,woff2,eat}",
        "downloadjs": "*.js",
        "angular-utils-pagination": "*.js",
        "angular-ui-bootstrap": "dist/*.{js,css,map}",
        "angular-route": "*.{js,css,map}",
        "angular-resource": "*.{js,css,map}",
        "angular-cookies": "*.{js,css,map}",
        "angular-messages": "*.{js,css,map}",
        "angular-translate": "dist/*.js",
        "angular": "*.{js,css,map}",
    };

    for (var dest in npm) {
        gulp.src(paths.npm + dest + "/" + npm[dest])
            .pipe(gulp.dest(paths.lib + dest));
    }

    // We need the static file loader as well. The folder structure for that is slightly different
    // so let's do it manually here.
    gulp.src(paths.npm + 'angular-translate/dist/angular-translate-loader-static-files/*.js')
        .pipe(gulp.dest(paths.lib + 'angular-translate'));

    // As a temporary hack, copy the moment.d.ts file into the min
    // folder as moment-with-locales.d.ts
    //gulp.src(paths.npm + "moment/moment.d.ts")
    //    .pipe(rename("moment-with-locales.d.ts"))
    //    .pipe(gulp.dest(paths.lib + "moment/min"));
});

gulp.task('default', [
    'copy'
]);
