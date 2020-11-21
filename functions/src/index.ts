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
			const exists = (snap.val() !== null);
			if (exists)
			{
				snap.forEach((childSnapshot) => {
					const childData = childSnapshot.child('userName').val();
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
			const exists = (snap.val() !== null);
			if (exists)
			{
				snap.forEach((childSnapshot) => {
					const childData = childSnapshot.child('userID').val();
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
			const exists = (snap.val() !== null);
			if (exists)
			{
				snap.forEach((childSnapshot) => {
					const childData = childSnapshot.child('userName').val();
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

export const getUsers = functions.https.onCall(async (data) => {
	const friendsIDS = data;
	const dbRef = admin.database().ref("users");
	let friends = "";
	return new Promise((resolve, reject) => {
		dbRef.orderByValue().once("value").then((snap) => {
			const exists = (snap.val() !== null);
			if (exists)
			{
				friendsIDS.forEach(element => {
					snap.forEach((childSnapshot) => {
						const childData = childSnapshot.child('userID').val();
						if(childData === element) {
							friends += JSON.stringify(childSnapshot.val()) + "#";
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

export const getUsersArray = functions.https.onCall(async (data) => {
	const friendsIDS = data;
	const dbRef = admin.database().ref("users");

	class UserArray
	{
		users: Array<User> = [];
	}

	class User
	{
		userName: string = "";
    userID: string = "";

    hair: string = "";
    face: string = "";
    kit: string = "";
    body: string = "";

    offlineGamesPlayed: number = 0;
    onlineGamesPlayed: number = 0;
    offlineGamesWon: number = 0;
    onlineGamesWon: number = 0;

    friends: Array<string> = [];
    blocked: Array<string> = [];

    level: number = 0;
    experience: number = 0;
    experienceToNextLevel: number = 0;

    achievments: Array<Achievment> = [];

    gamesWonInARow: number = 0;

    adsBlocked: boolean = false;

    gems: number = 0;

    unlockedHair: Array<string> = [];
    unlockedFace: Array<string> = [];
    unlockedClothes: Array<string> = [];

    nextRewardUnlock: string = "";
    dailyRewardGotten: boolean = false;
    rewardCounter: number = 0;
	}

	class Achievment
	{
    name: string = "";
    unlocked: boolean = false;
    description: string = "";
    iconName: string = "";
    iconNameGreyscale: string = "";
	}

	const sendBackData: UserArray = new UserArray();
	return new Promise((resolve, reject) => {
		dbRef.orderByValue().once("value").then((snap) => {
			const exists = (snap.val() !== null);
			if (exists)
			{
				friendsIDS.forEach(element => {
					snap.forEach((childSnapshot) => {
						const childData = childSnapshot.child('userID').val();
						if(childData === element)
						{
							// const user: User = childSnapshot.val();
							sendBackData.users.push(childSnapshot.val());
						}
					})
				});
				resolve(JSON.stringify(sendBackData));
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

export const leaderboardsOpened = functions.https.onCall(async (data) => {
	const dbRef = admin.database().ref("users");
	const userUID: string = data[1];

	const selection: number = parseInt(data[0], 10);

	class Leader {
		playerName: string;
		stat: number;
		standing: number;
		id: string;
		constructor(n, s, st, i){
			this.playerName = n;
			this.stat = s;
			this.standing = st;
			this.id = i;
		}

		Standing(value){
			this.standing = value;
		}
		Stat(){
			return this.stat;
		}
		LeaderID(){
			return this.id;
		}
	};

	class LoadedLeaderboards{
		loadBoards: Array<Leader> = [];
	}
	const leader: LoadedLeaderboards = new LoadedLeaderboards();

	return new Promise((resolve, reject) => {
		dbRef.orderByValue().once("value").then((snap) => {
			const exists = (snap.val() !== null);
			if (exists)
			{
				snap.forEach((childSnapshot) => {
					if(childSnapshot.child('userName').val() !== ""){
						let counter: number = 1;
						const currentName: string = childSnapshot.child('userName').val();
						const uid: string = childSnapshot.child('userID').val();
						let currentStat: number = 0;
						switch(selection)
						{
								case 0:
									currentStat = childSnapshot.child('onlineGamesWon').val();
								break;

								case 1:
									if(childSnapshot.child('onlineGamesWon').val() > 0)
									{
										currentStat = childSnapshot.child('onlineGamesWon').val() / childSnapshot.child('onlineGamesPlayed').val() * 100;
									}
									break;

								case 2:
									currentStat = childSnapshot.child('offlineGamesWon').val();
									break;

								case 3:
									if(childSnapshot.child('offlineGamesWon').val() > 0)
									{
										currentStat = (childSnapshot.child('offlineGamesWon').val() /childSnapshot.child('offlineGamesPlayed').val() ) * 100;
									}
									break;
						}
						const l: Leader = new Leader(currentName, currentStat, counter, uid);
						leader.loadBoards.push(l);
						counter++;
					}
				})
				leader.loadBoards = leader.loadBoards.sort(function(a, b) {
				  return b.Stat() - a.Stat();
				});
				const pages: number = (leader.loadBoards.length / 100);
				let brokendownLeaderboardArrays: Array<LoadedLeaderboards> = [];
				let firstArrayIndex: number = 0;
				let standingCounter: number = 1;
				for(let i: number = 0; i < pages; i++){
						const lbArray: LoadedLeaderboards = new LoadedLeaderboards();
						for (let x: number = 0; x < 100; x++){
							if(leader.loadBoards.length > 0)
							{
								const l: Leader = leader.loadBoards[0];
								if(l.LeaderID() === userUID){
									firstArrayIndex = i;
								}
								l.Standing(standingCounter);
								standingCounter++;
								lbArray.loadBoards.push(l);
								leader.loadBoards.shift();
							}
							else
							{
								break;
							}
						}
						brokendownLeaderboardArrays.push(lbArray);
				}

				class Buttons{
					buttonNumbers: Array<number> = [];
				}

				const but: Buttons = new Buttons();

				if(firstArrayIndex <= 2){
					but.buttonNumbers.push(0);
          but.buttonNumbers.push(1);
          but.buttonNumbers.push(2);
          but.buttonNumbers.push(3);
          but.buttonNumbers.push(4);
				}
				else if (firstArrayIndex >= pages - 2)
        {
            but.buttonNumbers.push(pages - 4);
            but.buttonNumbers.push(pages - 3);
            but.buttonNumbers.push(pages - 2);
            but.buttonNumbers.push(pages - 1);
            but.buttonNumbers.push(pages);
        }
        else
        {
            but.buttonNumbers.push(firstArrayIndex - 2);
            but.buttonNumbers.push(firstArrayIndex - 1);
            but.buttonNumbers.push(firstArrayIndex);
            but.buttonNumbers.push(firstArrayIndex + 1);
            but.buttonNumbers.push(firstArrayIndex + 2);
        }

				const arrayToSend: LoadedLeaderboards = brokendownLeaderboardArrays[firstArrayIndex];
				resolve(JSON.stringify(arrayToSend) + '®' + JSON.stringify(but) + '®' + firstArrayIndex);
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

export const leaderboardPage = functions.https.onCall(async (data) => {
	const dbRef = admin.database().ref("users");
	const pageNumber: number = data[1];
	const selection: number = parseInt(data[0], 10);

	class Leader {
		playerName: string;
		stat: number;
		standing: number;
		id: string;
		constructor(n, s, st, i){
			this.playerName = n;
			this.stat = s;
			this.standing = st;
			this.id = i;
		}

		Standing(value){
			this.standing = value;
		}
		Stat(){
			return this.stat;
		}
		LeaderID(){
			return this.id;
		}
	};

	class LoadedLeaderboards{
		loadBoards: Array<Leader> = [];
	}
	const leader: LoadedLeaderboards = new LoadedLeaderboards();

	return new Promise((resolve, reject) => {
		dbRef.orderByValue().once("value").then((snap) => {
			const exists = (snap.val() !== null);
			if (exists)
			{
				snap.forEach((childSnapshot) => {
					if(childSnapshot.child('userName').val() !== ""){
						let counter: number = 1;
						const currentName: string = childSnapshot.child('userName').val();
						const uid: string = childSnapshot.child('userID').val();
						let currentStat: number = 0;
						switch(selection)
						{
								case 0:
									currentStat = childSnapshot.child('onlineGamesWon').val();
								break;

								case 1:
									if(childSnapshot.child('onlineGamesWon').val() > 0)
									{
										currentStat = childSnapshot.child('onlineGamesWon').val() / childSnapshot.child('onlineGamesPlayed').val() * 100;
									}
									break;

								case 2:
									currentStat = childSnapshot.child('offlineGamesWon').val();
									break;

								case 3:
									if(childSnapshot.child('offlineGamesWon').val() > 0)
									{
										currentStat = (childSnapshot.child('offlineGamesWon').val() /childSnapshot.child('offlineGamesPlayed').val() ) * 100;
									}
									break;
						}
						const l: Leader = new Leader(currentName, currentStat, counter, uid);
						leader.loadBoards.push(l);
						counter++;
					}
				})
				leader.loadBoards = leader.loadBoards.sort(function(a, b) {
				  return b.Stat() - a.Stat();
				});
				const pages: number = (leader.loadBoards.length / 100);
				let brokendownLeaderboardArrays: Array<LoadedLeaderboards> = [];
				let standingCounter: number = 1;
				for(let i: number = 0; i < pages; i++){
						const lbArray: LoadedLeaderboards = new LoadedLeaderboards();
						for (let x: number = 0; x < 100; x++){
							if(leader.loadBoards.length > 0)
							{
								const l: Leader = leader.loadBoards[0];
								l.Standing(standingCounter);
								standingCounter++;
								lbArray.loadBoards.push(l);
								leader.loadBoards.shift();
							}
							else
							{
								break;
							}
						}
						brokendownLeaderboardArrays.push(lbArray);
				}

				class Buttons{
					buttonNumbers: Array<number> = [];
				}

				const but: Buttons = new Buttons();

				if(pageNumber <= 2){
					but.buttonNumbers.push(0);
          but.buttonNumbers.push(1);
          but.buttonNumbers.push(2);
          but.buttonNumbers.push(3);
          but.buttonNumbers.push(4);
				}
				else if (pageNumber >= pages - 2)
        {
            but.buttonNumbers.push(pages - 4);
            but.buttonNumbers.push(pages - 3);
            but.buttonNumbers.push(pages - 2);
            but.buttonNumbers.push(pages - 1);
            but.buttonNumbers.push(pages);
        }
        else
        {
            but.buttonNumbers.push(pageNumber - 2);
            but.buttonNumbers.push(pageNumber - 1);
            but.buttonNumbers.push(pageNumber);
            but.buttonNumbers.push(pageNumber + 1);
            but.buttonNumbers.push(pageNumber + 2);
        }

				const arrayToSend: LoadedLeaderboards = brokendownLeaderboardArrays[pageNumber];
				resolve(JSON.stringify(arrayToSend) + '®' + JSON.stringify(but));
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

export const getLastPage = functions.https.onCall(async (data) => {
	const dbRef = admin.database().ref("users");
	const selection: number = data;

	class Leader {
		playerName: string;
		stat: number;
		standing: number;
		id: string;
		constructor(n, s, st, i){
			this.playerName = n;
			this.stat = s;
			this.standing = st;
			this.id = i;
		}

		Standing(value){
			this.standing = value;
		}
		Stat(){
			return this.stat;
		}
		LeaderID(){
			return this.id;
		}
	};

	class LoadedLeaderboards{
		loadBoards: Array<Leader> = [];
	}
	const leader: LoadedLeaderboards = new LoadedLeaderboards();

	return new Promise((resolve, reject) => {
		dbRef.orderByValue().once("value").then((snap) => {
			const exists = (snap.val() !== null);
			if (exists)
			{
				snap.forEach((childSnapshot) => {
					if(childSnapshot.child('userName').val() !== ""){
						let counter: number = 1;
						const currentName: string = childSnapshot.child('userName').val();
						const uid: string = childSnapshot.child('userID').val();
						let currentStat: number = 0;
						switch(selection)
						{
								case 0:
									currentStat = childSnapshot.child('onlineGamesWon').val();
								break;

								case 1:
									if(childSnapshot.child('onlineGamesWon').val() > 0)
									{
										currentStat = childSnapshot.child('onlineGamesWon').val() / childSnapshot.child('onlineGamesPlayed').val() * 100;
									}
									break;

								case 2:
									currentStat = childSnapshot.child('offlineGamesWon').val();
									break;

								case 3:
									if(childSnapshot.child('offlineGamesWon').val() > 0)
									{
										currentStat = (childSnapshot.child('offlineGamesWon').val() /childSnapshot.child('offlineGamesPlayed').val() ) * 100;
									}
									break;
						}
						const l: Leader = new Leader(currentName, currentStat, counter, uid);
						leader.loadBoards.push(l);
						counter++;
					}
				})
				leader.loadBoards = leader.loadBoards.sort(function(a, b) {
				  return b.Stat() - a.Stat();
				});
				const pages: number = (leader.loadBoards.length / 100);
				let brokendownLeaderboardArrays: Array<LoadedLeaderboards> = [];
				let standingCounter: number = 1;
				for(let i: number = 0; i < pages; i++){
						const lbArray: LoadedLeaderboards = new LoadedLeaderboards();
						for (let x: number = 0; x < 100; x++){
							if(leader.loadBoards.length > 0)
							{
								const l: Leader = leader.loadBoards[0];
								l.Standing(standingCounter);
								standingCounter++;
								lbArray.loadBoards.push(l);
								leader.loadBoards.shift();
							}
							else
							{
								break;
							}
						}
						if(lbArray.loadBoards.length > 0)
							brokendownLeaderboardArrays.push(lbArray);
				}

				class Buttons{
					buttonNumbers: Array<number> = [];
				}

				const but: Buttons = new Buttons();
        but.buttonNumbers.push(pages - 4);
        but.buttonNumbers.push(pages - 3);
        but.buttonNumbers.push(pages - 2);
        but.buttonNumbers.push(pages - 1);
        but.buttonNumbers.push(pages);

				const arrayToSend: LoadedLeaderboards = brokendownLeaderboardArrays[brokendownLeaderboardArrays.length - 1];
				const index: string = (brokendownLeaderboardArrays.length - 1).toString();
				resolve(JSON.stringify(arrayToSend) + '®' + JSON.stringify(but) + '®' + index);
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
