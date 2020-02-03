Dropzone.autoDiscover = false;

let dropzoneIntitiationScript = document.getElementById(
  "tracksDropzoneInitiationScript"
);

let formSessionId = dropzoneIntitiationScript.getAttribute("formSessionId");

let dropzoneId = dropzoneIntitiationScript.getAttribute("dropzoneId");

let acceptedFiles = dropzoneIntitiationScript.getAttribute("acceptedFiles");

let dropzoneUploadPath =
  dropzoneIntitiationScript.getAttribute("dropzoneUploadPath") + formSessionId;

let dropzoneDropPath =
  dropzoneIntitiationScript.getAttribute("dropzoneDropPath") + formSessionId;

// Dropzone class:
var myDropzone = new Dropzone(`#${dropzoneId}`, {
  url: dropzoneUploadPath,
  acceptedFiles: acceptedFiles,
  maxFilesize: 30,
  uploadMultiple: false,
  createImageThumbnails: true,
  maxFiles: 3,
  maxfilesexceeded: function(file) {
    this.removeAllFiles();
    this.addFile(file);
  },
  init: function() {
    var drop = this;
    this.on("error", function(file, errorMessage) {
      var errorDisplay = document.querySelectorAll("[data-dz-errormessage]");
      errorDisplay[errorDisplay.length - 1].innerHTML = errorMessage;

      //alert(maxFilesize);
      //this.removeAllFiles();
    });

    this.on("complete", function(file) {
      if (file.size > 30 * 1024 * 1024) {
        this.removeFile(file);
        alert("File must be less than 30MB");
        return false;
      }
    });

    this.on("success", function(file, jsonResponse) {
      console.log(jsonResponse);

      file.serverGuid = jsonResponse.guid;
    });

    this.on("addedfile", function(file) {
      // Capture the Dropzone instance as closure.
      var _this = this;

      var removeButton = Dropzone.createElement(
        `<button class="dz-remove" data-dz-remove ">Remove file</button>`
      );
      removeButton.addEventListener("click", function(e) {
        e.preventDefault();
        e.stopPropagation();
        console.log(file.serverGuid);

        if (file.status === "success") {
          fetch(dropzoneDropPath + `&fileGuid=${file.serverGuid}`, {
            method: "POST"
          })
            .then(response => response.json())
            .then(data => console.log(data));
        }

        _this.removeFile(file);
      });

      // Add the button to the file preview element.
      file.previewElement.appendChild(removeButton);
    });
  }
});

document.getElementById(dropzoneId).className = "dropzone";
