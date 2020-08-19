const PasswordHash = require("password-hash");
const MongoClient = require("mongodb").MongoClient;
const url = process.env.MongoDB_URL;
//("mongodb+srv://admin:admin123@arstangagamecluster-kp7od.mongodb.net/test?retryWrites=true&w=majority");
const urlLocal = "mongodb://localhost:27017/StangaDB";

module.exports = class DataBase {
  constructor(isLocal = false) {
    this.client = isLocal
      ? new MongoClient(urlLocal, { useUnifiedTopology: true })
      : new MongoClient(url, { useUnifiedTopology: true });
    this.database;
    this.DB;
    this.ConnectToDatabase();
  }

  ConnectToDatabase() {
    this.client.connect((err, db) => {
      if (err) throw err;
      this.database = db;
      this.DB = db.db("StangaDB");
    });
  }
  CreateAccount(name, password, callback) {
    let DB = this.DB;
    let hashedPassword = PasswordHash.generate(password);
    //check if user with this name is already exiest
    let query = { username: name };
    DB.collection("users")
      .find(query)
      .toArray(async function (err, result) {
        if (err) {
          throw err;
        }
        // user already exiests
        if (result[0] != undefined) {
          callback({
            valid: false,
            reason: "user already exiests",
          });
          return;
        }
        // insert new user to database
        let user = {
          username: name,
          password: hashedPassword,
          played: 0,
          won: 0,
          avatar: "Default",
        };
        await DB.collection("users").insertOne(
          user,
          { writeConcern: { w: "majority", j: true } },
          function (err, res) {
            if (err) {
              throw err;
            }
            callback({
              valid: true,
              reason: "Success create new account please sign in !",
            });
          }
        );
      });
  }

  SignIn(username, password, callback) {
    let DB = this.DB;
    let query = { username: username };
    DB.collection("users")
      .find(query)
      .toArray(function (err, result) {
        if (err) throw err;
        if (result[0] != undefined) {
          if (PasswordHash.verify(password, result[0].password)) {
            callback({
              valid: true,
              reason: "Success sign in!!",
            });
          } else {
            console.log("not match");
            callback({
              valid: false,
              reason: "Password does not match",
            });
          }
        } else {
          callback({
            valid: false,
            reason: "User does not exists",
          });
        }
      });
  }
  InitPlayerData(name, callback) {
    let DB = this.DB;
    let query = { username: name };
    DB.collection("users")
      .find(query)
      .toArray(function (err, result) {
        if (err) {
          throw err;
        }
        if (result[0] != undefined) {
          console.log(
            "played=" +
              result[0].played +
              " won=" +
              result[0].won +
              " Avatar=" +
              result[0].avatar
          );
          callback({
            played: result[0].played,
            won: result[0].won,
            avatar: result[0].avatar,
          });
        }
      });
  }
  async SaveGameResult(name, playedCounter, wonCounter) {
    let DB = this.DB;
    let query = { username: name };
    let newValue = { $set: { played: playedCounter, won: wonCounter } };
    await DB.collection("users").updateOne(
      query,
      newValue,
      { writeConcern: { w: "majority", j: true } },
      function (err, result) {
        if (err) {
          throw err;
        }
        console.log("update player(" + name + ") in DB");
      }
    );
  }

  async UpdateUsername(oldname, newname, callback) {
    let DB = this.DB;
    let query = { username: oldname };
    let newValue = { $set: { username: newname } };
    await DB.collection("users").updateOne(
      query,
      newValue,
      { writeConcern: { w: "majority", j: true } },
      function (err, result) {
        if (err) {
          callback({ isErr: true, msg: "Faild to updated username" });
          console.log(err.errmsg);
        } else {
          callback({ isErr: false, msg: "Successfuly updated username!" });
          console.log("update username to(" + newname + ") in DB");
        }
      }
    );
  }

  async UpdatePassword(name, newpassword, callback) {
    let DB = this.DB;
    let query = { username: name };
    let newValue = { $set: { password: PasswordHash.generate(newpassword) } };
    await DB.collection("users").updateOne(
      query,
      newValue,
      { writeConcern: { w: "majority", j: true } },
      function (err, result) {
        if (err) {
          callback({ isErr: true, msg: "Faild to updated password" });
          console.log(err.errmsg);
        } else {
          callback({ isErr: false, msg: "Successfuly updated password!" });
          console.log("update password player(" + name + ") in DB");
        }
      }
    );
  }

  async UpdateAvatar(name, newAvatar, callback) {
    let DB = this.DB;
    let query = { username: name };
    let newValue = { $set: { avatar: newAvatar } };
    await DB.collection("users").updateOne(
      query,
      newValue,
      { writeConcern: { w: "majority", j: true } },
      function (err, result) {
        if (err) {
          callback({ isErr: true, msg: "Faild to updated avarar" });
          console.log(err.errmsg);
        } else {
          callback({ isErr: false, msg: "Successfuly updated avatar!" });
          console.log("updated avatar player(" + name + ") in DB");
        }
      }
    );
  }
};
