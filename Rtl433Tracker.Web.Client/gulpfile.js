var gulp = require('gulp');
var del = require('del');

gulp.task('clean', function () {
    return del(['wwwroot/libs/**']);
})

gulp.task('css', function () {
    return gulp.src('node_modules/bootstrap/dist/css/bootstrap.min.css')
        .pipe(gulp.dest('wwwroot/libs/bootstrap/dist/css'));
});
