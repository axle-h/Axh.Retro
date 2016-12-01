var gulp = require('gulp'),
    rimraf = require("gulp-rimraf"),
    concat = require("gulp-concat"),
    cleancss = require("gulp-clean-css"),
    uglify = require("gulp-uglify"),
    copy = require("gulp-copy"),
    rename = require("gulp-rename"),
    ts = require("gulp-typescript"),
    less = require("gulp-less"),
    imagemin = require("gulp-imagemin"),
    insert = require("gulp-insert");

var paths = {
    webroot: "./wwwroot/",
    root: "./Client/"
};

// Global.
paths.js = paths.webroot + "js/";
paths.css = paths.webroot + "css/";
paths.images = paths.webroot + "images/";
paths.fonts = paths.webroot + "fonts/";
paths.srcImages = paths.root + "images/**/*";
paths.lib = paths.root + "lib/";
paths.ts = [paths.root + "typings/tsd.d.ts", paths.root + "src/**/*.ts"];
paths.less = paths.root + "style/Site.less";

// Script compile.
paths.jsDest = paths.js + "site.js";
paths.jsDepends = [paths.lib + "jquery/dist/jquery.js",
    paths.lib + "jquery-validation/dist/jquery.validate.js",
    paths.lib + "jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
    paths.lib + "bootstrap/dist/js/bootstrap.js"];
paths.jsDependsDest = paths.js + "depend.js";

// Fonts
paths.fontAwesome = paths.root + "lib/font-awesome/fonts/*";


gulp.task("clean", function () {
    return gulp.src([paths.webroot], { read: false })
      .pipe(rimraf());
});

gulp.task("build:js", function () {
    return gulp.src(paths.ts)
        .pipe(ts({
            noImplicitAny: true,
            noEmitOnError: true,
            removeComments: false,
            target: "es5",
            module: "commonjs",
            preserveConstEnums: true
        }))
        .pipe(concat(paths.jsDest))
        .pipe(gulp.dest("."))
        .pipe(uglify())
        .pipe(rename({ suffix: ".min" }))
        .pipe(gulp.dest("."));
});

gulp.task("build:jsDepend", function () {
    return gulp.src(paths.jsDepends)
        .pipe(concat(paths.jsDependsDest))
        .pipe(gulp.dest("."))
        .pipe(uglify())
        .pipe(rename({ suffix: ".min" }))
        .pipe(gulp.dest("."));
});

gulp.task("build:less", function () {
    return gulp.src(paths.less)
        .pipe(less())
        .pipe(gulp.dest(paths.css))
        .pipe(cleancss({ keepSpecialComments: 0 }))
        .pipe(rename({ suffix: ".min" }))
        .pipe(gulp.dest(paths.css));
});

gulp.task("build:images", function () {
    return gulp.src(paths.srcImages)
        .pipe(imagemin({
            progressive: true,
            svgoPlugins: [
                { removeViewBox: false },
                { cleanupIDs: false }
            ]
        }))
        .pipe(gulp.dest(paths.images));
});

gulp.task("build:fonts", function () {
    return gulp.src(paths.fontAwesome)
        .pipe(gulp.dest(paths.fonts));
});

gulp.task("build", ["build:js", "build:jsDepend", "build:less", "build:images", "build:fonts"]);