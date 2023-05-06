let sleepTime = 25; // in Minutes

/*
01 race  = 30s (approx minimum)
50 races = 1500s
50 races = 25m
*/

chrome.runtime.onInstalled.addListener(() => {
  chrome.storage.sync.set({ sleepTime });
  console.log('Default sleep time set to %cgreen', `sleepTime: ${sleepTime}`);
});