var gulp = require('gulp');
var exec = require('child_process').exec;
var shell = require('gulp-shell');
var dotenv = require('gulp-dotenv');
const mssql = require('mssql');
var log = require('fancy-log');

gulp.task('default', function() {
});

//
// setup requisite environment variables for development
//
gulp.task('set-dev-env', function() {
});

gulp.task('set-dev-db-env', function(cb) {
    process.env.DATABASE_HOST = shell('docker-machine ip sqlserver');
    process.env.DATABASE_USERNAME = 'pacyber_dev';
    process.env.DATABASE_PASSWORD = 'I h@te VSIMS';
    process.env.DATABASE_PORT = '1401';
    process.env.DATABASE_NAME = 'pacyber_dev';
    log(`set-dev-db-env():  DATABASE_PORT is ${process.env.DATABASE_PORT}.`);
    cb();
});

//
// Docker
//
gulp.task('is-docker-machine-running', function() {
    return shell('docker-machine status sqlserver');
});

gulp.task('does-docker-machine-exist', function(cb) {
    exec('docker-machine ls -q | grep \'^sqlserver\'', function(err, stdout, stderr) {
        if (err) {
            console.log(stderr);
            return;
        }

        console.log(stdout);
        return;
    });
});

gulp.task('create-database-environment', ['does-docker-machine-exist'], function(cb) {
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
gulp.task('create-db-user', ['set-dev-db-env'], (cb) => {
    log(`create-db-user():  username is ${process.env.DATABASE_USERNAME}`);
    var config = {
        server: process.env.DATABASE_HOST,
        database: process.env.DATABASE_NAME,
        user: process.env.DATABASE_USERNAME,
        password: process.env.DATABASE_PASSWORD,
        port: process.env.DATABASE_PORT
    };
    async () => {
        try {
            const pool = await mssql.connect(`mssql://${config.user}:${config.password}@${config.server},${config.port}/${config.database}`);
            const login_result = await mssql.query(`create login ${config.user} with password=${config.password}`);
            const user_result = await mssql.query(`create user ${config.user} for login ${config.user}`);
        } catch (e) {
            log(`create-db-user():  failed to create user:  ${e}.`);
        }
    }
    cb();
});

gulp.task('list-db-users', ['set-dev-db-env'], (cb) => {
    log(`list-db-users():  username is ${process.env.DATABASE_USERNAME}`);
    var config = {
        server: process.env.DATABASE_HOST,
        database: process.env.DATABASE_NAME,
        user: process.env.DATABASE_USERNAME,
        password: process.env.DATABASE_PASSWORD,
        port: process.env.DATABASE_PORT
    };
    async () => {
        try {
            const pool = await mssql.connect(`mssql://${config.user}:${config.password}@${config.server},${config.port}/${config.database}`);
            const login_result = await mssql.query(`select * from master.sys.server_principals`);
            log(`list-db-users():  ${login_result}`);
        } catch (e) {
            log(`list-db-users():  failed to list users:  ${e}.`);
        }
    }
    cb();
});

gulp.task('create-db', ['set-dev-db-env'], function(cb) {
    log(`create-db():  DATABASE_PORT is ${process.env.DATABASE_PORT}.`);
    var config = {
        server: process.env.DATABASE_HOST,
        database: process.env.DATABASE_NAME,
        user: process.env.DATABASE_USERNAME,
        password: process.env.DATABASE_PASSWORD,
        port: process.env.DATABASE_PORT
    };
    async () => {
        try {
            const pool = await mssql.connect(`mssql://${config.user}:${config.password}@${config.server},${config.port}/${config.database}`);
        } catch (e) {
            log(`Error connecting to SQL Server database ${config.server}:  ${e}.`);
        }
    }
});

//
// Running application
//
gulp.task('billing-app-debug', ['set-dev-env', 'set-dev-db-env'], function(cb) {
});
