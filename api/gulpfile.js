var gulp = require('gulp');
var exec = require('child_process').exec;
var shell = require('gulp-shell');
const mssql = require('mssql');

gulp.task('default', function() {
});

//
// setup requisite environment variables for development
//
gulp.task('set-dev-env', function() {
});

gulp.task('set-dev-db-env', function() {
    return gulp.src([])
        .pipe(process.env.DATABASE_HOST = shell('docker-machine ip sqlserver'))
        .pipe(process.env.DATABASE_USERNAME = 'pacyber_dev')
        .pipe(process.env.DATABASE_PASSWORD = 'I h@te VSIMS')
        .pipe(process.env.DATABASE_PORT = '1401')
        .pipe(process.env.DATABASE_NAME = 'pacyber_dev');
});

//
// Docker
//
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

//
// Create/configure Microsoft SQL Server databases and users
//
gulp.task('create-db', function(cb) {
    var config = {
        server: process.env.DATABASE_HOST,
        database: process.env.DATABASE_NAME,
        user: process.env.DATABASE_USERNAME,
        password: process.env.DATABASE_PASSWORD,
        port: process.env.DATABASE_PORT
    };
    var connection = new mssql.Connection(config);
    connection.connect().then(function() {
    });
});

//
// Running application
//
gulp.task('billing-app-debug', ['set-dev-env', 'set-dev-db-env'], function(cb) {

});
