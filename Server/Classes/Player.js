const shortId = require("shortid");
const Vector3 = require("./Vector3.js");

module.exports = class Player {
  constructor() {
    this.id = shortId.generate();
    this.username = "Default_Player";
    this.avatar = "Default";
    this.playedCounter = 0;
    this.wonCounter = 0;

    this.lobby = 0;
    this.score = 0;
    this.anim = "Running";
    this.position = new Vector3();
    this.rotation = new Vector3();
  }

  InitPlayer() {
    let player = this;
    player.position = new Vector3();
    player.rotation = new Vector3();
    player.score = 0;
    this.anim = "Running";
  }
  PrintPlayer() {
    return "(" + this.username + ":" + this.id + ")";
  }
};
