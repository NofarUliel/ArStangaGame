module.exports = class State {
  constructor() {
    this.GAME = "Game";
    this.START_GAME = "StartGame";
    this.DETECTED_MARKER = "DetectedMarker";
    this.LOBBY = "Lobby";
    this.SEND_INVITATION = "SendInvitation";
    this.RECEIVE_INVITATION = "ReceiveInvitation";
    this.RESULT_INVITATION = "ResultInvitation";
    this.UNCONECCTED_USERS = "UnconnectedUsers";
    this.GAME_OVER = "GameOver";
    this.PLAYER_LEFT_GAME = "PlayerLeftGame";
    this.currentState = this.LOBBY;
  }
  Print() {
    return "state: " + this.currentState;
  }
};
