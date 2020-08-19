const GameLobby = require("./Lobbies/GameLobby.js");
const KickBall = require("./KickBall.js");

module.exports = class Connection {
  constructor() {
    this.socket;
    this.player;
    this.server;
    this.lobby;
  }

  //Handles all our io events and where we should route them too to be handled
  createEvents() {
    let connection = this;
    let socket = connection.socket;
    let server = connection.server;
    let player = connection.player;

    socket.on("disconnect", function () {
      if (player.lobby != 0) {
        server.lobbys.forEach((lobby) => {
          if (lobby.id == player.lobby) {
            lobby.lobbyState.currentState = lobby.lobbyState.PLAYER_LEFT_GAME;
            socket.broadcast.to(lobby.id).emit("lobbyUpdate", {
              state: lobby.lobbyState.currentState,
              name: player.username,
            });
          }
        });
      } else {
        server.lobbys.forEach((lobby) => {
          if (lobby.id == player.lobby) {
            console.log("remove invitation");
            socket.broadcast.to(lobby.id).emit("removeInvitation", {
              id: player.id,
              name: player.username,
            });
          }
        });
      }
      server.onDisconnected(connection);
    });

    socket.on("createAccount", function (data) {
      console.log(
        "createAccount pass=" + data.password + "name=" + data.username
      );
      server.database.CreateAccount(data.username, data.password, (result) => {
        //Results will return a true or false based on if the account already exists or not
        console.log(result.valid + ":" + result.reason);
        socket.emit("signInUpMessage", {
          result: result.valid,
          message: result.reason,
        });
      });
    });
    socket.on("signIn", function (data) {
      //check if this user already logged in
      let loggedin = false;
      server.connections.forEach((connect) => {
        if (connect.player.username === data.username) {
          loggedin = true;
        }
      });
      if (loggedin) {
        console.log("This user (" + data.username + ") already logged in...");
        socket.emit("signInUpMessage", {
          result: false,
          message: "This user already logged in...",
        });
      } else {
        server.database.SignIn(data.username, data.password, (result) => {
          //Results will return a true or false based on if the account already exists or not
          console.log(result.valid + ":" + result.reason);
          if (result.valid) {
            player.username = data.username;
            server.database.InitPlayerData(data.username, (result) => {
              player.playedCounter = result.played;
              player.wonCounter = result.won;
              player.avatar = result.avatar;
              socket.emit("initUsers");
              socket.broadcast.emit("initUsers");
              server.connections.forEach((connect) => {
                socket.emit("saveUser", {
                  id: connect.player.id,
                  username: connect.player.username,
                  played: connect.player.playedCounter,
                  won: connect.player.wonCounter,
                  avatar: connect.player.avatar,
                });
                socket.broadcast.emit("saveUser", {
                  id: connect.player.id,
                  username: connect.player.username,
                  played: connect.player.playedCounter,
                  won: connect.player.wonCounter,
                  avatar: connect.player.avatar,
                });
              });

              socket.emit("signIn");
            });
          } else {
            socket.emit("signInUpMessage", {
              result: result.valid,
              message: "Incorrect username or password",
            });
          }
        });
      }
    });
    socket.on("updateUsername", function (data) {
      server.database.UpdateUsername(data.oldData, data.newData, (result) => {
        if (!result.isErr) {
          player.username = data.newData;
        }
        socket.emit("updateUsername", {
          err: result.isErr,
          msg: result.msg,
          name: data.newData,
        });
      });
    });

    socket.on("updatePassword", function (data) {
      server.database.UpdatePassword(data.oldData, data.newData, (result) => {
        socket.emit("updatePassword", {
          err: result.isErr,
          msg: result.msg,
        });
      });
    });
    socket.on("updateAvatar", function (data) {
      server.database.UpdateAvatar(data.oldData, data.newData, (result) => {
        if (!result.isErr) {
          player.avatar = data.newData;
        }
        socket.emit("updateAvatar", {
          err: result.isErr,
          msg: result.msg,
          avatar: data.newData,
        });
      });
    });
    socket.on("joinGame", function (data) {
      server.onAttemptToJoinGame(connection, data.currentLevel);
    });
    socket.on("createNewGame", function (data) {
      let gameLobbies = server.lobbys.filter((item) => {
        return item instanceof GameLobby;
      });
      let gamelobby = server.onCreateGameLobby(
        connection,
        gameLobbies,
        data.currentLevel
      );

      let connections = server.onGetAllConnections(connection);

      if (connections.length == 0) {
        gamelobby.lobbyState.currentState =
          gamelobby.lobbyState.UNCONECCTED_USERS;
        socket.emit("lobbyUpdate", {
          state: gamelobby.lobbyState.currentState,
        });
      } else {
        gamelobby.lobbyState.currentState =
          gamelobby.lobbyState.SEND_INVITATION;
        socket.emit("lobbyUpdate", {
          state: gamelobby.lobbyState.currentState,
        });
        socket.emit("invitationInit");
        connections.forEach((connect) => {
          socket.emit("invitationPlayer", {
            id: connect.player.id,
            name: connect.player.username,
          });
        });
      }
    });
    socket.on("lobbyUpdate", function () {
      if (connection.lobby instanceof GameLobby) {
        connection.lobby.lobbyState.currentState =
          connection.lobby.lobbyState.UNCONECCTED_USERS;
        socket.emit("lobbyUpdate", {
          state: connection.lobby.lobbyState.currentState,
        });
      }
    });
    socket.on("invitationAddPlayer", function () {
      let gamelobby;
      server.lobbys.forEach((lobby) => {
        if (lobby.id === player.lobby && player.lobby != 0) {
          gamelobby = lobby;
          return;
        }
      });
      let connections = server.onGetAllConnections(connection);
      if (gamelobby != undefined) {
        if (connections.length == 0) {
          gamelobby.lobbyState.currentState =
            gamelobby.lobbyState.UNCONECCTED_USERS;
          socket.emit("lobbyUpdate", {
            state: gamelobby.lobbyState.currentState,
          });
        } else {
          gamelobby.lobbyState.currentState =
            gamelobby.lobbyState.SEND_INVITATION;
          socket.emit("lobbyUpdate", {
            state: gamelobby.lobbyState.currentState,
          });
          socket.emit("invitationInit");
          connections.forEach((connect) => {
            if (connect.player.id != player.id) {
              socket.emit("invitationPlayer", {
                id: connect.player.id,
                name: connect.player.username,
              });
            }
          });
        }
      } else {
        console.error("invitationAddPlayer:canot find game lobby");
      }
    });
    socket.on("newGame", function (data) {
      if (player.lobby != 0) {
        server.lobbys.forEach((lobby) => {
          if (lobby.id == player.lobby) {
            console.log("new game");
            lobby.lobbyState.currentState = lobby.lobbyState.LOBBY;
            lobby.winner = "";
            lobby.countPlayerLoadedLevel = 0;
            lobby.playerTurn = "";
            socket.emit("lobbyUpdate", {
              state: lobby.lobbyState.currentState,
            });
            socket.broadcast.to(lobby.id).emit("lobbyUpdate", {
              state: lobby.lobbyState.currentState,
            });
          }
        });
      }
    });
    socket.on("sendInvitation", function (data) {
      server.connections.forEach((connect) => {
        if (connect.player.id == data.id) {
          connect.socket.emit("receiveInvitation", {
            lobby: connection.lobby.id,
            sendername: connection.player.username,
            level: connection.lobby.settings.gameMode,
          });
          console.log(
            "Send invitation to player " +
              connect.player.PrintPlayer() +
              " join lobby (" +
              connection.lobby.id +
              "," +
              connection.lobby.settings.gameMode +
              ")"
          );
          return;
        }
      });
    });
    socket.on("acceptInvitation", function (data) {
      server.connections.forEach((connect) => {
        if (connect.player.id == data.id) {
          server.onSwitchLobby(connect, data.lobby);
          return;
        }
      });
    });

    socket.on("denyInvitation", function (data) {
      connection.lobby.id = 0;
      let gameLobbies = server.lobbys.filter((item) => {
        return item instanceof GameLobby;
      });
      let gamelobby;
      gameLobbies.forEach((lobby) => {
        if (lobby.id == data.lobby) {
          gamelobby = lobby;
        }
      });
      if (gamelobby != undefined) {
        gamelobby.lobbyState.currentState =
          gamelobby.lobbyState.RESULT_INVITATION;
        server.connections.forEach((connect) => {
          if (connect.lobby.id == gamelobby.id) {
            connect.socket.emit("denyInvitation", { username: data.name });
            console.log(
              "player(" +
                data.id +
                "," +
                data.name +
                ") deny to join game lobby(" +
                gamelobby.id +
                ")"
            );
            connect.socket.emit("lobbyUpdate", {
              state: gamelobby.lobbyState.currentState,
            });
            return;
          }
        });
      } else {
        console.error("denyInvitation:gamelobby is undefined ");
      }
    });
    socket.on("loadedLevelGame", function (data) {
      let countLevel = connection.lobby.countPlayerLoadedLevel;
      let maxPlayers = connection.lobby.settings.maxPlayers;
      connection.lobby.countPlayerLoadedLevel = countLevel + 1;
      console.log(
        "countPlayerLoadedLevel=" +
          connection.lobby.countPlayerLoadedLevel +
          "max players=" +
          maxPlayers
      );
      if (connection.lobby.countPlayerLoadedLevel == maxPlayers) {
        console.log(
          "All players in game lobby(" + connection.lobby.id + ") loaded level"
        );
        socket.emit("allPlayersLoadedLevelGame");
        socket.broadcast
          .to(connection.lobby.id)
          .emit("allPlayersLoadedLevelGame");
      }
    });

    socket.on("markerDetected", function () {
      let countDetected = connection.lobby.countPlayerDetectedMarker;
      let maxPlayers = connection.lobby.settings.maxPlayers;
      connection.lobby.countPlayerDetectedMarker = countDetected + 1;
      console.log(
        "countPlayerDetecyed = " +
          connection.lobby.countPlayerDetectedMarker +
          "max players =" +
          maxPlayers
      );
      if (connection.lobby.countPlayerDetectedMarker == maxPlayers) {
        // connection.lobby.lobbyState.currentState =
        //   connection.lobby.lobbyState.DETECTED_MARKER;
        socket.emit("allPlayersDetectedMarker");
        socket.broadcast
          .to(connection.lobby.id)
          .emit("allPlayersDetectedMarker");
        console.log(
          "All players in game lobby(" +
            connection.lobby.id +
            ") detected marker"
        );
        connection.lobby.onStartGame(connection);
      }
    });

    socket.on("updateAnim", function (data) {
      player.anim = data.anim;
      socket.broadcast
        .to(connection.lobby.id)
        .emit("updateAnim", { id: player.id, anim: player.anim });
    });
    socket.on("updatePosition", function (data) {
      //console.log( "Server logic started at :", new Date().getTime(), "miliseconds" );
      //server logic
      if (
        connection.lobby.lobbyState.currentState ===
        connection.lobby.lobbyState.GAME
      ) {
        player.position.x = data.position.x;
        player.position.y = data.position.y;
        player.position.z = data.position.z;

        player.rotation.x = data.rotation.x;
        player.rotation.y = data.rotation.y;
        player.rotation.z = data.rotation.z;
        socket.broadcast.to(connection.lobby.id).emit("updatePosition", player);

        // console.log( "Server logic finished at : ", new Date().getTime(), "miliseconds");
      } else {
        console.err("UpdatePosition lobby state != GAME or empty");
      }
    });

    socket.on("updateBallPosition", function (data) {
      if (
        connection.lobby.lobbyState.currentState ===
        connection.lobby.lobbyState.GAME
      ) {
        connection.lobby.ball.position.x = data.position.x;
        connection.lobby.ball.position.y = data.position.y;
        connection.lobby.ball.position.z = data.position.z;

        socket.broadcast
          .to(connection.lobby.id)
          .emit("updateBallPosition", connection.lobby.ball);
      }
    });
    socket.on("playerKickBall", function (data) {
      let kickBall = new KickBall();
      kickBall.ballID = data.ballID;
      kickBall.direction.x = data.direction.x;
      kickBall.direction.y = data.direction.y;
      kickBall.direction.z = data.direction.z;
      kickBall.force = data.force;
      socket.broadcast.to(connection.lobby.id).emit("playerKickBall", kickBall);
      console.log("player kicked ", kickBall.direction);
    });

    socket.on("updateScore", function (data) {
      connection.lobby.connections.forEach((connect) => {
        if (connect.player.id === data.id) {
          connect.player.score = data.score;
          console.log(
            "updateScore player(" +
              data.id +
              ") to (" +
              connect.player.score +
              ")"
          );
        }
      });
      connection.socket.emit("updateScore", {
        player: data.player,
        score: data.score,
      });
      connection.socket.broadcast
        .to(connection.lobby.id)
        .emit("updateScore", { player: data.player, score: data.score });
    });

    socket.on("updateTurn", function (data) {
      connection.lobby.playerTurn = data.playerTurn;
      connection.socket.broadcast
        .to(connection.lobby.id)
        .emit("updateTurn", { playerTurn: data.playerTurn });
      console.log(
        "updateTurn to player(" +
          data.playerTurn +
          ") send(" +
          data.sendId +
          ")"
      );
    });
    socket.on("goalUI", function (data) {
      console.log(
        "player(" + data.player + ") enter goal (" + data.score + ")"
      );
      player.score = data.score;
      socket.emit("goalUI", { player: data.player, score: data.score });
      socket.broadcast
        .to(connection.lobby.id)
        .emit("goalUI", { player: data.player, score: data.score });
    });

    socket.on("goal", function (data) {
      socket.emit("updateScore", {
        player: data.player,
        score: data.score,
      });

      socket.emit("initPosition");
    });

    socket.on("gameOver", function (data) {
      if (connection.lobby.winner == "") {
        console.log("statuse : " + connection.lobby.lobbyState.currentState);
        connection.lobby.winner = data.winnerId;
        connection.lobby.lobbyState.currentState =
          connection.lobby.lobbyState.GAME_OVER;
        socket.emit("lobbyUpdate", {
          state: connection.lobby.lobbyState.currentState,
        });
        socket.broadcast.to(connection.lobby.id).emit("lobbyUpdate", {
          state: connection.lobby.lobbyState.currentState,
        });
        console.log("statuse : " + connection.lobby.lobbyState.currentState);
      }
    });
    socket.on("removePlayerFromLobby", function (e) {
      let gamelobbyId = connection.lobby.id;
      let gameLobby = connection.lobby;
      server.onSwitchLobby(connection, 0);
      if (
        gameLobby.lobbyState.currentState === gameLobby.lobbyState.GAME_OVER
      ) {
        //save result game to DB
        server.database.SaveGameResult(
          player.username,
          player.playedCounter,
          player.wonCounter
        );
      }
      // init player to a new game
      player.InitPlayer();
      //all players removed from lobby
      if (server.lobbys[gamelobbyId].connections.length == 0) {
        server.onCloseLobby(gamelobbyId);
      }
    });

    socket.on("playerLeaveGame", function (e) {
      let gamelobbyId = connection.lobby.id;
      let gameLobby = connection.lobby;
      server.onSwitchLobby(connection, 0);
      gameLobby.lobbyState.currentState = gameLobby.lobbyState.PLAYER_LEFT_GAME;
      socket.broadcast.to(gamelobbyId).emit("lobbyUpdate", {
        state: gameLobby.lobbyState.currentState,
        name: player.username,
      });
      // init player to a new game
      player.InitPlayer();
    });

    socket.on("ARcameraOff", function (e) {
      console.log("AR camera off (" + connection.player.username + ")");
      let ball = connection.lobby.ball;
      let gate1 = connection.lobby.gate1;
      let gate2 = connection.lobby.gate2;
      connection.lobby.isArCameraOff = true;
      connection.lobby.onSpawnAllPlayersIntoGame(connection);
      socket.emit("addScoreUI");
      socket.emit("addBall", { id: ball.id });
      socket.emit("addGate", gate1);
      socket.emit("addGate", gate2);
    });
  }
};
