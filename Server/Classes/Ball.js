let shortId = require("shortid");
let Vector3 = require("./Vector3.js");

module.exports = class Ball {
  constructor() {
    this.id = shortId.generate();
    this.position = new Vector3();
    this.lobby = 0;
  }

  Print() {
    return (
      "id:" +
      this.id +
      "position:" +
      this.position.Print() +
      "lobby game:" +
      this.lobby
    );
  }
};
