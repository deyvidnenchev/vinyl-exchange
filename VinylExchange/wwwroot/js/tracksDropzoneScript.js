(() =>{Dropzone.autoDiscover = false;

  var currentScript = document.currentScript;
  currentScript.id="tracksDropzoneScript"
  
  const dropzoneId = "tracksDropzone"
  
  const dropzoneElement = document.getElementById(dropzoneId)
  
  const formSessionId = dropzoneElement.attributes[1].value
  
  const dropzoneUploadPath =
    dropzoneElement.attributes[2].value + formSessionId;
  
  const dropzoneDropPath =
   dropzoneElement.attributes[3].value + formSessionId;
  
   const acceptedFiles = dropzoneElement.attributes[4].value
// Dropzone class:
var myDropzone = new Dropzone(`#${dropzoneId}`, {
  url: dropzoneUploadPath,
  acceptedFiles: acceptedFiles,
  maxFilesize: 30,
  uploadMultiple: false,
  createImageThumbnails: true,
  maxFiles: 20,
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
      console.log("added",jsonResponse);

      file.serverGuid = jsonResponse.fileId;
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
            .then(data => console.log("removed",data));
        }

        _this.removeFile(file);
      });

      // Add the button to the file preview element.
      file.previewElement.appendChild(removeButton);
    });
  }
});

document.getElementById(dropzoneId).className = "dropzone";
})()