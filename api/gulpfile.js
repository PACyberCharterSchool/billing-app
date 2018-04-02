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

gulp.task('create-database-environment', function(cb) {
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

gulp.task('init-db', function(cb) {
    exec('docker run -e \'ACCEPT_EULA=Y\' -e \'MSSQL_SA_PASSWORD=Br0ken horse carrot\' --name sqlserver -d -p 1401:1433 \'sqlserver\'', function(err, stdout, stderr) {
        if (err) {
            cb(err);
            console.log(stderr);
            return;
        }

        console.log(stdout);
    });
});


gulp.task('start-db', function(cb) {
    exec('docker start \'sqlserver\'', function(err, stdout, stderr) {
        if (err) {
            cb(err);
            console.log(stderr);
            return;
        }

        console.log(stdout);
    });
});

gulp.task('stop-db', (cb) => {
    exec('docker stop sqlserver', (err, stdout, stderr) => {
        if (err) {
            cb(err)
            console.log(stderr)
            return
        }

        console.log(stdout)
    })
})

//
// Create/configure Microsoft SQL Server databases and users
//
gulp.task('set-dev-db-env', (cb) => {
    process.env.DATABASE_HOST = shell('docker-machine ip sqlserver');
    // HACK(Erik): without docker-machine, shell returns the literal string below
    if (process.env.DATABASE_HOST === "[object Object]") {
        process.env.DATABASE_HOST = "localhost"
    }
    log(`set-dev-db-env():  DATABASE_HOST is ${process.env.DATABASE_HOST}`)

    process.env.DATABASE_USERNAME = 'sa';
    process.env.DATABASE_PASSWORD = 'Br0ken horse carrot';
    process.env.DATABASE_PORT = '1401';
    process.env.DATABASE_NAME = 'master';
    log(`set-dev-db-env():  DATABASE_PORT is ${process.env.DATABASE_PORT}.`);
    cb();
});

gulp.task('create-db-user', ['set-dev-db-env'], (cb) => {
    let config = {
        server: process.env.DATABASE_HOST,
        database: process.env.DATABASE_NAME,
        user: process.env.DATABASE_USERNAME,
        password: process.env.DATABASE_PASSWORD,
        port: process.env.DATABASE_PORT
    };
    let cxn = `mssql://${config.user}:${config.password}@${config.server}:${config.port}/${config.database}`

    let user = "pacyber_dev"
    let password = "I h@te VSIMS"
    return mssql.connect(cxn).
        then((pool) => {
            pool.request().query(`create login ${user} with password='${password}'`).
                then(() => pool.request().query(`create user ${user} for login ${user}`)).
                then(() => pool.request().query(`exec master..sp_addsrvrolemember @loginame='${user}',@rolename='dbcreator'`)).
                catch((err) => {
                    log(`list-db-users(): failed to list users: ${err}.`)
                }).
                then(() => pool.close())
        })
});

//
// Running application
//
gulp.task('billing-app-debug', ['set-dev-env', 'set-dev-db-env'], function(cb) {
});
