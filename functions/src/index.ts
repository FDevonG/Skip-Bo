import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';
admin.initializeApp();
// // Start writing Firebase Functions
// // https://firebase.google.com/docs/functions/typescript
//
export const nameCheck = functions.https.onCall(async (data) => {
	const name = data;
	const dbRef = admin.database().ref("users");;

	return new Promise((resolve, reject) => {
		dbRef.orderByValue().once("value").then((snap) => {
			var exists = (snap.val() !== null);
			if (exists)
			{
				snap.forEach((childSnapshot) => {
					var childData = childSnapshot.child('userName').val();
					if(childData === name) {
						resolve(childData);
					}
				})
			}
			resolve(null);
			}).catch(error => {
			console.log(error);
			reject(error);
		})
    });
})

export const getUser = functions.https.onCall(async (data) => {
	const userID = data;
	const dbRef = admin.database().ref("users");

	return new Promise((resolve, reject) => {
		dbRef.orderByValue().once("value").then((snap) => {
			var exists = (snap.val() !== null);
			if (exists)
			{
				snap.forEach((childSnapshot) => {
					var childData = childSnapshot.child('userID').val();
					if(childData === userID) {
						var json = JSON.stringify(childSnapshot.val());
						resolve(json);
					}
				})
			}
			resolve(null);
			}).catch(error => {
			console.log(error);
			reject(error);
		})
    });
})

export const findUser = functions.https.onCall(async (data) => {
	const userName = data;
	const dbRef = admin.database().ref("users");

	return new Promise((resolve, reject) => {
		dbRef.orderByValue().once("value").then((snap) => {
			var exists = (snap.val() !== null);
			if (exists)
			{
				snap.forEach((childSnapshot) => {
					var childData = childSnapshot.child('userName').val();
					if(childData === userName) {
						var json = JSON.stringify(childSnapshot.val());
						resolve(json);
					}
				})
			}
			resolve(null);
			}).catch(error => {
			console.log(error);
			reject(error);
		})
    });
})

export const getFreinds = functions.https.onCall(async (data) => {
	const friendsIDS = data;
	const dbRef = admin.database().ref("users");
	var friends = "";
	return new Promise((resolve, reject) => {
		dbRef.orderByValue().once("value").then((snap) => {
			var exists = (snap.val() !== null);
			if (exists)
			{
				friendsIDS.forEach(element => {
					snap.forEach((childSnapshot) => {
						var childData = childSnapshot.child('userID').val();
						if(childData === element) {
							friends += JSON.stringify(childSnapshot.val()) + "#";
							// friends.push(json);
						}
					})
				});
				resolve(friends);
			}
			else
			{
				resolve(null);
			}
			}).catch(error => {
			console.log(error);
			reject(error);
		})
    });
})
