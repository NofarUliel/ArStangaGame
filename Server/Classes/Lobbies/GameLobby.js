const LobbyBase = require("./LobbyBase.js");
const GameLobbySettings = require("./GameLobbySettings.js");
const Connection = require("../Connection.js");
const State = require("./State.js");
const Ball = require("../Ball.js");
const Gate = require("../Gate.js");

module.exports = class GameLobbby extends LobbyBase {
  constructor(id, settings = GameLobbySettings) {
    super(id);
    this.settings = settings;
    this.countPlayerLoadedLevel = 0;
    this.countPlayerDetectedMarker = 0;
    this.lobbyState = new State();
    this.ball = new Ball();
    this.gate1 = new Gate();
    this.gate2 = new Gate();
    this.playerTurn = "";
    this.winner = "";
    this.isArCameraOff;
  }

  onUpdate() {
    let lobby = this;
  }

  canEnterLobby(connection = Connection, currentLevel) {
    let lobby = this;
    let maxPlayerCount = lobby.settings.maxPlayers;
    let gameMode = lobby.settings.gameMode;
    let currentPlayerCount = lobby.connections.length;

    if (gameMode != currentLevel) {
      console.log("game mode=" + gameMode + " current level=" + currentLevel);
      return false;
    }
    if (currentPlayerCount + 1 > maxPlayerCount) {
      return false;
    }

    return true;
  }

  onEnterLobby(connection = Connection) {
    let lobby = this;
    let socket = connection.socket;

    super.onEnterLobby(connection);
    lobby.lobbyState.currentState = lobby.lobbyState.LOBBY;
    console.log("lobby state=" + lobby.lobbyState.currentState);
    if (lobby.connections.length == lobby.settings.maxPlayers) {
      console.log("We have enough players we can start the game");
      lobby.lobbyState.currentState = lobby.lobbyState.GAME;
      socket.emit("loadGame");
      socket.broadcast.emit("loadGame");
      socket.broadcast
        .to(lobby.id)
        .emit("lobbyUpdate", { state: lobby.lobbyState.currentState });
    }

    socket.emit("lobbyUpdate", { state: lobby.lobbyState.currentState });
    console.log("state:" + lobby.lobbyState.currentState);
  }
  onStartGame(connection = Connection) {
    let lobby = this;
    let socket = connection.socket;
    let connections = lobby.connections;
    let ball = lobby.ball;
    let gate1 = lobby.gate1;
    let gate2 = lobby.gate2;
    let playersDetectedMarker = lobby.countPlayerDetectedMarker;
    if (playersDetectedMarker == lobby.settings.maxPlayers) {
      //add players
      lobby.onSpawnAllPlayersIntoGame();
      //add score UI
      socket.emit("addScoreUI");
      socket.broadcast.to(lobby.id).emit("addScoreUI");
      //init ball
      ball.lobby = lobby.id;
      console.log(
        "Add ball (" + ball.id + ") to game lobby (",
        ball.lobby + ")"
      );
      socket.emit("addBall", { id: ball.id });
      socket.broadcast.to(lobby.id).emit("addBall", ball);

      //init gates
      gate1.playerId = connections[0].player.id;
      gate1.lobby = lobby.id;

      gate2.playerId = connections[1].player.id;
      gate2.lobby = lobby.id;

      socket.emit("addGate", gate1);
      socket.broadcast.to(lobby.id).emit("addGate", gate1);
      console.log("add gate 1");
      socket.emit("addGate", gate2);
      socket.broadcast.to(lobby.id).emit("addGate", gate2);
      console.log("add gate 2");

      // init turn
      lobby.playerTurn = connections[0].player.id;
      socket.emit("updateTurn", { playerTurn: lobby.playerTurn });
      socket.broadcast
        .to(lobby.id)
        .emit("updateTurn", { playerTurn: lobby.playerTurn });
      console.log("init turn to player (" + lobby.playerTurn + ")");
    }
  }

  onLeaveLobby(connection = Connection) {
    let lobby = this;
    super.onLeaveLobby(connection);
    connection.player.lobby = 0;
    connection.player.playedCounter++;
    if (this.winner === connection.player.id) {
      connection.player.wonCounter++;
    }
  }
  onSpawnAllPlayersIntoGame(connection = Connection) {
    let lobby = this;
    let connections = lobby.connections;
    connections = connections.sort(function (a, b) {
      var x = a.player.id.toLowerCase();
      var y = b.player.id.toLowerCase();
      if (x < y) {
        return -1;
      }
      if (x > y) {
        return 1;
      }
      return 0;
    });
    if (lobby.isArCameraOff) {
      lobby.addPlayer(connection);
    } else {
      connections.forEach((connection) => {
        lobby.addPlayer(connection);
      });
    }
  }

  addPlayer(connection = Connection) {
    let lobby = this;
    let connections = lobby.connections;
    let socket = connection.socket;

    connections.forEach((c) => {
      socket.emit("addPlayer", { id: c.player.id });
    });
  }

  Print() {
    return (
      super.Print() +
      this.settings.Print() +
      this.lobbyState.Print() +
      this.ball.Print()
    );
  }
};
