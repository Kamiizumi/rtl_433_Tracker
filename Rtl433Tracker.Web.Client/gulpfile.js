var gulp = require('gulp');
var del = require('del');

function clean() {
    return del(['wwwroot/libs/**']);
};

function libs() {
    return gulp.src('node_modules/**')
        .pipe(gulp.dest('wwwroot/libs'));
};

exports.build = gulp.series(clean, libs);
