var gulp = require('gulp');
var exec = require('child_process').exec;

gulp.task('default', function() {
});

gulp.task('doesSQLServerDockerMachineExist', function(cb) {
    exec('docker-machine ls -q | grep \'^sqlserver\'', function(err, stdout, stderr) {
        if (err) {
            console.log(stderr);
            return;
        }

        console.log(stdout);
        return;
    });
});

gulp.task('create-database-environment', ['doesSQLServerDockerMachineExist'], function(cb) {
    exec('docker inspect --type=image \'sqlserver\'', function(err, stdout, stderr) {
        if (err) {
            cb(err);
            console.log(stderr);
            return;
        }
    });

    exec('docker build -t \'sqlserver\' .', function(err, stdout, stderr) {
        if (err) {
            cb(err);
            console.log(stderr);
            return;
        }

        console.log(stdout);
    });
});

gulp.task('remove-database-environment', function(cb) {
    exec('docker image rm \'sqlserver\'', function(err, stdout, stderr) {
        if (err) {
            cb(err);
            console.log(stderr);
            return;
        }

        console.log(stdout);
    });
});

gulp.task('run-database-environment', function(cb) {
    exec('docker run \'sqlserver\'', function(err, stdout, stderr) {
        if (err) {
            cb(err);
            console.log(stderr);
            return;
        }

        console.log(stdout);
    });
});