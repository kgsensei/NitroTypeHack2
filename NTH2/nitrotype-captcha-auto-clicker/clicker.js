"use strict"

let minInterval = 25;
chrome.storage.sync.get("sleepTime", ({ sleepTime }) => {
  minInterval = sleepTime;
});

// This is for running in reCaptcha iframe
var sid = setInterval(async () => {
  if (
    window.location.href
      .match(/https:\/\/www.google.com\/recaptcha\/api\d\/anchor/) &&
    $("#recaptcha-anchor div.recaptcha-checkbox-checkmark")
      .length &&
    $("#recaptcha-anchor div.recaptcha-checkbox-checkmark")
      .is(':visible') &&
    isScrolledIntoView($("#recaptcha-anchor div.recaptcha-checkbox-checkmark")
      .get(0))
  ) {
    var execute = true;
    if (localStorage.getItem('accesstime')) {
      let timeNow = new Date().getTime();
      let timeLastExecuted = localStorage.getItem('accesstime') | 0;
      let minIntervalMs = minInterval * 60 * 1000;
      if (timeNow - timeLastExecuted < minIntervalMs) {
        execute = false;
      }
    }

    if (execute) {
      localStorage.setItem('accesstime', new Date().getTime());
      $("#recaptcha-anchor div.recaptcha-checkbox-checkmark").click();
    }
    clearInterval(sid);
  }
}, 8000);
