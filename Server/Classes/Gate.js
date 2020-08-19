const shortId = require("shortid");
const Vector3 = require("./Vector3.js");

module.exports = class Gate {
  constructor() {
    this.id = shortId.generate();
    this.lobby = 0;
    this.playerId = "";
  }
};
