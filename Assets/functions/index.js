const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp();

 // Create and Deploy Your First Cloud Functions
 // https://firebase.google.com/docs/functions/write-firebase-functions

 exports.helloWorld = functions.https.onRequest((request, response) => {
	 "use strict";
	 response.send("Hello from Firebase!");
 });

exports.GetUser = functions.https.onCall((data, context) => {
	"use strict";
	console.log("HI");
	const database = firebase.database();
	var ref = database.ref('users');
	var users = JSON.parse(ref.GetRawJsonValue());
	for (var user in users) {
		if (user.userID === context.auth.uid) {
			return JSON.parse(user);
		}
	}
});
