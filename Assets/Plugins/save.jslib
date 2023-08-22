mergeInto(LibraryManager.library, {

  Save: function (json) {
    window.localStorage.setItem("super-knight-v1-savegame", UTF8ToString(json));
  },

  Load: function () {
    var json = window.localStorage.getItem("super-knight-v1-savegame");
    if (json === null)
        json = ''

    var bufferSize = lengthBytesUTF8(json) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(json, buffer, bufferSize);
    return buffer;
  },
});
