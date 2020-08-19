let Vector3 = require("./Vector3.js");
module.exports = class KickBall {
  constructor() {
    this.ballID = 0;
    this.direction = new Vector3();
    this.force = 0;
  }
};
