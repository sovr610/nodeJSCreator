'use strict'
var express = require('express');
const path = require('path');
const fs = require('fs');
const sql = require('./mysqlDB');
var cors = require('cors');
const bodyParser = require('body-parser')
const logger = pino({
	prettyPrint: {
		colorize: true
	}
});
const app = express();
const port = 3000;
app.use(cors());
app.use(logger);
app.use(bodyParser.urlencoded({
	extended: false
}));
app.use(bodyParser.json());
app.use(function(req,res,next) {
	res.header('Access-Control-Allow-Origin','*');
	res.header('Access-Control-Allow-Headers','Origin, X-Requested-with, Content-Type, Accept');
	next();
});
app.get('/user', async(req,res,next) => {
	try{
		sql.getRowsInTableWithWhere({ tableName: 'user', where: 'user_id = ' + req.query.user_id}, function(data) {
			req.log.info(data);
			res.json(data);
		}, function(err) {
			req.log.error(err);
			next({error:err});
		});
	}
	catch (e) {
		req.log.error(e);
		next(e);
	}
});
 
app.put('/user', async(req, res, next) => {
	try{
		var user_ID = req.body.body.user_ID
		var firstName = req.body.body.firstName
		var lastname = req.body.body.lastname
		var values = [];
		values.push({ columnName: 'user_id', value: user_ID});
		values.push({ columnName: 'firstName', value: firstName});
		values.push({ columnName: 'lastName', value: lastname});
		sql.updateTable({ tableName: 'user', where: 'user_id = ' + req.query.user_id}, function(data) {
			req.log.info(data);
			res.json(data);
		}, function(err) {
			req.log.error(err);
			next({error:err});
		});
	}
	catch (e) {
		req.log.error(e);
		next(e);
	}
});
 
app.post('/user', async(req, res, next) => {
	try{
		var user_ID = req.body.body.user_ID
		var firstname = req.body.body.firstname
		var lastname = req.body.body.lastname
		var values = [];
		values.push({ columnName: 'user_id', value: user_ID});
		values.push({ columnName: 'firstName', value: firstname});
		values.push({ columnName: 'firstName', value: lastname});
		sql.insertIntoTable({ tableName: 'user', values: values }, function(data) {
			console.log(data);
		}, function(err) {
			console.log(err);
		});
	} catch (e) {
});
 
app.delete('/user', async(req, res, next) => {
	try{
		sql.deleteRowsInTable({ tableName: 'user', where: 'user_id = ' + req.query.user_id}, function(data) {
			console.log(data);
		}, function(err) {
			console.log(err);
		});
	} catch (e) {
		req.log.error(e);
		next(e);
	}
});
 
app.listen(port, () => {
	console.log('server is running on port ' + port);
	try{
		fs.readFile(path.join(__dirname, 'config.json'), 'utf8', function(err, contents) {
			if (err) throw err;
			var obj = JSON.parse(contents);
			sql.connect(obj.host, obj.user, obj.pass, obj.db);
		});
	} catch (e) {
		log.error(e);
	}
});
module.exports = app;
