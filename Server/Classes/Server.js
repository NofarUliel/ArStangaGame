const Player = require("./Player.js");
const Database = require("./Database.js");
const Connection = require("./Connection.js");

//Lobbies
const LobbyBase = require("./Lobbies/LobbyBase.js");
const GameLobby = require("./Lobbies/GameLobby.js");
const GameLobbySettings = require("./Lobbies/GameLobbySettings.js");

module.exports = class Server {
  constructor(isLocal = false) {
    this.database = new Database(isLocal);
    this.connections = [];
    this.lobbys = [];
    this.lobbys[0] = new LobbyBase(0);
  }

  //Interval update every 100 miliseconds
  onUpdate() {
    let server = this;

    //Update each lobby
    for (let id in server.lobbys) {
      server.lobbys[id].onUpdate();
    }
  }

  //Handle a new connection to the server
  onConnected(socket) {
    let server = this;
    let connection = new Connection();
    connection.socket = socket;
    connection.player = new Player();
    connection.server = server;

    let player = connection.player;
    let lobbys = server.lobbys;

    console.log("Added new player to the server (" + player.id + ")");
    //server.connections[player.id] = connection;
    server.connections.push(connection);
    //console.log("onconnect:" + server.connections.length);

    socket.join(player.lobby);
    connection.lobby = lobbys[player.lobby];
    connection.lobby.onEnterLobby(connection);

    return connection;
  }
  onDisconnected(connection = Connection) {
    let server = this;
    let id = connection.player.id;
    let connections = server.connections;

    for (var i = 0; i < connections.length; i++) {
      if (connections[i].player.id == id) {
        connections.splice(i, 1);
        break;
      }
    }

    console.log(
      "Player " + connection.player.PrintPlayer() + " has disconnected"
    );

    //Tell Other players currently in the lobby that we have disconnected from the game
    connection.socket.broadcast
      .to(connection.player.lobby)
      .emit("disconnected", {
        id: id,
      });

    //Preform lobby clean up
    if (connection.player.lobby != 0) {
      server.lobbys[connection.player.lobby].onLeaveLobby(connection);
    }
  }

  onAttemptToJoinGame(connection = Connection, currentLevel) {
    //Look through lobbies for a gamelobby
    //check if joinable
    //if not make a new game
    let server = this;
    let lobbyFound = false;
    let socket = connection.socket;

    let gameLobbies = server.lobbys.filter((item) => {
      return item instanceof GameLobby;
    });
    console.log("Found (" + gameLobbies.length + ") lobbies on the server");

    gameLobbies.forEach((lobby) => {
      if (!lobbyFound) {
        let canJoin = lobby.canEnterLobby(connection, currentLevel);

        if (canJoin) {
          lobbyFound = true;
          lobby.countPlayerLoadedLevel = 0;
          lobby.countPlayerDetectedMarker = 0;
          server.onSwitchLobby(connection, lobby.id);
        }
      }
    });

    //All game lobbies full or we have never created one
    if (!lobbyFound) {
      this.onCreateGameLobby(connection, gameLobbies, currentLevel);
    }
  }

  onCreateGameLobby(connection = Connection, gameLobbies, currentLevel) {
    let server = this;

    console.log("Making a new game lobby " + currentLevel);
    let gamelobby = new GameLobby(
      gameLobbies.length + 1,
      new GameLobbySettings(currentLevel)
    );
    server.lobbys.push(gamelobby);
    server.onSwitchLobby(connection, gamelobby.id);
    return gamelobby;
  }
  //return all players in looby 0
  onGetAllConnections(connection = Connection) {
    let server = this;
    let allConnections = server.connections;
    let otherConnections = [];
    allConnections.forEach((connect) => {
      // console.log("conect.loby=" + connect.lobby.id);
      if (connect.lobby.id == 0) {
        otherConnections.push(connect);
      }
    });
    // console.log("players:" + otherConnections.length);
    // console.log("players=" + allConnections.length);
    return otherConnections;
  }
  onSwitchLobby(connection = Connection, lobbyID) {
    let server = this;
    let lobbys = server.lobbys;
    let index;

    for (var i = 0; i < lobbys.length; i++) {
      if (lobbys[i].id === lobbyID) {
        index = i;
        break;
      }
    }

    connection.socket.join(lobbyID); // Join the new lobby's socket channel
    connection.lobby = lobbys[index]; //assign reference to the new lobby

    lobbys[connection.player.lobby].onLeaveLobby(connection);
    lobbys[index].onEnterLobby(connection);
  }
  onCloseLobby(lobbyId) {
    let server = this;
    server.lobbys.forEach((lobby) => {
      if (lobby.id === lobbyId) {
        var index = server.lobbys.indexOf(lobby);
        server.lobbys.splice(index, 1);
        console.log("Closed lobby (" + lobbyId + ")");
      }
    });
  }
};
