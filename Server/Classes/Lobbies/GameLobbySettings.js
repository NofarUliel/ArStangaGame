module.exports = class GameLobbySettings {
  constructor(gameMode) {
    this.gameMode = gameMode;
    this.maxPlayers = 2;
  }
  Print() {
    return "settings(" + this.gameMode + "," + this.maxPlayers + ")";
  }
};
